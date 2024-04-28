using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Match3Runner))]
[RequireComponent(typeof(Match3InputDetector))]
public class Match3InputManager : MonoBehaviour
{
    [SerializeField] float rayDistance = 50f;
    [SerializeField] float validDragDst;
    [SerializeField, NaughtyAttributes.Tag] string gridCellTag;

    public bool RecievingInput { get; private set; }

    Camera camera;

    GridCell _selectedCell;
    GridCell _clickedCell;

    Vector3 clickStartPos;
    Vector3 clickEndPos;
    bool potentialClick;

    GridCell SelectedCell
    {
        get => _selectedCell;
        set
        {
            if (_selectedCell) _selectedCell.IsPressed = false;
            _selectedCell = value;
            if (_selectedCell) _selectedCell.IsPressed = true;
        }
    }

    GridCell ClickedCell
    {
        get => _clickedCell;
        set
        {
            if (_clickedCell) _clickedCell.IsPressed = false;
            _clickedCell = value;
            if (_clickedCell) _clickedCell.IsPressed = true;
        }
    }

    bool IsDragValid_PosChecker(Vector3 startPos, Vector3 endPos) => Vector3.Distance(startPos, endPos) >= validDragDst;
    bool IsDragValid_GridChecker(Vector2Int cellCord_A, Vector2Int cellCord_B) => Mathf.Abs(cellCord_A.x - cellCord_B.x) + Mathf.Abs(cellCord_A.y - cellCord_B.y) == 1;

    Match3Runner match3Runner;
    Match3InputDetector inputDetector;

    void Awake()
    {
        camera = Camera.main;
        match3Runner = GetComponent<Match3Runner>();
        inputDetector = GetComponent<Match3InputDetector>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (match3Runner.IsSwapping) return;

        if (inputDetector.IsClickStart())
        {
            if (RayCast(out GridCell newCell))
            {
                clickStartPos = Input.mousePosition;
                RecievingInput = true;
                SelectedCell = newCell;
            }
        }

        if (RecievingInput && inputDetector.IsClickEnd())
        {
            clickEndPos = Input.mousePosition;

            if (RayCast(out GridCell newCell))
            {
                if ((potentialClick && ClickedCell == newCell) || (!potentialClick && newCell == SelectedCell))
                {
                    match3Runner.Click(_selectedCell);
                    potentialClick = !IsDragValid_PosChecker(clickStartPos, clickEndPos);
                }
                else if (!IsDragValid_PosChecker(clickStartPos, clickEndPos) && ClickedCell)
                {
                    if (Grid.AreNeighbors(ClickedCell.Coordinates, newCell.Coordinates))
                    {
                        match3Runner.Swap(ClickedCell, newCell);
                    }
                    potentialClick = false;
                }
            }

            if (potentialClick)
                ClickedCell = SelectedCell;
            else
                ClickedCell = null;

            RecievingInput = false;
            SelectedCell = null;
        }

        if (RecievingInput)
            Process();
    }

    void Process()
    {
        if (RayCast(out GridCell newCell))
        {
            if (newCell != SelectedCell)
            {
                if (IsDragValid_PosChecker(clickStartPos, clickEndPos) && IsDragValid_GridChecker(SelectedCell.Coordinates, newCell.Coordinates))
                {
                    if (SelectedCell.HasItem && newCell.HasItem)
                    {
                        match3Runner.Swap(SelectedCell, newCell);
                        RecievingInput = false;
                        SelectedCell = null;
                    }
                }
            }
        }
    }

    bool RayCast(out GridCell newCell)
    {
        if (Physics.Raycast(inputDetector.GetRay(), out RaycastHit newHitInfo, rayDistance))
        {
            var newHitObject = GetHitObjectFromHitInfo(newHitInfo);
            if (newHitObject.CompareTag(gridCellTag))
            {
                newCell = FindGridCellFromHitObject(newHitObject);
                return newCell != null;
            }
        }

        newCell = null;
        return false;
    }

    GridCell FindGridCellFromHitObject(GameObject hitObject)
    {
        var grid = match3Runner.Grid;
        for (int i = 0; i < grid.Width; ++i)
        {
            for (int j = 0; j < grid.Height; ++j)
            {
                if (grid[i, j] && grid[i, j].gameObject == hitObject)
                {
                    return grid[i, j];
                }
            }
        }

        return null;
    }

    GameObject GetHitObjectFromHitInfo(RaycastHit hitInfo)
    {
        return hitInfo.rigidbody ? hitInfo.rigidbody.gameObject : hitInfo.collider.gameObject;
    }
}
