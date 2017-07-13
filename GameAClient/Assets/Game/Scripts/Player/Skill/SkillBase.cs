/********************************************************************
** Filename : SkillBase
** Author : Dong
** Date : 2017/3/22 星期三 上午 10:35:13
** Summary : SkillBase
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Serializable]
    public class SkillBase
    {
        [SerializeField]
        protected ESkillType _eSkillType;
        
        [SerializeField]
        protected UnitBase _owner;
        protected Table_Skill _tableSkill;
        
        protected int _cdTime;
        protected int _chargeTime;
        protected int _singTime;

        protected int _projectileCount;
        protected int _currentCount;

        /// <summary>
        /// 攻击距离
        /// </summary>
        [SerializeField]
        protected int _castRange;
        [SerializeField]
        protected int _radius;

        /// <summary>
        /// 定时器
        /// </summary>
        [SerializeField]
        protected int _timerCD;
        protected int _timerCharge;
        protected int _timerSing;

        public int Id
        {
            get { return _tableSkill.Id; }
        }

        public Table_Skill TableSkill
        {
            get { return _tableSkill; }
        }

        public UnitBase Owner
        {
            get { return _owner; }
        }

        public int Radius
        {
            get { return _radius; }
        }

        public int CastRange
        {
            get { return _castRange; }
        }

        public ESkillType ESkillType
        {
            get { return _eSkillType; }
        }

        public SkillBase(int id, UnitBase ower)
        {
            _tableSkill = TableManager.Instance.GetSkill(id);
            _owner = ower;
            _cdTime = _tableSkill.CDTime;
            _chargeTime = _tableSkill.ChargeTime;
            _singTime = _tableSkill.SingTime;
            _castRange = _tableSkill.CastRange * ConstDefineGM2D.ServerTileScale;
            _projectileCount = _tableSkill.ProjectileCount;
            _currentCount = _projectileCount;
            
            _timerSing = 0;
            _timerCD = 0;
        }

        internal void SetValue(int cdTime, int castRange, int singTime = 0)
        {
            _cdTime = cdTime;
            _castRange = castRange * ConstDefineGM2D.ServerTileScale;
            _singTime = singTime;
            if (_singTime > _cdTime)
            {
                LogHelper.Error("Error: _singTime{0} > _cdTime{1}", _singTime, _cdTime);
            }
        }

        internal virtual void Exit()
        {
        }

        public virtual void UpdateLogic()
        {
            if (_timerCD > 0)
            {
                _timerCD--;
            }
            if (_timerCharge > 0)
            {
                _timerCharge--;
                if (_timerCharge == 0)
                {
                    UpdateCurrentProjectileCount(_projectileCount);
                    //LogHelper.Debug("Charge End");
                }
            }
            if (_timerSing > 0)
            {
                _timerSing--;
                if (_timerSing == 0)
                {
                    OnSkillCast();
                }
            }
        }
        
        public bool Fire()
        {
            if (_currentCount == 0)
            {
                //LogHelper.Debug("Charging");
                return false;
            }
            if (_timerCD > 0)
            {
                return false;
            }
            _timerCD = _cdTime;
            _timerSing = _singTime;
            if (_timerSing == 0)
            {
                OnSkillCast();
            }
            return true;
        }
        
        protected virtual void OnSkillCast()
        {
            
        }

        public virtual void OnProjectileHit(ProjectileBase projectile)
        {
        }

        protected void CreateProjectile(int projectileId, IntVec2 pos, int angle, int delayRunTime = 0)
        {
            UpdateCurrentProjectileCount(--_currentCount);
            if (_currentCount == 0)
            {
                _timerCharge = _chargeTime;
            }
            var bullet =  PlayMode.Instance.CreateRuntimeUnit(projectileId, pos) as ProjectileBase;
            if (bullet != null)
            {
                bullet.Run(this, angle, delayRunTime);
            }
        }

        private void UpdateCurrentProjectileCount(int count)
        {
            if (_owner == null || !_owner.IsMain)
            {
                return;
            }
            if (_currentCount == count)
            {
                return;
            }
            _currentCount = Math.Min(_projectileCount, count);
            Messenger<int, int>.Broadcast(EMessengerType.OnMPChanged, _currentCount, _projectileCount);
        }

        protected IntVec2 GetBulletPos(int bulletId)
        {
            var tableUnit = UnitManager.Instance.GetTableUnit(bulletId);
            if (tableUnit == null)
            {
                return IntVec2.zero;
            }
            var dataSize = tableUnit.GetDataSize(0, Vector2.one);
            return _owner.FirePos - dataSize * 0.5f;
        }
    }
}
