﻿using SoyEngine;

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
        
        internal override void OnPlay()
        {
            base.OnPlay();
            if (_view != null)
            {
                _view.SetRendererEnabled(false);
            }
        }

        internal override void OnEdit()
        {
            base.OnEdit();
            if (_view != null)
            {
                _view.SetRendererEnabled(true);
            }
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
                OnTriggerEnter();
            }
        }

        protected virtual void OnTriggerEnter()
        {
        }

        protected virtual void OnTriggerExit()
        {
        }

        public override void UpdateLogic()
        {
            if (_trigger && _unit != null)
            {
                if (!_colliderGrid.Intersects(_unit.ColliderGrid))
                {
                    _trigger = false;
                    _unit = null;
                    OnTriggerExit();
                }
            }
        }
    }
}