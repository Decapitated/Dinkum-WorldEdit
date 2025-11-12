global using Squares = System.Collections.Generic.Dictionary<UnityEngine.Vector2Int, TapeMeasureSquare>;
global using Square = System.Collections.Generic.KeyValuePair<UnityEngine.Vector2Int, TapeMeasureSquare>;
global using Selection = System.Collections.Generic.HashSet<UnityEngine.Vector2Int>;

using MelonLoader;
using System.Collections;
using TMPro;
using UnityEngine;

[assembly: MelonInfo(typeof(WorldEditMod.Core), "WorldEditMod", "1.0.0", "Decapitated", null)]
[assembly: MelonGame("James Bendon", "Dinkum")]

namespace WorldEditMod
{
    public class Core : MelonMod
    {
        static public Core Instance { get; private set; }

        public bool HoldingTapeMeasure { get; private set; } = false;
        public Measure Measure {  get; private set; } = new Measure();

        internal Data Data { get; private set; }

        public override void OnInitializeMelon()
        {
            Instance = this;
            DivineDinkum.Core.Instance.OnSceneReady.Subscribe(OnSceneReady);
            DivineDinkum.Core.Instance.OnSceneUnready.Subscribe(OnSceneUnready);
        }

        public override void OnUpdate()
        {
            InventorySlot slot = Inventory.Instance.invSlots[Inventory.Instance.selectedSlot];
            HoldingTapeMeasure = slot.itemInSlot != null && slot.itemInSlot.name == "Inv_TapeMeasure";
            if (HoldingTapeMeasure && InputMaster.input.Interact())
            {
                Measure.ClearMeasurement();
            }
        }

        const float Width = 333;
        const float Height = Width * 1.0f;

        Rect mainWindowRect;
        public override void OnGUI()
        {
            var isMeasuring = Measure.Selector != null && Measure.Selector.IsMeasuring;
            if (HoldingTapeMeasure || isMeasuring)
            {
                mainWindowRect = GUILayout.Window(0,
                    new Rect(Screen.width - Width, Screen.height / 2.0f - Height / 2.0f, Width, Height),
                    Menu.MainWindow,
                    "World Edit");
            }
        }

        #region Setup
        void OnSceneReady()
        {
            Data = new();
            MelonCoroutines.Start(Setup());
        }

        void OnSceneUnready()
        {
            Measure.ClearMeasurement();
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
            if (Measure.SquarePrefab == null)
            {
                var squarePrefab = DivineDinkum.Utilities.FindResourceObject<GameObject>("MeasurementSquare");
                Measure.SquarePrefab = GameObject.Instantiate(squarePrefab);
                GameObject.Destroy(squarePrefab.transform.Find("Text (1)"));
                GameObject.Destroy(squarePrefab.transform.Find("Text (2)"));
            }
        }
        #endregion
    }
}