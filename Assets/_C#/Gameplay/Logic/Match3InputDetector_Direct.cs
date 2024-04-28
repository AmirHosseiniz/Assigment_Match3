using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3InputDetector_Direct : Match3InputDetector
{
    Camera camera;

    void Awake()
    {
        camera = Camera.main;
    }

    public override bool IsClickStart() => Input.GetMouseButtonDown(0) && !EventSystemExtension.IsPointerOverUIObject();

    public override bool IsClickEnd() => Input.GetMouseButtonUp(0);

    public override Ray GetRay() => camera.ScreenPointToRay(Input.mousePosition);
}
