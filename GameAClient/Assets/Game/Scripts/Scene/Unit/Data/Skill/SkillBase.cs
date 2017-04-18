/********************************************************************
** Filename : SkillBase
** Author : Dong
** Date : 2017/3/22 星期三 上午 10:35:13
** Summary : SkillBase
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Serializable]
    public class SkillBase
    {
        /// <summary>
        /// 攻击范围
        /// </summary>
        [SerializeField]protected int _range;
        /// <summary>
        /// CD时间
        /// </summary>
        [SerializeField]
        protected int _cdTime;
        /// <summary>
        /// 是否强力
        /// </summary>
        [SerializeField]
        protected bool _plus;
        /// <summary>
        /// 定时器
        /// </summary>
        [SerializeField]
        protected int _timerCD;

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

        public int Range
        {
            get { return _range; }
        }

        public bool Plus
        {
            get { return _plus; }
        }

        internal virtual void Enter(UnitBase ower, bool plus)
        {
            _owner = ower;
            _plus = plus;
            _timerCD = 0;
            _cdTime = 10;
            _bulletSpeed = 200;
        }

        internal virtual void Exit()
        {
        }

        public void Fire()
        {
            if (_timerCD > 0)
            {
                return;
            }
            _timerCD = _cdTime;
            //生成子弹
            var bullet = CreateBullet();
            if (bullet == null)
            {
                return;
            }
            LogHelper.Debug("Skill: {0} CreateBullet: {1}",this, bullet);
            bullet.Run(this);
        }

        protected virtual BulletBase CreateBullet()
        {
            return null;
        }

        public void UpdateLogic()
        {
            if (_timerCD > 0)
            {
                _timerCD--;
            }
        }

        public override string ToString()
        {
            return string.Format("Range: {0}, CdTime: {1}, Plus: {2}, TimerCd: {3}, BulletSpeed: {4}, Owner: {5}", _range, _cdTime, _plus, _timerCD, _bulletSpeed, _owner);
        }
    }
}
