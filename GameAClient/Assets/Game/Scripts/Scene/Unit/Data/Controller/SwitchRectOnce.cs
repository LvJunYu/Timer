using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 8107, Type = typeof(SwitchRectOnce))]
    public class SwitchRectOnce : UnitBase
    {
        protected List<UnitBase> _units;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            SetAllCross(true);
            return true;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            _units = DataScene2D.Instance.GetControlledUnits(_guid);
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
            _units = null;
        }

        public override void OnIntersect(UnitBase other)
        {
            if (other.IsMain)
            {
                OnTrigger(other);
            }
        }

        protected virtual void OnTrigger(UnitBase other)
        {
            OnTriggerStart(other);
            PlayMode.Instance.DestroyUnit(this);
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
                        unit.OnSwitchRectOnce();
                    }
                }
            }
        }
    }
}