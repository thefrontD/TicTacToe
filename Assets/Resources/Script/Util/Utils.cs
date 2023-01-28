using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Security.Policy;

public class Utils
{
    public static Quaternion QI
    {
        get
        {
            return Quaternion.identity;
        }
    }
    public static Quaternion QB
    {
        get
        {
            return Quaternion.Euler(0f, 180f, 0f);
        }
    }

    public static Quaternion QS
    {
        get
        {
            return Quaternion.Euler(-90f, 0f, 0f);
        }
    }

    public static Vector3 cardScaleOnHand
    {
        get
        {
            return new Vector3(13, 19, 0.1f);
        }
    }
    public static Vector3 cardScaleOnField
    {
        get
        {
            return new Vector3(9, 13, 0.1f);
        }
    }
    
    public static Vector3 MousePos
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.back * 30;
        }
    }

    public static Vector3 CardMousePos
    {
        get
        {
            Vector3 result = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            result.z = -5;
            return result;
        }
    }
}
