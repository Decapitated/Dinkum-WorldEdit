using System.Collections;
using UnityEngine;

namespace WorldEditMod.Select
{
    internal abstract class Selector
    {
        public abstract bool IsMeasuring { get; }
        public abstract bool IsFinished { get; }
        public abstract Vector2Int Origin { get; }

        public abstract void Use(Vector2Int pos);
        public abstract void Clear();
        public abstract IEnumerator Collect(Selection selection, Vector2Int pos, Func<Vector2Int, bool> shouldSkip);
    }
}
