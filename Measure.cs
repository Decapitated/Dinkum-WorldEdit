using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorldEditMod.Select;

namespace WorldEditMod
{
    public class Measure
    {
        public bool HoldingTapeMeasure { get; private set; } = false;

        internal GameObject SquarePrefab { get; set; } = null;

        internal Selector Selector = new Rectangle();

        internal Squares squares = [];
        bool isDirty = false;

        public void Dirty() => isDirty = true;

        internal void UseCustomTapeMeasure()
        {
            if (Selector == null) return;

            var wasMeasuring = Selector.IsMeasuring;
            var highlightPos = GetHighlighterPosition2D();
            Selector.Use(highlightPos);
            if (!wasMeasuring && Selector.IsMeasuring)
            {
                MelonCoroutines.Start(DoMeasurement());
            }
            Dirty();
        }

        internal void ClearMeasurement()
        {
            Selector.Clear();
            ClearSquares();
        }

        void ClearSquares()
        {
            foreach (var square in squares)
            {
                if (square.Value != null)
                {
                    GameObject.Destroy(square.Value.gameObject);
                }
            }
            squares.Clear();
        }

        internal void TransferOrAdd(Squares newSquares, Vector2Int currentTile)
        {
            TapeMeasureSquare newSquare;
            var currentHeight = WorldManager.Instance.heightMap[currentTile.x, currentTile.y];
            if (squares.ContainsKey(currentTile) && Mathf.Approximately(squares[currentTile].transform.position.y - 0.05f, currentHeight))
            {
                newSquare = squares[currentTile];
                squares.Remove(currentTile);
            }
            else
            {
                newSquare = GameObject.Instantiate(SquarePrefab).GetComponent<TapeMeasureSquare>();
            }
            newSquare.setPosition(currentTile.x, currentTile.y);
            newSquares.Add(currentTile, newSquare);
        }

        internal Vector2Int GetHighlighterPosition2D()
        {
            var highlightPos = DivineDinkum.Utilities.GetHighlighterPosition();
            return new Vector2Int((int)highlightPos.x, (int)highlightPos.z);
        }

        IEnumerator DoMeasurement()
        {
            while (Selector != null && Selector.IsMeasuring)
            {
                if (!Selector.IsFinished || isDirty)
                {
                    yield return null;

                    var highlightPos = GetHighlighterPosition2D();

                    var selected = new Selection();
                    yield return Selector.Collect(selected, highlightPos, ShouldSkip);

                    var filtered = new Selection();
                    foreach (var tile in selected)
                    {
                        if (ShouldSkip(tile)) continue;
                        filtered.Add(tile);
                    }
                    selected = filtered;

                    Operations.Operate(ref selected);

                    // Transfer or Add selected tiles.
                    var newSquares = new Squares();
                    foreach (var tile in selected)
                    {
                        TransferOrAdd(newSquares, tile);
                    }

                    // Cleanup old squares.
                    foreach (var square in squares)
                    {
                        GameObject.Destroy(square.Value.gameObject);
                    }

                    squares = newSquares;
                    isDirty = false;
                }
                yield return null;
            }
        }
        
        bool ShouldSkip(Vector2Int tile)
        {
            var isWater = WorldManager.Instance.waterMap[tile.x, tile.y];

            if (Core.Instance.Data.ignoreWater && isWater) return true;

            var startY = WorldManager.Instance.heightMap[Selector.Origin.x, Selector.Origin.y];
            var tileY = WorldManager.Instance.heightMap[tile.x, tile.y];
            var yDiff = tileY - startY;
            var isValidY = true;
            switch (Core.Instance.Data.limitYMode)
            {
                case Data.LimitYMode.Same:
                    isValidY = yDiff == 0;
                    break;
                case Data.LimitYMode.Less:
                    isValidY = yDiff < 0;
                    break;
                case Data.LimitYMode.LessOrSame:
                    isValidY = yDiff <= 0;
                    break;
                case Data.LimitYMode.Greater:
                    isValidY = yDiff > 0;
                    break;
                case Data.LimitYMode.GreaterOrSame:
                    isValidY = yDiff >= 0;
                    break;
            }

            return !isValidY;
        }
    }
}
