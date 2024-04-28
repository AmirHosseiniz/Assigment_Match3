using System.Collections;
using System.Collections.Generic;
using GameplayEnums.Cells;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public event System.Action CellsCreated;
    public event System.Action CellsItemDefined;

    [SerializeField] GridCell cellPrefab;
    [SerializeField] Transform cellParent;
    [SerializeField] Formation_Grid formation;
    [SerializeField] Transform itemParent;

    GridColumn[] columns;

    public Formation_Grid Formation => formation;

    public Vector3 Up => (formation.GetWorldPosition(0) - formation.GetWorldPosition(1)).normalized;
    public Vector3 Down => -Up;
    public GridColumn this[int index] => index >= 0 && index < Width ? columns[index] : null;

    public GridCell this[int x, int y]
    {
        get
        {
            var column = this[x];
            return column?[y];
        }
    }

    public int Width => columns.Length;

    public int Height => columns[0].Height;

    public Transform ItemParent => itemParent;

    public void CreateCells(GridData gridData)
    {
        int width = gridData.Width;
        int height = gridData.Height;

        formation.GridSize = new Vector2Int(height, width); //formation_grid's orientation logic is offset, hence why here width and height are swapped

        columns = new GridColumn[width];
        for (int i = 0; i < width; ++i)
        {
            columns[i] = new GridColumn(height);
            for (int j = 0; j < height; ++j)
            {
                if (gridData[i, j].type != TypeEnum.hole)
                {
                    Vector2Int coordinates = new Vector2Int(i, j);
                    Vector3 position = GetPosition(i, j);
                    Quaternion rotation = cellParent.rotation;
                    var cell = cellPrefab.Create(this, coordinates, position, rotation, cellParent);
                    cell.name = "Cell " + i + " " + j;
                    this[i][j] = cell;
                }
            }
        }
        CellsCreated?.Invoke();
    }

    public void OnItemsTypeDefined() => CellsItemDefined?.Invoke();

    public static bool AreNeighbors(Vector2Int cell1, Vector2Int cell2)
    {
        int dx = Mathf.Abs(cell1.x - cell2.x);
        int dy = Mathf.Abs(cell1.y - cell2.y);

        // Check if the cells are adjacent horizontally or vertically
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    public Vector3 GetPosition(int x, int y)
    {
        int cellIndex = x * Height + y;
        return formation.GetWorldPosition(cellIndex);
    }
}
