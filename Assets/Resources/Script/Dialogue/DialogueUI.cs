using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public SingleDialogue SingleDialogue => _dialogue;

    private bool _typingEnd;
    private bool _nextTyping;
    private bool _isAuto;
    private DialogueHolder _dialogueHolder;
    private SingleDialogue _dialogue;
    private List<GameObject> _totalPortrait;
    [SerializeField] private Color unHighlightedColor;

    [SerializeField] private GameObject normalUICanvas;

    //[SerializeField] private GameObject portraitPrefabs;
    [SerializeField] private Image rightPersonImage;
    [SerializeField] private Image leftPersonImage;
    [SerializeField] private Image dialogueWindow;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private StringSpriteDictionary portraitDictionary;

    private void Start()
    {
        _typingEnd = false;
        _nextTyping = false;
        _isAuto = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Yes!");
            EndTyping();
        }
    }

    public void StartDialogue(string dialogueIdx)
    {
        StartCoroutine(DialogueAnimaiton(dialogueIdx));
    }

    private void LoadDialogue(string dialogueIdx)
    {
        string path = Application.streamingAssetsPath;
        path += $"/Data/DialogueScripts/{dialogueIdx}.json";

        var pDataStringLoad = File.ReadAllText(path);
        Debug.Log(pDataStringLoad);
        _dialogueHolder = JsonConvert.DeserializeObject<DialogueHolder>(pDataStringLoad);
    }

    private bool GetNextContext()
    {
        if (!_dialogueHolder.CheckDialogueFin())
        {
            _dialogue = _dialogueHolder.GetNextDialogue();
            return true;
        }
        else
            return false;
    }

    private void SetPortrait(string name, string pos)
    {
        switch (pos)
        {
            case "Left":
                leftPersonImage.gameObject.SetActive(true);
                leftPersonImage.sprite = portraitDictionary[name];
                break;
            case "Right":
                rightPersonImage.gameObject.SetActive(true);
                rightPersonImage.sprite = portraitDictionary[name];
                break;
        }
    }

    private void SetPortrait()
    {
        switch (_dialogue.PortraitLocation)
        {
            case "Left":
                leftPersonImage.gameObject.SetActive(true);
                leftPersonImage.sprite = portraitDictionary[_dialogue.Name];
                leftPersonImage.color = Color.white;
                if(rightPersonImage.gameObject.activeSelf)
                    rightPersonImage.color = unHighlightedColor;
                break;
            case "Right":
                rightPersonImage.gameObject.SetActive(true);
                rightPersonImage.sprite = portraitDictionary[_dialogue.Name];
                rightPersonImage.color = Color.white;
                if(leftPersonImage.gameObject.activeSelf)
                    leftPersonImage.color = unHighlightedColor;
                break;
        }
    }

    private void InitPortrait()
    {
        leftPersonImage.gameObject.SetActive(false);
        rightPersonImage.gameObject.SetActive(false);

        for (int i = 0; i < _dialogueHolder.InitialSetting.Count / 2; i++)
        {
            SetPortrait(_dialogueHolder.InitialSetting[2 * i], _dialogueHolder.InitialSetting[2 * i + 1]);
        }
    }

    private void SetName()
    {
        nameText.text = _dialogue.Name;
    }

    private IEnumerator DialogueAnimaiton(string dialogueIdx)
    {
        normalUICanvas.SetActive(false);
        LoadDialogue(dialogueIdx);
        InitPortrait();

        while (GetNextContext())
        {
            _typingEnd = false;
            _nextTyping = false;

            SetName();
            SetPortrait();

            dialogueText.text = "";

            for (int i = 0; i < _dialogue.Context.Length; i++)
            {
                dialogueText.text += _dialogue.Context[i];
                yield return new WaitForSeconds(0.05f);
                if (_typingEnd)
                {
                    dialogueText.text = _dialogue.Context;
                    break;
                }
            }

            _typingEnd = true;

            if (!_isAuto)
            {
                while (!_nextTyping)
                {
                    yield return new WaitForFixedUpdate();
                }

                yield return new WaitForSeconds(0.2f);
            }
            else
                yield return new WaitForSeconds(1f);
        }

        DialogueManager.Instance.dialogueCallBack.Run();

        normalUICanvas.SetActive(true);
        gameObject.SetActive(false);
    }

    public void EndTyping()
    {
        if (!_typingEnd)
            _typingEnd = true;
        else
            _nextTyping = true;
    }

    public void SetAuto()
    {
        _isAuto = !_isAuto;
    }
}