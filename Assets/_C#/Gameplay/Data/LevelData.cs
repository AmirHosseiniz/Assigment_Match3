using System;
using System.Collections;
using System.Collections.Generic;
using Pickle;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    [SerializeField, HideInInspector] GridData gridData;
    [SerializeField, HideInInspector] int colorCount = 1;
    
    public GridData GridData => gridData;

    public int ColorCount
    {
        get => colorCount;
        set => colorCount = value;
    }
}
