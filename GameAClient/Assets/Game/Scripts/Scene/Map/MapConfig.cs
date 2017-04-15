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
        public static int MainPlayerId;
        public static int TerrainItemId;
        public static IntVec2 PermitMapSize;
        public static bool UseAOI ;

        public static bool Init()
        {
            UseAOI = true;
//            var matrixType = GM2DGame.Instance.EMatrixType;
//            switch (matrixType)
//            {
//                case EMatrixType.MT_JumpPlatform:
//                    EProjectCategory category = GM2DGame.Instance.Project.ProjectCategory;
//                    switch (category)
//                    {
//                        case EProjectCategory.PC_Relaxation:
//                        case EProjectCategory.PC_Puzzle:
                            MainPlayerId = 1001;
                            TerrainItemId = 4001;
                            FinalItemId = 5001;
                            PermitMapSize = new IntVec2(60, 30) *ConstDefineGM2D.ServerTileScale;
                            //UseAOI = false;
//                            break;
//                        case EProjectCategory.PC_Challenge:
//                            MainPlayerId = 1001;
//                            TerrainItemId = 4001;
//                            FinalItemId = 5001;
//                            PermitMapSize = new IntVec2(200, 200) * ConstDefineGM2D.ServerTileScale;
//                            break;
//                    }
//                    break;
//                case EMatrixType.MT_Sandbox:
//                    MainPlayerId = 31001;
//                    TerrainItemId = 34001;
//                    FinalItemId = 35001;
//                    break;
//                case EMatrixType.MT_RunCool:
//                    break;
//            }
            return true;
        }
    }
}