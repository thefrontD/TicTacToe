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
    [Serializable]
    struct IntentionIcon
    {
        public Intention Intention;
        public GameObject Icon;
    }
    [SerializeField] private IntentionIcon[] IntentionIcons;
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
        IntentionUpdate();
        BuffDebuffUpdate();
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
            buffDebuffText.text = String.Format("{0}", enemy.DebuffDictionary[Debuff.PowerDecrease]);
        }
        else
        {
            buffIcon.SetActive(false);
            debuffIcon.SetActive(false);
            buffDebuffText.text = String.Format("");
        }
    }

    public void IntentionUpdate()
    {
        (EnemyAction, int) enemyAction = enemy.EnemyActions.Peek();

        switch (enemyAction.Item1)
        {
            case EnemyAction.H1Attack:
            case EnemyAction.V1Attack:
            case EnemyAction.H2Attack:
            case EnemyAction.V2Attack:
            case EnemyAction.AllAttack:
            case EnemyAction.ColoredAttack:
            case EnemyAction.NoColoredAttack:
                activateIntentionIcon(Intention.Attack);
                break;
            case EnemyAction.WallSummon:
            case EnemyAction.WallsSummon:
                activateIntentionIcon(Intention.Wall);
                break;
            case EnemyAction.MobSummon:
                activateIntentionIcon(Intention.Minion);
                break;
            case EnemyAction.PowerIncrease:
            case EnemyAction.DamageDecrease:
                activateIntentionIcon(Intention.Buff);
                break;
            case EnemyAction.HpHealing:
                activateIntentionIcon(Intention.HPHealing);
                break;
            case EnemyAction.ShieldHealing:
                activateIntentionIcon(Intention.ShieldHealing);
                break;
            case EnemyAction.PlayerPowerDecrease:
            case EnemyAction.PlayerDamageIncrease:
            case EnemyAction.DrawCardDecrease:
            case EnemyAction.CardCostIncrease:
                activateIntentionIcon(Intention.Debuff);
                break;
            case EnemyAction.None:
                activateIntentionIcon(Intention.None);
                break;
        }
    }
    private void activateIntentionIcon(Intention intention)
    {
        for (int i = 0; i < IntentionIcons.Length; i++)
        {
            if (IntentionIcons[i].Intention == intention)
            {
                IntentionIcons[i].Icon.SetActive(true);
            }
            else
            {
                IntentionIcons[i].Icon.SetActive(false);
            }
        }
    }
}
