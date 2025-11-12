using Harmony;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WorldEditMod
{
    static internal class Operations
    {
        static public void Operate(ref List<Vector2Int> selection)
        {
            if (Core.Instance.Data.operatorMode == Data.OperatorMode.None)
                return;

            List<Vector2Int> operatedSelection = null;
            switch (Core.Instance.Data.operatorMode)
            {
                case Data.OperatorMode.Hollow:
                    operatedSelection = Hollow(selection);
                    break;
            }

            selection = operatedSelection;
        }

        static List<Vector2Int> Hollow(List<Vector2Int> selection)
        {
            var outsideSelection = new List<Vector2Int>();
            foreach (var square in selection)
            {
                var neighbours = GetNeighbours(selection, square);
                if (neighbours.Count < 4)
                {
                    outsideSelection.Add(square);
                }
            }
            return outsideSelection;
        }

        static readonly Vector2Int[] Neighbours = [
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
        ];
        static List<Vector2Int> GetNeighbours(List<Vector2Int> selection, Vector2Int tilePos)
        {
            var neighbours = new List<Vector2Int>();
            foreach (var neighbourDir in Neighbours)
            {
                var neighbourPos = tilePos + neighbourDir;
                if (selection.Contains(neighbourPos))
                {
                    neighbours.Add(neighbourPos);
                }
            }
            return neighbours;
        }
    }
}
