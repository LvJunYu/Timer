using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 8100, Type = typeof(SwitchTrigger))]
    public class SwitchTrigger : Magic
    {
        protected SwitchUnit _switchUnit;
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
            if (_switchUnit != null)
            {
                SetActiveState(_switchUnit.EActiveState);
            }
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
            SetActiveState(_eActiveState == EActiveState.Active?EActiveState.Deactive : EActiveState.Active);
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

        protected override void OnActiveStateChanged()
        {
            base.OnActiveStateChanged();
            if (_switchUnit != null)
            {
                _switchUnit.SetActiveState(_eActiveState);
            }
            ChangView();
        }

        protected virtual void ChangView()
        {
            if (_view != null)
            {
                if (_eActiveState == EActiveState.Active)
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