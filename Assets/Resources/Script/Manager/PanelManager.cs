using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelManager : Singleton<PanelManager>
{
    [SerializeField] private GameObject gameOverPanel;
    public GameObject GameOverPanel => gameOverPanel;
    [SerializeField] private GameObject gameClearPanel;
    public GameObject GameClearPanel => gameClearPanel;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button exitGameButton;
    public GameObject DirectionNotice;
    private string MoveNotice = "이동할 위치를 선택하세요";
    private string AttackNotice = "공격할 대상을 선택하세요";
    private string ColorNotice = "색칠할 위치를 선택하세요";

    public void SetDirectionNotice(States state){
        switch(state){
            case States.Attack:
                DirectionNotice.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = AttackNotice;
                break;
            case States.Move:
                DirectionNotice.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = MoveNotice;
                break;
            case States.Color:
                DirectionNotice.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ColorNotice;
                break;
        }
        return;
    }

    void Start()
    {
        nextButton.onClick.AddListener(LoadingManager.Instance.LoadWorldMap);
        mainMenuButton.onClick.AddListener(LoadingManager.Instance.LoadTitleScreen);
        exitGameButton.onClick.AddListener(Application.Quit);
    }
}
