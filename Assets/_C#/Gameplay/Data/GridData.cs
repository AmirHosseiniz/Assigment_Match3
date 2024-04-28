using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridData
{
    [SerializeField] ColumnData[] grid = new[] { new ColumnData(1) };

    public ColumnData this[int index] => grid[index];

    public CellData this[int x, int y] => grid[x][y];

    public int Width
    {
        get => grid.Length;
        set
        {
            Array.Resize(ref grid, value);
            ValidateColumns();
        }
    }

    public int Height
    {
        get => grid[0].Height;
        set
        {
            for (int i = 0; i < grid.Length; ++i)
            {
                grid[i].Height = value;
            }
        }
    }

    void ValidateColumns()
    {
        for (int i = 0; i < Width; ++i)
        {
            grid[i] ??= new ColumnData(Height);
        }
    }
}
