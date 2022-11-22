using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;

public class LoadingManager : Singleton<LoadingManager>
{
    public event Action OnSceneLoaded;

    private const float minimumLoadingTime = 2f;
    private const float loadingStartDuration = 0.7f;

    private int currentLevel = 0;

    private Canvas canvas;
    private Coroutine loadingCoroutine;

    private GameObject loadingScreen;
    private Animator loadingAnimator;

    private Dictionary<string, int> sceneDictionary = new Dictionary<string, int>();

    private bool _lockButton = false;

    private void Awake()
    {
        if (LoadingManager.Instance != this)
            Destroy(gameObject);

        //canvas = GetComponent<Canvas>();

        //loadingScreen = transform.GetChild(0).gameObject;
        //loadingAnimator = loadingScreen.GetComponent<Animator>();

        //loadingScreen.SetActive(false);

        DontDestroyOnLoad(this.gameObject);
    }

    private int[] values;
    
    
    public void StartGame()
    {
        
    }

    public void LoadTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void LoadBattleScene()
    {
        SceneManager.LoadScene("BattleScene");
    }

    public void LoadWorldMap()
    {
        StartCoroutine(SavingSequence("WorldMap"));
    }

    public void LoadWorldMap(bool isStart)
    {
        StartCoroutine(SavingSequence("WorldMap", isStart));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator SavingSequence(string sceneName, bool isStart = false)
    {
        if (!_lockButton)
        {
            _lockButton = true;
            
            if(!isStart)
            {
                Debug.Log("Yes!");
                GameManager.Instance.CurrentStage++;
        
                PlayerManager.Instance.SavePlayerData();
            }
        
            yield return new WaitForSeconds(0.5f);
        
            _lockButton = false;
            
            if(GameManager.Instance.CurrentStage == 111)
                SceneManager.LoadScene("TitleScreen");
            else
                SceneManager.LoadScene("WorldMap");
        }
    }
    
    
    /// <summary>
    /// 나중에 LoadingScreen 개발 이후에 사용할 예정 지금은 예정에 없음
    /// </summary>
    /// <param name="buildIndex"></param>
    /// <returns></returns>
    private bool LoadScene(int buildIndex)
    {
        if (loadingCoroutine != null) return false;

        loadingCoroutine = StartCoroutine(LoadingSequence(buildIndex));
        return true;
    }

    private IEnumerator LoadingSequence(int buildIndex)
    {
        loadingScreen.SetActive(true);
        loadingAnimator.SetBool("Loading", true);
        yield return new WaitForSeconds(loadingStartDuration);


        float timeElapsed = 0f;
        while(timeElapsed < minimumLoadingTime)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        loadingAnimator.SetBool("Loading", false);
        yield return new WaitForSeconds(loadingStartDuration);

        loadingScreen.SetActive(false);
    }
}
