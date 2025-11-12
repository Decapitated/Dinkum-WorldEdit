using UnityEngine;
using WorldEditMod.Select;

namespace WorldEditMod
{
    static internal class Menu
    {
        public enum Page
        {
            Main,
            SelectMode,
            LevelMode,
            OperatorMode,
            LimitYMode
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
                case Page.OperatorMode:
                    OperatorModePage(Core.Instance.Data);
                    break;
                case Page.LimitYMode:
                    LimitYModePage(Core.Instance.Data);
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
                GUILayout.Label("Shape");
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

                // Destory Tile Objects Toggle
                GUILayout.BeginHorizontal();
                GUILayout.Label("Destory Tile Objects");
                if (GUILayout.Button(data.destroyTileObjects ? "Enabled" : "Disabled"))
                {
                    data.destroyTileObjects = !data.destroyTileObjects;
                }
                GUILayout.EndHorizontal();

                // Operator Mode Page Toggle
                GUILayout.BeginHorizontal();
                GUILayout.Label("Operator");
                if (GUILayout.Button(data.operatorMode.ToString()))
                {
                    data.page = Page.OperatorMode;
                }
                GUILayout.EndHorizontal();

                // Limit Y Mode Page Toggle
                GUILayout.BeginHorizontal();
                GUILayout.Label("Y Limit");
                if (GUILayout.Button(data.limitYMode.ToString()))
                {
                    data.page = Page.LimitYMode;
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
                    Levelers.Level();
                }
                #endregion
            }
        }

        static void SelectModePage(Data data)
        {
            GUILayout.Label("Choose a Select Shape");
            var modes = Enum.GetValues(typeof(Data.SelectMode)).Cast<Data.SelectMode>();
            foreach (var mode in modes)
            {
                if (GUILayout.Button(mode.ToString()))
                {
                    data.selectMode = mode;
                    data.page = Page.Main;
                    Core.Instance.Measure.ClearMeasurement();
                    switch (mode)
                    {
                        case Data.SelectMode.Rectangle:
                            Core.Instance.Measure.Selector = new Rectangle();
                            break;
                        case Data.SelectMode.Circle:
                            Core.Instance.Measure.Selector = new Circle();
                            break;
                        case Data.SelectMode.Flood:
                            Core.Instance.Measure.Selector = new FloodFill();
                            break;
                    }
                }
            }
        }
        static void LevelModePage(Data data)
        {
            GUILayout.Label("Choose a Leveling Mode");
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

        static void OperatorModePage(Data data)
        {
            GUILayout.Label("Choose an Operation");
            var modes = Enum.GetValues(typeof(Data.OperatorMode)).Cast<Data.OperatorMode>();
            foreach (var mode in modes)
            {
                if (GUILayout.Button(mode.ToString()))
                {
                    data.operatorMode = mode;
                    data.page = Page.Main;
                    Core.Instance.Measure.Dirty();
                }
            }
        }
        static void LimitYModePage(Data data)
        {
            GUILayout.Label("Choose a Limit Y Mode");
            var modes = Enum.GetValues(typeof(Data.LimitYMode)).Cast<Data.LimitYMode>();
            foreach (var mode in modes)
            {
                if (GUILayout.Button(mode.ToString()))
                {
                    data.limitYMode = mode;
                    data.page = Page.Main;
                    Core.Instance.Measure.Dirty();
                }
            }
        }
    }
}
