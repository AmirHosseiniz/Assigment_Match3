using System;
using System.Collections;
using System.Collections.Generic;
using GameplayEnums.Cells;
using UnityEngine;

public class Block : GridItem
{
    public event System.Action<Block> Exploded;
    public event System.Action<Block> Clicked;
    public override TypeEnum Type => TypeEnum.block;

    GridCell _gridCell;
    public override GridCell GridCell
    {
        get => _gridCell;
        set
        {
            if (_gridCell)
            {
                foreach (var adjacentCell in adjacentCells)
                {
                    adjacentCell.Exploded -= AdjacentCellExplodedEventHandler;
                    adjacentCell.Merging -= AdjacentCellMergingEventHandler;
                }
            }
            
            adjacentCells.Clear();

            _gridCell = value;
            if (_gridCell)
            {
                adjacentCells.AddRange(_gridCell.GetAdjacentGridCells());
                foreach (var adjacentCell in adjacentCells)
                {
                    adjacentCell.Exploded += AdjacentCellExplodedEventHandler;
                    adjacentCell.Merging += AdjacentCellMergingEventHandler;
                }
            }
        }
    }

    List<GridCell> adjacentCells = new();

    public Block Create(Vector3 position, Quaternion rotation, Transform parent)
    {
        var instance = Get<Block>();
        instance.transform.SetParent(parent);
        instance.transform.position = position;
        instance.transform.localScale = Vector3.one;
        instance.transform.rotation = rotation;
        return instance;
    }
    
    
    public override bool Explode()
    {
        Exploded?.Invoke(this);
        gameObject.SetActive(false);
        return true;
    }

    public override bool Click()
    {
        Clicked?.Invoke(this);
        return false;
    }

    void OnDestroy()
    {
        foreach (var adjacentCell in adjacentCells)
        {
            adjacentCell.Exploded -= AdjacentCellExplodedEventHandler;
            adjacentCell.Merging -= AdjacentCellMergingEventHandler;
        }
    }

    void AdjacentCellExplodedEventHandler(GridCell adjacentCell, IGridItem gridItem)
    {
        if (gridItem is Dot)
        {
            GridCell.Explode();
        }
    }

    void AdjacentCellMergingEventHandler(GridCell adjacentCell, IGridItem gridItem)
    {
        if (gridItem is Dot)
        {
            GridCell.Explode();
        }
    }
}
