using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WorldEditMod.Select
{
    internal class FloodFill : Selector
    {
        const int MAX_DISTANCE = 50;

        Vector2Int? origin;

        public override bool IsMeasuring => origin != null;

        public override bool IsFinished => IsMeasuring;

        public override Vector2Int Origin => origin ?? Vector2Int.zero;

        public override void Clear()
        {
            origin = null;
        }

        public override void Use(Vector2Int pos)
        {
            origin = pos;
        }

        public override IEnumerator Collect(Selection selection, Vector2Int pos, Func<Vector2Int, bool> shouldSkip)
        {
            if (origin != null)
            {
                var visited = new Selection();
                var floodQueue = new Queue<Vector2Int>();

                visited.Add((Vector2Int)origin);
                floodQueue.Enqueue((Vector2Int)origin);

                while (floodQueue.Count > 0)
                {
                    var currentTile = floodQueue.Dequeue();
                    var originDistanceSqr = (currentTile - (Vector2Int)origin).sqrMagnitude;
                    if (originDistanceSqr < (MAX_DISTANCE * MAX_DISTANCE) && !shouldSkip(currentTile))
                    {
                        selection.Add(currentTile);
                        var neighbours = Operations.GetNeighbours(currentTile);
                        foreach (var neighbour in neighbours)
                        {
                            if (!visited.Contains(neighbour))
                            {
                                visited.Add(neighbour);
                                floodQueue.Enqueue(neighbour);
                            }
                        }
                    }
                }
            }
            yield break;
        }
    }
}
