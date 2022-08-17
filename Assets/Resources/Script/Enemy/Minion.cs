using System.Collections;
using UnityEngine;

public class Minion : MonoBehaviour, IAttackable
{
    private int minionHP;
    public int MinionHP
    {
        get { return minionHP; }
        set
        {
            if (value < 0) minionHP = 0;
            else minionHP = value;
        }
    }
    public int Row { get; set; }
    public int Col { get; set; }
    public void AttackedByPlayer(int damage)
    {
        MinionHP -= damage;
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
