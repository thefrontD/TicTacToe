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

    public void DisplayUI(MouseOverUIType UIType, Vector3 pos)
    {
        Debug.Log(UITypeToText(UIType));
        MouseOverUIPanel.SetActive(true);
        MouseOverText.text = UITypeToText(UIType);
        MouseOverUIPanel.transform.position = pos + new Vector3(MouseOverUIPanel.GetComponent<RectTransform>().sizeDelta.x/2 + 40,
        MouseOverUIPanel.GetComponent<RectTransform>().sizeDelta.y/2 + 40, 0) ;
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
        return "UIType�� ����(error)";
    }
}
