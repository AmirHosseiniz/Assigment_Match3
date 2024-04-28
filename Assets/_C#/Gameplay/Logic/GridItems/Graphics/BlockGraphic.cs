using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGraphic : GridItemGraphic<Block>
{
    protected override void Apply_Internal(Block block)
    {
        transform.SetParent(block.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    protected override void Unapply_Internal(Block block)
    {
        transform.SetParent(null);
        gameObject.SetActive(false);
    }
}
