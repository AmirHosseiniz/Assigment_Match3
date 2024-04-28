using System;
using UnityEngine;
using Pickle;

[AttributeUsage(AttributeTargets.Field)]
public class PrefabAttribute : PickleAttribute
{
    public PrefabAttribute() {
        LookupType = ObjectProviderType.Assets;
    }

}

