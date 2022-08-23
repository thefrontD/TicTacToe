using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    public void toNextTutorial(int idx)
    {
        switch (idx)
        {
            case 2:
                DialogueManager.Instance.dialogueCallBack.DialogueCallBack -= PlayerManager.Instance.Init;
                break;
            case 5:
                CardManager.Instance.DrawCard(1);
                break;
            case 7:
                PlayerManager.Instance.SetMana(1000);
                DialogueManager.Instance.dialogueCallBack.DialogueCallBack += CardManager.Instance.Tutorial4;
                break;
            case 8:
                DialogueManager.Instance.dialogueCallBack.DialogueCallBack -= CardManager.Instance.Tutorial4;
                break;
            case 10:
                PlayerManager.Instance.SetMana(1000);
                CardManager.Instance.DrawCard(2);
                break;
            case 13:
                PlayerManager.Instance.SetMana(1000);
                CardManager.Instance.DrawCard(5);
                break;
            case 17:
                PlayerManager.Instance.SetMana(1000);
                CardManager.Instance.DrawCard(4);
                break;
            default:
                break;
        }

        DialogueManager.Instance.StartDialogue(string.Format("Tutorial/Tutorial_{0}", idx));
    }
}