/********************************************************************
** Filename : SkillBase
** Author : Dong
** Date : 2017/3/22 星期三 上午 10:35:13
** Summary : SkillBase
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using Spine.Unity.MeshGeneration;
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

        protected int _projectileSpeed;

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

        protected int _damage;
        protected int _cure;
        protected int _shield;

        public int Id
        {
            get { return _tableSkill.Id; }
        }
        
        public int MpCost
        {
            get { return _tableSkill.MpCost; }
        }
        
        public int RpCost
        {
            get { return _tableSkill.RpCost; }
        }

        public UnitBase Owner
        {
            get { return _owner; }
        }

        public int CastRange
        {
            get { return _castRange; }
        }

        public ESkillType ESkillType
        {
            get { return _eSkillType; }
        }

        public int ProjectileSpeed
        {
            get { return _projectileSpeed; }
        }

        public bool IsBig
        {
            get { return RpCost > 0; }
        }

        public SkillBase(int id, UnitBase ower)
        {
            _owner = ower;
            _tableSkill = TableManager.Instance.GetSkill(id);
            if (_tableSkill == null)
            {
                LogHelper.Error("GetSkill Failed, {0}", id);
                return;
            }
            switch (_tableSkill.Id)
            {
                case 1:
                    _eSkillType = ESkillType.Water;
                    break;
                case 3:
                    _eSkillType = ESkillType.Fire;
                    break;
                case 5:
                    _eSkillType = ESkillType.Ice;
                    break;
                case 7:
                    _eSkillType = ESkillType.Jelly;
                    break;
                case 9:
                    _eSkillType = ESkillType.Clay;
                    break;
            }
            _cdTime = TableConvert.GetTime(_tableSkill.CDTime);
            _singTime = TableConvert.GetTime(_tableSkill.SingTime);
            _castRange = TableConvert.GetRange(_tableSkill.CastRange);
            _projectileSpeed = TableConvert.GetSpeed(_tableSkill.ProjectileSpeed);
            _damage = _tableSkill.Damage;
            _cure = _tableSkill.Cure;
            _timerSing = 0;
            _timerCD = 0;
            _timerCharge = 0;
        }

        internal void SetValue(int cdTime, int castRange, int singTime = 0)
        {
            _cdTime = TableConvert.GetTime(cdTime);
            _castRange = TableConvert.GetRange(castRange);
            _singTime = TableConvert.GetTime(singTime);
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
            if (_timerCD > 0)
            {
                return false;
            }
            _timerCD = _cdTime;
            if (_timerSing > 0)
            {
                return false;
            }
            _owner.StartSkill();
            _timerSing = _singTime;
            if (_timerSing == 0)
            {
                OnSkillCast();
            }
            return true;
        }
        
        protected void CreateProjectile(int projectileId, IntVec2 pos, int angle)
        {
            var bullet = PlayMode.Instance.CreateRuntimeUnit(projectileId, pos) as ProjectileBase;
            if (bullet != null)
            {
                bullet.Run(this, angle);
            }
        }

        protected IntVec2 GetProjectilePos(int bulletId)
        {
            var tableUnit = UnitManager.Instance.GetTableUnit(bulletId);
            if (tableUnit == null)
            {
                return IntVec2.zero;
            }
            var dataSize = tableUnit.GetDataSize(0, Vector2.one);
            return _owner.FirePos - dataSize * 0.5f;
        }
        
        protected virtual void OnSkillCast()
        {
            _owner.OnSkillCast();
            switch ((EBehaviorType)_tableSkill.BehaviorType)
            {
                case EBehaviorType.Common:
                    OnHit();
                    break;
                case EBehaviorType.RangeShoot:
                case EBehaviorType.ContinueShoot:
                    var count = _tableSkill.BehaviorValues[0];
                    //                    var delay = TableConvert.GetTime(_tableSkill.BehaviorValues[1]);
                    for (int i = 0; i < count; i++)
                    {
                        CreateProjectile(_tableSkill.ProjectileId, GetProjectilePos(_tableSkill.ProjectileId), _owner.ShootAngle);
                    }
                    break;
                case EBehaviorType.SectorShoot:
                case EBehaviorType.Summon:
                case EBehaviorType.Teleport:
                case EBehaviorType.HitDivide:
                    CreateProjectile(_tableSkill.ProjectileId, GetProjectilePos(_tableSkill.ProjectileId), _owner.ShootAngle);
                    break;
            }
        }

        protected List<UnitBase> GetHitUnits(IntVec2 centerPos, int hitLayerMask)
        {
            switch ((EEffcetMode)_tableSkill.EffectMode)
            {
                case EEffcetMode.Single:
                    break;
                case EEffcetMode.TargetCircle:
                    {
                        _radius = TableConvert.GetRange(_tableSkill.EffectValues[0]);
                        return ColliderScene2D.CircleCastAllReturnUnits(centerPos, _radius, hitLayerMask);
                    }
                case EEffcetMode.TargetGrid:
                    break;
                case EEffcetMode.TargetLine:
                    break;
                case EEffcetMode.SelfSector:
                    _radius = TableConvert.GetRange(_tableSkill.EffectValues[0]);
                    var units = ColliderScene2D.CircleCastAllReturnUnits(_owner.CenterPos, _radius, hitLayerMask);
                    for (int i = units.Count - 1; i >= 0; i--)
                    {
                        var unit = units[i];
                        var rel = _owner.CenterDownPos - unit.CenterDownPos;
                        if ((rel.x >= 0 && _owner.CurMoveDirection == EMoveDirection.Left) || (rel.x <= 0 && _owner.CurMoveDirection == EMoveDirection.Right))
                        {
                        }
                        else
                        {
                            units.RemoveAt(i);
                        }
                    }
                    return units;
                case EEffcetMode.SelfCircle:
                    {
                        _radius = TableConvert.GetRange(_tableSkill.EffectValues[0]);
                        return ColliderScene2D.CircleCastAllReturnUnits(_owner.CenterPos, _radius, hitLayerMask);
                    }
            }
            return null;
        }

        /// <summary>
        /// 生成陷阱
        /// </summary>
        protected void CreateTrap(IntVec2 centerPos)
        {
            if (_tableSkill.TrapId > 0)
            {
                LogHelper.Debug("AddTrap {0}", _tableSkill.TrapId);
                PlayMode.Instance.AddTrap(_tableSkill.TrapId, centerPos);
            }
        }

        protected void OnHit()
        {
            var centerDownPos = _owner.CenterDownPos;
            CreateTrap(centerDownPos);
            //临时写 TODO
            var units = GetHitUnits(centerDownPos, EnvManager.ActorLayer);
            if (units != null && units.Count > 0)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive && unit != _owner)
                    {
                        if (unit.IsActor)
                        {
                            OnActorHit(unit, centerDownPos);
                        }
                    }
                }
            }
        }

        public virtual void OnProjectileHit(ProjectileBase projectile)
        {
            CreateTrap(projectile.CenterPos);
            var units = GetHitUnits(projectile.CenterPos, JoyPhysics2D.GetColliderLayerMask(projectile.DynamicCollider.Layer));
            if (units != null && units.Count > 0)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive)
                    {
                        if (unit.IsActor)
                        {
                            OnActorHit(unit, projectile.CenterDownPos);
                        }
                        else if(unit.CanPainted)
                        {
                            OnPaintHit(unit, projectile);
                        }
                    }
                }
            }
        }

        private void OnActorHit(UnitBase unit, IntVec2 centerDownPos)
        {
            if (!unit.IsAlive)
            {
                return;
            }
            unit.OnHpChanged(-_damage);
            unit.OnHpChanged(_cure);
            //触发状态
            for (int i = 0; i < _tableSkill.TriggerStates.Length; i++)
            {
                if (_tableSkill.TriggerStates[i] > 0)
                {
                    unit.AddStates(_tableSkill.TriggerStates[i]);
                }
            }
            var forces = _tableSkill.KnockbackForces;
            if (forces.Length == 2)
            {
                var direction = unit.CenterDownPos - centerDownPos;
                unit.ExtraSpeed.x = direction.x >= 0 ? forces[0] : -forces[0];
                unit.ExtraSpeed.y = direction.y >= -320 ? forces[1] : -forces[1];
                unit.Speed = IntVec2.zero;
            }
//            LogHelper.Debug("OnActorHit, {0}", unit);
        }
        
        protected void OnPaintHit(UnitBase target,ProjectileBase projectile)
        {
            int length = ConstDefineGM2D.ServerTileScale;
            var guid = target.Guid;
            UnitBase neighborUnit;
            var curPos = projectile.CenterPos;
            if (curPos.y < target.ColliderGrid.YMin)
            {
                if (!ColliderScene2D.Instance.TryGetUnit(new IntVec3(guid.x, guid.y - length, guid.z), out neighborUnit))
                {
                    DoPaint(projectile, target, EDirectionType.Down);
                }
            }
            else if (curPos.y > target.ColliderGrid.YMax)
            {
                if (!ColliderScene2D.Instance.TryGetUnit(new IntVec3(guid.x, guid.y + length, guid.z), out neighborUnit))
                {
                    DoPaint(projectile, target, EDirectionType.Up);
                }
            }
            if (curPos.x < target.ColliderGrid.XMin)
            {
                if (!ColliderScene2D.Instance.TryGetUnit(new IntVec3(guid.x - length, guid.y, guid.z), out neighborUnit))
                {
                    DoPaint(projectile, target, EDirectionType.Left);
                }
            }
            else if (curPos.x > target.ColliderGrid.XMax)
            {
                if (!ColliderScene2D.Instance.TryGetUnit(new IntVec3(guid.x + length, guid.y, guid.z), out neighborUnit))
                {
                    DoPaint(projectile, target, EDirectionType.Right);
                }
            }
        }

        protected virtual void DoPaint(ProjectileBase projectile, UnitBase target, EDirectionType eDirectionType)
        {
            var paintDepth = PaintBlock.TileOffsetHeight;
            var centerPos = projectile.CenterPos;
            var maskRandom = projectile.MaskRandom;
            switch (eDirectionType)
            {
                case EDirectionType.Down:
                    {
                        var start = centerPos.x - _radius;
                        var end = centerPos.x + _radius;
                        target.DoPaint(start, end, EDirectionType.Down, _eSkillType, maskRandom);

                        if (start <= target.ColliderGrid.XMin)
                        {
                            start = target.ColliderGrid.YMin;
                            end = target.ColliderGrid.YMin + paintDepth;
                            target.DoPaint(start, end, EDirectionType.Left, _eSkillType, maskRandom, false);
                        }
                        if (end >= target.ColliderGrid.XMax)
                        {
                            start = target.ColliderGrid.YMin;
                            end = target.ColliderGrid.YMin + paintDepth;
                            target.DoPaint(start, end, EDirectionType.Right, _eSkillType, maskRandom, false);
                        }
                    }
                    break;
                case EDirectionType.Up:
                    {
                        var start = centerPos.x - _radius;
                        var end = centerPos.x + _radius;
                        target.DoPaint(start, end, EDirectionType.Up, _eSkillType, maskRandom);
                        if (start <= target.ColliderGrid.XMin)
                        {
                            start = target.ColliderGrid.YMax - paintDepth;
                            end = target.ColliderGrid.YMax;
                            target.DoPaint(start, end, EDirectionType.Left, _eSkillType, maskRandom, false);
                        }
                        if (end >= target.ColliderGrid.XMax)
                        {
                            start = target.ColliderGrid.YMax - paintDepth;
                            end = target.ColliderGrid.YMax;
                            target.DoPaint(start, end, EDirectionType.Right, _eSkillType, maskRandom, false);
                        }
                    }
                    break;
                case EDirectionType.Right:
                    {
                        var start = centerPos.y - _radius;
                        var end = centerPos.y + _radius;
                        target.DoPaint(start, end, EDirectionType.Right, _eSkillType, maskRandom);
                        if (start <= target.ColliderGrid.YMin)
                        {
                            start = target.ColliderGrid.XMax - paintDepth;
                            end = target.ColliderGrid.XMax;
                            target.DoPaint(start, end, EDirectionType.Down, _eSkillType, maskRandom, false);
                        }
                        if (end >= target.ColliderGrid.YMax)
                        {
                            start = target.ColliderGrid.XMax - paintDepth;
                            end = target.ColliderGrid.XMax;
                            target.DoPaint(start, end, EDirectionType.Up, _eSkillType, maskRandom, false);
                        }
                    }
                    break;
                case EDirectionType.Left:
                    {
                        var start = centerPos.y - _radius;
                        var end = centerPos.y + _radius;
                        target.DoPaint(start, end, EDirectionType.Left, _eSkillType, maskRandom);
                        if (start <= target.ColliderGrid.YMin)
                        {
                            start = target.ColliderGrid.XMin;
                            end = target.ColliderGrid.XMin + paintDepth;
                            target.DoPaint(start, end, EDirectionType.Down, _eSkillType, maskRandom, false);
                        }
                        if (end >= target.ColliderGrid.YMax)
                        {
                            start = target.ColliderGrid.XMin;
                            end = target.ColliderGrid.XMin + paintDepth;
                            target.DoPaint(start, end, EDirectionType.Up, _eSkillType, maskRandom, false);
                        }
                    }
                    break;
            }
        }
    }
}
