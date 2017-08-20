using System;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 10, PreferedPoolSize = 100, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
    public class Bullet : IPoolableObject
    {
        protected bool _run;
        protected Transform _trans;
        protected SpineObject _effectBullet;

        protected SkillBase _skill;
        protected Vector2 _direction;
        protected int _angle;
        protected IntVec2 _speed;
        protected IntVec2 _originPos;
        protected IntVec2 _curPos;
     
        protected int _maskRandom;
        protected int _destroy;
        
        public Vector2 Direction
        {
            get { return _direction; }
        }

        public int MaskRandom
        {
            get { return _maskRandom; }
        }
        
        public IntVec2 CurPos
        {
            get { return _curPos; }
        }

        public void OnGet()
        {
        }

        public void OnFree()
        {
            _run = false;
            _skill = null;
            _direction = Vector2.zero;
            _angle = 0;
            _speed = IntVec2.zero;
            _curPos = _originPos = IntVec2.zero;
            _maskRandom = 0;
            _destroy = 0;

            GameParticleManager.FreeSpineObject(_effectBullet);
            _effectBullet = null;
        }

        public void OnDestroyObject()
        {
        }

        public Bullet()
        {
            _trans = new GameObject("Bullet").transform;
            if (UnitManager.Instance != null) 
            {
                _trans.parent = UnitManager.Instance.GetParent(EUnitType.Bullet);
            }
        }

        public void Init(SkillBase skill, IntVec2 pos, int angle)
        {
            _maskRandom = UnityEngine.Random.Range(0, 2);
            _skill = skill;
            _curPos = _originPos = pos;
            
            _angle = angle;
            _direction = GM2DTools.GetDirection(_angle);
            
            _speed = new IntVec2((int) (_skill.ProjectileSpeed * _direction.x),
                (int) (_skill.ProjectileSpeed * _direction.y));
            
            _effectBullet = GameParticleManager.Instance.EmitOnce("M1BulletWater", _trans);
            _trans.eulerAngles = new Vector3(0, 0, -angle);
            UpdateTransPos();
            _run = true;
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
            if (!_run)
            {
                return;
            }
            _curPos += _speed;
            //超出最大射击距离
            if ((_curPos - _originPos).SqrMagnitude() >= _skill.CastRange * _skill.CastRange)
            {
                _curPos = _originPos + new IntVec2((int)(_skill.CastRange * _direction.x), (int)(_skill.CastRange * _direction.y));
                _destroy = 1;
            }
            RayHit2D hit;
            if (ColliderScene2D.Raycast(_curPos, _direction, out hit, _skill.ProjectileSpeed, EnvManager.PaintBulletHitLayer))
            {
                _curPos = GM2DTools.WorldToTile(hit.point);
                _destroy = 1;
            }
            UpdateTransPos();
            if (_destroy > 0)
            {
                OnDestroy();
            }

        }
        
        protected void OnDestroy()
        {
            _run = false;
            _skill.OnBulletHit(this);
            PoolFactory<Bullet>.Free(this);
//            if (_trans != null)
//            {
//                GameAudioManager.Instance.PlaySoundsEffects(_tableUnit.DestroyAudioName);
//                GameParticleManager.Instance.Emit(_tableUnit.DestroyEffectName, _trans.position, new Vector3(0, 0, _angle), Vector3.one, 1f);
//            }
        }
    }
}