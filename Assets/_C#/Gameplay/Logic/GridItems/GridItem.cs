using System;
using System.Collections;
using System.Collections.Generic;
using GameplayEnums.Cells;
using UnityEngine;

public abstract class GridItem : PooledMonobehaviour, IGridItem
{
    public event System.Action<bool> OnTopChanged;
    public event Action<float> HighlightChanged;
    public event Action GravityApplied;

    protected bool _onTop;

    public bool OnTop
    {
        get => _onTop;
        set
        {
            _onTop = value;
            OnTopChanged?.Invoke(_onTop);
        }
    }

    protected float _highlight;

    public float Highlight
    {
        get => _highlight;
        set
        {
            _highlight = value;
            HighlightChanged?.Invoke(_highlight);
        }
    }
    
    
    
    
    public abstract TypeEnum Type { get; }
    public abstract GridCell GridCell { get; set; }
    
    public abstract bool Explode();
    public abstract bool Click();
    public void OnGravityApplied()
    {
        GravityApplied?.Invoke();
    }
}
