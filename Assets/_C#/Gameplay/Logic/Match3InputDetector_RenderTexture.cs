using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Match3InputDetector_RenderTexture : Match3InputDetector
{
    [SerializeField] RawImage rawImage;
    [SerializeField] Camera match3Camera;

    EventTrigger eventTrigger;
    RectTransform rawImageRectTransform;

    bool pointerDown = false;
    float pointerDownTime;

    bool pointerUp = false;
    float pointerUpTime;

    void Awake()
    {
        rawImageRectTransform = rawImage.GetComponent<RectTransform>();
        
        eventTrigger = rawImage.gameObject.AddComponent<EventTrigger>();
        eventTrigger.AddListener(EventTriggerType.PointerDown, OnPointerDown);
        eventTrigger.AddListener(EventTriggerType.PointerUp, OnPointerUp);
    }

    void OnPointerDown(BaseEventData eventData) => pointerDown = true;

    void OnPointerUp(BaseEventData eventData) => pointerUp = true;

    public override bool IsClickStart() => pointerDown;

    public override bool IsClickEnd() => pointerUp;

    void LateUpdate()
    {
        pointerDown = false;
        pointerUp = false;
    }

    public override Ray GetRay()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImageRectTransform, Input.mousePosition, null, out localPoint);
        var minPoint = rawImageRectTransform.rect.min;
        var maxPoint = rawImageRectTransform.rect.max;

        Vector2 viewportPoint;
        viewportPoint.x = MathUtil.Map(minPoint.x, maxPoint.x, 0f, 1f, localPoint.x);
        viewportPoint.y = MathUtil.Map(minPoint.y, maxPoint.y, 0f, 1f, localPoint.y);

        return match3Camera.ViewportPointToRay(viewportPoint);
    }
}
