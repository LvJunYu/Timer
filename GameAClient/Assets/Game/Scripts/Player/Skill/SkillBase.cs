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
        protected int _totalCount;
        protected int _cdTime;
        protected int _chargerTime;
        protected int _duration;
        protected int _cure;
        protected int _damage;
        /// <summary>
        /// 攻击距离
        /// </summary>
        [SerializeField]
        protected int _range;
        [SerializeField]
        protected int _radius;

        protected int _currentCount;

        /// <summary>
        /// 定时器
        /// </summary>
        [SerializeField]
        protected int _timerCD;
        protected int _timerCharger;

        protected int _timerAnimation;
        protected int _cdAnimation;

        protected int _bulletId;
        [SerializeField]
        protected int _bulletSpeed;

        [SerializeField]
        protected UnitBase _owner;

        public int BulletSpeed
        {
            get { return _bulletSpeed; }
        }

        public UnitBase Owner
        {
            get { return _owner; }
        }

        public int Radius
        {
            get { return _radius; }
        }

        public int Range
        {
            get { return _range; }
        }

        public ESkillType ESkillType
        {
            get { return _eSkillType; }
        }

        internal virtual void Enter(UnitBase ower)
        {
            _owner = ower;
            _totalCount = 20;
            _currentCount = _totalCount;
            _timerCD = 0;
            _cdTime = 7;
            _chargerTime = 50;
            _radius = 320;
            _range = 6400;
            _bulletSpeed = 200;
            _timerAnimation = 0;
            _cdAnimation = 0;
        }

        internal void SetValue(int cdTime, int range, int cdAnimation = 0)
        {
            _cdTime = cdTime;
            _range = range * ConstDefineGM2D.ServerTileScale;
            _cdAnimation = cdAnimation;
            if (_cdAnimation > _cdTime)
            {
                LogHelper.Error("Error: _cdAnimation{0} > _cdTime{1}", _cdAnimation, _cdTime);
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
            if (_timerCharger > 0)
            {
                _timerCharger--;
                if (_timerCharger == 0)
                {
                    _currentCount = _totalCount;
                    //LogHelper.Debug("Charge End");
                }
            }
            if (_timerAnimation > 0)
            {
                _timerAnimation--;
                if (_timerAnimation == 0)
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
            _timerAnimation = _cdAnimation;
            if (_timerAnimation == 0)
            {
                OnSkillCast();
            }
            return true;
        }

        protected virtual void OnSkillCast()
        {
        }

        public virtual void OnBulletHit(BulletBase bullet)
        {
        }

        protected void CreateBullet(int bulletId, IntVec2 pos, int angle, int delayRunTime = 0)
        {
            _currentCount--;
            if (_currentCount == 0)
            {
                _timerCharger = _chargerTime;
            }
            var bullet =  PlayMode.Instance.CreateRuntimeUnit(bulletId, pos) as BulletBase;
            if (bullet != null)
            {
                bullet.Run(this, angle, delayRunTime);
            }
        }

        private void UpdateBulletCount(int count)
        {
            if (_owner == null || !_owner.IsMain)
            {
                return;
            }
            if (_currentCount == count)
            {
                return;
            }
            _currentCount = Math.Min(_totalCount, count);
            Messenger<int, int>.Broadcast(EMessengerType.OnMPChanged, _currentCount, _totalCount);
        }

        public int AddBullet(int count)
        {
            var oldMp = _currentCount;
            UpdateBulletCount(_currentCount + count);
            return _currentCount - oldMp;
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

        public override string ToString()
        {
            return string.Format("ESkillType: {0}, Range: {1}, Radius: {2}, BulletId: {3}", _eSkillType, _range, _radius, _bulletId);
        }
    }
}
