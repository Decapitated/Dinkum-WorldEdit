using Harmony;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WorldEditMod
{
    static internal class Operations
    {
        static public void Operate(ref Squares newSquares)
        {
            if (Core.Instance.Data.operatorMode == Data.OperatorMode.None)
                return;

            List<Vector2Int> operatedTiles = null;
            switch (Core.Instance.Data.operatorMode)
            {
                case Data.OperatorMode.Hollow:
                    operatedTiles = Hollow(newSquares);
                    break;
            }

            var operatedSquares = new Squares();
            foreach (var tile in operatedTiles)
            {
                Transfer(newSquares, operatedSquares, tile);
            }
            foreach (var square in newSquares)
            {
                GameObject.Destroy(square.Value.gameObject);
            }

            newSquares = operatedSquares;
        }

        static List<Vector2Int> Hollow(Squares newSquares)
        {
            var outsideSquares = new List<Vector2Int>();
            foreach (var square in newSquares)
            {
                var neighbours = GetNeighbours(newSquares, square.Key);
                if (neighbours.Count < 4)
                {
                    outsideSquares.Add(square.Key);
                }
            }
            return outsideSquares;
        }

        static void Transfer(Squares oldSquares, Squares newSquares, Vector2Int currentTile)
        {
            var newSquare = oldSquares[currentTile];
            oldSquares.Remove(currentTile);
            newSquares.Add(currentTile, newSquare);
        }

        static readonly Vector2Int[] Neighbours = [
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
        ];
        static List<Vector2Int> GetNeighbours(Squares squares, Vector2Int tilePos)
        {
            var neighbours = new List<Vector2Int>();
            foreach (var neighbourDir in Neighbours)
            {
                var neighbourPos = tilePos + neighbourDir;
                if (squares.ContainsKey(neighbourPos))
                {
                    neighbours.Add(neighbourPos);
                }
            }
            return neighbours;
        }
    }
}
