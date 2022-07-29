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
    [SerializeField] private int hp = 3;
    public int Hp { get => hp; set => hp = (value >= 0) ? value : 0; }

    [SerializeField] private int mana = 10;
    public int Mana { get => mana; set => mana = (value >= 0) ? value : 0; }
    
    private int row;
    public int Row { get => row; set => row = value; }
    
    private int col;
    public int Col { get => col; set => col = value; }
    
    private int _nextTurnDrawNum = 5;
    public int NextTrunDrawNum => _nextTurnDrawNum;

    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI manaText;
    private Dictionary<Debuff, (int, int)> _debuffDictionary;

    public BaseState state;
    public Queue<BaseState> StatesQueue;
    public List<Card> PlayerCard;

    void Start()
    {
        //List<States> statesList = new List<States>(){States.Color};
        //foreach (Card card in PlayerManager.Instance.PlayerCard)
        //{
        //    ColorState state1 = ((ColorCard)card).ColorState();
        //}
        PlayerCard = CardData.Instance._load("PlayerCard.json");
        PlayerCard.Shuffle();

        CardManager.Instance.SetUp();

        SetMana();
        SetHp();

        _debuffDictionary = new Dictionary<Debuff, (int, int)>();
        
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
        _debuffDictionary[Debuff.DamageIncrease] = (0, 0);
        _debuffDictionary[Debuff.PowerDecrease] = (0, 0);
        _debuffDictionary[Debuff.CardCostIncrease] = (0, 0);
        _debuffDictionary[Debuff.DrawCardDecrease] = (0, 0);
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

    public bool SetMana(int value = 0)
    {
        if (Mana + value < 0) return false;
        else Mana += value;

        manaText.text = String.Format("Mana : {0}", Mana);
        return true;
    }
    
    public bool SetHp(int value = 0)
    {
        if (Hp + value < 0) return false;
        else Hp += value;

        hpText.text = String.Format("HP : {0}", Hp);
        return true;
    }

    public bool MovePlayer(int x, int y)
    {
        if (BoardManager.Instance.MovePlayer(x, y))
            return true;
        else
            return false;
    }

    public void ToEnemyTurn()
    {
        CardManager.Instance.AllHandCardtoGrave();
        ChangeStates(new EnemyState());
    }
}
