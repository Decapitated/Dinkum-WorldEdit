using MelonLoader;
using Mirror;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(WorldEditMod.Core), "WorldEditMod", "1.0.0", "Decapitated", null)]
[assembly: MelonGame("James Bendon", "Dinkum")]

namespace WorldEditMod
{
    using Squares = Dictionary<Vector2Int, TapeMeasureSquare>;
    public class Core : MelonMod
    {
        static public Core Instance { get; private set; }

        static public GameObject SquarePrefab { get; private set; }
        static public MaterialPropertyBlock MPB { get; private set; }

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initializing...");
            Instance = this;
            DivineDinkum.Core.Instance.OnSceneReady.Subscribe(OnSceneReady);
        }

        public override void OnUpdate()
        {
            if (InputMaster.input.Interact())
            {
                ClearMeasurement();
            }
        }

        private void OnSceneReady()
        {
            MelonCoroutines.Start(Setup());
        }

        private IEnumerator Setup()
        {
            SetupMenuScreen();
            GetMeasurmentSquarePrefab();
            yield break;
        }

        private void SetupMenuScreen()
        {
            Transform quitTransform = DivineDinkum.MapCanvas.Instance.MenuButtons.transform.Find("Quit To Desktop");
            if (quitTransform == null)
            {
                LoggerInstance.Error("Failed to find Quit button.");
                return;
            }
            //yield return new WaitUntil(() => quitTransform.gameObject.activeInHierarchy);

            Transform quitText = quitTransform.Find("Text");
            if (quitText == null)
            {
                LoggerInstance.Error("Failed to find Text.");;
                return;
            }
            
            TextMeshProUGUI quitTMP = quitText.GetComponent<TextMeshProUGUI>();
            if (quitTMP == null)
            {
                LoggerInstance.Error("Failed to get TextMeshProUGUI component.");
                return;
            }

            quitTMP.SetText("Kill Game");
        }
    
        private void GetMeasurmentSquarePrefab()
        {
            var squarePrefab = DivineDinkum.Utilities.FindResourceObject<GameObject>("MeasurementSquare");
            SquarePrefab = GameObject.Instantiate(squarePrefab);
            GameObject.Destroy(squarePrefab.transform.Find("Text (1)"));
            GameObject.Destroy(squarePrefab.transform.Find("Text (2)"));
        }

        enum SelectMode
        {
            Rectangle,
            Circle
        }

        SelectMode selectMode = SelectMode.Circle;
        Vector2Int? startPosition = null;
        Vector2Int? endPosition = null;
        Squares squares = new();
        bool isDirty = false;

        public bool IsMeasuring
        {
            get => startPosition != null;
        }

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
            LoggerInstance.Msg("Start measuring...");
            startPosition = highlightPos;
            endPosition = null;
            MelonCoroutines.Start(DoMeasurement());
        }

        void StopMeasuring(Vector2Int highlightPos)
        {
            LoggerInstance.Msg("Stop measuring...");
            endPosition = highlightPos;
            isDirty = true;
        }

        void ClearMeasurement()
        {
            LoggerInstance.Msg("Clear measurement...");
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

        void TransferOrAdd(Squares newSquares, Vector2Int currentTile)
        {
            TapeMeasureSquare newSquare;
            if (squares.ContainsKey(currentTile))
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
            LoggerInstance.Msg("Do measurement...");
            while (startPosition != null)
            {
                if (endPosition == null || isDirty)
                {
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

                    Squares newSquares = new();
                    switch (selectMode)
                    {
                        case SelectMode.Rectangle:
                            newSquares = Rectangle((Vector2Int)startPosition, realEndPos);
                            break;
                        case SelectMode.Circle:
                            newSquares = Circle((Vector2Int)startPosition, realEndPos);
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

        Squares Rectangle(Vector2Int startPos, Vector2Int endPos)
        {
            Squares newSquares = new();
            Vector2Int diff = endPos - startPos;
            int xDir = (int)Mathf.Sign(diff.x);
            int zDir = (int)Mathf.Sign(diff.y);
            for (int z = 0; z <= Mathf.Abs(diff.y); z++)
            {
                for (int x = 0; x <= Mathf.Abs(diff.x); x++)
                {
                    var currentTile = startPos + new Vector2Int(x * xDir, z * zDir);
                    TransferOrAdd(newSquares, currentTile);   
                }
            }
            return newSquares;
        }

        Squares Circle(Vector2Int startPos, Vector2Int endPos)
        {
            int dx = endPos.x - startPos.x;
            int dy = endPos.y - startPos.y;

            int absDx = Mathf.Abs(dx);
            int absDy = Mathf.Abs(dy);

            // Smoothly blend between average and max distance
            // - Diagonals get a moderate radius
            // - Horizontal/vertical lines get full length
            float blended = (absDx + absDy + Mathf.Max(absDx, absDy)) / 3f;

            // Use the larger of the blended or true Euclidean distance
            int radius = Mathf.CeilToInt(Mathf.Max(blended, Mathf.Sqrt(dx * dx + dy * dy)));

            // Compute squared radius once
            int sqrRadius = radius * radius;

            Squares newSquares = new();
            for (int z = -radius; z <= radius; z++)
            {
                for (int x = -radius ; x <= radius; x++)
                {
                    int distSqr = x * x + z * z;

                    if (distSqr < sqrRadius)
                    {
                        var currentTile = new Vector2Int(
                            startPos.x + x,
                            startPos.y + z);
                        TransferOrAdd(newSquares, currentTile);
                    }
                }
            }
            return newSquares;
        }
    }
}