using MelonLoader;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[assembly: MelonInfo(typeof(WorldEditMod.Core), "WorldEditMod", "1.0.0", "Decapitated", null)]
[assembly: MelonGame("James Bendon", "Dinkum")]

namespace WorldEditMod
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initializing...");
            DivineDinkum.Core.Instance.OnSceneReady.AddListener(OnSceneReady);
            DivineDinkum.Core.Instance.OnSceneUnready.AddListener(OnSceneUnready);
        }

        private void OnSceneReady()
        {
            MelonCoroutines.Start(SetupMenuScreen());
        }

        private void OnSceneUnready()
        {

        }

        private IEnumerator SetupMenuScreen(Action<bool> callback = null)
        {
            Transform quitTransform = DivineDinkum.Core.Instance.MenuButtons.transform.Find("Quit To Desktop");
            if (quitTransform == null)
            {
                LoggerInstance.Error("Failed to find Quit button.");
                callback?.Invoke(false);
                yield break;
            }
            yield return DivineDinkum.Utilities.WaitForGameObject(quitTransform.gameObject);

            Transform quitText = quitTransform.Find("Text");
            if (quitText == null)
            {
                LoggerInstance.Error("Failed to find Text.");
                callback?.Invoke(false);
                yield break;
            }
            
            TextMeshProUGUI quitTMP = quitText.GetComponent<TextMeshProUGUI>();
            if (quitTMP == null)
            {
                LoggerInstance.Error("Failed to get TextMeshProUGUI component.");
                callback?.Invoke(false);
                yield break;
            }

            quitTMP.SetText("Kill Game");

            callback(true);
        }
    }
}