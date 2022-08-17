using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    private GameObject _dialogueCanvas;
    private DialogueUI _dialogueUI;
    public DialogueEndEvent dialogueCallBack;

    public void Start()
    {
        dialogueCallBack = new DialogueEndEvent();
        _dialogueCanvas = GameObject.FindGameObjectWithTag("DialogueCanvas");

        Debug.Log(_dialogueCanvas);

        _dialogueUI = _dialogueCanvas.GetComponent<DialogueUI>();
        dialogueCallBack.DialogueCallBack += PlayerManager.Instance.Init;
        _dialogueCanvas.SetActive(false);

        StartDialogue(string.Format("Enemy{0}", GameManager.Instance.CurrentStage%100));
    }

    public void StartDialogue(string dialogueIdx)
    {
        _dialogueCanvas.SetActive(true);
        _dialogueUI.StartDialogue(dialogueIdx);
    }
}