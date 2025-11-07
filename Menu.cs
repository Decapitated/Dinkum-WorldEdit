using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WorldEditMod
{
    static internal class Menu
    {
        public enum Page
        {
            Main,
            SelectMode
        }

        public static void MainWindow(int windowID)
        {
            GUILayout.BeginVertical();

            switch (Core.Instance.Data.page)
            {
                case Page.Main:
                    MainPage(Core.Instance.Data);
                    break;
                case Page.SelectMode:
                    SelectModePage(Core.Instance.Data);
                    break;
            }

            GUILayout.EndVertical();
        }

        static void MainPage(Data data)
        {
            if (GUILayout.Button(data.toggled ? "Enabled" : "Disabled"))
            {
                data.toggled = !data.toggled;
            }
            if (data.toggled)
            {
                #region Select Mode Page Toggle
                GUILayout.BeginHorizontal();
                GUILayout.Label("Select Mode");
                if (GUILayout.Button(data.selectMode.ToString()))
                {
                    data.page = Page.SelectMode;
                }
                GUILayout.EndHorizontal();
                #endregion
                #region Ignore Water Toggle
                GUILayout.BeginHorizontal();
                GUILayout.Label("Ignore Water");
                if (GUILayout.Button(data.ignoreWater ? "Enabled" : "Disabled"))
                {
                    data.ignoreWater = !data.ignoreWater;
                    Core.Instance.Dirty();
                }
                GUILayout.EndHorizontal();
                #endregion
                
            }
        }

        static void SelectModePage(Data data)
        {
            var modes = Enum.GetValues(typeof(Data.SelectMode)).Cast<Data.SelectMode>();
            foreach (var mode in modes)
            {
                if (GUILayout.Button(mode.ToString()))
                {
                    data.selectMode = mode;
                    data.page = Page.Main;
                    Core.Instance.Dirty();
                }
            }
        }
    }
}
