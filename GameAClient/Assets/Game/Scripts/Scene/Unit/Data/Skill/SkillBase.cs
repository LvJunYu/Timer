/********************************************************************
** Filename : SkillBase
** Author : Dong
** Date : 2017/3/22 星期三 上午 10:35:13
** Summary : SkillBase
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    public class SkillBase
    {
        /// <summary>
        /// 攻击范围
        /// </summary>
        protected int _range;
        /// <summary>
        /// CD时间
        /// </summary>
        protected int _cdTime;
        /// <summary>
        /// 是否强力
        /// </summary>
        protected bool _plus;
        /// <summary>
        /// 定时器
        /// </summary>
        protected int _timerCD;

        protected IntVec2 _bulletSpeed;

        internal virtual void Enter(bool plus)
        {
            _plus = plus;
            _timerCD = 0;
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
            bullet.Run(PlayMode.Instance.MainUnit.FirePos, _bulletSpeed);
        }

        protected virtual BulletBase CreateBullet()
        {
            return null;
        }

        public void Update()
        {
            if (_timerCD > 0)
            {
                _timerCD--;
            }
        }
    }
}
