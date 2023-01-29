using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DumpState : BaseState
{
    CardType dumpCardType;
    int dumpNum;
    List<CardUI> dumpableCardUIs = new List<CardUI>();

    public DumpState(CardType cardType, int dumpNum)
    {
        this.dumpCardType = cardType;
        this.dumpNum = dumpNum;
    }

    public override void DoAction(States state)
    {

    }

    public override void Enter()
    {
        // 버릴 수 있는 카드들을 리스트에 저장, 카드 Outline 설정
        foreach (CardUI cardui in CardManager.Instance.HandCardList)
        {
            if (cardui.Card.CardType == dumpCardType)
            {
                dumpableCardUIs.Add(cardui);
                cardui.HightLightCard(true);
            }
        }
        
        //안내문 활성화
        PanelManager.Instance.DirectionNotice.SetActive(true);
        PanelManager.Instance.SetDirectionNotice(States.Dump); 
    }

    public override void Update()
    {
    }

    public override void MouseEvent()
    {
        //안내문 비활성화
        PanelManager.Instance.DirectionNotice.SetActive(false);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData))
        {
            CardUI cardui = hitData.transform.gameObject.GetComponent<CardUI>();
            if (cardui != null && dumpableCardUIs.Contains(cardui))
            {
                // 카드 클릭 시 카드 버리기
                cardui.HightLightCard(false);
                CardManager.Instance.HandtoGrave(CardManager.Instance.HandCardList.IndexOf(cardui));
                PlayerManager.Instance.EndCurrentState();
            }
        }
    }

    public override void Exit()
    {
        // 카드 Outline 해제
        foreach (CardUI cardui in CardManager.Instance.HandCardList)
            cardui.HightLightCard(false);
        return;
    }
}