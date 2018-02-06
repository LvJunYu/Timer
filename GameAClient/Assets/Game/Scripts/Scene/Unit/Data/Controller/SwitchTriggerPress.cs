/********************************************************************
** Filename : SwitchTrigger
** Author : Dong
** Date : 2017/5/9 星期二 上午 10:32:53
** Summary : SwitchTrigger
***********************************************************************/

using System.Collections.Generic;

namespace GameA.Game
{
    [Unit(Id = 8101, Type = typeof(SwitchTriggerPress))]
    public class SwitchTriggerPress : SwitchTrigger
    {
        protected List<UnitBase> _gridCheckUnits = new List<UnitBase>();
        
        protected override void Clear()
        {
            base.Clear();
            _gridCheckUnits.Clear();
        }

        public void OnGridCheckEnter(UnitBase other)
        {
            if (_gridCheckUnits.Contains(other))
            {
                return;
            }
            _gridCheckUnits.Add(other);
            SetTrigger(EActiveState.Active);
        }

        public void OnGridCheckExit(UnitBase other)
        {
            _gridCheckUnits.Remove(other);
            if (_gridCheckUnits.Count == 0 && _units.Count == 0)
            {
                SetTrigger(EActiveState.Deactive);
            }
        }

        protected override void OnTrigger(UnitBase other)
        {
            if (other == _switchUnit || !UnitDefine.CanTrigger(other) || _units.Contains(other))
            {
                return;
            }
            _units.Add(other);
            SetTrigger(EActiveState.Active);
        }

        public override void UpdateLogic()
        {
            if (_trigger == EActiveState.Active)
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
                if (_gridCheckUnits.Count == 0 && _units.Count == 0)
                {
                    SetTrigger(EActiveState.Deactive);
                }
            }
        }

        protected override void ChangView()
        {
            if (_view != null)
            {
                if (_trigger == EActiveState.Active)
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
