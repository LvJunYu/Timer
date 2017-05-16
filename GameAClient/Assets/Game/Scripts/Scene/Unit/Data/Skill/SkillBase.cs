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
        /// 定时器
        /// </summary>
        [SerializeField]
        protected int _timerCD;

        protected int _timerAnimation;
        protected int _cdAnimation;

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

        public ESkillType ESkillType
        {
            get { return _eSkillType; }
        }

        internal virtual void Enter(UnitBase ower)
        {
            _owner = ower;
            _radius = 160;
            _range = 10*ConstDefineGM2D.ServerTileScale;
            _timerCD = 0;
            _cdTime = 8;
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

        public void UpdateLogic()
        {
            if (_timerCD > 0)
            {
                _timerCD--;
            }
            if (_timerAnimation > 0)
            {
                _timerAnimation--;
                if (_timerAnimation == 0)
                {
                    //生成子弹
                    CreateBullet();
                }
            }
        }

        public bool Fire()
        {
            if (_timerCD > 0)
            {
                return false;
            }
            _timerCD = _cdTime;
            _timerAnimation = _cdAnimation;
            if (_timerAnimation == 0)
            {
                //生成子弹
                CreateBullet();
                //LogHelper.Debug("Skill: {0} CreateBullet: {1}",this, bullet);
            }
            return true;
        }

        private BulletBase CreateBullet()
        {
            if (_bulletId == 0)
            {
                return null;
            }
            var bullet =  PlayMode.Instance.CreateRuntimeUnit(_bulletId, GetBulletPos(_bulletId), 0, Vector2.one) as BulletBase;
            if (bullet != null)
            {
                bullet.Run(this);
            }
            return bullet;
        }

        private IntVec2 GetBulletPos(int bulletId)
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
