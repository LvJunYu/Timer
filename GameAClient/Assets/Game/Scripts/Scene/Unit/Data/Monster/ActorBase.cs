/********************************************************************
** Filename : ActorBase
** Author : Dong
** Date : 2017/5/20 星期六 上午 10:51:33
** Summary : ActorBase
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    public class ActorBase : RigidbodyUnit
    {
        protected EffectManager _effectManager;

        public override EffectManager EffectMgr
        {
            get { return _effectManager; }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _effectManager = new EffectManager(this);
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _effectManager.Clear();
        }
    }
}
