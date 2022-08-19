using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueManager : Singleton<DialogueManager>
{
    private GameObject _dialogueCanvas;
    private DialogueUI _dialogueUI;
    public DialogueEndEvent dialogueCallBack;

    public void Awake()
    {
        dialogueCallBack = new DialogueEndEvent();
        _dialogueCanvas = GameObject.FindGameObjectWithTag("DialogueCanvas");

        Debug.Log(_dialogueCanvas);

        _dialogueUI = _dialogueCanvas.GetComponent<DialogueUI>();
        if(SceneManager.GetActiveScene().name == "BattleScene")
            dialogueCallBack.DialogueCallBack += PlayerManager.Instance.Init;
        _dialogueCanvas.SetActive(false);
    }

    public void StartDialogue(string dialogueIdx)
    {
        _dialogueCanvas.SetActive(true);
        _dialogueUI.StartDialogue(dialogueIdx);
    }
}