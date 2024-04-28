using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectTransformExtensions
{
    public static Vector2 GetHorizontalAnchor(this RectTransform rectTransform)
    {
        Vector2 horizontalAnchor = Vector2.zero;
        horizontalAnchor.x = rectTransform.anchorMin.x;
        horizontalAnchor.y = rectTransform.anchorMax.x;
        return horizontalAnchor;
    }

    public static Vector2 SetHorizontalAnchor(this RectTransform rectTransform, Vector2 value)
    {
        var anchorMin = rectTransform.anchorMin;
        anchorMin.x = value.x;
        rectTransform.anchorMin = anchorMin;
        var anchorMax = rectTransform.anchorMax;
        anchorMax.x = value.y;
        rectTransform.anchorMax = anchorMax;
        return value;
    }
}
