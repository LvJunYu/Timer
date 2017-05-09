/********************************************************************
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
        public static UnitDefine _instance;

        public static UnitDefine Instance
        {
            get { return _instance??(_instance = new UnitDefine()); }
        }

        public const int SwitchTriggerId = 5100;

        public bool IsSwitch(int id)
        {
            return id >= 5101 && id <= 5200;
        }

        public bool IsFakePart(int one, int other)
        {
            return (one == 4001 && other == 4013) || (one == 4013 && other == 4001);
        }
    }
}
