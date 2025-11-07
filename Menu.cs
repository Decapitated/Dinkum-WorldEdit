using UnityEngine;

namespace WorldEditMod
{
    static internal class Menu
    {
        public enum Page
        {
            Main,
            SelectMode,
            LevelMode
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
                case Page.LevelMode:
                    LevelModePage(Core.Instance.Data);
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
                #region Selection
                GUILayout.Label("Select");
                // Select Mode Page Toggle
                GUILayout.BeginHorizontal();
                GUILayout.Label("Mode");
                if (GUILayout.Button(data.selectMode.ToString()))
                {
                    data.page = Page.SelectMode;
                }
                GUILayout.EndHorizontal();
                // Ignore Water Toggle
                GUILayout.BeginHorizontal();
                GUILayout.Label("Ignore Water");
                if (GUILayout.Button(data.ignoreWater ? "Enabled" : "Disabled"))
                {
                    data.ignoreWater = !data.ignoreWater;
                    Core.Instance.Measure.Dirty();
                }
                GUILayout.EndHorizontal();
                #endregion
                #region Leveling
                GUILayout.Label("Level");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Mode");
                if (GUILayout.Button(data.levelMode.ToString()))
                {
                    data.page = Page.LevelMode;
                }
                GUILayout.EndHorizontal();
                switch (data.levelMode)
                {
                    case Data.LevelMode.Up:
                    case Data.LevelMode.Down:
                        GUILayout.BeginHorizontal();
                        GUILayout.Label($"{data.adjustAmount}", GUILayout.ExpandHeight(false));
                        data.adjustAmount = Mathf.RoundToInt(
                            GUILayout.HorizontalSlider(data.adjustAmount, 1.0f, 10.0f));
                        GUILayout.EndHorizontal();
                        break;
                }
                if (GUILayout.Button("Level"))
                {
                    Core.Instance.Level();
                }
                #endregion
                #region Operations
                GUILayout.Label("Operations");
                if (GUILayout.Button("Hollow"))
                {
                    data.page = Page.LevelMode;
                }
                #endregion
            }
        }

        static void SelectModePage(Data data)
        {
            GUILayout.Label("Choose a Select Mode");
            var modes = Enum.GetValues(typeof(Data.SelectMode)).Cast<Data.SelectMode>();
            foreach (var mode in modes)
            {
                if (GUILayout.Button(mode.ToString()))
                {
                    data.selectMode = mode;
                    data.page = Page.Main;
                    Core.Instance.Measure.Dirty();
                }
            }
        }
        static void LevelModePage(Data data)
        {
            var modes = Enum.GetValues(typeof(Data.LevelMode)).Cast<Data.LevelMode>();
            foreach (var mode in modes)
            {
                if (GUILayout.Button(mode.ToString()))
                {
                    data.levelMode = mode;
                    data.page = Page.Main;
                }
            }
        }
    }
}
