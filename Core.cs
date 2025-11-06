using MelonLoader;
using System.Collections;
using TMPro;
using UnityEngine;
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
            if (Input.GetKeyDown(KeyCode.Escape))
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

        enum Mode
        {
            Test
        }

        Mode mode = Mode.Test;
        Vector3? startPosition = null;
        Vector3? endPosition = null;
        Squares squares = new();

        public bool IsMeasuring
        {
            get => startPosition != null;
        }

        internal void UseCustomTapeMeasure()
        {
            var highlightPos = DivineDinkum.Utilities.GetHighlighterPosition();
            LoggerInstance.Msg(
                "\n Highlight \n" +
                "===========\n" +
                $"Pos: {highlightPos}");

            if (startPosition == null)
            {
                StartMeasuring(highlightPos);
            }
            else if (endPosition == null && highlightPos != startPosition)
            {
                StopMeasuring(highlightPos);
            }
            else
            {
                ClearMeasurement();
            }
        }

        void StartMeasuring(Vector3 highlightPos)
        {
            LoggerInstance.Msg("Start measuring...");
            startPosition = highlightPos;
            endPosition = null;
            MelonCoroutines.Start(DoMeasurement());
        }

        void StopMeasuring(Vector3 highlightPos)
        {
            LoggerInstance.Msg("Stop measuring...");
            endPosition = highlightPos;
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

        IEnumerator DoMeasurement()
        {
            LoggerInstance.Msg("Do measurement...");
            while (startPosition != null)
            {
                Vector3 realEndPos = endPosition ?? DivineDinkum.Utilities.GetHighlighterPosition();
                Squares newSquares = new();
                switch (mode)
                {
                    case Mode.Test:
                        newSquares = DoTest((Vector3)startPosition, realEndPos);
                        break;
                }
                foreach (var square in squares)
                {
                    GameObject.Destroy(square.Value.gameObject);
                }
                squares = newSquares;
                yield return null;
            }
        }

        Squares DoTest(Vector3 startPos, Vector3 endPos)
        {
            Squares newSquares = new();
            Vector3 diff = endPos - startPos;
            int xDir = (int)Mathf.Sign(diff.x);
            int zDir = (int)Mathf.Sign(diff.z);
            for (int z = 0; z <= Mathf.Abs(diff.z); z++)
            {
                for (int x = 0; x <= Mathf.Abs(diff.x); x++)
                {
                    var currentTile = new Vector2Int(
                        (int)(startPos.x + x * xDir),
                        (int)(startPos.z + z * zDir));
                    TransferOrAdd(newSquares, currentTile);   
                }
            }
            return newSquares;
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
    }
}