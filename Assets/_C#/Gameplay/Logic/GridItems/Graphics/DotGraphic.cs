using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotGraphic : GridItemGraphic<Dot>
{
    [SerializeField] Color corolatedColor = Color.white;

    public Color CorolatedColor { get => corolatedColor; }

    protected override void Apply_Internal(Dot dot)
    {
        transform.SetParent(dot.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    protected override void Unapply_Internal(Dot dot)
    {
        transform.SetParent(null);
        gameObject.SetActive(false);
    }
}
