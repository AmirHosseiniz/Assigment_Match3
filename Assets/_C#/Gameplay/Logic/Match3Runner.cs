using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using GameplayEnums.Cells;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Grid))]
[RequireComponent(typeof(RandomDotColorLeniencyProvider))]
public class Match3Runner : MonoBehaviour
{
    public event System.Action<Vector3, int, int> BoosterCreated;

    [SerializeField] float swapDuration = .25f;
    [SerializeField] AnimationCurve swapCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] float baseGravity = 9f;
    [SerializeField] AnimationCurve gravityMultiplierCurve = AnimationCurve.Constant(0f, 0f, 1f);
    [SerializeField] float fallInterval;
    [SerializeField] float mergeDuration = .25f;
    [Header("Grid items")]
    [SerializeField] Dot dotPrefab;
    [SerializeField] Block blockPrefab;
    [Header("Events")]
    [SerializeField] UnityEvent onInvalidSwap;

    public ScriptableLevelData loadedLevel { get; private set; }

    public Grid Grid { get; private set; }

    List<Swap> possibleSwaps = new();

    public Swap LastSwap { get; private set; }

    public bool IsSwapping { get; private set; }
    public bool IsApplyingGravity { get; private set; }

    int consecutiveMatch;

    Dictionary<GridCell, Coroutine> receivingGravityCells = new();
    List<GridCell> matchingGridCells = new();

    RandomDotColorLeniencyProvider randomDotColorLeniencyProvider;

    Transform GridItemParent => Grid.ItemParent;

    readonly MatchDetector horizontalMatchDetector = new HorizontalMatchDetector();
    readonly MatchDetector verticalMatchDetector = new VerticalMatchDetector();
    readonly MatchDetector squareMatchDetector = new SquareMatchDetector();
    readonly MatchDetector lShapedMatchDetector = new LShapedMatchDetector();
    readonly MatchDetector tShapedMatchDetector = new TShapedMatchDetector();

    public void GenerateAndRun(ScriptableLevelData level)
    {
        loadedLevel = level;

        var levelData = level.LevelData;
        var gridData = levelData.GridData;
        int width = gridData.Width;
        int height = gridData.Height;

        Grid = GetComponent<Grid>();
        randomDotColorLeniencyProvider = GetComponent<RandomDotColorLeniencyProvider>();

        Grid.CreateCells(gridData);

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                var cellData = gridData[i, j];

                if (cellData.type != TypeEnum.hole)
                {
                    var gridCell = Grid[i, j];

                    Vector3 position = gridCell.transform.position;
                    Quaternion rotation = GridItemParent.rotation;

                    IGridItem item = null;

                    if (cellData.type == TypeEnum.dot)
                    {
                        int colorGroup = cellData.color >= 0 ? cellData.color : GetRandomColorForCell(i, j, 0f);
                        item = dotPrefab.Create(colorGroup, position, rotation, GridItemParent);
                    }
                    else if (cellData.type == TypeEnum.block)
                    {
                        item = blockPrefab.Create(position, rotation, GridItemParent);
                    }

                    if (item != null)
                    {
                        if (!gridCell.TrySetItem(item))
                        {
                            Debug.LogError("Failed to set item to grid cell!");
                        }
                    }
                }
            }
        }

        Grid.OnItemsTypeDefined();

        UpdatePossibleSwaps();
        CheckRegeneration();
    }

    int GetRandomColorForCell(int x, int y, float leniency)
    {
        List<int> eligibleColors = new();
        for (int i = 0; i < loadedLevel.LevelData.ColorCount; ++i)
        {
            eligibleColors.Add(i);
        }

        if (x > 1)
        {
            var leftCell1 = Grid[x - 1, y];
            var leftCell2 = Grid[x - 2, y];

            if (leftCell1 && leftCell1.HasItem && leftCell1.Item is Dot leftDot1 &&
                leftCell2 && leftCell2.HasItem && leftCell2.Item is Dot leftDot2 &&
                leftDot1.ColorGroup == leftDot2.ColorGroup)
            {
                if (Random.value >= leniency) eligibleColors.Remove(leftDot1.ColorGroup);
            }
        }

        if (y > 1)
        {
            var topCell1 = Grid[x, y - 1];
            var topCell2 = Grid[x, y - 2];

            if (topCell1 && topCell1.HasItem && topCell1.Item is Dot topDot1 &&
                topCell2 && topCell2.HasItem && topCell2.Item is Dot topDot2 &&
                topDot1.ColorGroup == topDot2.ColorGroup)
            {
                if (Random.value >= leniency) eligibleColors.Remove(topDot1.ColorGroup);
            }
        }

        if (x > 0 && y > 0)
        {
            var topCell = Grid[x, y - 1];
            var leftCell = Grid[x - 1, y];
            var topLeftCell = Grid[x - 1, y - 1];

            if (topCell && topCell.HasItem && topCell.Item is Dot topDot &&
                leftCell && leftCell.HasItem && leftCell.Item is Dot leftDot &&
                topLeftCell && topLeftCell.HasItem && topLeftCell.Item is Dot topLeftDot &&
                topDot.ColorGroup == leftDot.ColorGroup && topDot.ColorGroup == topLeftDot.ColorGroup)
            {
                if (Random.value >= leniency) eligibleColors.Remove(topDot.ColorGroup);
            }
        }

        if (eligibleColors.Count > 0)
        {
            return eligibleColors[Random.Range(0, eligibleColors.Count)];
        }

        Debug.LogWarning($"No eligible colors found for cell at ({x}, {y}) that wouldn't create a match, returning a random color");
        return Random.Range(0, loadedLevel.LevelData.ColorCount);
    }

    public void Click(GridCell cell)
    {
        if (!receivingGravityCells.ContainsKey(cell) && !matchingGridCells.Contains(cell))
        {
            if (cell.Click())
            {
                StartCoroutine(Click());
            }
        }

        IEnumerator Click()
        {
            yield return null;

            ApplyGravity();
            UpdatePossibleSwaps();
        }
    }

    public void Swap(GridCell cellA, GridCell cellB)
    {
        if (!IsSwapping) StartCoroutine(Swap());

        IEnumerator Swap()
        {
            IsSwapping = true;

            var itemA = cellA.Item;
            var itemB = cellB.Item;


            if (itemA.Type != TypeEnum.block && itemB.Type != TypeEnum.block &&
                !receivingGravityCells.ContainsKey(cellA) && !receivingGravityCells.ContainsKey(cellB) &&
                !matchingGridCells.Contains(cellA) && !matchingGridCells.Contains(cellB))
            {
                itemA.OnTop = true;

                float startTime = Time.time;
                while (true)
                {
                    float t = (Time.time - startTime) / swapDuration;
                    float curveT = swapCurve.Evaluate(t);
                    itemA.transform.position = Vector3.Lerp(cellA.transform.position, cellB.transform.position, curveT);
                    itemB.transform.position = Vector3.Lerp(cellB.transform.position, cellA.transform.position, curveT);
                    if (t >= 1f) break;
                    yield return null;
                }

                itemA.OnTop = false;

                if (IsSwapPossible(cellA, cellB))
                {
                    LastSwap = new Swap { cellA = cellA, cellB = cellB };

                    bool unsetSuccessA = cellA.TryUnsetItem();
                    bool unsetSuccessB = cellB.TryUnsetItem();

                    if (unsetSuccessA && unsetSuccessB)
                    {
                        cellA.TrySetItem(itemB);
                        cellB.TrySetItem(itemA);

                        HandleMatches(true);
                    }
                    else //unsetting items failed for at least one of the cells, so cancel swapping, and re-set the items that were succesfully unset
                    {
                        if (unsetSuccessA) cellA.TrySetItem(itemA);
                        if (unsetSuccessB) cellB.TrySetItem(itemB);
                        itemA.transform.position = cellA.transform.position;
                        itemB.transform.position = cellB.transform.position;
                    }
                }
                else
                {
                    itemA.OnTop = true;

                    startTime = Time.time;
                    while (true)
                    {
                        float t = (Time.time - startTime) / swapDuration;
                        float curveT = swapCurve.Evaluate(t);
                        itemA.transform.position =
                            Vector3.Lerp(cellB.transform.position, cellA.transform.position, curveT);
                        itemB.transform.position =
                            Vector3.Lerp(cellA.transform.position, cellB.transform.position, curveT);
                        if (t >= 1f) break;
                        yield return null;
                    }

                    itemA.OnTop = false;

                    onInvalidSwap.Invoke();
                }
            }
            IsSwapping = false;
        }
    }

    bool IsSwapPossible(GridCell cellA, GridCell cellB) => possibleSwaps.FindIndex(swap => swap.Matches(cellA, cellB)) >= 0;

    void UpdatePossibleSwaps()
    {
        possibleSwaps = GetPossibleSwaps();
    }

    List<Swap> GetPossibleSwaps()
    {
        List<Swap> possibleSwaps = new();

        int width = Grid.Width;
        int height = Grid.Height;

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                var cell = Grid[i][j];
                if (cell && IsSwappable(cell))
                {
                    if (i < width - 1)
                    {
                        var otherCell = Grid[i + 1][j];
                        if (otherCell && IsSwappable(otherCell))
                        {
                            Grid[i][j] = otherCell;
                            Grid[i + 1][j] = cell;

                            if (HasMatch(i, j) || HasMatch(i + 1, j))
                            {
                                var swap = new Swap { cellA = cell, cellB = otherCell };
                                possibleSwaps.Add(swap);
                            }

                            Grid[i][j] = cell;
                            Grid[i + 1][j] = otherCell;
                        }
                    }

                    if (j < height - 1)
                    {
                        var otherCell = Grid[i][j + 1];
                        if (otherCell && IsSwappable(otherCell))
                        {
                            Grid[i][j] = otherCell;
                            Grid[i][j + 1] = cell;

                            if (HasMatch(i, j) || HasMatch(i, j + 1))
                            {
                                var swap = new Swap { cellA = cell, cellB = otherCell };
                                possibleSwaps.Add(swap);
                            }

                            Grid[i][j] = cell;
                            Grid[i][j + 1] = otherCell;
                        }
                    }
                }
            }
        }

        return possibleSwaps;

        bool IsSwappable(GridCell gridCell)
        {
            return !gridCell.HasItem || gridCell.Item.Type != TypeEnum.block;
        }
    }

    bool HandleMatches(bool isPlayerMatch)
    {
        List<Match> matches = new();

        var tShapedMatches = tShapedMatchDetector.DetectMatches(Grid);
        var lShapedMatches = lShapedMatchDetector.DetectMatches(Grid);
        var squareMatches = squareMatchDetector.DetectMatches(Grid);
        var horizontalMatches = horizontalMatchDetector.DetectMatches(Grid);
        var verticalMatches = verticalMatchDetector.DetectMatches(Grid);

        RemoveInvalidMatches(tShapedMatches);
        RemoveInvalidMatches(lShapedMatches);
        RemoveInvalidMatches(squareMatches);
        RemoveInvalidMatches(horizontalMatches);
        RemoveInvalidMatches(verticalMatches);


        if (tShapedMatches.Count > 0)
        {
            matches.AddRange(tShapedMatches);
            foreach (var horizontalMatch in horizontalMatches)
            {
                bool found = false;
                foreach (var tShapedMatch in tShapedMatches)
                {
                    if (horizontalMatch.cells.Find(cell => tShapedMatch.cells.Contains(cell)) != null)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) matches.Add(horizontalMatch);
            }
        }
        else if (lShapedMatches.Count > 0)
        {
            matches.AddRange(lShapedMatches);
            foreach (var horizontalMatch in horizontalMatches)
            {
                bool found = false;
                foreach (var lShapedMatch in lShapedMatches)
                {
                    if (horizontalMatch.cells.Find(cell => lShapedMatch.cells.Contains(cell)) != null)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) matches.Add(horizontalMatch);
            }

            foreach (var verticalMatch in verticalMatches)
            {
                bool found = false;
                foreach (var lShapedMatch in lShapedMatches)
                {
                    if (verticalMatch.cells.Find(cell => lShapedMatch.cells.Contains(cell)) != null)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) matches.Add(verticalMatch);
            }
        }
        else if (squareMatches.Count > 0)
        {
            matches.AddRange(squareMatches);
            foreach (var horizontalMatch in horizontalMatches)
            {
                bool found = false;
                foreach (var squareMatch in squareMatches)
                {
                    if (horizontalMatch.cells.Find(cell => squareMatch.cells.Contains(cell)) != null)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) matches.Add(horizontalMatch);
            }

            foreach (var verticalMatch in verticalMatches)
            {
                bool found = false;
                foreach (var squareMatch in squareMatches)
                {
                    if (verticalMatch.cells.Find(cell => squareMatch.cells.Contains(cell)) != null)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) matches.Add(verticalMatch);
            }

        }
        else if (horizontalMatches.Count > 0)
        {
            matches.AddRange(horizontalMatches);
            foreach (var verticalMatch in verticalMatches)
            {
                bool found = false;
                foreach (var horizontalMatch in horizontalMatches)
                {
                    if (verticalMatch.cells.Find(cell => horizontalMatch.cells.Contains(cell)) != null)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) matches.Add(verticalMatch);
            }

        }
        else
        {
            matches.AddRange(verticalMatches);
        }

        if (matches.Count > 0)
        {
            StartCoroutine(HandleMatches());
            return true;
        }
        return false;

        IEnumerator HandleMatches()
        {
            foreach (var match in matches)
            {
                matchingGridCells.AddRange(match.cells);
            }

            foreach (var match in matches)
            {
                foreach (var cell in match.cells)
                {
                    cell.Explode();
                    matchingGridCells.RemoveAll(listCell => listCell == cell);
                }
            }

            yield return null;

            if (isPlayerMatch) consecutiveMatch = 0;
            else ++consecutiveMatch;

            ApplyGravity();
            UpdatePossibleSwaps();
        }

        void RemoveInvalidMatches(List<Match> matches)
        {
            for (int i = matches.Count - 1; i >= 0; --i)
            {
                var match = matches[i];
                bool invalid = false;
                foreach (var cell in match.cells)
                {
                    if (receivingGravityCells.ContainsKey(cell) || matchingGridCells.Contains(cell))
                    {
                        invalid = true;
                        break;
                    }
                }
                if (invalid) matches.RemoveAt(i);
            }
        }
    }

    void ApplyGravity(float delay = .0f)
    {
        StartCoroutine(ApplyGravity());

        IEnumerator ApplyGravity()
        {
            IsApplyingGravity = true;
            if (delay > 0f) yield return new WaitForSeconds(delay);
            yield return ApplyGravityInternal();
            UpdatePossibleSwaps();
            if (!HandleMatches(false))
            {
                IsApplyingGravity = false;
            }
            CheckRegeneration();
        }

        IEnumerator ApplyGravityInternal()
        {
            float gravity = baseGravity * gravityMultiplierCurve.Evaluate(consecutiveMatch);

            int[] fallingCountsPerColumn = new int[Grid.Width];
            Array.Fill(fallingCountsPerColumn, 0);
            //make existing items fall
            for (int i = 0; i < Grid.Width; ++i)
            {
                for (int j = Grid.Height - 1; j >= 0; --j)
                {
                    var gridCell = Grid[i, j];
                    if (gridCell && gridCell.HasItem && gridCell.Item.Type != TypeEnum.block)
                    {
                        int bottom = -1;
                        for (int k = j; k < Grid.Height; ++k)
                        {
                            var belowCell = Grid[i, k];
                            if (belowCell)
                            {
                                if (!belowCell.HasItem)
                                {
                                    bottom = k;
                                }
                                else if (belowCell.Item.Type is TypeEnum.block)
                                {
                                    break;
                                }
                            }
                        }

                        if (bottom >= 0)
                        {
                            var targetGridCell = Grid[i, bottom];
                            float gravityDelay = fallingCountsPerColumn[i] * fallInterval;
                            if (receivingGravityCells.ContainsKey(targetGridCell))
                            {
                                var coroutine = receivingGravityCells[targetGridCell];
                                StopCoroutine(coroutine);
                                receivingGravityCells.Remove(targetGridCell);
                            }
                            ApplyGravityToGridCell(gridCell, targetGridCell, gravity, gravityDelay);
                            ++fallingCountsPerColumn[i];
                        }
                    }
                }
            }

            //create new items for empty cells
            for (int i = 0; i < Grid.Width; ++i)
            {
                int emptyCount = 0;
                for (int j = Grid.Height - 1; j >= 0; --j)
                {
                    var gridCell = Grid[i, j];
                    if (gridCell)
                    {
                        if (!gridCell.HasItem)
                        {
                            ++emptyCount;
                        }
                        else if (gridCell.Item.Type is TypeEnum.block)
                        {
                            break;
                        }
                    }
                }

                if (emptyCount > 0)
                {
                    for (int j = 0; j < Grid.Height; ++j)
                    {
                        var gridCell = Grid[i, j];
                        if (gridCell)
                        {
                            if (!gridCell.HasItem)
                            {
                                Vector3 topPos = Grid.GetPosition(i, 0);
                                Vector3 position = topPos + emptyCount * Grid.Formation.CellSize.y * Grid.Up;
                                --emptyCount;
                                Quaternion rotation = GridItemParent.rotation;
                                int colorGroup = GetRandomColorForCell(i, j, randomDotColorLeniencyProvider.Leniency);
                                var dot = dotPrefab.Create(colorGroup, position, rotation, GridItemParent);
                                if (!gridCell.TrySetItem(dot))
                                {
                                    Debug.LogError("Failed to set item to grid cell!");
                                }
                                float gravityDelay = (fallingCountsPerColumn[i] + emptyCount) * fallInterval;
                                if (receivingGravityCells.ContainsKey(gridCell))
                                {
                                    var coroutine = receivingGravityCells[gridCell];
                                    StopCoroutine(coroutine);
                                    receivingGravityCells.Remove(gridCell);
                                }
                                ApplyGravityToItem(dot, gridCell, gravity, gravityDelay);
                            }
                            else if (gridCell.Item.Type is TypeEnum.block)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            yield return new WaitUntil(() => receivingGravityCells.Count <= 0);
        }

        void ApplyGravityToGridCell(GridCell fromCell, GridCell toCell, float gravity, float delay)
        {
            var item = fromCell.Item;
            fromCell.TryUnsetItem();
            toCell.TrySetItem(item);
            ApplyGravityToItem(item, toCell, gravity, delay);
        }

        void ApplyGravityToItem(IGridItem item, GridCell targetGridCell, float gravity, float delay)
        {
            var coroutine = StartCoroutine(ApplyGravityToItem());
            receivingGravityCells.Add(targetGridCell, coroutine);

            IEnumerator ApplyGravityToItem()
            {
                Vector3 targetPosition = targetGridCell.transform.position;
                if (delay > 0f) yield return new WaitForSeconds(delay);

                float fallSpeed = 0f;
                while (item.transform.position != targetPosition)
                {
                    fallSpeed += gravity * Time.deltaTime;
                    item.transform.position = Vector3.MoveTowards(item.transform.position, targetPosition, fallSpeed * Time.deltaTime);
                    yield return null;
                }

                item.OnGravityApplied();
                receivingGravityCells.Remove(targetGridCell);
            }
        }
    }

    void CheckRegeneration()
    {
        if (possibleSwaps.Count > 0) return;

        RegenerateGrid();
    }

    void RegenerateGrid()
    {
        Debug.LogWarning("Regenerating grid");
        for (int i = 0; i < Grid.Width; ++i)
        {
            for (int j = 0; j < Grid.Height; ++j)
            {
                var gridCell = Grid[i, j];
                if (gridCell && gridCell.HasItem && gridCell.Item is Dot oldDot)
                {
                    gridCell.TryUnsetItem();
                    oldDot.gameObject.SetActive(false);

                    Vector3 position = gridCell.transform.position;
                    Quaternion rotation = GridItemParent.rotation;
                    int colorGroup = GetRandomColorForCell(i, j, 0f);
                    var newDot = dotPrefab.Create(colorGroup, position, rotation, GridItemParent);

                    gridCell.TrySetItem(newDot);
                }
            }
        }

        UpdatePossibleSwaps();
        CheckRegeneration();
    }



    bool HasMatch(int x, int y)
    {
        if (horizontalMatchDetector.HasMatch(Grid, x, y)) return true;
        if (verticalMatchDetector.HasMatch(Grid, x, y)) return true;
        if (squareMatchDetector.HasMatch(Grid, x, y)) return true;
        //it's not necessary to check LShaped and TShaped matches since they contain horizontal/vertical matches, which are being checked for
        return false;
    }
}

[System.Serializable]
public class Swap
{
    public GridCell cellA;
    public GridCell cellB;

    public bool Matches(GridCell cellA, GridCell cellB)
    {
        return (this.cellA == cellA && this.cellB == cellB) || (this.cellA == cellB && this.cellB == cellA);
    }

}
