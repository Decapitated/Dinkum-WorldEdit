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

        public enum OperatorMode
        {
            None,
            Hollow
        }
        
        public enum LimitYMode
        {
            None,
            Same,
            Less,
            LessOrSame,
            Greater,
            GreaterOrSame
        }

        public Menu.Page page = Menu.Page.Main;

        public bool toggled = false;
        public SelectMode selectMode = SelectMode.Rectangle;
        public bool ignoreWater = true;
        public LevelMode levelMode = LevelMode.Player;
        public int adjustAmount = 1;
        public OperatorMode operatorMode = OperatorMode.None;
        public LimitYMode limitYMode = LimitYMode.None;

        public Data() { }
    }
}
