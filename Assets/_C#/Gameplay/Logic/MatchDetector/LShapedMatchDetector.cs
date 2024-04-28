using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class LShapedMatchDetector : MatchDetector
{
    public override List<Match> DetectMatches(Grid grid)
    {
        List<Match> matches = new();

        for (int j = 0; j < grid.Height; ++j)
        {
            for (int i = 0; i < grid.Width; ++i)
            {
                var gridCell = grid[i, j];
                if (gridCell && gridCell.HasItem && gridCell.Item is Dot dot)
                {
                    var colorGroup = dot.ColorGroup;
                    
                    //check L with corner at top left
                    var rightGridCell1 = grid[i + 1, j];
                    var rightGridCell2 = grid[i + 2, j];
                    var bottomGridCell1 = grid[i, j + 1];
                    var bottomGridCell2 = grid[i, j + 2];

                    if (GridCellMatches(rightGridCell1, colorGroup) &&
                        GridCellMatches(rightGridCell2, colorGroup) &&
                        GridCellMatches(bottomGridCell1, colorGroup) &&
                        GridCellMatches(bottomGridCell2, colorGroup))
                    {
                        var match = new Match { type = MatchTypeEnum.LShaped };
                        
                        match.AddCell(gridCell);
                        match.AddCell(rightGridCell1);
                        match.AddCell(rightGridCell2);
                        match.AddCell(bottomGridCell1);
                        match.AddCell(bottomGridCell2);

                        int k = i + 3;
                        while (k < grid.Width && GridCellMatches(grid[k, j], colorGroup))
                        {
                            match.AddCell(grid[k, j]);
                            ++k;
                        }

                        k = j + 3;
                        while (k < grid.Height && GridCellMatches(grid[i, k], colorGroup))
                        {
                            match.AddCell(grid[i, k]);
                            ++k;
                        }
                        
                        matches.Add(match);
                    }
                    
                    //check L with corner at bottom left
                    var topGridCell1 = grid[i, j - 1];
                    var topGridCell2 = grid[i, j - 2];

                    if (GridCellMatches(rightGridCell1, colorGroup) &&
                        GridCellMatches(rightGridCell2, colorGroup) &&
                        GridCellMatches(topGridCell1, colorGroup) &&
                        GridCellMatches(topGridCell2, colorGroup))
                    {
                        var match = new Match { type = MatchTypeEnum.LShaped };
                        
                        match.AddCell(gridCell);
                        match.AddCell(rightGridCell1);
                        match.AddCell(rightGridCell2);
                        match.AddCell(topGridCell1);
                        match.AddCell(topGridCell2);

                        int k = i + 3;
                        while (k < grid.Width && GridCellMatches(grid[k, j], colorGroup))
                        {
                            match.AddCell(grid[k, j]);
                            ++k;
                        }

                        k = j - 3;
                        while (k < grid.Height && GridCellMatches(grid[i, k], colorGroup))
                        {
                            match.AddCell(grid[i, k]);
                            --k;
                        }
                        
                        matches.Add(match);
                    }
                    
                    //check L with corner at top right
                    var leftGridCell1 = grid[i - 1, j];
                    var leftGridCell2 = grid[i - 2, j];

                    if (GridCellMatches(leftGridCell1, colorGroup) &&
                        GridCellMatches(leftGridCell2, colorGroup) &&
                        GridCellMatches(bottomGridCell1, colorGroup) &&
                        GridCellMatches(bottomGridCell2, colorGroup))
                    {
                        var match = new Match { type = MatchTypeEnum.LShaped };
                        
                        match.AddCell(gridCell);
                        match.AddCell(leftGridCell1);
                        match.AddCell(leftGridCell2);
                        match.AddCell(bottomGridCell1);
                        match.AddCell(bottomGridCell2);

                        int k = i -3;
                        while (k < grid.Width && GridCellMatches(grid[k, j], colorGroup))
                        {
                            match.AddCell(grid[k, j]);
                            --k;
                        }

                        k = j + 3;
                        while (k < grid.Height && GridCellMatches(grid[i, k], colorGroup))
                        {
                            match.AddCell(grid[i, k]);
                            ++k;
                        }
                        
                        matches.Add(match);
                    }
                    
                    //check L with corner at bottom right
                    if (GridCellMatches(leftGridCell1, colorGroup) &&
                        GridCellMatches(leftGridCell2, colorGroup) &&
                        GridCellMatches(topGridCell1, colorGroup) &&
                        GridCellMatches(topGridCell2, colorGroup))
                    {
                        var match = new Match { type = MatchTypeEnum.LShaped };
                        
                        match.AddCell(gridCell);
                        match.AddCell(leftGridCell1);
                        match.AddCell(leftGridCell2);
                        match.AddCell(topGridCell1);
                        match.AddCell(topGridCell2);

                        int k = i -3;
                        while (k < grid.Width && GridCellMatches(grid[k, j], colorGroup))
                        {
                            match.AddCell(grid[k, j]);
                            --k;
                        }

                        k = j - 3;
                        while (k < grid.Height && GridCellMatches(grid[i, k], colorGroup))
                        {
                            match.AddCell(grid[i, k]);
                            --k;
                        }
                        
                        matches.Add(match);
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
            bool hasTop = VerticalMatchDetector.GetTopLength(grid, x, y) >= 2;
            bool hasBottom = VerticalMatchDetector.GetBottomLength(grid, x, y) >= 2;
            bool hasLeft = HorizontalMatchDetector.GetLeftLength(grid, x, y) >= 2;
            bool hasRight = HorizontalMatchDetector.GetRightLength(grid, x, y) >= 2;

            bool hasVertical = hasTop || hasBottom;
            bool hasHorizontal = hasLeft || hasRight;

            return hasVertical && hasHorizontal;
        }
        return false;
    }
}
