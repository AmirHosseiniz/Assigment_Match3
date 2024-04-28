using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotGraphicManager : GridItemGraphicManager<Dot, DotGraphic>
{
    protected override bool SetOnAwake => false;
    protected override bool SetOnEnable => false;
    protected override bool SetOnStart => false;

    protected override DotGraphic GetCurrentGraphics()
    {
        return gridItemGraphicProvider.GetDotGraphic(gridItem.ColorGroup);
    }

    protected override void Awake()
    {
        base.Awake();
        gridItem.ColorGroupSet += DotColorGroupSetEventHandler;
        gridItem.Exploded += DotExplodedEventHandler;
    }

    void OnDestroy()
    {
        gridItem.ColorGroupSet -= DotColorGroupSetEventHandler;
        gridItem.Exploded -= DotExplodedEventHandler;
    }

    void DotColorGroupSetEventHandler(Dot _) => SetGraphics();

    void DotExplodedEventHandler(Dot _) => gridItemGraphicProvider.GetExplodeParticlePrefab(gridItem.ColorGroup).Create_Rotation(gridItem.transform);
}
