using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI manaText;

    void Start()
    {
        PlayerManager.Instance.OnPlayerDataUpdate += UpdateUIText;
        UpdateUIText();
    }
    
    void Update()
    {
        
    }
    
    private void UpdateUIText()
    {
        hpText.text = String.Format("{0}/{1}", PlayerManager.Instance.Hp, PlayerManager.Instance.MaxHp);
        manaText.text = String.Format("{0}/{1}", PlayerManager.Instance.Mana, PlayerManager.Instance.MaxMana);
    }
}
