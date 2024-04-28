using System;
using System.Collections;
using System.Collections.Generic;
using GameplayEnums.Cells;
using Pickle;
using UnityEngine;

[CreateAssetMenu]
public class GridItemGraphicProvider : ScriptableObject
{
    [Header("Dot")]
    [SerializeField, Pickle(ObjectProviderType.Assets)] DotGraphic[] dotGraphicsPrefabs;
    [SerializeField, Pickle(ObjectProviderType.Assets)] DotGraphic fallbackDotGraphic;
    [SerializeField, Pickle(ObjectProviderType.Assets)] PooledParticleSystem[] dotExplodeParticlePrefabs;

    [Header("Other")]
    [SerializeField, Pickle(ObjectProviderType.Assets)] BlockGraphic blockGraphicPrefab;
    [SerializeField] PooledParticleSystem blockExplodeParticlePrefab;

    public DotGraphic[] DotGraphicsPrefabs { get => dotGraphicsPrefabs; }

    public DotGraphic GetDotGraphic(int colorGroup)
    {
        if (colorGroup >= 0 && colorGroup < dotGraphicsPrefabs.Length)
        {
            return dotGraphicsPrefabs[colorGroup].Create<DotGraphic>();
        }
        return fallbackDotGraphic.Create<DotGraphic>();
    }

    public PooledParticleSystem GetExplodeParticlePrefab(int colorGroup)
    {
        if (colorGroup >= 0 && colorGroup < dotExplodeParticlePrefabs.Length)
        {
            return dotExplodeParticlePrefabs[colorGroup];
        }
        return dotExplodeParticlePrefabs[0];
    }

    public BlockGraphic GetBlockGraphic() => blockGraphicPrefab.Create<BlockGraphic>();

    public PooledParticleSystem GetBlockExplodeParticlePrefab() => blockExplodeParticlePrefab;
}
