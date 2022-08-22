using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : Singleton<PanelManager>
{
    [SerializeField] private GameObject gameOverPanel;
    public GameObject GameOverPanel => gameOverPanel;
    [SerializeField] private GameObject gameClearPanel;
    public GameObject GameClearPanel => gameClearPanel;
    [SerializeField] private Button nextButton;

    void Start()
    {
        nextButton.onClick.AddListener(LoadingManager.Instance.LoadWorldMap);
    }
}
