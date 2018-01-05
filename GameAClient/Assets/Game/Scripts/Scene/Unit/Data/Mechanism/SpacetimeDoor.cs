using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 4016, Type = typeof(SpacetimeDoor))]
    public class SpacetimeDoor : BlockBase
    {
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

            if (_view.PairTrans != null)
            {
                _view.PairTrans.SetActiveEx(!GameRun.Instance.IsPlaying);
            }
            return true;
        }

        public static void OnSpacetimeDoor(PairUnit pairUnit, UnitDesc unitDesc)
        {
            var sender = pairUnit.Sender;
            if (sender == null)
            {
                return;
            }

            int sceneIndex = unitDesc.SceneIndx;
            if (sceneIndex == Scene2DManager.Instance.CurSceneIndex)
            {
                LogHelper.Error("The Spacetime Door is in the same scene");
                return;
            }

            UnitBase unit;
            if (!Scene2DManager.Instance.GetColliderScene2D(sceneIndex).TryGetUnit(unitDesc.Guid, out unit))
            {
                LogHelper.Error("can not get unit");
                return;
            }

            Scene2DManager.Instance.ChangeScene(sceneIndex);
            sender.OnSpacetimeDoor(unit.CurPos);
            pairUnit.Sender = null;
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnTrigger(other);
            }

            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnTrigger(other);
            }

            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnTrigger(other);
            }

            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnTrigger(other);
            }

            return base.OnRightHit(other, ref x, checkOnly);
        }

        private void OnTrigger(UnitBase other)
        {
            if (!other.CanPassBlackHole)
            {
                return;
            }

            if (other.IsAlive && other.EUnitState == EUnitState.Normal)
            {
                PairUnitManager.Instance.OnPairTriggerEnter(other, this);
            }
        }
    }
}