using MelonLoader;
using Mirror;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

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

        internal Data Data { get; private set; } = new ();

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

        const float Width = 333;
        const float Height = Width * 0.666f;

        Rect mainWindowRect;
        public override void OnGUI()
        {
            mainWindowRect = GUILayout.Window(0,
                new Rect(Screen.width - Width, Screen.height / 2.0f - Height / 2.0f, Width, Height),
                Menu.MainWindow,
                "World Edit");
        }

        void OnSceneReady()
        {
            MelonCoroutines.Start(Setup());
        }

        IEnumerator Setup()
        {
            SetupMenuScreen();
            GetMeasurmentSquarePrefab();
            yield break;
        }

        void SetupMenuScreen()
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
    
        void GetMeasurmentSquarePrefab()
        {
            var squarePrefab = DivineDinkum.Utilities.FindResourceObject<GameObject>("MeasurementSquare");
            SquarePrefab = GameObject.Instantiate(squarePrefab);
            GameObject.Destroy(squarePrefab.transform.Find("Text (1)"));
            GameObject.Destroy(squarePrefab.transform.Find("Text (2)"));
        }

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
            Dirty();
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

        public void Dirty()
        {
            isDirty = true;
        }

        internal void TransferOrAdd(Squares newSquares, Vector2Int currentTile)
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
                    switch (Data.selectMode)
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
        
        void Level()
        {

        }
    }
}