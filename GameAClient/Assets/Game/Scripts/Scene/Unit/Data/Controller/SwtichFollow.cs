/********************************************************************
** Filename : SwtichFollow
** Author : Dong
** Date : 2017/3/16 星期四 下午 8:58:22
** Summary : SwtichFollow
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 8103, Type = typeof(SwtichFollow))]
    public class SwtichFollow : Switch
    {
        protected override void CreateParticle()
        {
            _effectStart = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectSwitchStart", _trans);
            _effectRun = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectSwitchFollowRun", _trans);
            if (_effectRun != null)
            {
                _effectRun.Play();
            }
        }
    }
}
