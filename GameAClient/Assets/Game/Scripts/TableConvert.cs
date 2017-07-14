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
            return (int) (speed * ConstDefineGM2D.ServerTileScale * ConstDefineGM2D.FixedDeltaTime);
        }

        /// <summary>
        /// 1米 = 1000
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetTime(int time)
        {
            return (int) (time * ConstDefineGM2D.FixedFrameCount * 0.001f);
        }

        /// <summary>
        /// 1米 = 10
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static int GetRange(int range)
        {
            return (int) (range * ConstDefineGM2D.ServerTileScale * 0.1f);
        }
    }
}