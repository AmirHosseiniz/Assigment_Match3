using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public abstract class MatchDetector
{
    public abstract List<Match> DetectMatches(Grid grid);

    public abstract bool HasMatch(Grid grid, int x, int y);

    protected static bool GridCellMatches(GridCell gridCell, int colorGroup)
    {
        return gridCell && gridCell.HasItem && gridCell.Item is Dot dot && dot.ColorGroup == colorGroup;
    }
}
