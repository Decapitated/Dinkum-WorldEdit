using MelonLoader;
using System.Collections;
using UnityEngine;

namespace WorldEditMod
{
    public class Measure
    {
        public bool HoldingTapeMeasure { get; private set; } = false;

        internal GameObject SquarePrefab { get; set; } = null;

        internal Squares squares = [];
        Vector2Int? startPosition = null;
        Vector2Int? endPosition = null;
        bool isDirty = false;

        public bool IsMeasuring { get => startPosition != null; }

        internal void UseCustomTapeMeasure()
        {
            var highlightPos = DivineDinkum.Utilities.GetHighlighterPosition();
            var highlightPos2D = new Vector2Int((int)highlightPos.x, (int)highlightPos.z);
            if (startPosition == null)
            {
                StartMeasuring(highlightPos2D);
            }
            else if (endPosition == null)
            {
                StopMeasuring(highlightPos2D);
            }
        }

        void StartMeasuring(Vector2Int highlightPos)
        {
            startPosition = highlightPos;
            endPosition = null;
            MelonCoroutines.Start(DoMeasurement());
        }

        void StopMeasuring(Vector2Int highlightPos)
        {
            endPosition = highlightPos;
            Dirty();
        }

        internal void ClearMeasurement()
        {
            startPosition = null;
            endPosition = null;
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
            while (startPosition != null)
            {
                if (endPosition == null || isDirty)
                {
                    yield return null;
                    Vector2Int realEndPos;
                    if (endPosition != null)
                    {
                        realEndPos = (Vector2Int)endPosition;
                    }
                    else
                    {
                        var highlightPos = DivineDinkum.Utilities.GetHighlighterPosition();
                        realEndPos = new Vector2Int((int)highlightPos.x, (int)highlightPos.z);
                    }

                    Squares newSquares = [];
                    switch (Core.Instance.Data.selectMode)
                    {
                        case Data.SelectMode.Rectangle:
                            newSquares = Selectors.Rectangle((Vector2Int)startPosition, realEndPos);
                            break;
                        case Data.SelectMode.Circle:
                            newSquares = Selectors.Circle((Vector2Int)startPosition, realEndPos);
                            break;
                    }
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
