using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 4016, Type = typeof(BlackHole))]
    public class BlackHole : BlockBase
    {
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
//            InitAssetRotation(true);
            if (_withEffect != null)
            {
                SetRelativeEffectPos(_withEffect.Trans, (EDirectionType) Rotation);
            }
            return true;
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
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].IsAlive)
                {
                    if (GM2DTools.OnDirectionHit(units[i], sender, (EMoveDirection) (unitDesc.Rotation + 1)))
                    {
                        return;
                    }
                }
            }
            var speed = IntVec2.zero;
            switch ((EDirectionType) unitDesc.Rotation)
            {
                case EDirectionType.Up:
                    speed.x = (pairUnit.TriggeredCnt / 2) % 2 == 0 ? 45 : -45;
                    speed.y = 165;
                    break;
                case EDirectionType.Down:
                    break;
                case EDirectionType.Left:
                    speed.x = -60;
                    speed.y = 1;
                    break;
                case EDirectionType.Right:
                    speed.x = 60;
                    speed.y = 1;
                    break;
            }
            var targetMin = new IntVec2(checkGrid.XMin, checkGrid.YMin);
            sender.OnPortal(sender.TableUnit.ColliderToRenderer(targetMin, sender.Rotation), speed);
            pairUnit.Sender = null;
            if (UnitDefine.IsMain(unitDesc.Id))
            {
                Messenger.Broadcast(EMessengerType.OnPlayerEnterPortal);
            }
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (other.SpeedY <= 0 && Rotation == (int) EDirectionType.Up)
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
                if (other.SpeedY > 0 && Rotation == (int) EDirectionType.Down)
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
                if (Rotation == (int) EDirectionType.Left)
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
                if (Rotation == (int) EDirectionType.Right)
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