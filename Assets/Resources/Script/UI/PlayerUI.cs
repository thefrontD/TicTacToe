using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI apText;
    [SerializeField] private DebuffGameObjectDictionary _iconPrefabDic;
    [SerializeField] private Transform buffDebuffBar;

    private Dictionary<Debuff, GameObject> _iconObjectDic;

    void Start()
    {
        PlayerManager.Instance.OnPlayerHpManaUpdate += UpdatePlayerHPManaUI;
    }
    
    void Update()
    {
        
    }
    
    public void UpdatePlayerHPManaUI()
    {
        hpText.text = String.Format("{0}/{1}", PlayerManager.Instance.Hp, PlayerManager.Instance.MaxHp);
        manaText.text = String.Format("{0}/{1}", PlayerManager.Instance.Mana, PlayerManager.Instance.MaxMana);
    }

    public void UpdatePlayerBuffUI(Debuff debuff, bool newBuff) {
        if (PlayerManager.Instance.DebuffDictionary[debuff] == 0) {
            if(_iconObjectDic.ContainsKey(debuff)){
                Destroy(_iconObjectDic[debuff]);
                _iconObjectDic.Remove(debuff);
            }
        }
        else{
            if(newBuff){
                GameObject newIcon = Instantiate(_iconPrefabDic[debuff], buffDebuffBar);
                _iconObjectDic.Add(debuff, newIcon);
            }

            _iconObjectDic[debuff].GetComponentInChildren<TextMeshProUGUI>().text = String.Format("{0}", PlayerManager.Instance.DebuffDictionary[Debuff.PowerIncrease]);
        }
    }
    
    public void UpdatePlayerAPUI()
    {
        apText.text = String.Format("Next Attack\n{0}", PlayerManager.Instance.BaseAp + PlayerManager.Instance.Ap);
    }

}