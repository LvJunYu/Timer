using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 8106, Type = typeof(SwitchRect))]
    public class SwitchRect : UnitBase
    {
        protected bool _trigger;
        protected UnitBase _mainUnit;
        protected List<UnitBase> _units;

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _view.SetRendererEnabled(!GameRun.Instance.IsPlay);
            return true;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            _units = DataScene2D.CurScene.GetControlledUnits(_guid);
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

        protected override void Clear()
        {
            base.Clear();
            _mainUnit = null;
            _units = null;
            _trigger = false;
        }

        public override void OnIntersect(UnitBase other)
        {
            if (other.IsPlayer)
            {
                OnTrigger(other);
            }
        }

        protected virtual void OnTrigger(UnitBase other)
        {
            if (_trigger)
            {
                return;
            }
            _trigger = true;
            _mainUnit = other;
            OnTriggerStart(other);
        }

        public override void UpdateLogic()
        {
            if (_trigger)
            {
                if (_mainUnit != null)
                {
                    if (!_colliderGrid.Intersects(_mainUnit.ColliderGrid))
                    {
                        _trigger = false;
                        OnTriggerEnd();
                    }
                }
            }
        }

        protected void OnTriggerStart(UnitBase other)
        {
            if (_units != null)
            {
                for (int i = 0; i < _units.Count; i++)
                {
                    var unit = _units[i];
                    if (unit != null && unit.IsAlive)
                    {
                        unit.OnSwitchRectStart(this);
                    }
                }
            }
        }

        protected void OnTriggerEnd()
        {
            if (_units != null)
            {
                for (int i = 0; i < _units.Count; i++)
                {
                    var unit = _units[i];
                    if (unit != null && unit.IsAlive)
                    {
                        unit.OnSwitchRectEnd(this);
                    }
                }
            }
        }
    }
}