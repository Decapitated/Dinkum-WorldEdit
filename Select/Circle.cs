using UnityEngine;

namespace WorldEditMod.Select
{
    internal class Circle : StartEndSelector
    {
        public override Selection Collect(Vector2Int pos, Func<Vector2Int, bool> shouldSkip)
        {
            var selection = new Selection();
            if (IsMeasuring)
            {
                var realEnd = end ?? pos;
                Vector2Int diff = (Vector2Int)(realEnd - start);

                int absDx = Mathf.Abs(diff.x);
                int absDy = Mathf.Abs(diff.y);

                // Smoothly blend between average and max distance
                // - Diagonals get a moderate radius
                // - Horizontal/vertical lines get full length
                float blended = (absDx + absDy + Mathf.Max(absDx, absDy)) / 3f;

                // Use the larger of the blended or true Euclidean distance
                int radius = Mathf.CeilToInt(Mathf.Max(blended, Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y)));

                // Compute squared radius once
                int sqrRadius = radius * radius;
                for (int z = -radius; z <= radius; z++)
                {
                    for (int x = -radius; x <= radius; x++)
                    {
                        int distSqr = x * x + z * z;

                        if (distSqr < sqrRadius)
                        {
                            var currentTile = new Vector2Int(
                                ((Vector2Int)start).x + x,
                                ((Vector2Int)start).y + z);
                            if (!shouldSkip(currentTile))
                            {
                                selection.Add(currentTile);
                            }
                        }
                    }
                }
            }
            return selection;
        }
    }
}
