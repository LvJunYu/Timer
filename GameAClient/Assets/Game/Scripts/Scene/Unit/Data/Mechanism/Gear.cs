using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 5017, Type = typeof(Gear))]
    public class Gear : BlockBase
    {
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _animation.Init("Run");
            return true;
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

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            base.Hit(unit, eDirectionType);
            OnTrigger(unit);
        }

        private void OnTrigger(UnitBase other)
        {
            if (other.IsActor && other.IsAlive)
            {
                other.InSaw();
            }
        }
    }
}