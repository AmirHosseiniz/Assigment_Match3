using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation_Grid : Formation
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] Vector3 cellSize;
    [SerializeField] bool centerX;
#if UNITY_EDITOR
    [SerializeField] int gizmoCount;
#endif

    public Vector2Int GridSize
    {
        get => gridSize;
        set
        {
            gridSize = value;
            initialized = false;
        }
    }
    public Vector3 CellSize => cellSize;

    int countInLayer;
    Vector2 centerCoordinates;

    bool initialized = false;

    private void Awake()
    {
        Init();
        initialized = true;
    }

    private void Init()
    {
        countInLayer = gridSize.x * gridSize.y;
        centerCoordinates.x = (gridSize.x - 1) * .5f;
        centerCoordinates.y = (gridSize.y - 1) * .5f;
    }

    public override Vector3 GetLocalPosition(int index)
    {
        if (!initialized) Init();

        Vector3 position = Vector3.zero;
        int layer = index / countInLayer;
        position.y += layer * cellSize.y;
        int indexInLayer = index % countInLayer;
        Vector2 coordinates = new Vector2(indexInLayer % gridSize.x, indexInLayer / gridSize.x);
        if (centerX)
        {
            coordinates.x += Mathf.FloorToInt(centerCoordinates.x);
            coordinates.x %= gridSize.x;
            //if (coordinates.x > gridSize.x - 1) coordinates.x %= gridSize.x - 1;
        }
        Vector2 coordinatesOffset = coordinates - centerCoordinates;
        position.x += cellSize.x * coordinatesOffset.x;
        position.z += cellSize.z * coordinatesOffset.y;
        return position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (gizmoCount > 0)
        {
            Init();
            Color startColor = Color.white;
            Color endColor = Color.black;
            for (int i = 0; i < gizmoCount; ++i)
            {
                Gizmos.color = Color.Lerp(startColor, endColor, (float)i / gizmoCount);
                //Gizmos.DrawCube(transform.position + GetLocalPosition(i) + .25f * cellSize.y * Vector3.up, cellSize * .5f);
                Gizmos.DrawCube(GetWorldPosition(i), cellSize * .5f);
            }
        }
    }
#endif
}
