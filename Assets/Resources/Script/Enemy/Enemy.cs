using DG.Tweening;
using EPOOutline;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour, IAttackable
{
    [SerializeField] private ParticleSystem deathParticle;
    private string _enemyName;
    public string EnemyName => _enemyName;
    
    private int _enemyMaxShield;
    public int EnemyMaxShield
    {
        get { return _enemyMaxShield; }
        set
        {
            if (value < 0) _enemyMaxShield = 0;
            else _enemyMaxShield = value;
        }
    }
    
    [SerializeField] private int _enemyShield;
    public int EnemyShield
    {
        get { return _enemyShield; }
        set { _enemyShield = value; }
    }
    
    private int _enemyMaxHp;
    public int EnemyMaxHP
    {
        get { return _enemyMaxHp; }
        set
        {
            // TODO: enemyMaxHp가 현재 Hp보다 낮아지면 현재 Hp도 깎아야 하지 않을까?
            if (value < 0) _enemyMaxHp = 0;
            else _enemyMaxHp = value;
        }
    }

    [SerializeField] private int _enemyHp;
    public int EnemyHP
    {
        get { return _enemyHp; }
        set { _enemyHp = value; }
    }

    private int _enemyPower;
    public int EnemyPower => _enemyPower;

    private int _previousPlayerRow;
    public int PreviousPlayerRow => _previousPlayerRow;
    private int _previousPlayerCol;
    public int PreviousPlayerCol => _previousPlayerCol;

    private Dictionary<Debuff,  int> _debuffDictionary;
    public Dictionary<Debuff, int> DebuffDictionary => _debuffDictionary;

    private (EnemyAction, EnemyAction) _previousAttack = (EnemyAction.None, EnemyAction.None);

    private List<(int, int)> _previousAttackTargetSpaces;
    private List<(int, int)> _currentAttackTargetSpaces;
    private List<(int, int)> overlapPoint;

    public Queue<(EnemyAction, int)> EnemyActions;
    
    public void AttackedByPlayer(int damage, int attackCount)
    {
        StartCoroutine(AttackedByPlayerCoroutine(damage, attackCount));
    }

    public IEnumerator AttackedByPlayerCoroutine(int damage, int attackCount)
    {
        for (int i = 0; i < attackCount; i++)
        {
            DamageEnemyShield(damage);
            PlayAttackFromPlayerEffect();

            if(PlayerManager.Instance.GOD)
            {
                EnemyManager.Instance.EnemyList.Remove(this);
                StartCoroutine(EnemyDeathCoroutine());
            }

            if (EnemyShield == 0)
            {
                DamageEnemyHp(PlayerManager.Instance.BaseAp + PlayerManager.Instance.Ap);
                
                PlayerManager.Instance.ResetAp();
                /*
                int bingoCount = BoardManager.Instance.CountBingo(BoardColor.Player);  
                
                if (bingoCount > 0)
                {
                    for (int ii = 0; ii < BoardManager.Instance.BoardSize; ii++)
                    {
                        for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
                        {
                            BoardManager.Instance.GameBoard[ii][j].SetBoardColor(BoardColor.None);
                            BoardManager.Instance.GameBoard[ii][j].ActivateBingoEffect(false);
                        }
                    }

                    _enemyHp -= (int) Math.Pow(2, bingoCount - 1);
                    PlayerManager.Instance.BingoAttack = true;
                }
                */
                if (EnemyHP <= 0)
                {
                    EnemyManager.Instance.EnemyList.Remove(this);
                    StartCoroutine(EnemyDeathCoroutine());
                }
            }
            EnemyUI.ShieldUIUpdate();
            EnemyUI.HPUIUpdate();
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator EnemyDeathCoroutine()
    {
        transform.parent.DORotate(new Vector3(0, 0, 0), 0.6f, RotateMode.Fast).SetEase(Ease.InQuart);
        SoundManager.Instance.PlaySE("Death");

        yield return new WaitForSeconds(1.0f);

        deathParticle.Play();

        yield return new WaitForSeconds(1.0f);
        
        GameManager.Instance.GameClear();
        Destroy(this.gameObject);
    }

    private void PlayAttackFromPlayerEffect()
    {
        int leftOrRight = Random.Range(0, 2);
        int angle;
        if (leftOrRight == 0)
            angle = Random.Range(10, 70);
        else
            angle = Random.Range(-10, -70);
        print(angle);
        Quaternion rotation = Quaternion.Euler(angle, -90, 90);
        Instantiate(PlayerManager.Instance.AttackEffect, transform.position + new Vector3(0, 0, -3), rotation);  // 자동으로 destroy된다.
        //SE 재생
        SoundManager.Instance.PlaySE("PlayerAttack", 0.5f);
    }

    public EnemyUI EnemyUI;
    [SerializeField] private Material spriteOutlineMaterial;
    [SerializeField] private Material cardboardOutlineMaterial;
    [SerializeField] private Color spriteOutlineColor;
    [SerializeField] private Color originalSpriteOutlineColor = Color.white;

    public void InitEnemyData(EnemyDataHolder enemyDataHolder)
    {
        _previousAttackTargetSpaces = new List<(int, int)>();
        _currentAttackTargetSpaces = new List<(int, int)>();
        _previousAttack = (EnemyAction.None, EnemyAction.None);
        _enemyName = enemyDataHolder.EnemyName;
        _enemyMaxHp = enemyDataHolder.EnemyHP;
        _enemyMaxShield = enemyDataHolder.EnemyShield;
        _enemyHp = enemyDataHolder.EnemyHP;
        _enemyShield = enemyDataHolder.EnemyShield;
        EnemyActions = enemyDataHolder.EnemyAction;
        _debuffDictionary = new Dictionary<Debuff, int>();
        gameObject.GetComponent<Outlinable>().enabled = false;
        foreach(Debuff debuff in Enum.GetValues(typeof(Debuff)))
            _debuffDictionary[debuff] = 0;
        overlapPoint = new List<(int, int)>();
        EnemyUI.InitUI();
    }
    
    /// <summary>
    /// �� Enemy�� EnemyActions Queue���� EnemyAction �ϳ��� dequeue�� ��, �ش� Action�� �°� �ൿ�� ��, �ٽ� enqueue�Ѵ�.
    /// </summary>
    public bool DoEnemyAction()
    {
        (EnemyAction, int) enemyAction = EnemyActions.Dequeue();
        bool _isGameOver = false;
        
        Debug.Log(enemyAction.Item1);

        switch ((int)enemyAction.Item1 / 100)
        {
            case 0:
                _isGameOver = EnemyAttack(enemyAction);
                break;
            case 1:
                _isGameOver = EnemySummon(enemyAction);
                break;
            case 2:
                EnemyColor(enemyAction);
                break;
            case 3:
                EnemyBuff(enemyAction);
                break;
            case 4:
                EnemyDebuff(enemyAction);
                break;
        }

        EnemyActions.Enqueue(enemyAction);  // 다시 넣는다

        EnemyUI.IntentionUpdate();

        return _isGameOver;
    }

    private bool EnemyAttack((EnemyAction, int) enemyAction)
    {
        int damage = enemyAction.Item2;

        bool _isGameOver = false;

        if(_debuffDictionary[Debuff.PowerIncrease] > 0)
            damage = (int)(damage * (1 + _debuffDictionary[Debuff.PowerIncrease] / 100f));
        if(_debuffDictionary[Debuff.PowerDecrease] > 0)
            damage = (int)(damage * (1 - _debuffDictionary[Debuff.PowerDecrease] / 100f));

        _previousAttack.Item2 = _previousAttack.Item1;
        _previousAttack.Item1 = enemyAction.Item1;
        
        // 직전에 action을 수행한 space들을 저장한다.
        _previousAttackTargetSpaces.Clear();
        foreach (var elem in _currentAttackTargetSpaces)
            _previousAttackTargetSpaces.Add(elem);
        _currentAttackTargetSpaces.Clear();

        switch (enemyAction.Item1)
        {
            case EnemyAction.H1Attack:
                for (int c = 0; c < BoardManager.Instance.BoardSize; c++)
                {
                    _currentAttackTargetSpaces.Add((_previousPlayerRow, c));
                    if(BoardManager.Instance.BoardObjects[_previousPlayerRow][c] == BoardObject.Player)
                        _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.V1Attack:
                for (int r = 0; r < BoardManager.Instance.BoardSize; r++)
                {
                    _currentAttackTargetSpaces.Add((r, _previousPlayerCol));
                    if(BoardManager.Instance.BoardObjects[r][_previousPlayerCol] == BoardObject.Player)
                        _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.H2Attack:
            {
                List<int> pool = Enumerable.Range(0, BoardManager.Instance.BoardSize).ToList();  // [ 0, 1, 2, ..., boardSize-1 ]
                pool.RemoveAt(_previousPlayerRow);
                List<int> rows = Extensions.ChooseDifferentRandomElements(pool, 2);
                foreach (int r in rows)
                {
                    for (int c = 0; c < BoardManager.Instance.BoardSize; c++)
                    {
                        _currentAttackTargetSpaces.Add((r, c));
                        if (BoardManager.Instance.BoardObjects[r][c] == BoardObject.Player)
                            _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                    }
                }
                break;
            }
            case EnemyAction.V2Attack:
            {
                List<int> pool = Enumerable.Range(0, BoardManager.Instance.BoardSize).ToList();  // [ 0, 1, 2, ..., boardSize-1 ]
                pool.RemoveAt(_previousPlayerCol);
                List<int> cols = Extensions.ChooseDifferentRandomElements(pool, 2);
                foreach (int c in cols)
                {
                    for (int r = 0; r < BoardManager.Instance.BoardSize; r++)
                    {
                        _currentAttackTargetSpaces.Add((r, c));
                        if (BoardManager.Instance.BoardObjects[r][c] == BoardObject.Player)
                            _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                    }
                }
                break;
            }
            case EnemyAction.ColoredAttack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(BoardManager.Instance.BoardColors[i][j] == BoardColor.None) continue;
                        else{
                            if(BoardManager.Instance.BoardObjects[i][j] == BoardObject.Player)
                                _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                            _currentAttackTargetSpaces.Add((i, j));
                        }
                    }
                }
                break;
            case EnemyAction.NoColoredAttack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(BoardManager.Instance.BoardColors[i][j] != BoardColor.None) continue;
                        else{
                            if(BoardManager.Instance.BoardObjects[i][j] == BoardObject.Player)
                                _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                            _currentAttackTargetSpaces.Add((i, j));
                        }
                    }
                }
                break;
            case EnemyAction.AllAttack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
                        _currentAttackTargetSpaces.Add((i, j));
                _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                break;
        }

        StartCoroutine(PlayEnemyAttackEffect(_currentAttackTargetSpaces, _isGameOver));

        return _isGameOver;
    }

    private void DamageEnemyHp(int damage)
    {
        _enemyHp -= damage;
    }
    private void DamageEnemyShield(int damage)
    {
        EnemyShield = EnemyShield > damage ? EnemyShield - damage : 0;
    }

    private IEnumerator PlayEnemyAttackEffect(List<(int, int)> attackedSpaces, bool isGameOver)
    {
        int boardSize = BoardManager.Instance.BoardSize;
        bool[,] attacked = new bool[boardSize, boardSize];  // 모두 false로 초기화
        foreach ((int, int) coord in attackedSpaces)
            attacked[coord.Item1, coord.Item2] = true;

        // 위부터 아래 방향으로 순서대로 쾅쾅쾅 터뜨리는 코드
        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                if (attacked[r, c])
                {
                    Vector3 position = BoardManager.Instance.GameBoard[r][c].transform.position;
                    Instantiate(EnemyManager.Instance.EnemyAttackEffect, position + new Vector3(0, 0, -6), Quaternion.identity);  // 자동으로 destroy된다.
                }
            }
            yield return new WaitForSeconds(0.3f);
        }

        if(isGameOver)
            GameManager.Instance.GameOver();
    }

    private bool EnemySummon((EnemyAction, int) enemyAction)
    {
        bool _isGameOver = false;
        
        int damage = enemyAction.Item2;

        if(_debuffDictionary[Debuff.PowerIncrease] > 0)
            damage = (int)(damage * (1 + _debuffDictionary[Debuff.PowerIncrease] / 100f));
        if(_debuffDictionary[Debuff.PowerDecrease] > 0)
            damage = (int)(damage * (1 - _debuffDictionary[Debuff.PowerDecrease] / 100f));
        if(overlapPoint.Count == 0)
            return _isGameOver;
        else
        {
            foreach((int, int) elems in overlapPoint)
                _isGameOver = BoardManager.Instance.SummonWalls(elems.Item1, elems.Item2, damage);
        }

        return _isGameOver;
    }

    public void GetOverLapPosition((EnemyAction, int) enemyAction)
    {
        overlapPoint.Clear();

        if(_previousAttackTargetSpaces.Count == 0 || _currentAttackTargetSpaces.Count == 0) return;
        foreach ((int, int) item in _previousAttackTargetSpaces)
            if(_currentAttackTargetSpaces.Contains(item)) overlapPoint.Add(item);
        if (enemyAction.Item1 == EnemyAction.WallSummon && overlapPoint.Count != 0)
        {
            overlapPoint.Shuffle();
            overlapPoint = new List<(int, int)>() {overlapPoint[0]};
        }
    }

    public void HighlightOverlapPoint()
    {
        if(overlapPoint.Count == 0)
            return;
        else
        {
            foreach ((int, int) p in overlapPoint)
                BoardManager.Instance.GameBoard[p.Item1][p.Item2].SetHighlight(BoardSituation.WillSummon);
        }
    }

    private void EnemyColor((EnemyAction, int) enemyAction)
    {
        EnemyAction action = enemyAction.Item1;
        List<(int, int)> colorTargetSpaces = new List<(int, int)>();
        switch (action)
        {
            /// 플레이어가 위치한 가로줄 1줄에 적의 색을 칠한다
            case EnemyAction.ColorPlayerH1:
                for (int c = 0; c < BoardManager.Instance.BoardSize; c++)
                    colorTargetSpaces.Add((_previousPlayerRow, c));
                break;

            /// 플레이어가 위치한 세로줄 1줄에 적의 색을 칠한다
            case EnemyAction.ColorPlayerV1:
                for (int r = 0; r < BoardManager.Instance.BoardSize; r++)
                    colorTargetSpaces.Add((r, _previousPlayerCol));
                break;

            /// 랜덤한 칸(이미 적의 색이 있는 칸은 제외)에 적의 색을 칠한다
            case EnemyAction.ColorRandom:
            {
                List<(int, int)> notEnemyColorSpaces = new List<(int, int)>();
                for (int r = 0; r < BoardManager.Instance.BoardSize; r++)
                    for (int c = 0; c < BoardManager.Instance.BoardSize; c++)
                        if (BoardManager.Instance.BoardColors[r][c] != BoardColor.Enemy)
                            notEnemyColorSpaces.Add((r, c));
                (int row, int col) targetSpace = Extensions.ChooseDifferentRandomElements(notEnemyColorSpaces, 1)[0];
                colorTargetSpaces.Add(targetSpace);
                break;
            }

            /// 플레이어의 색칠된 칸 중 랜덤한 1 칸에 적의 색을 칠한다
            case EnemyAction.Color1BlueColored:
            {
                List<(int, int)> playerColorSpaces = new List<(int, int)>();
                for (int r = 0; r < BoardManager.Instance.BoardSize; r++)
                    for (int c = 0; c < BoardManager.Instance.BoardSize; c++)
                        if (BoardManager.Instance.BoardColors[r][c] == BoardColor.Player)
                            playerColorSpaces.Add((r, c));
                (int row, int col) targetSpace = Extensions.ChooseDifferentRandomElements(playerColorSpaces, 1)[0];
                colorTargetSpaces.Add(targetSpace);
                break;
            }

            /// 플레이어의 색칠된 칸 중 랜덤한 2 칸에 적의 색을 칠한다
            case EnemyAction.Color2BlueColored:
            {
                List<(int, int)> playerColorSpaces = new List<(int, int)>();
                for (int r = 0; r < BoardManager.Instance.BoardSize; r++)
                    for (int c = 0; c < BoardManager.Instance.BoardSize; c++)
                        if (BoardManager.Instance.BoardColors[r][c] == BoardColor.Player)
                            playerColorSpaces.Add((r, c));
                List<(int, int)> targetSpaces = Extensions.ChooseDifferentRandomElements(playerColorSpaces, 2);
                colorTargetSpaces.AddRange(targetSpaces);
                break;
            }

            /// 플레이어의 색칠된 칸 모두에 적의 색을 칠한다
            case EnemyAction.ColorAllBlueColored:
            {
                for (int r = 0; r < BoardManager.Instance.BoardSize; r++)
                    for (int c = 0; c < BoardManager.Instance.BoardSize; c++)
                        if (BoardManager.Instance.BoardColors[r][c] == BoardColor.Player)
                            colorTargetSpaces.Add((r, c));
                break;
            }

            /// 랜덤한 가로줄 1줄에 적의 색을 칠한다
            case EnemyAction.ColorH1:
            {
                int row = Random.Range(0, BoardManager.Instance.BoardSize);
                for (int c = 0; c < BoardManager.Instance.BoardSize; c++)
                    colorTargetSpaces.Add((row, c));
                break;
            }

            /// 랜덤한 세로줄 1줄에 적의 색을 칠한다
            case EnemyAction.ColorV1:
            {
                int col = Random.Range(0, BoardManager.Instance.BoardSize);
                for (int r = 0; r < BoardManager.Instance.BoardSize; r++)
                    colorTargetSpaces.Add((r, col));
                break;
            }

            /// 랜덤한 2*2 칸에 적의 색을 칠한다(좌상,좌하,우상,우하 4곳 중 랜덤 1곳)
            case EnemyAction.Color2x2:
                // TODO: 그럼 4x4 칸에서 가운데 또는 변을 칠하는 건 없나?
                break;

            /// 틱택토 판의 4꼭짓점에 적의 색을 칠한다(1,3,7,9번 칸)
            case EnemyAction.ColorCorner:
            {
                int boardSize = BoardManager.Instance.BoardSize;
                colorTargetSpaces.Add((0, 0));
                colorTargetSpaces.Add((0, boardSize - 1));
                colorTargetSpaces.Add((boardSize - 1, 0));
                colorTargetSpaces.Add((boardSize - 1, boardSize - 1));
                break;
            }

            // 틱택토 판의 4변에 적의 색을 칠한다(2,4,6,8번 칸)
            case EnemyAction.ColorSide:
                // TODO: 그럼 4x4 칸에서는?
                break;
        }

        foreach ((int row, int col) space in colorTargetSpaces)
        {
            print($"Enemy Color Target: ({space.row}, {space.col})");
        }
    }
    
    private void EnemyBuff((EnemyAction, int) enemyAction)
    {
        switch (enemyAction.Item1)
        {
            case EnemyAction.PowerIncrease:
                SetDebuff(Debuff.PowerIncrease, enemyAction.Item2);
                break;
            case EnemyAction.DamageDecrease:
                break;
            case EnemyAction.HpHealing:
                EnemyHealHp(enemyAction.Item2);
                break;
            case EnemyAction.ShieldHealing:
                EnemyHealShield(enemyAction.Item2);
                break;
        }
        EnemyUI.BuffDebuffUpdate();
    }
    
    private void EnemyDebuff((EnemyAction, int) enemyAction)
    {
        switch (enemyAction.Item1)
        {
            case EnemyAction.PlayerPowerDecrease:
                PlayerManager.Instance.SetDebuff(Debuff.PowerDecrease, enemyAction.Item2);
                break;
            case EnemyAction.PlayerDamageIncrease:
                PlayerManager.Instance.SetDebuff(Debuff.DamageIncrease, enemyAction.Item2);
                break;

            /// 드로우 수 1 감소
            case EnemyAction.DrawCardDecrease:
                PlayerManager.Instance.SetDebuff(Debuff.DrawCardDecrease, enemyAction.Item2);
                break;

            /// 다음 턴 이동 카드를 사용할 수 없음
            case EnemyAction.BanMoveCard:
                break;

            /// 다음 턴 공격 카드를 사용할 수 없음
            case EnemyAction.BanAttackCard:
                break;

            /// 다음 턴 색칠 카드를 사용할 수 없음
            case EnemyAction.BanColorCard:
                break;

            /// 다음 턴 시작 마나가 1 감소한다
            case EnemyAction.ManaReave:
                break;

            /// 다음 턴 이동 카드의 소비 마나가 1 증가한다
            case EnemyAction.MoveCardCostIncrease1:
                break;

            /// 다음 턴 공격 카드의 소비 마나가 1 증가한다
            case EnemyAction.AttackCardCostIncrease1:
                break;

            /// 다음 턴 색칠 카드의 소비 마나가 1 증가한다
            case EnemyAction.ColorCardCostIncrease1:
                break;

            //case EnemyAction.CardCostIncrease:
            //    PlayerManager.Instance.SetDebuff(Debuff.CardCostIncrease, enemyAction.Item2);
            //    break;
        }
        EnemyUI.BuffDebuffUpdate();
    }

    public void SetDebuff(Debuff debuff, int value)
    {
        Debug.Log("SetDebuff");
        if(_debuffDictionary[debuff] + value < 0)
            _debuffDictionary[debuff] = 0;
        else
            _debuffDictionary[debuff] += value;
        if(value >0){
            if(debuff == Debuff.PowerDecrease ){
                transform.Find("DebuffEffect").gameObject.SetActive(true);
                SoundManager.Instance.PlaySE("Debuff", 1.0f);
            }
            else if(debuff == Debuff.Heal){
                transform.Find("HealEffect").gameObject.SetActive(true);
                SoundManager.Instance.PlaySE("HealShield", 0.5f);
            }
            else{
                transform.Find("BuffEffect").gameObject.SetActive(true);
                SoundManager.Instance.PlaySE("Buff", 0.5f);
            }
        }
        EnemyUI.BuffDebuffUpdate();
    }

    public void setPreviousPos(int row, int col){
        _previousPlayerRow = row;
        _previousPlayerCol = col;
    }

    public void EnemyHealHp(int num)
    {
        Debug.Log("EnemyHPHeal!");
        SoundManager.Instance.PlaySE("HealShield");
        _enemyHp = _enemyHp + num > _enemyMaxHp ? _enemyMaxHp : _enemyHp + num;
        EnemyUI.ShieldUIUpdate();
    }

    public void EnemyHealShield(int num)
    {
        Debug.Log("EnemyShieldHeal!");
        SoundManager.Instance.PlaySE("HealHP");
        _enemyShield = _enemyShield + num > _enemyMaxShield ? _enemyMaxShield : _enemyShield + num;
        EnemyUI.ShieldUIUpdate();
    }

    public void EnemyOutlineEffect()
    {
        spriteOutlineMaterial.DOColor(spriteOutlineColor, 0.5f).SetLoops(-1, LoopType.Yoyo);
        cardboardOutlineMaterial.DOColor(spriteOutlineColor, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopEnemyOutlineEffect()
    {
        spriteOutlineMaterial.DOKill();
        cardboardOutlineMaterial.DOKill();
        InitEnemyOutline();
    }

    public void InitEnemyOutline()
    {
        spriteOutlineMaterial.SetColor("_Color", originalSpriteOutlineColor);
        cardboardOutlineMaterial.SetColor("_Color", originalSpriteOutlineColor);
        Debug.Log("init");
    }

    void Start()
    {
        InitEnemyOutline();
    }
}