using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;

public class SceneManager : Singleton<SceneManager>
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

    private void Awake()
    {
        if (SceneManager.Instance != this)
            Destroy(gameObject);

        canvas = GetComponent<Canvas>();

        loadingScreen = transform.GetChild(0).gameObject;
        loadingAnimator = loadingScreen.GetComponent<Animator>();

        loadingScreen.SetActive(false);

        DontDestroyOnLoad(this.gameObject);
    }

    private int[] values;
    
    
    public void LoadTitleScreen(){
        LoadNextScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private bool LoadNextScene(int buildIndex)
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
