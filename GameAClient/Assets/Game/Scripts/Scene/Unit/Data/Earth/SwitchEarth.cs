/********************************************************************
** Filename : SwitchEarth
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:31:12
** Summary : SwitchEarth
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 4003, Type = typeof(SwitchEarth))]
    public class SwitchEarth : BlockBase
    {
        protected bool _finalCtrlBySwitch;
        protected bool _currentCtrlBySwitch;
        protected SpineObject _effect;

        public bool CurrentCtrlBySwitch
        {
            set
            {
                if (_currentCtrlBySwitch == value)
                {
                    return;
                }
                _currentCtrlBySwitch = value;
                if (_view != null)
                {
                    _view.SetRendererEnabled(!_currentCtrlBySwitch);
                }
                if (_effect != null)
                {
                    _effect.SetActive(_currentCtrlBySwitch);
                }
                _canLazerCross = _currentCtrlBySwitch;
            }
        }

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        protected override void Clear()
        {
            base.Clear();
            _canLazerCross = false;
            _finalCtrlBySwitch = false;
            _currentCtrlBySwitch = false;
            if (_view != null)
            {
                _view.SetRendererEnabled(!_currentCtrlBySwitch);
            }
            if (_effect != null)
            {
                _effect.SetActive(_currentCtrlBySwitch);
            }
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            if (_effect != null)
            {
                GameParticleManager.FreeSpineObject(_effect);
                _effect = null;
            }
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            if (_trans != null)
            {
                _view.SetRendererEnabled(!_currentCtrlBySwitch);
                _effect = GameParticleManager.Instance.EmitLoop("M1EffectSwitchEarth", _trans.position + new Vector3(0,-0.6f,0));
                _effect.Trans.parent = _trans;
                _effect.SetActive(_currentCtrlBySwitch);
            }
            return true;
        }

        internal override void OnOtherSwitch()
        {
            base.OnOtherSwitch();
            if (_switchPressUnits.Count == 0)
            {
                _finalCtrlBySwitch = !_finalCtrlBySwitch;
                SetCtrlBySwitchState(_finalCtrlBySwitch);
            }
        }

        internal override bool OnSwitchPressStart(SwitchPress switchPress)
        {
            if (!base.OnSwitchPressStart(switchPress))
            {
                return false;
            }
            _finalCtrlBySwitch = true;
            SetCtrlBySwitchState(_finalCtrlBySwitch);
            return true;
        }

        internal override bool OnSwitchPressEnd(SwitchPress switchPress)
        {
            if (!base.OnSwitchPressEnd(switchPress))
            {
                return false;
            }
            if (_switchPressUnits.Count == 0)
            {
                _finalCtrlBySwitch = false;
                SetCtrlBySwitchState(_finalCtrlBySwitch);
            }
            return true;
        }

        private void SetCtrlBySwitchState(bool value)
        {
            if (_currentCtrlBySwitch == value)
            {
                return;
            }
            if (_currentCtrlBySwitch && !_finalCtrlBySwitch)
            {
                var units = ColliderScene2D.GridCastAllReturnUnits(_colliderGrid);
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (IsBlockedBy(unit))
                    {
                        return;
                    }
                }
                CurrentCtrlBySwitch = value;
            }
            else
            {
                CurrentCtrlBySwitch = value;
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            SetCtrlBySwitchState(_finalCtrlBySwitch);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_currentCtrlBySwitch)
            {
                return false;
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_currentCtrlBySwitch)
            {
                return false;
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_currentCtrlBySwitch)
            {
                return false;
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_currentCtrlBySwitch)
            {
                return false;
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }
    }
}
