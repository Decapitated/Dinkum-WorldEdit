using UnityEngine;

namespace WorldEditMod
{
    static internal class Levelers
    {
        static public void Level()
        {
            switch (Core.Instance.Data.levelMode)
            {
                case Data.LevelMode.Player:
                    Levelers.Player();
                    break;
                case Data.LevelMode.Up:
                case Data.LevelMode.Down:
                    var diff = Core.Instance.Data.adjustAmount * (Core.Instance.Data.levelMode == Data.LevelMode.Up ? 1 : -1);
                    Levelers.Adjust(diff);
                    break;
                case Data.LevelMode.Maximum:
                    Levelers.Maximum();
                    break;
                case Data.LevelMode.Minimum:
                    Levelers.Minimum();
                    break;
                case Data.LevelMode.Average:
                    Levelers.Average();
                    break;
            }
            Core.Instance.Measure.Dirty();
        }

        static public void Player()
        {
            var playerPosition = NetworkMapSharer.Instance.localChar.gameObject.transform.position;
            var dinkPos = DivineDinkum.Utilities.UnityToDinkumPosition(playerPosition);
            var playerHeight = WorldManager.Instance.heightMap[(int)dinkPos.x, (int)dinkPos.z];
            CallOnSquares((tileHeight) =>
            {
                return playerHeight - tileHeight;
            });
        }

        static public void Maximum()
        {
            var maxSquare = Core.Instance.Measure.squares.Aggregate(
                (l, r) => {
                    var tileHeightL = WorldManager.Instance.heightMap[l.Key.x, l.Key.y];
                    var tileHeightR = WorldManager.Instance.heightMap[r.Key.x, r.Key.y];
                    return tileHeightL > tileHeightR ? l : r;
                });
            var maxY = WorldManager.Instance.heightMap[maxSquare.Key.x, maxSquare.Key.y];
            CallOnSquares((tileHeight) =>
            {
                return maxY - tileHeight;
            });
        }

        static public void Minimum()
        {
            var minSquare = Core.Instance.Measure.squares.Aggregate(
                (l, r) => {
                    var tileHeightL = WorldManager.Instance.heightMap[l.Key.x, l.Key.y];
                    var tileHeightR = WorldManager.Instance.heightMap[r.Key.x, r.Key.y];
                    return tileHeightL < tileHeightR ? l : r;
                });
            var minY = WorldManager.Instance.heightMap[minSquare.Key.x, minSquare.Key.y];
            CallOnSquares((tileHeight) =>
            {
                return minY - tileHeight;
            });
        }
        
        static public void Average()
        {
            var averageY = Mathf.RoundToInt(
                (float)Core.Instance.Measure.squares.Aggregate(new HashSet<int>(),
                    (list, square) => {
                        var tileHeight = WorldManager.Instance.heightMap[square.Key.x, square.Key.y];
                        if (!list.Contains(tileHeight))
                        {
                            list.Add(tileHeight);
                        }
                        return list;
                    },
                    (list) => list.Average()));
            CallOnSquares((tileHeight) =>
            {
                return averageY - tileHeight;
            });
        }

        static public void Adjust(int diff)
        {
            CallOnSquares((tileHeight) =>
            {
                return diff;
            });
        }

        static void CallOnSquares(Func<int, int> routine)
        {
            foreach (var square in Core.Instance.Measure.squares)
            {
                var tileHeight = WorldManager.Instance.heightMap[square.Key.x, square.Key.y];
                var diff = routine(tileHeight);
                NetworkMapSharer.Instance.changeTileHeight(diff, square.Key.x, square.Key.y);
            }
        }
    }
}
