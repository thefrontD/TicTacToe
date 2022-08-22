using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : Singleton<PanelManager>
{
    [SerializeField] private GameObject gameOverPanel;
    public GameObject GameOverPanel => gameOverPanel;
    [SerializeField] private GameObject gameClearPanel;
    public GameObject GameClearPanel => gameClearPanel;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
