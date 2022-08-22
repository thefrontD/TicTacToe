using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UpperUI : MonoBehaviour
{
    public int clearedStage = 0;
    public GameObject SettingPanel;
    public GameObject CardListPanel;
    public GameObject StageIdentifier;
    public GameObject PlayerProfile;
    // Start is called before the first frame update
    void Start()
    {
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

    public void ToggleCardListPanel(){
        if(CardListPanel.activeSelf)
            CardListPanel.SetActive(false);
        else
            CardListPanel.SetActive(true);
    }

    public void ToggleSettingPanel(){
        if(SettingPanel.activeSelf)
            SettingPanel.SetActive(false);
        else
            SettingPanel.SetActive(true);
    }
}
