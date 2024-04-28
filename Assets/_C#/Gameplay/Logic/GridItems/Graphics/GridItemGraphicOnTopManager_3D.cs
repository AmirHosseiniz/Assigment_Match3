using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridItemGraphicOnTopManager_3D : GridItemGraphicOnTopManager
{
    [SerializeField] Transform content;
    [SerializeField] Vector3 defaultPosition = Vector3.zero;
    [SerializeField] Vector3 onTopPosition = .5f * Vector3.up;

    protected override bool OnTop_Internal
    {
        set => content.localPosition = _onTop ? onTopPosition : defaultPosition;
    }
}
