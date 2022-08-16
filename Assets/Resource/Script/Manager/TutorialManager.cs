using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    void Start()
    {
        DialogueManager.Instance.StartDialogue("Tutorial/Dialogue1");
    }

    public void toNextTutorial(int idx)
    {
        switch (idx)
        {
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                CardManager.Instance.DrawCard(1);
                break;
            case 6:
                break;
        }
        
        DialogueManager.Instance.StartDialogue(string.Format("Tutorial/Dialogue{0}", idx));
    }
}