using System;
using System.Collections.Generic;
using System.Text;
using static WorldEditMod.Menu;

namespace WorldEditMod
{
    internal class Data
    {
        public enum SelectMode
        {
            Rectangle,
            Circle
        }

        public enum LevelMode
        {
            Player,
            Up, Down,
            Maximum,
            Minimum,
            Average
        }
        
        public Page page = Page.Main;

        public bool toggled = false;
        public SelectMode selectMode = SelectMode.Rectangle;
        public bool ignoreWater = true;
        public LevelMode levelMode = LevelMode.Player;
        public int adjustAmount = 1;

        public Data() { }
    }
}
