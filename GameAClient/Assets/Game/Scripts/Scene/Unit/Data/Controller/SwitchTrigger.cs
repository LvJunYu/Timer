using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 8100, Type = typeof(SwitchTrigger))]
    public class SwitchTrigger : Magic
    {
        protected SwitchUnit _switchUnit;
        protected bool _trigger;
        protected List<UnitBase> _units = new List<UnitBase>();

        public SwitchUnit SwitchUnit
        {
            get { return _switchUnit; }
            set { _switchUnit = value; }
        }
        
        public bool Trigger
        {
            get { return _trigger; }
            set
            {
                _trigger = value;
                if (_trigger)
                {
                    OnTriggerStart(this);
                }
            }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            SetAllCross(true);
            SetSortingOrderBack();
            return true;
        }

        protected override void InitAssetPath()
        {
            InitAssetRotation();
        }

        protected override void Clear()
        {
            base.Clear();
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

        public void UpdateSwitchPos(IntVec2 deltaPos)
        {
            if (_isAlive)
            {
                _deltaPos = deltaPos;
                _curPos += _deltaPos;
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                UpdateTransPos();
            }
        }

        protected void OnTriggerStart(UnitBase other)
        {
            ChangView(true);
            if (_switchUnit != null)
            {
                _switchUnit.OnTriggerStart(other);
            }
        }

        protected void OnTriggerEnd()
        {
            if (!_trigger)
            {
                ChangView(false);
                if (_switchUnit != null)
                {
                    _switchUnit.OnTriggerEnd();
                }
            }
        }

        protected virtual void ChangView(bool on)
        {
            if (_view != null)
            {
                if (on)
                {
                    _view.ChangeView("M1SwitchTriggerPressOn_" + _unitDesc.Rotation);
                }
                else
                {
                    _view.ChangeView("M1SwitchTriggerPressOff_" + _unitDesc.Rotation);
                }
            }
        }
    }
}