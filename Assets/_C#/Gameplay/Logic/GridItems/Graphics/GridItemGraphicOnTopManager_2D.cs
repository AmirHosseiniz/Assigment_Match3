using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridItemGraphicOnTopManager_2D : GridItemGraphicOnTopManager
{
    [SerializeField] int defaultSortingOrder;
    [SerializeField] int onTopSortingOrder;

    SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected override bool OnTop_Internal
    {
        set => spriteRenderer.sortingOrder = _onTop ? onTopSortingOrder : defaultSortingOrder;
    }
}
