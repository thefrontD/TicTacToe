using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class UpperUI : MonoBehaviour
{
    public int clearedStage = 0;
    public GameObject SettingPanel;
    public GameObject CardListPanel;
    public GameObject StageIdentifier;
    public GameObject PlayerProfile;
    // Start is called before the first frame update
    private RectTransform _rectTransform;
    private bool _cardListOnOpen = false;
    private bool _cardListOnWorking;
    void Start()
    {
        _rectTransform = CardListPanel.GetComponent<RectTransform>();
        clearedStage = (GameManager.Instance.CurrentStage % 100) - 1;
        
        //stage 표기 변경
        StageIdentifier.GetComponent<TextMeshProUGUI>().text = "Stage 1-"+(clearedStage+1).ToString();
        //PlayerProfile 표기 변경
        PlayerProfile.GetComponent<TextMeshProUGUI>().text ="처치한 적 수: "+ clearedStage.ToString() 
                                                             + "\n현재 스테이지: " +(clearedStage+1).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleCardListPanel()
    {
        ShowCardList(PlayerManager.Instance.PlayerCard);
    }

    public void ToggleDeckCardList()
    {
        //ShowCardList(CardManager.Instance.DeckList);
    }

    public void ToggleGraveCardList()
    {
        //ShowCardList(CardManager.Instance.GraveList);
    }

    private void ShowCardList(List<Card> CardList)
    {
        if (CardListPanel.activeSelf)
        {
            CardListPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, -1055f), 1f)
                .SetEase(Ease.InQuad)
                .OnComplete(() => { CardListPanel.GetComponent<CardListPanel>().DeleteCard();
                    CardListPanel.SetActive(false); });
        }
        else
        {
            CardListPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, -25f), 1f)
                .SetEase(Ease.OutQuad)
                .OnStart(() => { CardListPanel.GetComponent<CardListPanel>().PrintCard(CardList);
                    CardListPanel.SetActive(true); });
        }
    }

    public void ToggleSettingPanel(){
        if(SettingPanel.activeSelf)
            SettingPanel.SetActive(false);
        else
            SettingPanel.SetActive(true);
    }
}
