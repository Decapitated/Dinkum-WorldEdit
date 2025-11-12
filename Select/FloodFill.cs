using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WorldEditMod.Select
{
    internal class FloodFill : Selector
    {
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

        public override List<Vector2Int> Collect(Vector2Int pos, Func<Vector2Int, bool> shouldSkip)
        {
            throw new NotImplementedException();
        }
    }
}
