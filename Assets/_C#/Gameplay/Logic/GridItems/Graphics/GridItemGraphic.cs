using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridItemGraphic<T> : PooledMonobehaviour, IGridItemGraphic where T : IGridItem
{
    public event Action<IGridItem> Applied;
    public event Action<IGridItem> Unppplied;

    [SerializeField] bool autoApplyToParent;

    void Start()
    {
        if (autoApplyToParent)
        {
            var gridItem = GetComponentInParent<T>();
            Apply(gridItem);
        }
    }

    public U Create<U>() where U : GridItemGraphic<T>
    {
        var instance = Get<U>();
        instance.transform.localScale = Vector3.one;
        return instance;
    }

    public void Apply(T gridItem)
    {
        Apply_Internal(gridItem);
        Applied?.Invoke(gridItem);
    }

    public void Unapply(T gridItem)
    {
        Unapply_Internal(gridItem);
        Unppplied?.Invoke(gridItem);
    }

    protected abstract void Apply_Internal(T gridItem);

    protected abstract void Unapply_Internal(T gridItem);
}

public interface IGridItemGraphic
{
    public event System.Action<IGridItem> Applied;
    public event System.Action<IGridItem> Unppplied;
}
