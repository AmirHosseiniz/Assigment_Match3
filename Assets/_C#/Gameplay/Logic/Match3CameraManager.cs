using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Match3CameraManager : MonoBehaviour
{
    [SerializeField] Camera[] cameras;
    [SerializeField] Match3Runner match3Runner;
    [SerializeField] float border;
 
    void Start()
    {
        var gridWidth = match3Runner.Grid.Width;
        var gridHeight = match3Runner.Grid.Height;

        var maxSide = Mathf.Max(gridWidth, gridHeight);

        var orthographicSize = (maxSide + border) / 2f;

        foreach (var camera in cameras)
        {
            camera.orthographicSize = orthographicSize;
        }
    }
}
