/********************************************************************
** Filename : EffectBase
** Author : Dong
** Date : 2017/5/17 星期三 下午 2:25:19
** Summary : EffectBase
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    /// <summary>
    /// Effects do not have to be directly linked to the ability,they can also be triggerd by other effects.
    /// </summary>
    public class EffectBase
    {
        public virtual bool IsGain
        {
            get { return false; }
        }

        public virtual void Init(params object[] values)
        {
        }

        public virtual bool OnAttached(ActorBase target)
        {
            return true;
        }
        
        public virtual bool OnRemoved(ActorBase target)
        {
            return true;
        }

        public virtual void Update()
        {
        }
    }
}
