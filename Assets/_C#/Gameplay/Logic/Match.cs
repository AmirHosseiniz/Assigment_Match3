using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    [System.Serializable]
    public class Match
    {
        public MatchTypeEnum type;
        public List<GridCell> cells = new();

        public void AddCell(GridCell cell) => cells.Add(cell);
    }

    public enum MatchTypeEnum
    {
        Horizontal,
        Vertical,
        TShaped,
        LShaped,
        Square,
    }
}
