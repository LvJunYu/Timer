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
        protected UnityNativeParticleItem _effectBullet;
        protected Table_Unit _tableUnit;

        protected SkillBase _skill;
        protected Vector2 _direction;
        protected float _angle;
        protected IntVec2 _speed;
        protected IntVec2 _originPos;
        protected IntVec2 _curPos;
     
        protected int _maskRandom;
        protected int _destroy;

        protected UnitBase _targetUnit;

        protected int _hitLayer;
        
        public Vector2 Direction
        {
            get { return _direction; }
        }

        public float Angle
        {
            get { return _angle; }
        }

        public int MaskRandom
        {
            get { return _maskRandom; }
        }
        
        public IntVec2 CurPos
        {
            get { return _curPos; }
        }

        public UnitBase TargetUnit
        {
            get { return _targetUnit; }
        }

        public void OnGet()
        {
            _maskRandom = UnityEngine.Random.Range(0, 2);
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
            _targetUnit = null;
            GameParticleManager.FreeParticleItem(_effectBullet);
            _effectBullet = null;
            _hitLayer = 0;
        }

        public void OnDestroyObject()
        {
            if (_trans != null)
            {
                UnityEngine.Object.Destroy(_trans.gameObject);
            }
        }

        public Bullet()
        {
            _trans = new GameObject("Bullet").transform;
            if (UnitManager.Instance != null) 
            {
                _trans.parent = UnitManager.Instance.GetParent(EUnitType.Bullet);
            }
        }

        public void Init(SkillBase skill, IntVec2 pos, float angle)
        {
            _tableUnit = UnitManager.Instance.GetTableUnit(skill.TableSkill.ProjectileId);
            _skill = skill;
            _hitLayer = _skill.Owner.IsMain ? EnvManager.BulletHitLayer : EnvManager.BulletHitLayerWithMainPlayer;
            _curPos = _originPos = pos;
            
            _angle = angle;
            _direction = GM2DTools.GetDirection(_angle);
            
            _speed = new IntVec2((int) (_skill.ProjectileSpeed * _direction.x),
                (int) (_skill.ProjectileSpeed * _direction.y));
            
            _effectBullet = GameParticleManager.Instance.GetUnityNativeParticleItem(_tableUnit.Model, _trans);
            if (_effectBullet != null)
            {
                _effectBullet.Play();
            }
            _trans.eulerAngles = new Vector3(0, 0, -angle);
            UpdateTransPos();
            _run = true;
        }

        private void UpdateTransPos()
        {
            if (_trans != null)
            {
                _trans.position = GM2DTools.TileToWorld(_curPos, GetZ());
            }
        }
        
        protected float GetZ()
        {
            return -(_curPos.x + _curPos.y * 1.5f) * UnitDefine.UnitSorttingLayerRatio;
        }

        public void UpdateLogic()
        {
            if (!_run)
            {
                return;
            }
            //MagicSwith Brick Cloud
            var hits = ColliderScene2D.RaycastAll(_curPos, _direction, _skill.ProjectileSpeed, _hitLayer);
            if (hits.Count > 0)
            {
                for (int i = 0; i < hits.Count; i++)
                {
                    var hit = hits[i];
                    if (UnitDefine.IsBulletBlock(hit.node.Id))
                    {
                       var units = ColliderScene2D.GetUnits(hit);
                        for (var j = 0; j < units.Count; j++)
                        {
                            var unit = units[j];
                            if (unit != _skill.Owner && unit.IsAlive && !unit.CanBulletCross)
                            {
                                _targetUnit = unit;
                                _curPos = hit.point;
                                _destroy = 1;
                                if (UnitDefine.IsMagicSwitch(unit.Id))
                                {
                                    var switchMagic = unit as SwitchMagic;
                                    if (switchMagic != null)
                                    {
                                        switchMagic.OnTrigger();
                                    }
                                }
                                else if (UnitDefine.BrickId == unit.Id)
                                {
                                    var brick = unit as Brick;
                                    if (brick != null)
                                    {
                                        brick.DestroyBrick();
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            UpdateTransPos();
            if (_destroy > 0)
            {
                OnDestroy();
            }
            else
            {
                _curPos += _speed;
                //超出最大射击距离
                if ((_curPos - _originPos).SqrMagnitude() >= _skill.CastRange * _skill.CastRange)
                {
                    _curPos = _originPos + new IntVec2((int)(_skill.CastRange * _direction.x), (int)(_skill.CastRange * _direction.y));
                    _destroy = 1;
                }
            }
        }

        protected void OnDestroy()
        {
            _run = false;
            if (_trans != null)
            {
                GameAudioManager.Instance.PlaySoundsEffects(_tableUnit.DestroyAudioName);
                GameParticleManager.Instance.Emit(_tableUnit.DestroyEffectName, _trans.position, new Vector3(0, 0, _angle), Vector3.one, 1f);
            }
            _skill.OnBulletHit(this);
        }
    }
}