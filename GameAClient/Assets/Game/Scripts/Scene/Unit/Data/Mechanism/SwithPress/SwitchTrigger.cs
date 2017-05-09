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
    [Unit(Id = 5100, Type = typeof(SwitchTrigger))]
    public class SwitchTrigger : UnitBase
    {
        protected SwitchPress _switchPress;
        protected bool _trigger;
        protected List<UnitBase> _units = new List<UnitBase>();
        protected bool _colliderTrigger;
        protected List<UnitBase> _colliderUnits = new List<UnitBase>();

        public SwitchPress SwitchPress
        {
            get { return _switchPress; }
            set { _switchPress = value; }
        }

        internal override void Reset()
        {
            base.Reset();
            _trigger = false;
            _colliderTrigger = false;
            _switchPress = null;
            _units.Clear();
            _colliderUnits.Clear();
        }

        public override void OnHit(UnitBase other)
        {
            OnTrigger(other);
        }

        public void OnColliderEnter(UnitBase other)
        {
            if (_colliderUnits.Contains(other))
            {
                return;
            }
            _colliderUnits.Add(other);
            if (_colliderTrigger)
            {
                return;
            }
            _colliderTrigger = true;
            OnTriggerStart(other);
        }

        public void OnColliderExit(UnitBase other)
        {
            _colliderUnits.Remove(other);
            if (_colliderUnits.Count == 0)
            {
                _colliderTrigger = false;
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

        public void UpdateLogic(float deltaTime)
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
                _view.ChangeView("M1SwitchTriggeOn_"+_unitDesc.Rotation);
            }
            if (_switchPress != null)
            {
                _switchPress.OnTriggerStart(other);
            }
        }

        protected void OnTriggerEnd()
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
