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
        public override bool OnAttached(ActorBase target)
        {
            target.CanMotor = false;
            target.CanAttack = false;
            if (target.Animation != null)
            {
                target.Animation.Reset();
                target.Animation.PlayOnce("OnIce");
            }
//            if (_effect == null)
//            {
//                _effect = GameParticleManager.Instance.EmitLoop("M1Ice1", target.Trans);
//            }
//            _effect.Trans.localPosition = new Vector3(0, -0.1f, target.CurMoveDirection == EMoveDirection.Left ? 0.01f : -0.01f);
//            _effect.Trans.rotation = Quaternion.identity;
//            _effect.SetActive(true);
            return true;
        }

        public override bool OnRemoved(ActorBase target)
        {
            target.CanMotor = true;
            target.CanAttack = true;
//            if (_effect != null)
//            {
//                _effect.SetActive(false);
//            }
            return true;
        }
    }
}
