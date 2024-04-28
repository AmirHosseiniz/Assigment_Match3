using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Formation : MonoBehaviour
{
    public abstract Vector3 GetLocalPosition(int index);
    public Vector3 GetWorldPosition(int index)
    {
        var localPosition = GetLocalPosition(index);
        return transform.TransformPoint(localPosition);
    }
}
