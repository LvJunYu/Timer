/********************************************************************
** Filename : ActorBase
** Author : Dong
** Date : 2017/5/20 星期六 上午 10:51:33
** Summary : ActorBase
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

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
        OutofMap,
    }

    public class ActorBase : RigidbodyUnit
    {
        protected EffectManager _effectManager;
        protected EDieType _eDieType;
        protected int _stunTimer;
        protected int _fireTimer;

        public override EffectManager EffectMgr
        {
            get { return _effectManager; }
        }

        public override EDieType EDieType
        {
            get { return _eDieType; }
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
            _eDieType = EDieType.None;
            _stunTimer = 0;
            _fireTimer = 0;
        }

        internal override void InLazer()
        {
            _eDieType = EDieType.Lazer;
            OnDead();
            if (_animation != null)
            {
                _animation.PlayOnce("DeathLazer");
            }
        }

        internal override void InWater()
        {
            if (_eDieType == EDieType.Fire)
            {
                //跳出水里
                ExtraSpeed.y = 240;
                _animation.ClearTrack(1);
                OutFire();
                return;
            }
            _eDieType = EDieType.Water;
            OnDead();
            if (_animation != null)
            {
                _animation.PlayOnce("DeathWater");
            }
        }

        protected virtual void OutFire()
        {
            _eDieType = EDieType.None;
        }

        internal override void InFire()
        {
            if (_eDieType == EDieType.Fire)
            {
                return;
            }
            _eDieType = EDieType.Fire;
            if (_animation != null)
            {
                _animation.PlayLoop("OnFire", 1, 1);
            }
        }

        protected override bool OutOfMap()
        {
            if (base.OutOfMap())
            {
                _eDieType = EDieType.OutofMap;
                return true;
            }
            return false;
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (_isAlive && _eDieType == EDieType.Fire)
            {
                if (unit.IsAlive)
                {
                    unit.InFire();
                }
            }
        }
    }
}
