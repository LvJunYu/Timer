/********************************************************************
** Filename : ActorBase
** Author : Dong
** Date : 2017/5/20 星期六 上午 10:51:33
** Summary : ActorBase
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
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
        protected List<BuffBase> _currentBuffs = new List<BuffBase>();
        
        protected EDieType _eDieType;
        protected int _attackedTimer;
        protected int _fireTimer;

        protected float _hpRecover = 200 * ConstDefineGM2D.FixedDeltaTime;
        protected int _hpRecoverTimer = 3 * ConstDefineGM2D.FixedFrameCount;

        public int AttackedTimer
        {
            get { return _attackedTimer; }
        }

        public override EDieType EDieType
        {
            get { return _eDieType; }
        }
        
        protected override void Clear()
        {
            base.Clear();
            _eDieType = EDieType.None;
            _attackedTimer = 0;
            _fireTimer = 0;
            RemoveAllBuffs();
        }

        public override BuffBase AddBuff(EBuffType eBuffType, int time, params EffectBase[] effects)
        {
            //已经有了的就移除掉重新加上。相斥的比较优先级，优先级小的移除掉
            if (HasBuff(eBuffType))
            {
                RemoveBuff(eBuffType);
            }
            var buff = EffectMgr.GetBuff(eBuffType, time, effects);
             if (!buff.OnAttached(this))
             {
                 EffectMgr.FreeBuff(buff);
             }
            _currentBuffs.Add(buff);
            return buff;
        }
         
         public override void RemoveBuff(EBuffType eBuffType)
         {
             var buff = GetBuff(eBuffType);
             if (buff == null)
             {
                 return;
             }
             if (buff.OnRemoved(this))
             {
                 _currentBuffs.Remove(buff);
                 EffectMgr.FreeBuff(buff);
             }
         }

        private BuffBase GetBuff(EBuffType eBuffType)
        {
            for (int i = 0; i < _currentBuffs.Count; i++)
            {
                if (_currentBuffs[i].EBuffType == eBuffType)
                {
                    return _currentBuffs[i];
                }
            }
            return null;
        }

        public override bool HasBuff(EBuffType eBuffType)
        {
            for (int i = 0; i < _currentBuffs.Count; i++)
            {
                if (_currentBuffs[i].EBuffType == eBuffType)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool ExcuteEffect(EffectBase effect)
        {
            return effect.OnAttached(this);
        }

        public void RemoveAllBuffs()
        {
            for (int i = 0; i < _currentBuffs.Count; i++)
            {
                _currentBuffs[i].OnRemoved(this);
            }
            _currentBuffs.Clear();
        }

        public override void RemoveAllDebuffs()
        {
            for (int i = _currentBuffs.Count - 1; i >= 0; i--)
            {
                if (!_currentBuffs[i].IsGain)
                {
                    _currentBuffs[i].OnRemoved(this);
                    _currentBuffs.RemoveAt(i);
                }
            }
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
