using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WorldEditMod
{
    static internal class Menu
    {
        public class Data
        {
            public enum Page
            {
                Main,
                SelectMode
            }

            public Page page = Page.Main;

            public bool toggled = false;
            public Core.SelectMode selectMode = Core.SelectMode.Rectangle;
            public bool ignoreWater = true;

            public Data() { }
        }

        public static void MainWindow(int windowID)
        {
            GUILayout.BeginVertical();

            switch (Core.Instance.Data.page)
            {
                case Data.Page.Main:
                    MainPage(Core.Instance.Data);
                    break;
                case Data.Page.SelectMode:
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
                GUILayout.Label("Select Mode");
                if (GUILayout.Button(data.selectMode.ToString()))
                {
                    data.page = Data.Page.SelectMode;
                }
                if (GUILayout.Button(data.ignoreWater ? "Ignoring Water" : "Affecting Water"))
                {
                    data.ignoreWater = !data.ignoreWater;
                    Core.Instance.Dirty();
                }
            }
        }

        static void SelectModePage(Data data)
        {
            var modes = Enum.GetValues(typeof(Core.SelectMode)).Cast<Core.SelectMode>();
            foreach (var mode in modes)
            {
                if (GUILayout.Button(mode.ToString()))
                {
                    data.selectMode = mode;
                    data.page = Data.Page.Main;
                    Core.Instance.Dirty();
                }
            }
        }
    }
}
