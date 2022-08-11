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
    public int Row { get; set; }
    public int Col { get; set; }
    public void AttackedByPlayer(int damage)
    {
        WallHP -= damage;
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
