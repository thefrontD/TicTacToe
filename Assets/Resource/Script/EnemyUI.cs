using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private GameObject ShieldUI;
    [SerializeField] private GameObject HPUI;
    [SerializeField] private GameObject buffIcon;
    [SerializeField] private GameObject debuffIcon;
    [SerializeField] private TextMeshProUGUI buffDebuffText;
    private Enemy enemy;
    private GameObject[] HP_Containers;
    private GameObject[] HP_Icons;

    private void Awake()
    {
        enemy = this.transform.GetComponentInParent<Enemy>();
    }

    public void InitUI()
    {
        HP_Containers = new GameObject[HPUI.transform.childCount];
        HP_Icons = new GameObject[HPUI.transform.childCount];
        for (int i = 0; i < HP_Containers.Length; i++)
        {
            HP_Containers[i] = HPUI.transform.GetChild(i).gameObject;
            HP_Icons[i] = HP_Containers[i].transform.GetChild(0).gameObject;
            HP_Containers[i].SetActive(false);
            HP_Icons[i].SetActive(false);
        }
        for (int i = 0; i < enemy.EnemyMaxHP; i++)
        {
            HP_Containers[i].SetActive(true);
            HP_Icons[i].SetActive(true);
        }
        HPUIUpdate();
        ShieldUIUpdate();
    }

    public void HPUIUpdate()
    {
        for (int i = 0; i < enemy.EnemyMaxHP; i++)
        {
            HP_Icons[i].SetActive(false);
        }
        for (int i = 0; i < enemy.EnemyHP; i++)
        {
            HP_Icons[i].SetActive(true);
        }
    }

    public void ShieldUIUpdate()
    {
        ShieldUI.GetComponent<Slider>().value = (float)enemy.EnemyShield / (float)enemy.EnemyMaxShield;
        ShieldUI.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = enemy.EnemyShield.ToString() + "/" + enemy.EnemyMaxShield.ToString();
    }

    public void BuffDebuffUpdate()
    {
        if (enemy.DebuffDictionary[Debuff.PowerIncrease] != 0)
        {
            buffIcon.SetActive(true);
            debuffIcon.SetActive(false);
            buffDebuffText.text = String.Format("{0}", enemy.DebuffDictionary[Debuff.PowerIncrease]);
        }
        else if (enemy.DebuffDictionary[Debuff.PowerDecrease] != 0)
        {
            buffIcon.SetActive(false);
            debuffIcon.SetActive(true);
            buffDebuffText.text = String.Format("{0}", enemy.DebuffDictionary[Debuff.PowerIncrease]);
        }
        else
        {
            buffIcon.SetActive(false);
            debuffIcon.SetActive(false);
            buffDebuffText.text = String.Format("");
        }
    }

    public void IntentionUpdate()
    {/*
        switch (enemy.EnemyActions)
        {
            case H1Attack:
                break;
                
                 * H1Attack, V1Attack, H2Attack, V2Attack, AllAttack, ColoredAttack, NoColoredAttack,
    WallSummon=10, WallsSummon, MobSummon,
    PowerIncrease=20, DamageDecrease, HpHealing, ArmorHealing,
    PlayerPowerDecrease=30, PlayerDamageIncrease, DrawCardDecrease, CardCostIncrease,
    None=200
                 */
    }
}
