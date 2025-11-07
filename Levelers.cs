using UnityEngine;

namespace WorldEditMod
{
    //using Square = KeyValuePair<Vector2Int, TapeMeasureSquare>;
    using Squares = Dictionary<Vector2Int, TapeMeasureSquare>;

    static internal class Levelers
    {
        static public void Player(Squares squares)
        {
            var playerPosition = NetworkMapSharer.Instance.localChar.gameObject.transform.position;
            var dinkPos = DivineDinkum.Utilities.UnityToDinkumPosition(playerPosition);
            var playerHeight = WorldManager.Instance.heightMap[(int)dinkPos.x, (int)dinkPos.z];
            CallOnSquares(squares, (tileHeight) =>
            {
                return playerHeight - tileHeight;
            });
        }

        static public void Maximum(Squares squares)
        {
            var maxSquare = squares.Aggregate(
                (l, r) => {
                    var tileHeightL = WorldManager.Instance.heightMap[l.Key.x, l.Key.y];
                    var tileHeightR = WorldManager.Instance.heightMap[r.Key.x, r.Key.y];
                    return tileHeightL > tileHeightR ? l : r;
                });
            var maxY = WorldManager.Instance.heightMap[maxSquare.Key.x, maxSquare.Key.y];
            CallOnSquares(squares, (tileHeight) =>
            {
                return maxY - tileHeight;
            });
        }

        static public void Minimum(Squares squares)
        {
            var minSquare = squares.Aggregate(
                (l, r) => {
                    var tileHeightL = WorldManager.Instance.heightMap[l.Key.x, l.Key.y];
                    var tileHeightR = WorldManager.Instance.heightMap[r.Key.x, r.Key.y];
                    return tileHeightL < tileHeightR ? l : r;
                });
            var minY = WorldManager.Instance.heightMap[minSquare.Key.x, minSquare.Key.y];
            CallOnSquares(squares, (tileHeight) =>
            {
                return minY - tileHeight;
            });
        }
        
        static public void Average(Squares squares)
        {
            var averageY = Mathf.RoundToInt(
                (float)squares.Aggregate(new HashSet<int>(),
                    (list, square) => {
                        var tileHeight = WorldManager.Instance.heightMap[square.Key.x, square.Key.y];
                        if (!list.Contains(tileHeight))
                        {
                            list.Add(tileHeight);
                        }
                        return list;
                    },
                    (list) => list.Average()));
            CallOnSquares(squares, (tileHeight) =>
            {
                return averageY - tileHeight;
            });
        }

        static public void Adjust(Squares squares, int diff)
        {
            CallOnSquares(squares, (tileHeight) =>
            {
                return diff;
            });
        }

        static void CallOnSquares(Squares squares, Func<int, int> routine)
        {
            foreach (var square in squares)
            {
                var tileHeight = WorldManager.Instance.heightMap[square.Key.x, square.Key.y];
                var diff = routine(tileHeight);
                NetworkMapSharer.Instance.changeTileHeight(diff, square.Key.x, square.Key.y);
            }
        }
    }
}
