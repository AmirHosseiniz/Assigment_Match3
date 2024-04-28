using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BlockGraphic))]
public abstract class BlockGraphicClickedAnimator : MonoBehaviour
{
    BlockGraphic blockGraphic;

    void Awake()
    {
        blockGraphic = GetComponent<BlockGraphic>();
        blockGraphic.Applied += AppliedEventHandler;
        blockGraphic.Unppplied += UnappliedEventHandler;
    }

    void OnDestroy()
    {
        blockGraphic.Applied -= AppliedEventHandler;
        blockGraphic.Unppplied -= UnappliedEventHandler;
    }

    void AppliedEventHandler(IGridItem block)
    {
        ((Block)block).Clicked += ClickedEventHandler;
    }

    void UnappliedEventHandler(IGridItem block)
    {
        ((Block)block).Clicked -= ClickedEventHandler;
    }

    void ClickedEventHandler(Block block)
    {
        AnimateClicked();
    }

    protected abstract void AnimateClicked();
}
