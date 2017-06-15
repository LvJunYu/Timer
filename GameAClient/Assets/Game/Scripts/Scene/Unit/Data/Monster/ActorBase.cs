﻿/********************************************************************
** Filename : ActorBase
** Author : Dong
** Date : 2017/5/20 星期六 上午 10:51:33
** Summary : ActorBase
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

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
        protected int _attackedTimer;
        protected int _fireTimer;

        public int AttackedTimer
        {
            get { return _attackedTimer; }
        }

        public override EffectManager EffectMgr
        {
            get { return _effectManager; }
        }

        public override EDieType EDieType
        {
            get { return _eDieType; }
        }

        public override bool CanControlledBySwitch
        {
            get { return false; }
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
            _attackedTimer = 0;
            _fireTimer = 0;
        }

        internal override void InLazer()
        {
            if (!_isAlive || IsInvincible)
            {
                return;
            }
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
                OutFire();
                return;
            }
            if (!_isAlive || IsInvincible)
            {
                return;
            }
            _eDieType = EDieType.Water;
            OnDead();
            if (_animation != null)
            {
                _animation.PlayOnce("DeathWater");
            }
        }

        internal override void OutFire()
        {
            if (_eDieType == EDieType.Fire)
            {
                _fireTimer = 0;
                _animation.Reset();
                _eDieType = EDieType.None;
            }
        }

        internal override void InFire()
        {
            if (!_isAlive || IsInvincible)
            {
                return;
            }
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

        protected override bool CheckOutOfMap()
        {
            if (base.CheckOutOfMap())
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
