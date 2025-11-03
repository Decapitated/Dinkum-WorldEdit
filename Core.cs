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
            MelonCoroutines.Start(ModSetup());
        }

        private IEnumerator ModSetup()
        {
            yield return new WaitUntil(() => DivineDinkum.Core.Instance.IsSetup);
            LoggerInstance.Msg("DivineDinkum is ready!");
        }
    }
}