using MelonLoader;
using System.Collections;
using UnityEngine;

namespace WorldEditMod
{
    public class Measure
    {
        public bool HoldingTapeMeasure { get; private set; } = false;
        public bool IsMeasuring { get => StartPosition != null; }

        internal GameObject SquarePrefab { get; set; } = null;

        internal Squares squares = [];
        internal Vector2Int? StartPosition { get; private set; } = null;
        internal Vector2Int? EndPosition { get; private set; } = null;
        bool isDirty = false;

        internal void UseCustomTapeMeasure()
        {
            var highlightPos = DivineDinkum.Utilities.GetHighlighterPosition();
            var highlightPos2D = new Vector2Int((int)highlightPos.x, (int)highlightPos.z);
            if (StartPosition == null)
            {
                StartMeasuring(highlightPos2D);
            }
            else if (EndPosition == null)
            {
                StopMeasuring(highlightPos2D);
            }
        }

        void StartMeasuring(Vector2Int highlightPos)
        {
            StartPosition = highlightPos;
            EndPosition = null;
            MelonCoroutines.Start(DoMeasurement());
        }

        void StopMeasuring(Vector2Int highlightPos)
        {
            EndPosition = highlightPos;
            Dirty();
        }

        internal void ClearMeasurement()
        {
            StartPosition = null;
            EndPosition = null;
            ClearSquares();
        }

        void ClearSquares()
        {
            foreach (var square in squares)
            {
                GameObject.Destroy(square.Value.gameObject);
            }
            squares.Clear();
        }

        public void Dirty()
        {
            isDirty = true;
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

        IEnumerator DoMeasurement()
        {
            while (StartPosition != null)
            {
                if (EndPosition == null || isDirty)
                {
                    yield return null;
                    Vector2Int realEndPos;
                    if (EndPosition != null)
                    {
                        realEndPos = (Vector2Int)EndPosition;
                    }
                    else
                    {
                        var highlightPos = DivineDinkum.Utilities.GetHighlighterPosition();
                        realEndPos = new Vector2Int((int)highlightPos.x, (int)highlightPos.z);
                    }

                    Squares newSquares = Selectors.Select((Vector2Int)StartPosition, realEndPos);

                    Operations.Operate(ref newSquares);

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

    }
}
