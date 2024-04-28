using System;

[Serializable]
public class GridColumn
{
    public GridCell[] column;

    public int Height => column.Length;

    public GridColumn(int height)
    {
        column = new GridCell[height];
    }
        
    public GridCell this[int index]
    {
        get => index >= 0 && index < Height ? column[index] : null;
        set
        {
            if (index >= 0 && index < Height) column[index] = value;   
        }
    }
}