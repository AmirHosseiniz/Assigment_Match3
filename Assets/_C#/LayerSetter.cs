using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSetter : MonoBehaviour
{
    [SerializeField, NaughtyAttributes.Layer] int layer;
    [SerializeField] bool setOnStart = true;
    [SerializeField] bool setOnAwake;
    [SerializeField] bool setOnEnable;

    void Start()
    {
        if (setOnStart) SetLayer();
    }

    void Awake()
    {
        if (setOnAwake) SetLayer();
    }

    private void OnEnable()
    {
        if (setOnEnable) SetLayer();
    }

    public void SetLayer() => SetLayerRecursive(gameObject, layer);

    public void SetLayer(int layer)
    {
        this.layer = layer;
        SetLayer();
    }

    public static void SetLayerRecursive(GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in gameObject.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }
}
