using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class HorizontalMatchDetector : MatchDetector
{
    public override List<Match> DetectMatches(Grid grid)
    {
        List<Match> matches = new();

        for (int j = 0; j < grid.Height; ++j)
        {
            for (int i = 0; i < grid.Width - 2;)
            {
                var gridCell = grid[i, j];
                if (gridCell && gridCell.HasItem && gridCell.Item is Dot dot)
                {
                    var colorGroup = dot.ColorGroup;

                    var rightGridCell1 = grid[i + 1, j];
                    var rightGridCell2 = grid[i + 2, j];

                    if (GridCellMatches(rightGridCell1, colorGroup) &&
                        GridCellMatches(rightGridCell2, colorGroup))
                    {
                        var match = new Match { type = MatchTypeEnum.Horizontal };
                        do
                        {
                            match.AddCell(grid[i, j]);
                            ++i;
                        } while (i < grid.Width && GridCellMatches(grid[i, j], colorGroup));
                        
                        matches.Add(match);
                        continue;
                    }
                }

                ++i;
            }
        }

        return matches;
    }

    public override bool HasMatch(Grid grid, int x, int y)
    {
        var gridCell = grid[x, y];
        
        if (gridCell && gridCell.HasItem && gridCell.Item is Dot dot)
        {
            int length = 1 + GetLeftLength(grid, x, y) + GetRightLength(grid, x, y);

            if (length >= 3)
            {
                return true;
            }
        }
        return false;
    }

    public static int GetLeftLength(Grid grid, int x, int y)
    {
        int length = 0;
        
        var gridCell = grid[x, y];
        
        if (gridCell && gridCell.HasItem && gridCell.Item is Dot dot)
        {
            int colorGroup = dot.ColorGroup;
            
            for (int i = x - 1; i >= 0; --i)
            {
                var otherCell = grid[i, y];
                if (GridCellMatches(otherCell, colorGroup))
                {
                    ++length;
                }
                else break;
            }
        }
        return length;
    }

    public static int GetRightLength(Grid grid, int x, int y)
    {
        int length = 0;
        
        var gridCell = grid[x, y];
        
        if (gridCell && gridCell.HasItem && gridCell.Item is Dot dot)
        {
            int colorGroup = dot.ColorGroup;
            
            for (int i = x + 1; i < grid.Width; ++i)
            {
                var otherCell = grid[i, y];
                if (GridCellMatches(otherCell, colorGroup))
                {
                    ++length;
                }
                else break;
            }
        }
        return length;
    }
}
