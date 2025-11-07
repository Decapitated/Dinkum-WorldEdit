global using Squares = System.Collections.Generic.Dictionary<UnityEngine.Vector2Int, TapeMeasureSquare>;
global using Square = System.Collections.Generic.KeyValuePair<UnityEngine.Vector2Int, TapeMeasureSquare>;

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

        //static public MaterialPropertyBlock MPB { get; private set; }

        public bool HoldingTapeMeasure { get; private set; } = false;
        public Measure Measure {  get; private set; } = new Measure();

        internal Data Data { get; private set; } = new();

        public override void OnInitializeMelon()
        {
            Instance = this;
            DivineDinkum.Core.Instance.OnSceneReady.Subscribe(OnSceneReady);
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
        const float Height = Width * 1.333f;

        Rect mainWindowRect;
        public override void OnGUI()
        {
            if (HoldingTapeMeasure || Measure.IsMeasuring)
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
            Measure.SquarePrefab = GameObject.Instantiate(squarePrefab);
            GameObject.Destroy(squarePrefab.transform.Find("Text (1)"));
            GameObject.Destroy(squarePrefab.transform.Find("Text (2)"));
        }
        #endregion
    }
}