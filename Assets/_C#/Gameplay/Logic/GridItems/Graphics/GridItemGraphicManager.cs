using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridItemGraphicManager<T, U> : MonoBehaviour 
    where T : IGridItem 
    where U : GridItemGraphic<T>
{
    [SerializeField] protected GridItemGraphicProvider gridItemGraphicProvider;
    
    protected abstract bool SetOnAwake { get; }
    protected abstract bool SetOnEnable { get; }
    protected abstract bool SetOnStart { get; }

    protected T gridItem;

    U appliedGraphics;

    protected virtual void Awake()
    {
        gridItem = GetComponent<T>();
        if (SetOnAwake) SetGraphics();
    }

    void OnEnable()
    {
        if (SetOnEnable) SetGraphics();
    }

    protected virtual void Start()
    {
        if (SetOnStart) SetGraphics();
    }

    protected void SetGraphics()
    {
        if (appliedGraphics) appliedGraphics.Unapply(gridItem);
        appliedGraphics = GetCurrentGraphics();
        appliedGraphics.Apply(gridItem);
    }

    protected abstract U GetCurrentGraphics();
}
