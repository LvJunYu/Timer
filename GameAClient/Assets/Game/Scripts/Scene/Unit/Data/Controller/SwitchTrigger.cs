using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 8100, Type = typeof(SwitchTrigger))]
    public class SwitchTrigger : Magic
    {
        protected SwitchUnit _switchUnit;
        protected List<UnitBase> _units = new List<UnitBase>();
        protected EActiveState _trigger;

        public SwitchUnit SwitchUnit
        {
            get { return _switchUnit; }
            set
            {
                _switchUnit = value;
                SetTrigger(_switchUnit.EActiveState);
            }
        }

        public EActiveState Trigger
        {
            get { return _trigger; }
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
            if (!UnitDefine.CanTrigger(other.Id) || _units.Contains(other))
            {
                return;
            }
            _units.Add(other);
            SetTrigger(_trigger == EActiveState.Active ? EActiveState.Deactive : EActiveState.Active);
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

        protected virtual void SetTrigger(EActiveState value)
        {
            if (_trigger != value)
            {
                _trigger = value;
                OnTriggerChanged();
            }
        }

        protected virtual void OnTriggerChanged()
        {
            if (_switchUnit != null)
            {
                _switchUnit.OnTriggerChanged(_trigger);
            }
            ChangView();
//            LogHelper.Debug("OnTriggerChanged {0}", _trigger);
        }

        protected virtual void ChangView()
        {
            if (_view != null)
            {
                if (_trigger == EActiveState.Active)
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