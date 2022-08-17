using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class MouseOverUIManager : Singleton<MouseOverUIManager>
{
    [SerializeField] private GameObject MouseOverUIPanel;
    [SerializeField] private TextMeshProUGUI MouseOverText;
    [Serializable]
    struct UITypeText
    {
        public MouseOverUIType Type;
        public string Text;
    }
    [SerializeField] private UITypeText[] UITypeTexts;
    public void DisplayUI(MouseOverUIType UIType)
    {
        Debug.Log(UITypeToText(UIType));
        MouseOverUIPanel.SetActive(true);
        MouseOverText.text = UITypeToText(UIType);

    }
    public void EraseUI()
    {
        MouseOverUIPanel.SetActive(false);
    }
    private string UITypeToText(MouseOverUIType UIType)
    {
        for (int i = 0; i < UITypeTexts.Length; i++)
        {
            if(UIType == UITypeTexts[i].Type)
            {
                return UITypeTexts[i].Text;
            }
        }
        return "UITypeÀÌ ¾ø´Ù(error)";
    }
}
