﻿/********************************************************************
** Filename : UnitDefine
** Author : Dong
** Date : 2017/1/10 星期二 下午 9:52:42
** Summary : UnitDefine
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    public class UnitDefine
    {
        public const int SwitchTriggerId = 5100;

        public const int ClayId = 4011;
        public const int BlueStoneId = 4101;
        public const int BlueStoneRotateId = 4103;
        public const int BoxId = 5004;
        public const int RollerId = 5005;
        public const int BillboardId = 7001;

        public const int PlayerTableId = 1001;
        public const int FinalDoorId = 5001;

        public static bool IsEnergy(int id)
        {
            return id >= 6101 && id <= 6105;
        }

        public static bool IsSwitch(int id)
        {
            return id >= 5101 && id <= 5200;
        }

        public static bool IsFakePart(int one, int other)
        {
            return (one == 4001 && other == 4013) || (one == 4013 && other == 4001);
        }

        public static bool IsEarth(int id)
        {
            return id == 4001 || id == 4002;
        }

        public static bool IsPlant(int id)
        {
            return id == 7002 || id == 7003 || id == 7004;
        }
    }
}
