/********************************************************************
** Filename : TransparentEarth
** Author : Dong
** Date : 2016/12/29 星期四 下午 9:13:12
** Summary : TransparentEarth
***********************************************************************/

namespace GameA.Game
{
    [Unit(Id = 4004, Type = typeof (TransparentEarth))]
    public class TransparentEarth : BlockBase
    {
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _canLazerCross = true;
            _canFanCross = true;
            SetSortingOrderBack();
            return true;
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other is Box)
            {
                return false;
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnRightUpHit(UnitBase other, ref int x, ref int y, bool checkOnly = false)
        {
            if (other is Box)
            {
                return false;
            }
            return base.OnRightUpHit(other, ref x, ref y, checkOnly);
        }

        public override bool OnLeftUpHit(UnitBase other, ref int x, ref int y, bool checkOnly = false)
        {
            if (other is Box)
            {
                return false;
            }
            return base.OnLeftUpHit(other, ref x, ref y, checkOnly);
        }
    }
}