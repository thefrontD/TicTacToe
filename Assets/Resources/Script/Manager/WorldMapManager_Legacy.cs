using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.UI;
using TMPro;

/*
public timeToMove: 캐릭터가 이동하는데 걸리는 시간
clearedStage를 변화시키면, 캐릭터가 몇번째 스테이지에서 다음 스테이지로 이동하는 애니메이션을 보여줄 지 변경 가능
stage를 나타내는 바의 Cylinder을 기준으로 움직이도록 만들었기 때문에 Cylinder위치를 수정해도 원하는대로 작동
*/
//setting temp image 집어넣어둠 적용 어떻게 하는지 찾아볼것
/*public class WorldMapManager : Singleton<WorldMapManager>
{
    public GameObject PlayerPrefab;
    public float timeToMove = 2;
    public int clearedStage = 0;
    public GameObject[] stages = new GameObject[11];
    public GameObject SettingPanel;
    public GameObject CardListPanel;
    public GameObject StageIdentifier;
    public GameObject PlayerProfile;
    private GameObject Player;
    
    public void Start()
    {
        Screen.SetResolution(1920, 1080, true);

        if (GameManager.Instance.CurrentStage == 101)
        {
            DialogueManager.Instance.dialogueCallBack.DialogueCallBack += Init;
            DialogueManager.Instance.StartDialogue("Prologue");
        }
        else
        {
            Init(this, EventArgs.Empty);
        }
    }

    public void Init(object sender, EventArgs e)
    {
        clearedStage = (GameManager.Instance.CurrentStage % 100) - 1;
        Vector3 spawnPoint = stages[clearedStage].GetComponent<Transform>().position;
        Vector3 nextPoint = stages[clearedStage + 1].GetComponent<Transform>().position;
        spawnPoint += new Vector3(0,0.5f,0);
        nextPoint += new Vector3(0,0.5f,0);
        Player = Instantiate(PlayerPrefab, spawnPoint, Quaternion.Euler(0, 105, 0));
        Sequence mySequence = DOTween.Sequence();
        mySequence.PrependInterval(1);
        mySequence.Append(Player.GetComponent<Transform>().DOJump(nextPoint, 1, 1, 2, false));
        SoundManager.Instance.PlaySE("WorldMapMove");
        mySequence.InsertCallback(5, LoadingManager.Instance.LoadBattleScene);
        
        //stage 표기 변경
        // StageIdentifier.GetComponent<TextMeshProUGUI>().text = "Stage 1-"+(clearedStage+1).ToString();
        // //PlayerProfile 표기 변경
        // PlayerProfile.GetComponent<TextMeshProUGUI>().text ="처치한 적 수: "+ clearedStage.ToString() 
        //                                                      + "\n현재 스테이지: " +(clearedStage+1).ToString();
    }

    void Update()
    {
        
    }

    // public void ToggleCardListPanel(){
    //     if(CardListPanel.activeSelf)
    //         CardListPanel.SetActive(false);
    //     else
    //         CardListPanel.SetActive(true);
    // }

    // public void ToggleSettingPanel(){
    //     if(SettingPanel.activeSelf)
    //         SettingPanel.SetActive(false);
    //     else
    //         SettingPanel.SetActive(true);
    // }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.UI;
using TMPro;

/*
public timeToMove: 캐릭터가 이동하는데 걸리는 시간
clearedStage를 변화시키면, 캐릭터가 몇번째 스테이지에서 다음 스테이지로 이동하는 애니메이션을 보여줄 지 변경 가능
stage를 나타내는 바의 Cylinder을 기준으로 움직이도록 만들었기 때문에 Cylinder위치를 수정해도 원하는대로 작동
*/
//setting temp image 집어넣어둠 적용 어떻게 하는지 찾아볼것
/*public class WorldMapManager : Singleton<WorldMapManager>
{
    public GameObject PlayerPrefab;
    public float timeToMove = 2;
    public int clearedStage = 0;
    public GameObject[] stages = new GameObject[11];
    public GameObject SettingPanel;
    public GameObject CardListPanel;
    public GameObject StageIdentifier;
    public GameObject PlayerProfile;
    private GameObject Player;
    
    public void Start()
    {
        Screen.SetResolution(1920, 1080, true);

        if (GameManager.Instance.CurrentStage == 101)
        {
            DialogueManager.Instance.dialogueCallBack.DialogueCallBack += Init;
            DialogueManager.Instance.StartDialogue("Prologue");
        }
        else
        {
            Init(this, EventArgs.Empty);
        }
    }

    public void Init(object sender, EventArgs e)
    {
        clearedStage = (GameManager.Instance.CurrentStage % 100) - 1;
        Vector3 spawnPoint = stages[clearedStage].GetComponent<Transform>().position;
        Vector3 nextPoint = stages[clearedStage + 1].GetComponent<Transform>().position;
        spawnPoint += new Vector3(0,0.5f,0);
        nextPoint += new Vector3(0,0.5f,0);
        Player = Instantiate(PlayerPrefab, spawnPoint, Quaternion.Euler(0, 105, 0));
        Sequence mySequence = DOTween.Sequence();
        mySequence.PrependInterval(1);
        mySequence.Append(Player.GetComponent<Transform>().DOJump(nextPoint, 1, 1, 2, false));
        SoundManager.Instance.PlaySE("WorldMapMove");
        mySequence.InsertCallback(5, LoadingManager.Instance.LoadBattleScene);
        
        //stage 표기 변경
        // StageIdentifier.GetComponent<TextMeshProUGUI>().text = "Stage 1-"+(clearedStage+1).ToString();
        // //PlayerProfile 표기 변경
        // PlayerProfile.GetComponent<TextMeshProUGUI>().text ="처치한 적 수: "+ clearedStage.ToString() 
        //                                                      + "\n현재 스테이지: " +(clearedStage+1).ToString();
    }

    void Update()
    {
        
    }

    // public void ToggleCardListPanel(){
    //     if(CardListPanel.activeSelf)
    //         CardListPanel.SetActive(false);
    //     else
    //         CardListPanel.SetActive(true);
    // }

    // public void ToggleSettingPanel(){
    //     if(SettingPanel.activeSelf)
    //         SettingPanel.SetActive(false);
    //     else
    //         SettingPanel.SetActive(true);
    // }
}*/
