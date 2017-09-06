﻿/********************************************************************
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
        protected bool _activeState;
        protected SpineObject _effect;

        public bool ActiveState
        {
            set
            {
                if (_activeState == value)
                {
                    return;
                }
                _activeState = value;
                UpdateActiveState();
            }
        }

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        protected override void Clear()
        {
            base.Clear();
            UpdateActiveState();
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
                _effect = GameParticleManager.Instance.EmitLoop("M1EffectSwitchEarth", _trans.position + new Vector3(0,-0.6f,0));
                _effect.Trans.parent = _trans;
                UpdateActiveState();
            }
            return true;
        }
        
        private void UpdateActiveState()
        {
            if (_view != null)
            {
                _view.SetRendererEnabled(_activeState);
            }
            if (_effect != null)
            {
                _effect.SetActive(!_activeState);
            }
            SetAllCross(!_activeState);
        }

        private void SetActiveState(bool value)
        {
            if (_activeState == value)
            {
                return;
            }
            if (!_activeState && _ctrlBySwitch)
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
                ActiveState = value;
            }
            else
            {
                ActiveState = value;
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            SetActiveState(_ctrlBySwitch ? !_activeState : _activeState);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!_activeState)
            {
                return false;
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!_activeState)
            {
                return false;
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!_activeState)
            {
                return false;
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!_activeState)
            {
                return false;
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }
    }
}
