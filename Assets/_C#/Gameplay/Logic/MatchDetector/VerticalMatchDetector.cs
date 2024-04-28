using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class VerticalMatchDetector : MatchDetector
{
    public override List<Match> DetectMatches(Grid grid)
    {
        List<Match> matches = new();

        for (int i = 0; i < grid.Width; ++i)
        {
            for (int j = 0; j < grid.Height - 2;)
            {
                var gridCell = grid[i, j];
                if (gridCell && gridCell.HasItem && gridCell.Item is Dot dot)
                {
                    var colorGroup = dot.ColorGroup;

                    var bottomGridCell1 = grid[i, j + 1];
                    var bottomGridCell2 = grid[i, j + 2];

                    if (GridCellMatches(bottomGridCell1, colorGroup) &&
                        GridCellMatches(bottomGridCell2, colorGroup))
                    {
                        var match = new Match { type = MatchTypeEnum.Vertical };
                        do
                        {
                            match.AddCell(grid[i, j]);
                            ++j;
                        } while (j < grid.Height && GridCellMatches(grid[i, j], colorGroup));
                        
                        matches.Add(match);
                        continue;
                    }
                }

                ++j;
            }
        }

        return matches;
    }

    public override bool HasMatch(Grid grid, int x, int y)
    {
        var gridCell = grid[x, y];
        
        if (gridCell && gridCell.HasItem && gridCell.Item is Dot dot)
        {
            int length = 1 + GetTopLength(grid, x, y) + GetBottomLength(grid, x, y);

            if (length >= 3) return true;
        }
        return false;
    }

    public static int GetTopLength(Grid grid, int x, int y)
    {
        int length = 0;
        
        var gridCell = grid[x, y];
        
        if (gridCell && gridCell.HasItem && gridCell.Item is Dot dot)
        {
            int colorGroup = dot.ColorGroup;
            
            for (int j = y - 1; j >= 0; --j)
            {
                var otherCell = grid[x, j];
                if (GridCellMatches(otherCell, colorGroup))
                {
                    ++length;
                }
                else break;
            }
        }
        return length;
    }

    public static int GetBottomLength(Grid grid, int x, int y)
    {
        int length = 0;
        
        var gridCell = grid[x, y];
        
        if (gridCell && gridCell.HasItem && gridCell.Item is Dot dot)
        {
            int colorGroup = dot.ColorGroup;
            
            for (int j = y + 1; j < grid.Height; ++j)
            {
                var otherCell = grid[x, j];
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
