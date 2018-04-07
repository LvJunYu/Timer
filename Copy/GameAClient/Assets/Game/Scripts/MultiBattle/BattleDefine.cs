﻿/********************************************************************
** Filename : BattleDefine
** Author : Dong
** Date : 2017/7/6 星期四 下午 3:33:07
** Summary : BattleDefine
***********************************************************************/

namespace GameA.Game
{
    public class BattleDefine
    {
        public const int IceLifeTime = 3000;
        
        public const int WallJumpBanInputTime = 10;
        public const int QuickenTime = 3 * ConstDefineGM2D.FixedFrameCount;

        public const int MaxWingCount = 20;
        
        public const int DamageDurationFrame =  (int) (0.2f * ConstDefineGM2D.FixedFrameCount);
        
        public const int HpStayTime =  4 * ConstDefineGM2D.FixedFrameCount;
    }
}
