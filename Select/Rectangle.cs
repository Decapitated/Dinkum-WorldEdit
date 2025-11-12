using UnityEngine;

namespace WorldEditMod.Select
{
    internal class Rectangle : StartEndSelector
    {
        public override Selection Collect(Vector2Int pos, Func<Vector2Int, bool> shouldSkip)
        {
            var selection = new Selection();
            if (IsMeasuring)
            {
                var realEnd = end ?? pos;
                Vector2Int diff = (Vector2Int)(realEnd - start);

                int xDir = (int)Mathf.Sign(diff.x);
                int zDir = (int)Mathf.Sign(diff.y);

                for (int z = 0; z <= Mathf.Abs(diff.y); z++)
                {
                    for (int x = 0; x <= Mathf.Abs(diff.x); x++)
                    {
                        var currentTile = (Vector2Int)start + new Vector2Int(x * xDir, z * zDir);
                        if (!shouldSkip(currentTile))
                        {
                            selection.Add(currentTile);
                        }
                    }
                }
            }
            return selection;
        }
    }
}
