using UnityEngine;

namespace WorldEditMod.Select
{
    internal class Rectangle : StartEndSelector
    {
        public override List<Vector2Int> Collect(Vector2Int pos)
        {
            var selection = new List<Vector2Int>();
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
                        selection.Add(currentTile);
                    }
                }
            }
            return selection;
        }
    }
}
