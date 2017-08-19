using System;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class Bullet : IPoolableObject
    {
        protected bool _run;
        protected Transform _trans;
        protected SpineObject _effectBullet;

        protected IntVec2 _speed;
        protected IntVec2 _curPos;
        protected IntVec2 _originPos;

        protected SkillBase _skill;
     
        protected int _angle;
        protected int _maskRandom;
        
        public int Angle
        {
            get { return _angle; }
        }

        public int MaskRandom
        {
            get { return _maskRandom; }
        }
        
        public void OnGet()
        {
        }

        public void OnFree()
        {
            _run = false;
            _skill = null;
            _speed = IntVec2.zero;
            _angle = 0;
            _originPos = IntVec2.zero;
            GameParticleManager.FreeSpineObject(_effectBullet);
            _effectBullet = null;
        }

        public void OnDestroyObject()
        {
        }

        public Bullet()
        {
            _trans = new GameObject().transform;
            if (UnitManager.Instance != null) 
            {
                _trans.parent = UnitManager.Instance.GetParent(EUnitType.Bullet);
            }
        }

        public void Init(SkillBase skill, IntVec2 pos, int angle)
        {
            _skill = skill;
            _angle = angle;
            _curPos = _originPos = pos;
            
            _trans.eulerAngles = new Vector3(0, 0, -_angle);
            UpdateTransPos();
            
            _maskRandom = UnityEngine.Random.Range(0, 2);
            var rad = _angle * Mathf.Deg2Rad;
            _speed = new IntVec2((int)(_skill.ProjectileSpeed * Math.Sin(rad)), (int)(_skill.ProjectileSpeed * Math.Cos(rad)));
        }

        private void UpdateTransPos()
        {
            if (_trans != null)
            {
                _trans.position = GM2DTools.TileToWorld(_curPos);
            }
        }

        public void UpdateLogic()
        {
        }
    }
}