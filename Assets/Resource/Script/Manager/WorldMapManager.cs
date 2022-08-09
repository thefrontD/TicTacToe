using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
/*
public timeToMove: 캐릭터가 이동하는데 걸리는 시간
clearedStage를 변화시키면, 캐릭터가 몇번째 스테이지에서 다음 스테이지로 이동하는 애니메이션을 보여줄 지 변경 가능
stage를 나타내는 바의 Cylinder을 기준으로 움직이도록 만들었기 때문에 Cylinder위치를 수정해도 원하는대로 작동
*/
//setting temp image 집어넣어둠 적용 어떻게 하는지 찾아볼것
public class WorldMapManager : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public float timeToMove = 2;
    public int clearedStage = 0;
    public GameObject[] stages = new GameObject[11];
    public GameObject SettingPanel;
    public Text StageIdentifier;
    public Text PlayerProfile;
    public List<Card> PlayerCard;
    private GameObject Player;
    void Start()
    {
        //플레이어 오브젝트 이동 애니메이션
        //stage를 나타내는cylinder을 array로 받아 적용
        Vector3 spawnPoint = stages[clearedStage].GetComponent<Transform>().position;
        Vector3 nextPoint = stages[clearedStage + 1].GetComponent<Transform>().position;
        spawnPoint += new Vector3(0,-2,0);
        nextPoint += new Vector3(0,-2,0);
        Player = Instantiate(PlayerPrefab, spawnPoint, Quaternion.identity);
        Sequence mySequence = DOTween.Sequence();
        mySequence.PrependInterval(1);
        mySequence.Append(Player.GetComponent<Transform>().DOMove(nextPoint, 2));
        
        //stage 표기 변경
        StageIdentifier.GetComponent<Text>().text = "Stage 1-"+(clearedStage+1).ToString();
        //PlayerProfile 표기 변경
        PlayerProfile.GetComponent<Text>().text = "처치한 적 수: "+ clearedStage.ToString() 
                                                + "\n현재 스테이지: " +(clearedStage+1).ToString();
        //todo: 한 스테이지에 처치한 몹 하나라고 가정한 것 나중에 json으로 적용하면 변경 필요

        //CardListUI 세팅
        PlayerCard = CardData.Instance._load("PlayerCard.json");
    }

    void Update()
    {
        
    }

    public void ToggleSettingPanel(){
        if(SettingPanel.activeSelf)
            SettingPanel.SetActive(false);
        else
            SettingPanel.SetActive(true);
    }
}
