using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int MapID;
    public int Type;
    public int Difficulty;
    public int MonsterID;

    public List<int> Border;    //Border[i] = j 
                                // i -> 1:left 2:up 3:right 4: down
                                // j -> 1:Wall 2:Bush   

    public void SetPosition(int i , int j)
    {
        transform.position = new Vector3(i, j, 0);
    }
    public void SetType(int i)
    {
        Type = i;
    }
}
