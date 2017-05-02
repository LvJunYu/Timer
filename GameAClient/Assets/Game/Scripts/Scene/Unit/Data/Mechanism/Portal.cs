/********************************************************************
** Filename : PortalUnit
** Author : Dong
** Date : 2017/1/5 星期四 下午 9:28:53
** Summary : Portal
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using Spine.Unity;

namespace GameA.Game
{
    [Unit(Id = 5003, Type = typeof(Portal))]
    public class Portal : BlockBase
    {
        protected override bool OnInit()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _animation = new AnimationSystem();
            return _animation.Init(this, "Run");
        }

        public static void OnPortal(PairUnit pairUnit, UnitDesc unitDesc)
        {
            var sender = pairUnit.Sender;
            if (sender == null)
            {
                return;
            }
            var tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                return;
            }
            var colliderGrid = tableUnit.GetColliderGrid(ref unitDesc);
            //检测周围是否有空间可以传送
            var checkGrid = GM2DTools.CalculateFireColliderGrid(sender.Id, colliderGrid, unitDesc.Rotation);
            var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid,
                JoyPhysics2D.GetColliderLayerMask(sender.DynamicCollider.Layer));
            var speed = IntVec2.zero;
            //for (int i = 0; i < units.Count; i++)
            //{
            //    if (GM2DTools.OnDirectionHit(units[i], sender, unitDesc.Rotation))
            //    {
            //        return;
            //    }
            //}
            switch ((ERotationType)unitDesc.Rotation)
            {
                case ERotationType.Up:
                    speed.x = (pairUnit.TriggeredCnt / 2) % 2 == 0 ? 45 : -45;
                    speed.y = 165;
                    break;
                case ERotationType.Down:
                    break;
                case ERotationType.Left:
                    speed.x = -60;
                    break;
                case ERotationType.Right:
                    speed.x = 60;
                    break;
            }
            var targetMin = new IntVec2(checkGrid.XMin,checkGrid.YMin);
            sender.OnPortal(sender.TableUnit.ColliderToRenderer(targetMin, sender.Rotation), speed);
            pairUnit.Sender = null;
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (other.SpeedY <= 0 && Rotation == (int)ERotationType.Up)
                {
                    OnTrigger(other);
                }
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (other.SpeedY > 0 && Rotation == (int)ERotationType.Down)
                {
                    OnTrigger(other);
                }
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (Rotation == (int)ERotationType.Left)
                {
                    OnTrigger(other);
                }
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (Rotation == (int)ERotationType.Right)
                {
                    OnTrigger(other);
                }
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        private void OnTrigger(UnitBase other)
        {
            if (!other.CanPortal)
            {
                return;
            }
            if (other.IsAlive && other.EUnitState != EUnitState.Portaling)
            {
                PairUnitManager.Instance.OnPairTriggerEnter(other, this);
            }
        }
    }
}
