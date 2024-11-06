using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStat
{
    [SerializeField]
    private int baseValue;

    public CharacterStat(int initialValue)
    {
        baseValue = initialValue;
    }

    public int GetValue()
    {
        return baseValue;
    }

    public void SetValue(int value)
    {
        baseValue = value;
    }

    internal void SetValue(object value)
    {
        throw new NotImplementedException();
    }
}
