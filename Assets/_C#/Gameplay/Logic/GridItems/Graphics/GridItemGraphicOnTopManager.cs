using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IGridItemGraphic))]
public abstract class GridItemGraphicOnTopManager : MonoBehaviour
{
    IGridItemGraphic gridItemGraphic;

    protected bool _onTop;
    public bool OnTop
    {
        get => _onTop;
        set
        {
            _onTop = value;
            OnTop_Internal = value;
        }
    }

    protected abstract bool OnTop_Internal { set; }

    protected virtual void Awake()
    {
        gridItemGraphic = GetComponent<IGridItemGraphic>();
        gridItemGraphic.Applied += AppliedEventHandler;
        gridItemGraphic.Unppplied += UnappliedEventHandler;
    }

    protected virtual void OnDestroy()
    {
        gridItemGraphic.Applied -= AppliedEventHandler;
        gridItemGraphic.Unppplied -= UnappliedEventHandler;
    }

    void AppliedEventHandler(IGridItem gridItem)
    {
        gridItem.OnTopChanged += OnTopChangedEventHandler;
        OnTop = gridItem.OnTop;
    }

    void UnappliedEventHandler(IGridItem gridItem)
    {
        gridItem.OnTopChanged -= OnTopChangedEventHandler;
    }

    void OnTopChangedEventHandler(bool onTop)
    {
        OnTop = onTop;
    }
}
