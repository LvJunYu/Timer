/********************************************************************
** Filename : BlockBase
** Author : Dong
** Date : 2017/4/18 星期二 上午 11:29:18
** Summary : BlockBase
***********************************************************************/

namespace GameA.Game
{
    public class BlockBase : Magic
    {
        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnUpClampSpeed(other);
                y = GetUpHitMin();
            }
            return true;
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnDownClampSpeed(other);
                y = GetDownHitMin(other);
            }
            return true;
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnLeftClampSpeed(other);
                x = GetLeftHitMin(other);
            }
            return true;
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnRightClampSpeed(other);
                x = GetRightHitMin();
            }
            return true;
        }

        public override bool OnLeftUpHit(UnitBase other, ref int x, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnUpClampSpeed(other);
                x = other.ColliderGrid.XMin;
                y = GetUpHitMin();
            }
            return true;
        }

        public override bool OnRightUpHit(UnitBase other, ref int x, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnUpClampSpeed(other);
                x = other.ColliderGrid.XMin;
                y = GetUpHitMin();
            }
            return true;
        }

        public override bool OnLeftDownHit(UnitBase other, ref int x, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnLeftClampSpeed(other);
                x = GetLeftHitMin(other);
                y = other.ColliderGrid.YMin;
            }
            return true;
        }

        public override bool OnRightDownHit(UnitBase other, ref int x, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnRightClampSpeed(other);
                x = GetRightHitMin();
                y = other.ColliderGrid.YMin;
            }
            return true;
        }
    }
}
