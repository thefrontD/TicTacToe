using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private GameObject buffIcon;
    [SerializeField] private GameObject debuffIcon;
    [SerializeField] private TextMeshProUGUI buffDebuffText;

    void Start()
    {
        PlayerManager.Instance.OnPlayerDataUpdate += UpdatePlayerUI;
    }
    
    void Update()
    {
        
    }
    
    private void UpdatePlayerUI()
    {
        hpText.text = String.Format("{0}/{1}", PlayerManager.Instance.Hp, PlayerManager.Instance.MaxHp);
        manaText.text = String.Format("{0}/{1}", PlayerManager.Instance.Mana, PlayerManager.Instance.MaxMana);
        if (PlayerManager.Instance.DebuffDictionary[Debuff.PowerIncrease] != 0)
        {
            buffIcon.SetActive(true);
            debuffIcon.SetActive(false);
            buffDebuffText.text = String.Format("{0}", PlayerManager.Instance.DebuffDictionary[Debuff.PowerIncrease]);
        }
        else if (PlayerManager.Instance.DebuffDictionary[Debuff.PowerDecrease] != 0)
        {
            buffIcon.SetActive(false);
            debuffIcon.SetActive(true);
            buffDebuffText.text = String.Format("{0}", PlayerManager.Instance.DebuffDictionary[Debuff.PowerDecrease]);
        }
        else
        {
            buffIcon.SetActive(false);
            debuffIcon.SetActive(false);
            buffDebuffText.text = String.Format("");
        }
    }
}
