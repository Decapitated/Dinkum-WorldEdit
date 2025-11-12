using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WorldEditMod.Select
{
    internal abstract class StartEndSelector : Selector
    {
        protected Vector2Int? start;
        protected Vector2Int? end;

        public override bool IsMeasuring => start != null;
        public override bool IsFinished => start != null && end != null;
        public override Vector2Int Origin => start ?? Vector2Int.zero;

        public override void Clear()
        {
            start = null;
            end = null;
        }

        public override void Use(Vector2Int pos)
        {
            if (start == null)
            {
                start = pos;
            }
            else if (end == null)
            {
                end = pos;
            }
        }
    }
}
