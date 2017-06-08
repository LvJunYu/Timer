/********************************************************************
** Filename : EffectIce
** Author : Dong
** Date : 2017/5/17 星期三 下午 3:00:14
** Summary : EffectIce
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Effect(Name = "EffectIce", Type = typeof(EffectIce))]
    public class EffectIce : EffectBase
    {
        private SpineObject _effect;

        public override void OnAttached(BulletBase bullet)
        {
            //黏液不能攻击主角，直接喷涂到地面上
            if (_owner.IsMain)
            {
                return;
            }
            _owner.CanMotor = false;
            _owner.CanAttack = false;
            if (_owner.Animation != null)
            {
                _owner.Animation.PlayOnce("OnIce");
                LogHelper.Debug("OnIce");
            }
            if (_effect == null)
            {
                _effect = GameParticleManager.Instance.EmitLoop("M1Ice1", _owner.Trans);
                _effect.Trans.localPosition = new Vector3(0, -0.1f, 0.01f);
            }
            _effect.SetActive(true);
        }

        public override void OnRemoved()
        {
            _owner.CanMotor = true;
            _owner.CanAttack = true;
            if (_effect != null)
            {
                _effect.SetActive(false);
            }
        }
    }
}
