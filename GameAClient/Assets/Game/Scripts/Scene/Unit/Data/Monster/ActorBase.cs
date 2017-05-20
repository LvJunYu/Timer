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
    public enum EDieType
    {
        None,
        /// <summary>
        /// 被机关之类的弄死
        /// </summary>
        Normal,
        Lazer,
        Water,
        Fire,
    }

    public class ActorBase : RigidbodyUnit
    {
        protected EffectManager _effectManager;
        protected EDieType _eDieType;

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
            if (_effectManager != null)
            {
                _effectManager.Clear();
            }
        }

        internal override void OnLazer()
        {
            _eDieType = EDieType.Lazer;
            OnDead();
            if (_animation != null)
            {
                _animation.PlayOnce("DeathLazer");
            }
        }

        internal override void OnWater()
        {
            if (_eDieType == EDieType.Fire)
            {
                //跳出水里
                OutFire();
                _eDieType = EDieType.None;
                return;
            }
            _eDieType = EDieType.Water;
            OnDead();
            if (_animation != null)
            {
                _animation.PlayOnce("DeathDrown");
            }
        }

        internal override void OnFire()
        {
            _eDieType = EDieType.Fire;
            if (_animation != null)
            {
                _animation.PlayLoop("OnFire", 1, 1);
            }
        }
    }
}
