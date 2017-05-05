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
        [SerializeField]
        protected ESkillType _eSkillType;

        /// <summary>
        /// 攻击举例
        /// </summary>
        [SerializeField] protected int _range;
        [SerializeField]protected int _radius;
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

        protected int _bulletId;

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

        public bool Plus
        {
            get { return _plus; }
        }

        public ESkillType ESkillType
        {
            get { return _eSkillType; }
        }

        internal virtual void Enter(UnitBase ower, bool plus)
        {
            _owner = ower;
            _plus = plus;
            _radius = 160;
            _range = 8*ConstDefineGM2D.ServerTileScale;
            _timerCD = 0;
            _cdTime = 10;
            _bulletSpeed = 200;
        }

        internal virtual void Exit()
        {
        }

        public void UpdateLogic()
        {
            if (_timerCD > 0)
            {
                _timerCD--;
            }
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
            //LogHelper.Debug("Skill: {0} CreateBullet: {1}",this, bullet);
            bullet.Run(this);
        }

        private BulletBase CreateBullet()
        {
            if (_bulletId == 0)
            {
                return null;
            }
            var rotation = (byte)(_owner.ShootRot/90);
            return PlayMode.Instance.CreateRuntimeUnit(_bulletId, GetBulletPos(_bulletId, rotation), rotation, Vector2.one) as BulletBase;
        }

        private IntVec2 GetBulletPos(int bulletId, byte rotation)
        {
            var tableUnit = UnitManager.Instance.GetTableUnit(bulletId);
            if (tableUnit == null)
            {
                return IntVec2.zero;
            }
            var dataSize = tableUnit.GetDataSize(rotation, Vector2.one);
            return _owner.FirePos - dataSize * 0.5f;
        }

        public override string ToString()
        {
            return string.Format("Range: {0}, CdTime: {1}, Plus: {2}, TimerCd: {3}, BulletSpeed: {4}, Owner: {5}", _radius, _cdTime, _plus, _timerCD, _bulletSpeed, _owner);
        }
    }
}
