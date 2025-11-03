using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WorldEditMod
{
    public class Utilities
    {
        GameObject mapCanvas;
        GameObject menuScreen;
        GameObject menuButtons;

        bool setup = false;
        bool finished = false;

        public bool IsSetup
        {
            get { return setup && finished; }
        }

        public Utilities()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!setup && scene.isLoaded && scene.name == "scene1")
            {
                MelonCoroutines.Start(WaitForSceneObjects(scene));
            }
        }

        private IEnumerator WaitForSceneObjects(Scene scene)
        {
            Core.LOGGER.Msg("Waiting for scene...");
            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (GameObject rootObject in rootObjects)
            {
                if (rootObject.activeInHierarchy && rootObject.name == "MapCanvas")
                {
                    setup = true; // ensure this runs only once
                    mapCanvas = rootObject;

                    yield return MelonCoroutines.Start(Setup());
                    finished = true;
                    yield break;
                }
            }

            Core.LOGGER.Error("MapCanvas not found — retrying...");
            yield return new WaitForSeconds(1f);
            MelonCoroutines.Start(WaitForSceneObjects(scene)); // retry once
        }

        private IEnumerator Setup()
        {
            Core.LOGGER.Msg("Setting up...");

            Transform msTransform = mapCanvas.transform.Find("MenuScreen");
            if (msTransform == null)
            {
                Core.LOGGER.Error("MenuScreen not found.");
                yield return null;
            }

            menuScreen = msTransform.gameObject;

            Transform mbTransform = menuScreen.transform.Find("MenuButtons");
            if (mbTransform == null)
            {
                Core.LOGGER.Error("MenuButtons not found.");
                yield return null;
            }

            menuButtons = mbTransform.gameObject;
            Transform ogButtonTransform = null;
            try
            {
                ogButtonTransform = menuButtons.transform.GetChild(0);
            }
            catch { }

            if (ogButtonTransform == null)
            {
                Core.LOGGER.Error("Original button not found.");
                yield return null;
            }

            Transform textTransform = ogButtonTransform.Find("Text");
            if (textTransform == null)
            {
                Core.LOGGER.Error("Text not found.");
                yield return null;
            }

            TextMeshProUGUI tmpgui = textTransform.GetComponent<TextMeshProUGUI>();
            if (tmpgui != null)
            {
                Core.LOGGER.Msg("Waiting for text...");
                yield return new WaitUntil(() => tmpgui.gameObject.activeInHierarchy);
                //yield return new WaitForSeconds(1.0f);
                tmpgui.SetText("Decapitated");
            }
            else
            {
                Core.LOGGER.Error("Failed to get TextMeshProUGUI.");
            }
        }
    }
}
