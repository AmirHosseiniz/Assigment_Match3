using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridCell : MonoBehaviour
{
    public event System.Action<GridCell, IGridItem> ItemSet;
    public event System.Action<GridCell, IGridItem> ItemUnset;
    public event System.Action<GridCell, IGridItem> Exploded;
    public event System.Action<GridCell, IGridItem> Merging;
    public event System.Action CellsCreated;
    public event System.Action CellsDefined;

    [SerializeField] PooledParticleSystem explodeParticlePrefab;
    [SerializeField] UnityEvent exploded;

    public Vector2Int Coordinates { get; private set; }

    public Grid Grid { get; private set; }

    public bool IsPressed { get; set; }

    [field: NaughtyAttributes.ShowNonSerializedField] public IGridItem Item { get; private set; }

    public bool HasItem => Item != null;

    public bool IsItemTypeHole => HasItem && Item.Type == GameplayEnums.Cells.TypeEnum.hole;

    public bool TrySetItem(IGridItem item)
    {
        if (!HasItem)
        {
            Item = item;
            Item.GridCell = this;
            ItemSet?.Invoke(this, item);
            return true;
        }

        return false;
    }

    public bool TryUnsetItem(IGridItem item)
    {
        if (Item == item) return TryUnsetItem();
        return false;
    }

    public bool TryUnsetItem()
    {
        if (HasItem)
        {
            var item = Item;
            Item = null;
            item.GridCell = null;
            ItemUnset?.Invoke(this, item);
            return true;
        }
        return false;
    }

    public GridCell Create(Grid grid, Vector2Int coordinates, Vector3 position, Quaternion rotation, Transform parent)
    {
        var instance = Instantiate(this, position, rotation, parent);
        instance.Grid = grid;
        grid.CellsCreated += instance.CellsCreatedEventHandler;
        grid.CellsItemDefined += instance.CellsDefinedEventHandler;
        instance.Coordinates = coordinates;
        instance.name = $"{nameof(GridCell)} ({coordinates.x}, {coordinates.y})";
        return instance;
    }

    void OnDestroy()
    {
        Grid.CellsCreated -= CellsCreatedEventHandler;
        Grid.CellsItemDefined -= CellsDefinedEventHandler;
    }

    public List<GridCell> GetAdjacentGridCells()
    {
        List<GridCell> adjacentGridCells = new(4);
        var rightCell = Grid[Coordinates.x + 1, Coordinates.y];
        var leftCell = Grid[Coordinates.x - 1, Coordinates.y];
        var topCell = Grid[Coordinates.x, Coordinates.y - 1];
        var bottomCell = Grid[Coordinates.x, Coordinates.y + 1];

        if (rightCell) adjacentGridCells.Add(rightCell);
        if (leftCell) adjacentGridCells.Add(leftCell);
        if (topCell) adjacentGridCells.Add(topCell);
        if (bottomCell) adjacentGridCells.Add(bottomCell);

        return adjacentGridCells;
    }

    public void InvokeMerging()
    {
        Merging?.Invoke(this, Item);
    }

    public void Explode()
    {
        if (HasItem)
        {
            if (Item.Explode())
            {
                var item = Item;
                TryUnsetItem();
                if (explodeParticlePrefab) explodeParticlePrefab.Create_Rotation(transform);
                exploded.Invoke();
                Exploded?.Invoke(this, item);
            }
        }
    }

    public bool Click()
    {
        if (HasItem)
        {
            if (Item.Click())
            {
                if (explodeParticlePrefab) explodeParticlePrefab.Create_Rotation(transform);
                exploded.Invoke();
                Exploded?.Invoke(this, Item);
                return TryUnsetItem();
            }
        }
        return false;
    }

    public int GetObstacleCount() => HasItem ? 1 : 0;

    void CellsCreatedEventHandler()
    {
        CellsCreated?.Invoke();
    }

    void CellsDefinedEventHandler()
    {
        CellsDefined?.Invoke();
    }
}
