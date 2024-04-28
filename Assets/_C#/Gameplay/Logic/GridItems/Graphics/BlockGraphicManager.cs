using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGraphicManager : GridItemGraphicManager<Block, BlockGraphic>
{
    protected override bool SetOnAwake => true;
    protected override bool SetOnEnable => false;
    protected override bool SetOnStart => false;
    protected override BlockGraphic GetCurrentGraphics()
    {
        return gridItemGraphicProvider.GetBlockGraphic();
    }

    protected override void Awake()
    {
        base.Awake();
        gridItem.Exploded += BlockExplodedEventHandler;
    }

    void OnDestroy()
    {
        gridItem.Exploded -= BlockExplodedEventHandler;
    }

    void BlockExplodedEventHandler(Block _)
    {
       var explodeParticlePrefab = gridItemGraphicProvider.GetBlockExplodeParticlePrefab();
       if (explodeParticlePrefab)
       {
            explodeParticlePrefab.Create_Rotation(gridItem.transform);   
       }   
    }
}
