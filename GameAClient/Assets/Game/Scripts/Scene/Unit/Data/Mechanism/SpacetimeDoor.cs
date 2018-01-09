using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 5019, Type = typeof(SpacetimeDoor))]
    public class SpacetimeDoor : BlockBase
    {
        private UnitBase _outUnit;
        private int _outTimer;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }

            SetSortingOrderBackground();
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            if (_withEffect != null)
            {
                SetRelativeEffectPos(_withEffect.Trans, (EDirectionType) Rotation);
            }

            if (GameRun.Instance.IsPlaying && _view.PairTrans != null)
            {
                _view.PairTrans.SetActiveEx(false);
            }

            return true;
        }

        public override void UpdateLogic()
        {
            if (_outTimer > 0)
            {
                _outTimer--;
            }
            else
            {
                if (_outUnit != null && !_colliderGrid.Intersects(_outUnit.ColliderGrid))
                {
                    _outUnit = null;
                }
            }
            base.UpdateLogic();
        }

        public static void OnSpacetimeDoor(PairUnit pairUnit, bool enterADoor)
        {
            var sender = pairUnit.Sender;
            if (sender == null) return;
            UnitDesc unitDesc = enterADoor ? pairUnit.UnitB : pairUnit.UnitA;
            int sceneIndex = enterADoor ? pairUnit.UnitBScene : pairUnit.UnitAScene;
            if (sceneIndex == Scene2DManager.Instance.CurSceneIndex)
            {
                LogHelper.Error("The Spacetime Door is in the same scene");
                return;
            }
            Scene2DManager.Instance.ChangeScene(sceneIndex);

            UnitBase unit;
            if (!Scene2DManager.Instance.CurColliderScene2D.TryGetUnit(unitDesc.Guid, out unit))
            {
                LogHelper.Error("can not get unit");
                return;
            }

            SpacetimeDoor spacetimeDoor = unit as SpacetimeDoor;
            if (spacetimeDoor == null)
            {
                LogHelper.Error("the out spacetimeDoor is null");
                return;
            }

            sender.EnterSpacetimeDoor(spacetimeDoor.CurPos);
            spacetimeDoor.OnUnitOut(sender);
            pairUnit.Sender = null;
        }

        private void OnUnitOut(UnitBase unit)
        {
            _outUnit = unit;
            _outTimer = 10;
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnTrigger(other);
            }

            if (other.IsPlayer || UnitDefine.IsBullet(other.Id)) return false;
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnTrigger(other);
            }

            if (other.IsPlayer || UnitDefine.IsBullet(other.Id)) return false;
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnTrigger(other);
            }

            if (other.IsPlayer || UnitDefine.IsBullet(other.Id)) return false;
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnTrigger(other);
            }

            if (other.IsPlayer || UnitDefine.IsBullet(other.Id)) return false;
            return base.OnRightHit(other, ref x, checkOnly);
        }

        private void OnTrigger(UnitBase other)
        {
            if (other == _outUnit || !other.CanPassBlackHole)
            {
                return;
            }

            if (other.IsAlive && other.EUnitState == EUnitState.Normal)
            {
                PairUnitManager.Instance.OnPairTriggerEnter(other, this);
            }
        }

        protected override void Clear()
        {
            base.Clear();
            _outUnit = null;
            _outTimer = 0;
        }
    }
}