using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Player의 데이터를 저장하는 부분으로 Player와 관련된 데이터, 메소드는 해당 Manager에 작성 바람
/// States 역시 Player에 종속되는 부분으로 판정함
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private int maxHp = 3;
    public int MaxHp { get => maxHp; set => maxHp = (value >= 0) ? value : 0; }
    private int _hp = 3;
    public int Hp => _hp;

    [SerializeField] private int maxMana = 10;
    public int MaxMana => maxMana;
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

    [SerializeField] private TextMeshProUGUI shieldText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI manaText;

    public BaseState state;
    public Queue<BaseState> StatesQueue;
    public List<Card> PlayerCard;

    void Awake(){
        PlayerCard = CardData.Instance._load("PlayerCard.json");
    }

    void Start()
    {
        //List<States> statesList = new List<States>(){States.Color};
        //foreach (Card card in PlayerManager.Instance.PlayerCard)
        //{
        //    ColorState state1 = ((ColorCard)card).ColorState();
        //}
        PlayerCard.Shuffle();

        CardManager.Instance.SetUp();

        _debuffDictionary = new Dictionary<Debuff, int>();
        InitDebuffDictionary();

        SetMana();
        DamageToPlayer();
        
        StatesQueue = new Queue<BaseState>();
        state = new NormalState(5, true);
        state.Enter();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
            state.MouseEvent();
    }

    void FixedUpdate()
    {
        state.Update();
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
    }
    
    public void ChangeStates(BaseState newState)
    {
        StartCoroutine(ChangeStatesCoroutine(newState));
    }
    
    public IEnumerator ChangeStatesCoroutine(BaseState newState)
    {
        Debug.Log(state);
        state.Exit();
        yield return new WaitForSeconds(0.5f);
        state = newState;
        Debug.Log(state);
        state.Enter();
    }

    public bool SetMana(int value = 0, CardType cardType = CardType.None)
    {
        if(_debuffDictionary[Debuff.CardCostIncrease] != 0) value--;
        else{
            switch(cardType){
                case CardType.Attack:
                    if(_debuffDictionary[Debuff.AttackCardCostIncrease] != 0) value--;
                    break;
                case CardType.Move:
                    if(_debuffDictionary[Debuff.MoveCardCostIncrease] != 0) value--;
                    break;
                case CardType.Color:
                    if(_debuffDictionary[Debuff.ColorCardCostIncrease] != 0) value--;
                    break;
            }
        }

        if (_mana + value < 0) return false;
        else if(_mana + value > MaxMana) _mana = MaxMana;
        else _mana += value;

        manaText.text = String.Format("Mana : {0}", _mana);
        return true;
    }
    
    public bool SetHp(int value = 0)
    {
        if (_hp + value < 0) return false;
        else if(_hp + value > MaxHp) _hp = MaxHp;
        else _hp += value;
        hpText.text = String.Format("HP : {0}", _hp);
        return true;
    }
    
    public bool DamageToPlayer(int value = 0)
    {
        if(_shield != 0){
            if(_shield + value < 0)
            {
                value += _shield;
                if (_hp + value < 0) return false;
                else if(_hp + value > MaxHp) _hp = MaxHp;
                else _hp += value;
            }
            else _shield += value;
        }

        shieldText.text = String.Format("Shield : {0}", _shield);
        hpText.text = String.Format("HP : {0}", _hp);
        return true;
    }

    public bool MovePlayer(int row, int col)
    {
        return BoardManager.Instance.MovePlayer(row, col);
    }

    public void ToEnemyTurn()
    {
        CardManager.Instance.AllHandCardtoGrave();
        ChangeStates(new EnemyState());
    }
}
