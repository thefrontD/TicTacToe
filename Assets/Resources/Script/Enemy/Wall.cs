﻿using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Wall : MonoBehaviour, IAttackable
{
    [SerializeField] private ParticleSystem destructionEffect;
    
    private int wallHP = 10;
    public int WallHP
    {
        get { return wallHP; }
        set
        {
            if (value < 0) wallHP = 0;
            else wallHP = value;
        }
    }

    public int Row;
    public int Col;

    public void Init(int row, int col)
    {
        this.Row = row;
        this.Col = col;
    }
    
    public void AttackedByPlayer(int damage)
    {
        WallHP -= damage;

        if (WallHP <= 0)
        {
            BoardManager.Instance.BoardObjects[Row][Col] = BoardObject.None;
            BoardManager.Instance.BoardAttackables[Row][Col] = null;
            //destructionEffect.Play();
            //transform.DOMoveZ().onComplete();
            Destroy(this.gameObject);
        }
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
