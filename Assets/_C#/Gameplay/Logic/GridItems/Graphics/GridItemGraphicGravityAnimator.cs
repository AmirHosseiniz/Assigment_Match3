using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IGridItemGraphic))]
public abstract class GridItemGraphicGravityAnimator : MonoBehaviour
{
    IGridItemGraphic gridItemGraphic;

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
        gridItem.GravityApplied += OnGravityAppliedEventHandler;
    }

    void UnappliedEventHandler(IGridItem gridItem)
    {
        gridItem.GravityApplied -= OnGravityAppliedEventHandler;
    }

    void OnGravityAppliedEventHandler()
    {
        AnimateGravity();
    }

    protected abstract void AnimateGravity();
}
