/********************************************************************
** Filename : BattleDefine
** Author : Dong
** Date : 2017/7/6 星期四 下午 3:33:07
** Summary : BattleDefine
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    public class BattleDefine
    {
        public const int MaxSpeedX = 60;
        public const int MaxQuickenSpeedX = 120;
        
        public const int StunTime = 2000;
        
        public const int IceLifeTime = 2000;
        public const int IceSwordLifeTime = 5000;
        
        public const int WallJumpBanInputTime = 20;
        public const int QuickenTime = 3 * ConstDefineGM2D.FixedFrameCount;
    }
}
