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
        protected bool _trigger;
        protected SpineObject _effect;
        protected List<UnitBase> _switchPressUnits = new List<UnitBase>();

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        protected override void Clear()
        {
            base.Clear();
            _trigger = false;
            _switchPressUnits.Clear();
            SetEffect();
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
            }
            SetEffect();
            return true;
        }

        internal override void OnOtherSwitch()
        {
            base.OnOtherSwitch();
            if (_switchPressUnits.Count == 0)
            {
                _trigger = !_trigger;
                SetEffect();
            }
        }

        internal override void OnSwitchPressStart(SwitchPress switchPress)
        {
            if (_switchPressUnits.Contains(switchPress))
            {
                return;
            }
            _switchPressUnits.Add(switchPress);
            if (_trigger)
            {
                return;
            }
            _trigger = true;
            SetEffect();
        }

        internal override void OnSwitchPressEnd(SwitchPress switchPress)
        {
            _switchPressUnits.Remove(switchPress);
            if (_switchPressUnits.Count == 0)
            {
                _trigger = false;
                SetEffect();
            }
        }

        private void SetEffect()
        {
            if (_view != null)
            {
                _view.SetRendererEnabled(!_trigger);
            }
            if (_effect != null)
            {
                _effect.Trans.gameObject.SetActiveEx(_trigger);
            }
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_trigger)
            {
                return false;
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (_trigger)
            {
                return false;
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_trigger)
            {
                return false;
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (_trigger)
            {
                return false;
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }
    }
}
