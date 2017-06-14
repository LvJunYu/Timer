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
        protected bool _finalTrigger;
        protected bool _currentTrigger;
        protected SpineObject _effect;
        protected List<UnitBase> _switchPressUnits = new List<UnitBase>();

        public bool CurrentTrigger
        {
            set
            {
                if (_currentTrigger == value)
                {
                    return;
                }
                _currentTrigger = value;
                if (_view != null)
                {
                    _view.SetRendererEnabled(!_currentTrigger);
                }
                if (_effect != null)
                {
                    _effect.SetActive(_currentTrigger);
                }
                _canLazerCross = _currentTrigger;
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
            _finalTrigger = false;
            _currentTrigger = false;
            _switchPressUnits.Clear();
            if (_view != null)
            {
                _view.SetRendererEnabled(!_currentTrigger);
            }
            if (_effect != null)
            {
                _effect.SetActive(_currentTrigger);
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
                _view.SetRendererEnabled(!_currentTrigger);
                _effect = GameParticleManager.Instance.EmitLoop("M1EffectSwitchEarth", _trans.position + new Vector3(0,-0.6f,0));
                _effect.Trans.parent = _trans;
                _effect.SetActive(_currentTrigger);
            }
            return true;
        }

        internal override void OnOtherSwitch()
        {
            base.OnOtherSwitch();
            if (_switchPressUnits.Count == 0)
            {
                _finalTrigger = !_finalTrigger;
                SetState(_finalTrigger);
            }
        }

        internal override void OnSwitchPressStart(SwitchPress switchPress)
        {
            if (_switchPressUnits.Contains(switchPress))
            {
                return;
            }
            _switchPressUnits.Add(switchPress);
            _finalTrigger = true;
            SetState(_finalTrigger);
        }

        internal override void OnSwitchPressEnd(SwitchPress switchPress)
        {
            _switchPressUnits.Remove(switchPress);
            if (_switchPressUnits.Count == 0)
            {
                _finalTrigger = false;
                SetState(_finalTrigger);
            }
        }

        private void SetState(bool value)
        {
            if (_currentTrigger == value)
            {
                return;
            }
            if (_currentTrigger && !_finalTrigger)
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
                CurrentTrigger = value;
            }
            else
            {
                CurrentTrigger = value;
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            SetState(_finalTrigger);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_currentTrigger)
            {
                return false;
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_currentTrigger)
            {
                return false;
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_currentTrigger)
            {
                return false;
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_currentTrigger)
            {
                return false;
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }
    }
}
