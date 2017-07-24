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
using UnityEngine.VR.WSA.WebCam;

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

        public SkillBase(int id, UnitBase ower)
        {
            _owner = ower;
            _tableSkill = TableManager.Instance.GetSkill(id);
            _cdTime = TableConvert.GetTime(_tableSkill.CDTime);
            _singTime = TableConvert.GetTime(_tableSkill.SingTime);
            _castRange = TableConvert.GetRange(_tableSkill.CastRange);
            _projectileSpeed = TableConvert.GetSpeed(_tableSkill.ProjectileSpeed);
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
            _timerSing = _singTime;
            if (_timerSing == 0)
            {
                OnSkillCast();
            }
            return true;
        }
        
        protected void CreateProjectile(int projectileId, IntVec2 pos, int angle, int delayRunTime = 0)
        {
            var bullet =  PlayMode.Instance.CreateRuntimeUnit(projectileId, pos) as ProjectileBase;
            if (bullet != null)
            {
                bullet.Run(this, angle, delayRunTime);
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
            switch ((EBehaviorType)_tableSkill.BehaviorType)
            {
                case EBehaviorType.ContinueShoot:
                    var count = _tableSkill.BehaviorValues[0];
                    var delay = TableConvert.GetTime(_tableSkill.BehaviorValues[1]);
                    for (int i = 0; i < count; i++)
                    {
                            CreateProjectile(_tableSkill.ProjectileId, GetProjectilePos(_tableSkill.ProjectileId), _owner.ShootAngle, delay * i);
                    }
                    break;
                case EBehaviorType.Common:
                case EBehaviorType.SectorShoot:
                case EBehaviorType.Summon:
                case EBehaviorType.Teleport:
                case EBehaviorType.HitDivide:
                    CreateProjectile(_tableSkill.ProjectileId, GetProjectilePos(_tableSkill.ProjectileId), _owner.ShootAngle);
                    break;
            }
        }

        public virtual void OnProjectileHit(ProjectileBase projectile)
        {
            List<UnitBase> units = null;
            switch ((EEffcetMode)_tableSkill.EffectMode)
            {
                case EEffcetMode.Single:
                    break;
                case EEffcetMode.TargetCircle:
                    {
                        _radius = TableConvert.GetRange(_tableSkill.EffectValues[0]) + 1;
                        units = ColliderScene2D.CircleCastAllReturnUnits(projectile.CenterPos, _radius, JoyPhysics2D.GetColliderLayerMask(projectile.DynamicCollider.Layer));
                    }
                    break;
                case EEffcetMode.TargetGrid:
                    break;
                case EEffcetMode.TargetLine:
                    break;
                case EEffcetMode.SelfSector:
                    break;
                case EEffcetMode.SelfCircle:
                {
                    _radius = TableConvert.GetRange(_tableSkill.EffectValues[0]) + 1;
                    units = ColliderScene2D.CircleCastAllReturnUnits(_owner.CenterPos, _radius, JoyPhysics2D.GetColliderLayerMask(projectile.DynamicCollider.Layer));
                }
                    break;
            }
            if (units != null && units.Count > 0)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive)
                    {
                        if (unit.IsActor)
                        {
                            OnActorHit(unit, projectile);
                        }
                        else if(unit.CanPainted)
                        {
                            OnPaintHit(unit, projectile);
                        }
                    }
                }
            }
        }

        private void OnActorHit(UnitBase unit, ProjectileBase projectile)
        {
            unit.OnHpChanged(_damage);
            if (!unit.IsAlive)
            {
                return;
            }
            //触发状态
            for (int i = 0; i < _tableSkill.TriggerStates.Length; i++)
            {
                if (_tableSkill.TriggerStates[i] > 0)
                {
                    unit.AddStates(_tableSkill.TriggerStates[i]);
                }
            }
            //生成陷阱
            if (_tableSkill.TrapId > 0)
            {
                PlayMode.Instance.AddTrap(_tableSkill.TrapId);
            }
            var forces = _tableSkill.KnockbackForces;
            if (forces.Length == 2)
            {
                var direction = unit.CenterDownPos - projectile.CenterDownPos;
                unit.ExtraSpeed.x = direction.x >= 0 ? forces[0] : -forces[0];
                unit.ExtraSpeed.y = direction.y >= -320 ? forces[1] : -forces[1];
                unit.Speed = IntVec2.zero;
                unit.CurBanInputTime = 20;
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
