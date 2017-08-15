using System.Collections.Generic;

namespace GameA.Game
{
    [Unit(Id = 8100, Type = typeof(SwitchTrigger))]
    public class SwitchTrigger : UnitBase
    {
        protected SwitchUnit _switchUnit;
        protected bool _trigger;
        protected List<UnitBase> _units = new List<UnitBase>();

        public SwitchUnit SwitchUnit
        {
            get { return _switchUnit; }
            set { _switchUnit = value; }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _canLazerCross = true;
            _canMagicCross = true;
            _canBridgeCross = true;
            _canFanCross = true;
            SetSortingOrderBack();
            return false;
        }

        protected override void InitAssetPath()
        {
            InitAssetRotation();
        }

        protected override void Clear()
        {
            base.Clear();
            _trigger = false;
            _units.Clear();
        }

        public override void OnIntersect(UnitBase other)
        {
            OnTrigger(other);
        }

        protected virtual void OnTrigger(UnitBase other)
        {
            if (_units.Contains(other))
            {
                return;
            }
            _units.Add(other);
            _trigger = !_trigger;
            if (_trigger)
            {
                OnTriggerStart(other);
            }
            else
            {
                OnTriggerEnd();
            }
        }

        public override void UpdateLogic()
        {
            if (_units.Count > 0)
            {
                for (int i = _units.Count - 1; i >= 0; i--)
                {
                    if (_units[i] == null || !_colliderGrid.Intersects(_units[i].ColliderGrid))
                    {
                        _units.RemoveAt(i);
                    }
                }
            }
        }

        protected void OnTriggerStart(UnitBase other)
        {
            if (_view != null)
            {
                _view.ChangeView("M1SwitchTriggerOn_" + _unitDesc.Rotation);
            }
            if (_switchUnit != null)
            {
                _switchUnit.OnTriggerStart(other);
            }
        }

        protected void OnTriggerEnd()
        {
            if (!_trigger)
            {
                if (_view != null)
                {
                    _view.ChangeView("M1SwitchTriggerOff_" + _unitDesc.Rotation);
                }
                if (_switchUnit != null)
                {
                    _switchUnit.OnTriggerEnd();
                }
            }
        }
    }
}