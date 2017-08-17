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
    [Unit(Id = 8101, Type = typeof(SwitchTriggerPress))]
    public class SwitchTriggerPress : SwitchTrigger
    {
        protected bool _gridCheckTrigger;
        protected List<UnitBase> _gridCheckUnits = new List<UnitBase>();
        
        protected override void Clear()
        {
            base.Clear();
            _gridCheckTrigger = false;
            _gridCheckUnits.Clear();
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
                if (!_trigger && !_gridCheckTrigger)
                {
                    OnTriggerEnd();
                }
            }
        }

        protected override void OnTrigger(UnitBase other)
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
                if (_units.Count == 0)
                {
                    _trigger = false;
                    if (!_trigger && !_gridCheckTrigger)
                    {
                        OnTriggerEnd();
                    }
                }
            }
        }

        protected override void ChangView(bool on)
        {
            if (_view != null)
            {
                if (on)
                {
                    _view.ChangeView("M1SwitchTriggerOn_" + _unitDesc.Rotation);
                }
                else
                {
                    _view.ChangeView("M1SwitchTriggerOff_" + _unitDesc.Rotation);
                }
            }
        }
    }
}
