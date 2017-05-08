/********************************************************************
** Filename : SwitchPress
** Author : Dong
** Date : 2017/5/8 星期一 下午 2:54:54
** Summary : SwitchPress
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace GameA.Game
{
    [Unit(Id = 5111, Type = typeof(SwitchPress))]
    public class SwitchPress : BlockBase
    {
        protected bool _trigger;
        protected List<UnitBase> _units = new List<UnitBase>();
        protected bool _colliderTrigger;
        protected List<UnitBase> _colliderUnits = new List<UnitBase>();

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            SetSortingOrderFront();
            return true;
        }

        internal override void Reset()
        {
            base.Reset();
            _trigger = false;
            _colliderTrigger = false;
            _units.Clear();
            _colliderUnits.Clear();
        }

        public override void OnHit(UnitBase other)
        {
            OnTrigger(other);
        }

        public override void OnColliderEnter(UnitBase other)
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

        public override void OnColliderExit(UnitBase other)
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
            //if (_switchUnit != null)
            //{
            //    _switchUnit.OnTrigger(other);
            //}
        }

        protected void OnTriggerEnd()
        {
            //if (_switchUnit != null)
            //{
            //    _switchUnit.OnTriggerEnd();
            //}
        }
    }
}
