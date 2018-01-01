/********************************************************************
** Filename : MapConfig
** Author : Dong
** Date : 2016/8/10 星期三 下午 4:37:47
** Summary : MapConfig
***********************************************************************/

namespace GameA.Game
{
    public class MapConfig
    {
        public static int FinalItemId;
        public static int SpawnId;
        public static int TerrainItemId;
        public static bool UseAOI ;

        public static bool Init()
        {
            UseAOI = true;
            TerrainItemId = UnitDefine.TerrainId;
            SpawnId = UnitDefine.SpawnId;
            FinalItemId = UnitDefine.FinalDoorId;
            return true;
        }
    }
}