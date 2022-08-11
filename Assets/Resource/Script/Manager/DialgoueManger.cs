using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class DialogueEndCallBack : EventArgs
{
    public DialogueEndCallBack()
    {
    }
}

public class DialogueEndEvent
{
    public event EventHandler DialogueCallBack;

    public void Run()
    {
        if (DialogueCallBack != null)
        {
            DialogueCallBack(this, new DialogueEndCallBack());
        }
    }
}

public class DialogueManager : Singleton<DialogueManager>
{
    private GameObject _dialogueCanvas;
    private DialogueUI _dialogueUI;
    public DialogueEndEvent dialogueCallBack;

    public DialogueManager()
    {
        _dialogueCanvas = GameObject.FindWithTag("DialogueCanvas");
        _dialogueUI = _dialogueCanvas.GetComponent<DialogueUI>();
        dialogueCallBack = new DialogueEndEvent();
        _dialogueCanvas.SetActive(false);
    }

    public void StartDialogue(string dialogueIdx)
    {
        _dialogueCanvas.SetActive(true);
        _dialogueUI.StartDialogue(dialogueIdx);
    }
}