/********************************************************************
** Filename : EffectBase
** Author : Dong
** Date : 2017/5/17 星期三 下午 2:25:19
** Summary : EffectBase
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    public class EffectBase
    {
        protected UnitBase _owner;
        protected ESkillType _eSkillType;

        public ESkillType ESkillType
        {
            get { return _eSkillType; }
        }

        public virtual void Init(UnitBase target)
        {
            _owner = target;
        }

        public virtual void OnAttachedAgain(BulletBase bullet)
        {
        }

        public virtual void OnAttached(BulletBase bullet)
        {
        }

        public virtual void OnRemoved()
        {
        }
    }
}
