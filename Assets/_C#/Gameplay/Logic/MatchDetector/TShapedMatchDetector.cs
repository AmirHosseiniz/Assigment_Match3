using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class TShapedMatchDetector : MatchDetector
{
    public override List<Match> DetectMatches(Grid grid)
    {
        List<Match> matches = new();

        for (int j = 0; j < grid.Height; ++j)
        {
            for (int i = 0; i < grid.Width - 1; ++i)
            {
                var gridCell = grid[i, j];
                if (gridCell && gridCell.HasItem && gridCell.Item is Dot dot)
                {
                    var colorGroup = dot.ColorGroup;
                    
                    //check T with head on left, pivot at center of head
                    var rightGridCell1 = grid[i + 1, j];
                    var rightGridCell2 = grid[i + 2, j];
                    var topGridCell1 = grid[i, j - 1];
                    var bottomGridCell1 = grid[i, j + 1];

                    if (GridCellMatches(rightGridCell1, colorGroup) &&
                        GridCellMatches(rightGridCell2, colorGroup) &&
                        GridCellMatches(topGridCell1, colorGroup) &&
                        GridCellMatches(bottomGridCell1, colorGroup))
                    {
                        var match = new Match { type = MatchTypeEnum.TShaped };
                        
                        match.AddCell(gridCell);
                        match.AddCell(rightGridCell1);
                        match.AddCell(rightGridCell2);
                        match.AddCell(topGridCell1);
                        match.AddCell(bottomGridCell1);

                        int k = i + 3;
                        while (k < grid.Width && GridCellMatches(grid[k, j], colorGroup))
                        {
                            match.AddCell(grid[k, j]);
                            ++k;
                        }

                        k = j - 2;
                        while (k >= 0 && GridCellMatches(grid[i, k], colorGroup))
                        {
                            match.AddCell(grid[i, k]);
                            --k;
                        }

                        k = j + 2;
                        while (k < grid.Height && GridCellMatches(grid[i, k], colorGroup))
                        {
                            match.AddCell(grid[i, k]);
                            ++k;
                        }

                        matches.Add(match);
                    }

                    //check T with head on right, pivot at the bottom of pillar
                    var rightGridCell2topGridCell1 = grid[i + 2, j - 1];
                    var rightGridCell2bottomGridCell1 = grid[i + 2, j + 1];

                    if (GridCellMatches(rightGridCell1, colorGroup) &&
                        GridCellMatches(rightGridCell2, colorGroup) &&
                        GridCellMatches(rightGridCell2topGridCell1, colorGroup) &&
                        GridCellMatches(rightGridCell2bottomGridCell1, colorGroup))
                    {
                        var match = new Match { type = MatchTypeEnum.TShaped };
                        
                        match.AddCell(gridCell);
                        match.AddCell(rightGridCell1);
                        match.AddCell(rightGridCell2);
                        match.AddCell(rightGridCell2topGridCell1);
                        match.AddCell(rightGridCell2bottomGridCell1);

                        int k = i - 1;
                        while (k >= 0 && GridCellMatches(grid[k, j], colorGroup))
                        {
                            match.AddCell(grid[k, j]);
                            --k;
                        }

                        k = j - 2;
                        while (k >= 0 && GridCellMatches(grid[i + 2, k], colorGroup))
                        {
                            match.AddCell(grid[i + 2, k]);
                            --k;
                        }

                        k = j + 2;
                        while (k < grid.Height && GridCellMatches(grid[i + 2, k], colorGroup))
                        {
                            match.AddCell(grid[i + 2, k]);
                            ++k;
                        }

                        matches.Add(match);
                    }

                    //check T with head on bottom, pivot at center of head
                    var leftGridCell1 = grid[i - 1, j];
                    var topGridCell2 = grid[i, j - 2];
                    
                    if (GridCellMatches(leftGridCell1, colorGroup) &&
                        GridCellMatches(rightGridCell1, colorGroup) &&
                        GridCellMatches(topGridCell1, colorGroup) &&
                        GridCellMatches(topGridCell2, colorGroup))
                    {
                        var match = new Match { type = MatchTypeEnum.TShaped };
                        
                        match.AddCell(gridCell);
                        match.AddCell(leftGridCell1);
                        match.AddCell(rightGridCell1);
                        match.AddCell(topGridCell1);
                        match.AddCell(topGridCell2);

                        int k = i - 2;
                        while (k >= 0 && GridCellMatches(grid[k, j], colorGroup))
                        {
                            match.AddCell(grid[k, j]);
                            --k;
                        }

                        k = i + 2;
                        while (k < grid.Width && GridCellMatches(grid[k, j], colorGroup))
                        {
                            match.AddCell(grid[k, j]);
                            ++k;
                        }

                        k = j - 3;
                        while (k >= 0 && GridCellMatches(grid[i, k], colorGroup))
                        {
                            match.AddCell(grid[i, k]);
                            --k;
                        }

                        matches.Add(match);
                    }

                    //check T with head at top, pivot at center of head
                    var bottomGridCell2 = grid[i, j + 2];
                    
                    if (GridCellMatches(leftGridCell1, colorGroup) &&
                        GridCellMatches(rightGridCell1, colorGroup) &&
                        GridCellMatches(bottomGridCell1, colorGroup) &&
                        GridCellMatches(bottomGridCell2, colorGroup))
                    {
                        var match = new Match { type = MatchTypeEnum.TShaped };
                        
                        match.AddCell(gridCell);
                        match.AddCell(leftGridCell1);
                        match.AddCell(rightGridCell1);
                        match.AddCell(bottomGridCell1);
                        match.AddCell(bottomGridCell2);

                        int k = i - 2;
                        while (k >= 0 && GridCellMatches(grid[k, j], colorGroup))
                        {
                            match.AddCell(grid[k, j]);
                            --k;
                        }

                        k = i + 2;
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
            int topLength = VerticalMatchDetector.GetTopLength(grid, x, y);
            int bottomLength = VerticalMatchDetector.GetBottomLength(grid, x, y);
            int leftLength = HorizontalMatchDetector.GetLeftLength(grid, x, y);
            int rightLength = HorizontalMatchDetector.GetRightLength(grid, x, y);
            
            //check vertical T 
            bool hasHorizontal = leftLength > 0 && rightLength > 0;
            if (hasHorizontal)
            {
                bool hasTop = topLength >= 2;
                if (hasTop) return true;

                bool hasBottom = bottomLength >= 2;
                if (hasBottom) return true;
            }

            //check horizontal T
            
            bool hasVertical = topLength > 0 && bottomLength > 0;
            if (hasVertical)
            {
                bool hasLeft = leftLength > 2;
                if (hasLeft) return true;

                bool hasRight = rightLength > 2;
                if (hasRight) return true;
            }
        }
        return false;
    }
}
