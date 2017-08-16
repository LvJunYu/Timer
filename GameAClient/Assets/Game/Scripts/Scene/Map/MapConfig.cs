/********************************************************************
** Filename : MapConfig
** Author : Dong
** Date : 2016/8/10 星期三 下午 4:37:47
** Summary : MapConfig
***********************************************************************/

using SoyEngine.Proto;
using SoyEngine;

namespace GameA.Game
{
    public class MapConfig
    {
        public static int FinalItemId;
        public static int SpawnId;
        public static int TerrainItemId;
        public static IntVec2 PermitMapSize;
        public static bool UseAOI ;

        public static bool Init()
        {
            UseAOI = false;
            TerrainItemId = 4001;
            SpawnId = 1001;
            FinalItemId = 5001;
            PermitMapSize = new IntVec2(60, 30) * ConstDefineGM2D.ServerTileScale;
            return true;
        }
    }
}