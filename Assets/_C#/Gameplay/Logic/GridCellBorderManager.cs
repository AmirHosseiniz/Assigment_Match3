using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellBorderManager : MonoBehaviour
{
    [Header("Top")]
    [SerializeField] GameObject topCenterBorder;
    [SerializeField] GameObject topRightBorder;
    [SerializeField] GameObject topLeftBorder;
    [Header("Right")]
    [SerializeField] GameObject rightCenterBorder;
    [SerializeField] GameObject rightTopBorder;
    [SerializeField] GameObject rightBottomBorder;
    [Header("Bottom")]
    [SerializeField] GameObject bottomCenterBorder;
    [SerializeField] GameObject bottomRightBorder;
    [SerializeField] GameObject bottomLeftBorder;
    [Header("Left")]
    [SerializeField] GameObject leftCenterBorder;
    [SerializeField] GameObject leftTopBorder;
    [SerializeField] GameObject leftBottomBorder;
    [Header("Corners")]
    [SerializeField] GameObject topRightCornerBorder;
    [SerializeField] GameObject bottomRightCornerBorder;
    [SerializeField] GameObject bottomLeftCornerBorder;
    [SerializeField] GameObject topLeftCornerBorder;

    GridCell gridCell;

    void Awake()
    {
        gridCell = GetComponentInParent<GridCell>();

        gridCell.CellsDefined += SetBorders;
    }

    void OnDestroy()
    {
        gridCell.CellsDefined -= SetBorders;
    }

    void SetBorders()
    {
        var grid = gridCell.Grid;

        int x = gridCell.Coordinates.x;
        int y = gridCell.Coordinates.y;

        bool topCell = (grid[x, y - 1] && !grid[x, y - 1].IsItemTypeHole) && y > 0;
        bool topRightCell = (grid[x + 1, y - 1] && !grid[x + 1, y - 1].IsItemTypeHole) && y > 0;
        bool topLeftCell = (grid[x - 1, y - 1] && !grid[x - 1, y - 1].IsItemTypeHole) && y > 0;
        bool rightCell = grid[x + 1, y] && !grid[x + 1, y].IsItemTypeHole;
        bool leftCell = grid[x - 1, y] && !grid[x - 1, y].IsItemTypeHole;
        bool bottomRightCell = grid[x + 1, y + 1] && !grid[x + 1, y + 1].IsItemTypeHole;
        bool bottomCell = grid[x, y + 1] && !grid[x, y + 1].IsItemTypeHole;
        bool bottomLeftCell = grid[x - 1, y + 1] && !grid[x - 1, y + 1].IsItemTypeHole;

        bool topRightCornerActive = !topCell && !topRightCell && !rightCell;
        bool bottomRightCornerActive = !bottomCell && !bottomRightCell && !rightCell;
        bool bottomLeftCornerActive = !bottomCell && !bottomLeftCell && !leftCell;
        bool topLeftCornerActive = !topCell && !topLeftCell && !leftCell;

        topRightCornerBorder.SetActive(topRightCornerActive);
        bottomRightCornerBorder.SetActive(bottomRightCornerActive);
        bottomLeftCornerBorder.SetActive(bottomLeftCornerActive);
        topLeftCornerBorder.SetActive(topLeftCornerActive);

        topCenterBorder.SetActive(!topCell);
        topLeftBorder.SetActive(!topCell && !topLeftCornerActive);
        topRightBorder.SetActive(!topCell && !topRightCornerActive);

        rightCenterBorder.SetActive(!rightCell);
        rightTopBorder.SetActive(!rightCell && !topRightCornerActive);
        rightBottomBorder.SetActive(!rightCell && !bottomRightCornerActive);

        bottomCenterBorder.SetActive(!bottomCell);
        bottomRightBorder.SetActive(!bottomCell && !bottomRightCornerActive);
        bottomLeftBorder.SetActive(!bottomCell && !bottomLeftCornerActive);

        leftCenterBorder.SetActive(!leftCell);
        leftTopBorder.SetActive(!leftCell && !topLeftCornerActive);
        leftBottomBorder.SetActive(!leftCell && !bottomLeftCornerActive);
    }
}
