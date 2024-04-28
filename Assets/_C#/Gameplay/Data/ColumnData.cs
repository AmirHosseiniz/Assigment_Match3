using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColumnData
{
    [SerializeField] CellData[] column = Array.Empty<CellData>();

    public ColumnData(int height)
    {
        Height = height;
    }

    public CellData this[int index]
    {
        get => column[index];
        set => column[index] = value;
    }

    public int Height
    {
        get => column.Length;
        set
        {
            Array.Resize(ref column, value);
            ValidateCells();
        }
    }

    void ValidateCells()
    {
        for (int i = 0; i < column.Length; ++i)
        {
            column[i] ??= new CellData();
        }
    }
}
