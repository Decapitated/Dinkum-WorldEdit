using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WorldEditMod
{
    using Squares = Dictionary<Vector2Int, TapeMeasureSquare>;
    static internal class Levelers
    {
        static public void Player(Squares squares)
        {
            var playerPosition = NetworkMapSharer.Instance.localChar.gameObject.transform.position;
            var dinkPos = DivineDinkum.Utilities.UnityToDinkumPosition(playerPosition);
            var playerHeight = WorldManager.Instance.heightMap[(int)dinkPos.x, (int)dinkPos.z];
            foreach (var square in squares)
            {
                var tilePos = square.Key;
                var tileHeight = WorldManager.Instance.heightMap[tilePos.x, tilePos.y];
                var diff = playerHeight - tileHeight;
                NetworkMapSharer.Instance.changeTileHeight(diff, tilePos.x, tilePos.y);
            }
        }

        static public void Adjust(Squares squares, int diff)
        {
            foreach (var square in squares)
            {
                var tilePos = square.Key;
                NetworkMapSharer.Instance.changeTileHeight(diff, tilePos.x, tilePos.y);
            }
        }
    }
}
