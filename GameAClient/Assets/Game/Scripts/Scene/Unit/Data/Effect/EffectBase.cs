using SoyEngine;

namespace GameA.Game
{
    public class EffectBase : UnitBase
    {
        protected bool _trigger;
        protected UnitBase _unit;
        
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            SetAllCross(true);
            SetSortingOrderFrontest();
            return true;
        }
        
        protected override void Clear()
        {
            base.Clear();
            _trigger = false;
            _unit = null;
        }

        public override void OnIntersect(UnitBase other)
        {
            if (other.IsMain)
            {
                if (_trigger)
                {
                    return;
                }
                _trigger = true;
                _unit = other;
                OnTrigger();
            }
        }

        protected virtual void OnTrigger()
        {
            GameParticleManager.Instance.Emit(_tableUnit.Model, _trans);
        }
        
        public virtual void UpdateLogic(float deltaTime)
        {
            if (_trigger && _unit != null)
            {
                if (!_colliderGrid.Intersects(_unit.ColliderGrid))
                {
                    _trigger = false;
                    _unit = null;
                }
            }
        }
    }
}