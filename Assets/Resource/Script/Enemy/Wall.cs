using System.Collections;
using UnityEngine;

public class Wall : MonoBehaviour, IAttackable
{
    private int wallHP;
    public int WallHP
    {
        get { return wallHP; }
        set
        {
            if (value < 0) wallHP = 0;
            else wallHP = value;
        }
    }
    public void ReduceHP(int damage)
    {
        WallHP -= damage;
    }
}
