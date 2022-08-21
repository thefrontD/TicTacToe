using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EPOOutline;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Player의 데이터를 저장하는 부분으로 Player와 관련된 데이터, 메소드는 해당 Manager에 작성 바람
/// States 역시 Player에 종속되는 부분으로 판정함
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
    private PlayerDataHolder _holder;
    
    private int _maxHp;
    public int MaxHp { get => _maxHp; set => _maxHp = (value >= 0) ? value : 0; }
    private int _hp = 3;
    public int Hp => _hp;

    private int _maxMana;
    public int MaxMana => _maxMana;
    private int _mana = 10;
    public int Mana => _mana;
    
    private int _shield;
    public int Shield => _shield;

    private int row;
    public int Row { get => row; set => row = value; }
    
    private int col;
    public int Col { get => col; set => col = value; }
    
    private int _nextTurnDrawNum = 5;
    public int NextTrunDrawNum => _nextTurnDrawNum;

    private Dictionary<Debuff,  int> _debuffDictionary;
    public Dictionary<Debuff, int> DebuffDictionary => _debuffDictionary;

    private bool _bingoAttack = false;
    public bool BingoAttack { get => _bingoAttack; set => _bingoAttack = value; }

    private bool _tutorialTrigger = false;
    public bool TutorialTrigger => _tutorialTrigger;
    
    private int _tutorialPhase = 0;
    public int TutorialPhase { get => _tutorialPhase; set => _tutorialPhase = value; }
    private int _tutorialSubPhase = 0;
    public int TutorialSubPhase { get => _tutorialSubPhase; set => _tutorialSubPhase = value; }

    private bool _clickable = true;
    
    public bool tutorial4Trigger = false;
    public event Action OnPlayerDataUpdate;

    public BaseState state;
    public Queue<BaseState> StatesQueue;
    public List<Card> PlayerCard;
    
    void Start()
    {
        if (GameManager.Instance.CurrentStage == 101)
        {
            _tutorialPhase = 1;
            _tutorialTrigger = true;
        }
        
        state = new NormalState();
        if(!_tutorialTrigger)
            DialogueManager.Instance.StartDialogue(string.Format("Enemy{0}", GameManager.Instance.CurrentStage%100));
        else
        {
            DialogueManager.Instance.dialogueCallBack.DialogueCallBack += NextTutorialNum;
            DialogueManager.Instance.StartDialogue(string.Format("Tutorial/Tutorial_{0}", _tutorialPhase));
        }
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && _clickable)
        {
            SoundManager.Instance.PlaySE("Click", 0.5f);
            state.MouseEvent();
        }
    }

    void FixedUpdate()
    {
        state.Update();
    }
    
    private void Init()
    {
        int floor = GameManager.Instance.CurrentStage/100;
        int stage = GameManager.Instance.CurrentStage%100;
        string stageData = floor.ToString() + "_" + stage.ToString();

        if (stage == 1)
            _tutorialTrigger = true;
        else
            _tutorialTrigger = false;
        
        BoardManager.Instance.BoardLoading(stageData);
        PlayerLoading();
        EnemyManager.Instance.EnemyLoading(stageData);
    }
    
    public void Init(object sender, EventArgs arg)
    {
        int floor = GameManager.Instance.CurrentStage/100;
        int stage = GameManager.Instance.CurrentStage%100;
        string stageData = floor.ToString() + "_" + stage.ToString();

        BoardManager.Instance.BoardLoading(stageData);
        PlayerLoading();
        EnemyManager.Instance.EnemyLoading(stageData);
    }

    public void PlayerLoading()
    {
        _debuffDictionary = new Dictionary<Debuff, int>();
        InitDebuffDictionary();
        
        if (_tutorialTrigger)
            PlayerCard = CardData.Instance._load("TutorialCard");
        else
            PlayerCard = CardData.Instance._load(string.Format("PlayerCard{0}", GameManager.Instance.PlayerNum));
        
        _holder = PlayerData.Instance._load(string.Format("PlayerData{0}", GameManager.Instance.PlayerNum));
        //GameManager.Instance.CurrentStage = _holder.CurrentStage;
        _maxHp = _holder.MaxHp;
        _hp = _holder.Hp;
        _maxMana = _holder.MaxMana;
        _mana = _holder.Mana;

        if (!_tutorialTrigger)
            PlayerCard.Shuffle();

        CardManager.Instance.SetUp();

        SetMana();
        DamageToPlayer();
        
        StatesQueue = new Queue<BaseState>();
        if (_tutorialTrigger)
            state = new NormalState(3, true);
        else
            state = new NormalState(5, true);
        state.Enter();
    }

    public void SavePlayerData()
    {
        PlayerData.Instance.saveData(new PlayerDataHolder(GameManager.Instance.CurrentStage,
            _maxHp, _hp, _maxMana, _mana), string.Format("PlayerData{0}", GameManager.Instance.PlayerNum));
        CardData.Instance.saveData(PlayerCard, string.Format("PlayerCard{0}", GameManager.Instance.PlayerNum));
    }

    private void InitDebuffDictionary()
    {
        foreach(Debuff debuff in Enum.GetValues(typeof(Debuff)))
            _debuffDictionary[debuff] = 0;
    } 

    public void SetDebuff(Debuff debuff, int value)
    {
        if(_debuffDictionary[debuff] + value < 0)
            _debuffDictionary[debuff] = 0;
        else
            _debuffDictionary[debuff] += value;
        
        OnPlayerDataUpdate?.Invoke();
    }
    
    public void ChangeStates(BaseState newState)
    {
        StartCoroutine(ChangeStatesCoroutine(newState));
    }
    
    private IEnumerator ChangeStatesCoroutine(BaseState newState)
    {
        Debug.Log(state);
        state.Exit();
        yield return new WaitForSeconds(0.5f);
        state = newState;
        Debug.Log(state);
        state.Enter();
    }

    /// <summary>
    /// 현재 마나를 value만큼 더한다. 마나가 0보다 낮아지면 false를 반환한다.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cardType"></param>
    /// <returns></returns>
    public bool SetMana(int value = 0, CardType cardType = CardType.None, bool ignoreDebuff = false)
    {
        if (!ignoreDebuff)
        {
            if (_debuffDictionary[Debuff.CardCostIncrease] != 0) value--;
            else
            {
                switch (cardType)
                {
                    case CardType.Attack:
                        if (_debuffDictionary[Debuff.AttackCardCostIncrease] != 0) value--;
                        break;
                    case CardType.Move:
                        if (_debuffDictionary[Debuff.MoveCardCostIncrease] != 0) value--;
                        break;
                    case CardType.Color:
                        if (_debuffDictionary[Debuff.ColorCardCostIncrease] != 0) value--;
                        break;
                }
            }
        }

        if (_mana + value < 0) return false;
        else if(_mana + value > MaxMana) _mana = MaxMana;
        else _mana += value;

        OnPlayerDataUpdate?.Invoke();
        return true;
    }
    
    /// <summary>
    /// 현재 체력을 value만큼 더한다. 체력이 0 이하가 되면 false를 반환한다.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool SetHp(int value = 0)
    {
        // TODO: 피해 or 회복하는 이펙트?
        if (_hp + value <= 0) return false;
        else if(_hp + value > MaxHp) _hp = MaxHp;
        else _hp += value;
        OnPlayerDataUpdate?.Invoke();
        return true;
    }
    
    public bool DamageToPlayer(int value = 0)
    {
        if(_shield != 0){
            if(_shield + value < 0)
            {
                value += _shield;
                if (_hp + value <= 0)
                {
                    _hp = 0;
                    GameManager.Instance.GameOver();
                    return true;
                }
                else if(_hp + value > MaxHp) _hp = MaxHp;
                else _hp += value;
            }
            else _shield += value;
        }
        else
        {
            if (_hp + value <= 0)
            {
                _hp = 0;
                GameManager.Instance.GameOver();
                return true;
            }
            else if(_hp + value > MaxHp) _hp = MaxHp;
            else _hp += value;
        }

        OnPlayerDataUpdate?.Invoke();
        return false;
    }

    public bool MovePlayer(int row, int col, MoveCardEffect effect = MoveCardEffect.Slide)
    {
        Debug.Log(string.Format("{0} : {1}", row, col));
        return BoardManager.Instance.MovePlayer(row, col, effect);
    }

    public void ToEnemyTurn()
    {
        foreach (CardUI card in CardManager.Instance.HandCardList)
            card.gameObject.GetComponent<Outlinable>().enabled = false;
        CardManager.Instance.AllHandCardtoGrave();
        SoundManager.Instance.PlaySE("TurnOver");
        ChangeStates(new EnemyState());
    }

    public void NextTutorialNum(object sender, EventArgs e)
    {
        _tutorialPhase++;
    }
}
