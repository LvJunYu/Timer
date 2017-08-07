/********************************************************************
** Filename : SwitchTrigger
** Author : Dong
** Date : 2017/5/9 星期二 上午 10:32:53
** Summary : SwitchTrigger
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 8101, Type = typeof(SwitchTrigger))]
    public class SwitchTrigger : UnitBase
    {
        protected SwitchPress _switchPress;
        protected bool _trigger;
        protected List<UnitBase> _units = new List<UnitBase>();
        protected bool _gridCheckTrigger;
        protected List<UnitBase> _gridCheckUnits = new List<UnitBase>();

        public SwitchPress SwitchPress
        {
            get { return _switchPress; }
            set { _switchPress = value; }
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
            _gridCheckTrigger = false;
            _units.Clear();
            _gridCheckUnits.Clear();
        }

        public override void OnIntersect(UnitBase other)
        {
            OnTrigger(other);
        }

        public void OnGridCheckEnter(UnitBase other)
        {
            if (_gridCheckUnits.Contains(other))
            {
                return;
            }
            _gridCheckUnits.Add(other);
            if (_gridCheckTrigger)
            {
                return;
            }
            _gridCheckTrigger = true;
            OnTriggerStart(other);
        }

        public void OnGridCheckExit(UnitBase other)
        {
            _gridCheckUnits.Remove(other);
            if (_gridCheckUnits.Count == 0)
            {
                _gridCheckTrigger = false;
                OnTriggerEnd();
            }
        }

        protected virtual void OnTrigger(UnitBase other)
        {
            if (_units.Contains(other))
            {
                return;
            }
            _units.Add(other);
            if (_trigger)
            {
                return;
            }
            _trigger = true;
            OnTriggerStart(other);
        }

        public override void UpdateLogic()
        {
            if (_trigger)
            {
                for (int i = _units.Count - 1; i >= 0; i--)
                {
                    if (_units[i] == null || !_colliderGrid.Intersects(_units[i].ColliderGrid))
                    {
                        _units.RemoveAt(i);
                    }
                }
                if (_units.Count == 0)
                {
                    _trigger = false;
                    OnTriggerEnd();
                }
            }
        }

        protected void OnTriggerStart(UnitBase other)
        {
            if (_view != null)
            {
                _view.ChangeView("M1SwitchTriggerOn_"+_unitDesc.Rotation);
            }
            if (_switchPress != null)
            {
                _switchPress.OnTriggerStart(other);
            }
        }

        protected void OnTriggerEnd()
        {
            if (!_trigger && !_gridCheckTrigger)
            {
                if (_view != null)
                {
                    _view.ChangeView("M1SwitchTriggerOff_" + _unitDesc.Rotation);
                }
                if (_switchPress != null)
                {
                    _switchPress.OnTriggerEnd();
                }
            }
        }
    }
}
