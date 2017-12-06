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
        protected EActiveState _curActiveState;
        protected SpineObject _effect;

        public EActiveState CurActiveState
        {
            set
            {
                if (_curActiveState == value)
                {
                    return;
                }
                _curActiveState = value;
                UpdateActiveState();
            }
        }

        public override bool CanControlledBySwitch
        {
            get { return true; }
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

        public override UnitExtra UpdateExtraData()
        {
            var extra = base.UpdateExtraData();
            _curActiveState = _eActiveState;
            UpdateActiveState();
            return extra;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            if (_trans != null)
            {
                _effect = GameParticleManager.Instance.EmitLoop("M1EffectSwitchEarth",
                    _trans.position + new Vector3(0, -0.6f, 0));
                _effect.Trans.parent = _trans;
                UpdateActiveState();
            }
            return true;
        }

        private void UpdateActiveState()
        {
            if (_view != null)
            {
                _view.SetRendererEnabled(_curActiveState == EActiveState.Active);
            }
            if (_effect != null)
            {
                _effect.SetActive(_curActiveState == EActiveState.Deactive);
            }
            SetCross(_curActiveState == EActiveState.Deactive);
        }

        private void SetCurrentActiveState(EActiveState value)
        {
            if (_curActiveState == value)
            {
                return;
            }
            if (_curActiveState != EActiveState.Active && _eActiveState == EActiveState.Active)
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
                CurActiveState = value;
            }
            else
            {
                CurActiveState = value;
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            SetCurrentActiveState(_eActiveState);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_curActiveState != EActiveState.Active)
            {
                return false;
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_curActiveState != EActiveState.Active)
            {
                return false;
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_curActiveState != EActiveState.Active)
            {
                return false;
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_curActiveState != EActiveState.Active)
            {
                return false;
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }
    }
}