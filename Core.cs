using MelonLoader;
using System.Collections;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(WorldEditMod.Core), "WorldEditMod", "1.0.0", "Decapitated", null)]
[assembly: MelonGame("James Bendon", "Dinkum")]

namespace WorldEditMod
{
    public class Core : MelonMod
    {
        static public GameObject SquarePrefab { get; private set; }
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initializing...");
            DivineDinkum.Core.Instance.OnSceneReady.Subscribe(OnSceneReady);
            DivineDinkum.Core.Instance.OnSceneUnready.Subscribe(OnSceneUnready);
        }

        private void OnSceneReady()
        {
            MelonCoroutines.Start(Setup());
        }

        private void OnSceneUnready()
        {

        }

        private IEnumerator Setup()
        {
            yield return SetupMenuScreen();
            yield return GetMeasurmentSquarePrefab();
        }

        private IEnumerator SetupMenuScreen()
        {
            Transform quitTransform = DivineDinkum.MapCanvas.Instance.MenuButtons.transform.Find("Quit To Desktop");
            if (quitTransform == null)
            {
                LoggerInstance.Error("Failed to find Quit button.");
                yield break;
            }
            //yield return new WaitUntil(() => quitTransform.gameObject.activeInHierarchy);

            Transform quitText = quitTransform.Find("Text");
            if (quitText == null)
            {
                LoggerInstance.Error("Failed to find Text.");;
                yield break;
            }
            
            TextMeshProUGUI quitTMP = quitText.GetComponent<TextMeshProUGUI>();
            if (quitTMP == null)
            {
                LoggerInstance.Error("Failed to get TextMeshProUGUI component.");
                yield break;
            }

            quitTMP.SetText("Kill Game");
        }
    
        private IEnumerator GetMeasurmentSquarePrefab()
        {
            var squarePrefab = DivineDinkum.Utilities.FindResourceObject<GameObject>("MeasurementSquare");
            //squarePrefab.transform.detach
            SquarePrefab = GameObject.Instantiate(squarePrefab);
            yield break;
        }
    }
}