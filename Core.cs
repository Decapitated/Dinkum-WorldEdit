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
        public static MelonLogger.Instance LOGGER = null;
        public static Utilities UTILITIES = null;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initializing...");
            LOGGER = LoggerInstance;
            UTILITIES = new Utilities();
            MelonCoroutines.Start(ModSetup());
        }

        private IEnumerator ModSetup()
        {
            yield return new WaitUntil(() => UTILITIES.IsSetup);
        }
    }
}