using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class SquareMatchDetector : MatchDetector
{
    public override List<Match> DetectMatches(Grid grid)
    {
        List<Match> matches = new();
        List<GridCell> cellsToIgnore = new();

        for (int i = 0; i < grid.Width - 1; ++i)
        {
            for (int j = 0; j < grid.Height - 1; ++j)
            {
                var gridCell = grid[i, j];
                if (gridCell)
                {
                    if (cellsToIgnore.Contains(gridCell)) continue;
                    
                    if (gridCell.HasItem && gridCell.Item is Dot dot)
                    {
                        var colorGroup = dot.ColorGroup;

                        var rightCell = grid[i + 1, j];
                        var bottomCell = grid[i, j + 1];
                        var bottomRightCell = grid[i + 1, j + 1];

                        if (GridCellMatches(rightCell, colorGroup) &&
                            GridCellMatches(bottomCell, colorGroup) &&
                            GridCellMatches(bottomRightCell, colorGroup))
                        {
                            var match = new Match { type = MatchTypeEnum.Square };

                            match.AddCell(gridCell);
                            match.AddCell(rightCell);
                            match.AddCell(bottomCell);
                            match.AddCell(bottomRightCell);

                            cellsToIgnore.Add(rightCell);
                            cellsToIgnore.Add(bottomCell);
                            cellsToIgnore.Add(bottomRightCell);

                            GridCell[] extraCells = 
                            {
                                grid[i, j - 1], //top left
                                grid[i + 1, j - 1], //top right
                                grid[i + 2, j], //right top
                                grid[i + 2, j + 1], //right bottom
                                grid[i, j + 2], //bottom left
                                grid[i + 1, j + 2], //bottom right
                                grid[i - 1, j], //left top
                                grid[i - 1, j + 1] //left bottom
                            };

                            foreach (var extraCell in extraCells)
                            {
                                if (GridCellMatches(extraCell, colorGroup))
                                {
                                    match.AddCell(extraCell);
                                    cellsToIgnore.Add(extraCell);
                                }
                            }
                            
                            matches.Add(match);
                        }
                    }
                }
            }
        }

        return matches;
    }

    public override bool HasMatch(Grid grid, int x, int y)
    {
        var gridCell = grid[x, y];

        if (gridCell && gridCell.HasItem && gridCell.Item is Dot dot)
        {
            int colorGroup = dot.ColorGroup;
            
            

            GridCell[] neighboringCells = 
            {
                grid[x, y - 1], //top
                grid[x + 1, y - 1], //top right
                grid[x + 1, y], //right
                grid[x + 1, y + 1], //bottom right
                grid[x, y + 1], //bottom
                grid[x - 1, y + 1], //bottom left
                grid[x - 1, y], //left
                grid[x - 1, y - 1], //top left
            };

            for (int i = 0; i <= 6; i += 2)
            {
                if (GridCellMatches(neighboringCells[i % neighboringCells.Length], colorGroup) &&
                    GridCellMatches(neighboringCells[(i + 1) % neighboringCells.Length], colorGroup) &&
                    GridCellMatches(neighboringCells[(i + 2) % neighboringCells.Length], colorGroup))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
