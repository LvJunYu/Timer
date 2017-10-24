/********************************************************************
** Filename : BrickUnit
** Author : Dong
** Date : 2016/10/20 星期四 上午 11:52:39
** Summary : Brick
***********************************************************************/

using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 4006, Type = typeof(Brick))]
    public class Brick : BlockBase
    {
        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (other.IsMain || UnitDefine.IsBullet(other.Id))
                {
                    DestroyBrick();
                }
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            Check(other, checkOnly);
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.CanDashBrick)
            {
                DestroyBrick();
                return false;
            }
            Check(other, checkOnly);
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.CanDashBrick)
            {
                DestroyBrick();
                return false;
            }
            Check(other, checkOnly);
            return base.OnRightHit(other, ref x, checkOnly);
        }

        protected void Check(UnitBase other, bool checkOnly)
        {
            if (!checkOnly)
            {
                if (UnitDefine.IsBullet(other.Id))
                {
                    DestroyBrick();
                }
            }
        }

        public void DestroyBrick()
        {
            if (!_isAlive)
            {
                return;
            }
            PlayMode.Instance.DestroyUnit(this);
            OnDead();
            Messenger.Broadcast(EMessengerType.OnBreakBrick);
        }
    }
}