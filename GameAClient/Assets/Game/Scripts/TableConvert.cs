using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class TableConvert
    {
        /// <summary>
        /// 米每秒 ->格子每帧
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static int GetSpeed(int speed)
        {
            return Mathf.RoundToInt(speed * ConstDefineGM2D.ServerTileScale * ConstDefineGM2D.FixedDeltaTime);
        }

        /// <summary>
        /// 1米 = 1000
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetTime(int time)
        {
            return Mathf.RoundToInt(time * ConstDefineGM2D.FixedFrameCount * 0.001f);
        }

        /// <summary>
        /// 1米 = 10
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static int GetRange(int range)
        {
            return Mathf.RoundToInt(range * ConstDefineGM2D.ServerTileScale * 0.1f);
        }
    }
}