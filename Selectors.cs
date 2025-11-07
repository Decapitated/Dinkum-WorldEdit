using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WorldEditMod
{
    static internal class Selectors
    {
        static bool ShouldSkip(Vector2Int tile)
        {
            var isWater = WorldManager.Instance.waterMap[tile.x, tile.y];
            return (Core.Instance.Data.ignoreWater && isWater);
        }

        static internal Squares Rectangle(Vector2Int startPos, Vector2Int endPos)
        {
            Squares newSquares = [];
            Vector2Int diff = endPos - startPos;
            int xDir = (int)Mathf.Sign(diff.x);
            int zDir = (int)Mathf.Sign(diff.y);
            for (int z = 0; z <= Mathf.Abs(diff.y); z++)
            {
                for (int x = 0; x <= Mathf.Abs(diff.x); x++)
                {
                    var currentTile = startPos + new Vector2Int(x * xDir, z * zDir);
                    if (ShouldSkip(currentTile))
                    {
                        continue;
                    }
                    Core.Instance.Measure.TransferOrAdd(newSquares, currentTile);
                }
            }
            return newSquares;
        }

        static internal Squares Circle(Vector2Int startPos, Vector2Int endPos)
        {
            int dx = endPos.x - startPos.x;
            int dy = endPos.y - startPos.y;

            int absDx = Mathf.Abs(dx);
            int absDy = Mathf.Abs(dy);

            // Smoothly blend between average and max distance
            // - Diagonals get a moderate radius
            // - Horizontal/vertical lines get full length
            float blended = (absDx + absDy + Mathf.Max(absDx, absDy)) / 3f;

            // Use the larger of the blended or true Euclidean distance
            int radius = Mathf.CeilToInt(Mathf.Max(blended, Mathf.Sqrt(dx * dx + dy * dy)));

            // Compute squared radius once
            int sqrRadius = radius * radius;

            Squares newSquares = new();
            for (int z = -radius; z <= radius; z++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    int distSqr = x * x + z * z;

                    if (distSqr < sqrRadius)
                    {
                        var currentTile = new Vector2Int(
                            startPos.x + x,
                            startPos.y + z);
                        if (ShouldSkip(currentTile))
                        {
                            continue;
                        }
                        Core.Instance.Measure.TransferOrAdd(newSquares, currentTile);
                    }
                }
            }
            return newSquares;
        }
    }
}
