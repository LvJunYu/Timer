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
        protected EPaintType _epaintType;
        
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

        protected int _energyTotal = 1000;
        protected int _energyCost;
        protected int _regenSpeed;
        protected int _currentEnergy;
        protected int _energyTimer;
        
        protected List<Bullet> _bullets = new List<Bullet>();

        public int Id
        {
            get { return _tableSkill.Id; }
        }
   
        public UnitBase Owner
        {
            get { return _owner; }
        }

        public int CastRange
        {
            get { return _castRange; }
        }

        public EPaintType EPaintType
        {
            get { return _epaintType; }
        }

        public int ProjectileSpeed
        {
            get { return _projectileSpeed; }
        }

        public Table_Skill TableSkill
        {
            get { return _tableSkill; }
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
                    _epaintType = EPaintType.Water;
                    break;
                case 2:
                    _epaintType = EPaintType.Clay;
                    break;
                case 3:
                    _epaintType = EPaintType.Jelly;
                    break;
                case 4:
                    _epaintType = EPaintType.Fire;
                    break;
                case 5:
                    _epaintType = EPaintType.Ice;
                    break;
            }
            SetTimerCD(0);
            _cdTime = TableConvert.GetTime(_tableSkill.CDTime);
            _singTime = TableConvert.GetTime(_tableSkill.SingTime);
            _castRange = TableConvert.GetRange(_tableSkill.CastRange);
            _projectileSpeed = TableConvert.GetSpeed(_tableSkill.ProjectileSpeed);
            _damage = _tableSkill.Damage;
            _energyCost = _tableSkill.Cost;
            _regenSpeed = _tableSkill.RegenSpeed;
            SetEnergy(_energyTotal);
            _timerSing = 0;
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

        public virtual void Clear()
        {
            for (int i = 0; i < _bullets.Count; i++)
            {
                var bullet = _bullets[i];
                if (bullet != null)
                {
                    PoolFactory<Bullet>.Free(bullet);
                }
            }
            _bullets.Clear();
        }

        internal virtual void Exit()
        {
        }

        public virtual void UpdateLogic()
        {
            _energyTimer++;
            if (_energyTimer == ConstDefineGM2D.FixedFrameCount)
            {
                SetEnergy(_currentEnergy + _regenSpeed);
                _energyTimer = 0;
            }
            if (_timerCD > 0)
            {
                SetTimerCD(_timerCD - 1);
            }
            if (_timerSing > 0)
            {
                _timerSing--;
                if (_timerSing == 0)
                {
                    OnSkillCast();
                }
            }
            if (_bullets.Count > 0)
            {
                for (int i = 0; i < _bullets.Count; i++)
                {
                    _bullets[i].UpdateLogic();
                }
            }
        }
        
        public bool Fire()
        {
            if (_energyCost > 0 && _currentEnergy < _energyCost)
            {
                LogHelper.Debug("Energy is not enough! {0} | {1}", _currentEnergy, _energyCost);
                return false;
            }
            if (_timerCD > 0)
            {
                return false;
            }
            SetTimerCD(_cdTime);
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
            SetEnergy(_currentEnergy - _energyCost);
            return true;
        }
        
        private void SetEnergy(int value)
        {
            var mp = Mathf.Clamp(value, 0, _energyTotal);
            if (_currentEnergy != mp)
            {
                _currentEnergy = mp;
                if (_owner != null)
                {
                    switch ((ECostType)_tableSkill.CostType)
                    {
                        case ECostType.Magic:
                            if (_owner.View != null)
                            {
                                _owner.View.StatusBar.SetMP(_currentEnergy, _energyTotal);
                            }
                            break;
                        case ECostType.Rage:
                            if (_owner.IsMain)
                            {
                                Messenger<float, float>.Broadcast(EMessengerType.OnSkill2CDChanged, _currentEnergy, _energyTotal);
                            }
                            break;
                    }
                }
            }
        }

        private void SetTimerCD(int value)
        {
            if (_timerCD == value)
            {
                return;
            }
            _timerCD = value;
            if (_owner.IsMain && _tableSkill.CostType == (int)ECostType.Magic)
            {
                Messenger<float, float>.Broadcast(EMessengerType.OnSkill2CDChanged, _timerCD, _cdTime);
            }
        }

        protected void CreateProjectile(int projectileId, IntVec2 pos, int angle)
        {
            if (_epaintType == EPaintType.Water || _epaintType == EPaintType.Jelly || _epaintType == EPaintType.Clay)
            {
                var bullet = PoolFactory<Bullet>.Get();
                bullet.Init(this,pos, angle);
                _bullets.Add(bullet);
                return;
            }
            {
                var bullet = PlayMode.Instance.CreateRuntimeUnit(projectileId, pos) as ProjectileBase;
                if (bullet != null)
                {
                    bullet.Run(this, angle);
                }
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
                    CreateProjectile(_tableSkill.ProjectileId, GetProjectilePos(_tableSkill.ProjectileId), _owner.ShootAngle);
                    break;
                case EBehaviorType.ContinueShoot:
                    var count = _tableSkill.BehaviorValues[0];
//                    var delay = TableConvert.GetTime(_tableSkill.BehaviorValues[1]);
//                    for (int i = 0; i < count; i++)
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

        protected void OnHit()
        {
            var centerDownPos = _owner.CenterDownPos;
            CreateTrap(centerDownPos);
            //临时写 TODO
            var units = GetHitUnits(centerDownPos);
            if (units != null && units.Count > 0)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive && unit != _owner)
                    {
                        if (unit.IsActor)
                        {
                            var dir = unit.CenterPos - centerDownPos;
                            OnActorHit(unit, new Vector2(dir.x,dir.y));
                        }
                    }
                }
            }
        }

        public void OnBulletHit(Bullet bullet)
        {
            _bullets.Remove(bullet);
            CreateTrap(bullet.CurPos);
            var units = GetHitUnits(bullet.CurPos);
            if (units != null && units.Count > 0)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive)
                    {
                        if (unit.IsActor)
                        {
                            OnActorHit(unit, bullet.Direction);
                        }
                        else if(unit.CanPainted && _epaintType != EPaintType.None)
                        {
//                            OnPaintHit(unit, bullet);
                        }
                    }
                }
            }
        }

        public virtual void OnProjectileHit(ProjectileBase projectile)
        {
            CreateTrap(projectile.CenterPos);
            var units = GetHitUnits(projectile.CenterPos);
            if (units != null && units.Count > 0)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive)
                    {
                        if (unit.IsActor)
                        {
                            OnActorHit(unit, projectile.Direction);
                        }
                        else if(unit.CanPainted && _epaintType != EPaintType.None)
                        {
                            OnPaintHit(unit, projectile);
                        }
                    }
                }
            }
        }

        private void OnActorHit(UnitBase unit, Vector2 direction)
        {
            if (!unit.IsAlive)
            {
                return;
            }
            if (!unit.IsInvincible)
            {
                var forces = _tableSkill.KnockbackForces;
                if (forces.Length == 2)
                {
                    unit.Speed = IntVec2.zero;
                    unit.CurBanInputTime = 10;
                    if (direction.x > 0)
                    {
                        unit.ExtraSpeed.x = forces[0];
                    }
                    else if (direction.x < 0)
                    {
                        unit.ExtraSpeed.x = -forces[0];
                    }
                    if (direction.y > 0)
                    {
                        unit.ExtraSpeed.y = forces[1];
                    }
                    else if (direction.y < 0)
                    {
                        unit.ExtraSpeed.y = -forces[1];
                    }
                }
            }
            //触发状态
            unit.AddStates(_tableSkill.AddStates);
            unit.RemoveStates(_tableSkill.RemoveStates);
            unit.OnHpChanged(-_damage);
        }

        protected List<UnitBase> GetHitUnits(IntVec2 centerPos)
        {
            var hitLayerMask = GetTargetType();
            switch ((EEffcetMode) _tableSkill.EffectMode)
            {
                case EEffcetMode.Single:
                    break;
                case EEffcetMode.TargetCircle:
                {
                    _radius = TableConvert.GetRange(_tableSkill.EffectValues[0]);
                    return ColliderScene2D.CircleCastAllReturnUnits(centerPos, _radius, hitLayerMask);
                }
                case EEffcetMode.TargetGrid:
                {
                    _radius = TableConvert.GetRange(_tableSkill.EffectValues[0]);
                    var grid = new Grid2D(centerPos.x - _radius, centerPos.y - _radius, centerPos.x + _radius - 1, centerPos.y + _radius - 1);
                    return ColliderScene2D.GridCastAllReturnUnits(grid, hitLayerMask);
                }
                case EEffcetMode.TargetLine:
                    break;
                case EEffcetMode.SelfSector:
                    _radius = TableConvert.GetRange(_tableSkill.EffectValues[0]);
                    var units = ColliderScene2D.CircleCastAllReturnUnits(_owner.CenterPos, _radius, hitLayerMask);
                    for (int i = units.Count - 1; i >= 0; i--)
                    {
                        var unit = units[i];
                        var rel = _owner.CenterDownPos - unit.CenterDownPos;
                        if ((rel.x >= 0 && _owner.CurMoveDirection == EMoveDirection.Left) ||
                            (rel.x <= 0 && _owner.CurMoveDirection == EMoveDirection.Right))
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
        
        protected int GetTargetType()
        {
            int layer = 0;
            if (HasTargetType(ETargetType.Earth))
            {
                layer |= EnvManager.ItemLayer;
            }
            if (HasTargetType(ETargetType.Monster))
            {
                layer |= EnvManager.MonsterLayer;
            }
            if (HasTargetType(ETargetType.MainPlayer))
            {
                layer |= EnvManager.MainPlayerLayer;
            }
            if (HasTargetType(ETargetType.RemotePlayer))
            {
                layer |= EnvManager.RemotePlayer;
            }
            if (HasTargetType(ETargetType.Self))
            {
//                layer |= EnvManager.ItemLayer;
            }
            return layer;
        }

        private bool HasTargetType(ETargetType eTargetType)
        {
            return ((1<<(int) eTargetType) & _tableSkill.TargetType) != 0;
        }
        
        protected void OnPaintHit(UnitBase target,ProjectileBase projectile)
        {
            int length = ConstDefineGM2D.ServerTileScale;
            var guid = target.Guid;
            UnitBase neighborUnit;
            int v = 0;
            var curPos = projectile.CenterPos;
            if (curPos.y < target.ColliderGrid.YMin)
            {
                if (!ColliderScene2D.Instance.TryGetUnit(new IntVec3(guid.x, guid.y - length, guid.z), out neighborUnit)
                || !neighborUnit.OnDownHit(projectile,ref v, true))
                {
                    DoPaint(projectile, target, EDirectionType.Down);
                }
            }
            else if (curPos.y > target.ColliderGrid.YMax)
            {
                if (!ColliderScene2D.Instance.TryGetUnit(new IntVec3(guid.x, guid.y + length, guid.z), out neighborUnit)
                || !neighborUnit.OnUpHit(projectile,ref v, true))

                {
                    DoPaint(projectile, target, EDirectionType.Up);
                }
            }
            if (curPos.x < target.ColliderGrid.XMin)
            {
                if (!ColliderScene2D.Instance.TryGetUnit(new IntVec3(guid.x - length, guid.y, guid.z), out neighborUnit)
                || !neighborUnit.OnLeftHit(projectile,ref v, true))
                {
                    DoPaint(projectile, target, EDirectionType.Left);
                }
            }
            else if (curPos.x > target.ColliderGrid.XMax)
            {
                if (!ColliderScene2D.Instance.TryGetUnit(new IntVec3(guid.x + length, guid.y, guid.z), out neighborUnit)
                || !neighborUnit.OnRightHit(projectile,ref v, true))
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
                        target.DoPaint(start, end, EDirectionType.Down, _epaintType, maskRandom);

                        if (start <= target.ColliderGrid.XMin)
                        {
                            start = target.ColliderGrid.YMin;
                            end = target.ColliderGrid.YMin + paintDepth;
                            target.DoPaint(start, end, EDirectionType.Left, _epaintType, maskRandom, false);
                        }
                        if (end >= target.ColliderGrid.XMax)
                        {
                            start = target.ColliderGrid.YMin;
                            end = target.ColliderGrid.YMin + paintDepth;
                            target.DoPaint(start, end, EDirectionType.Right, _epaintType, maskRandom, false);
                        }
                    }
                    break;
                case EDirectionType.Up:
                    {
                        var start = centerPos.x - _radius;
                        var end = centerPos.x + _radius;
                        target.DoPaint(start, end, EDirectionType.Up, _epaintType, maskRandom);
                        if (start <= target.ColliderGrid.XMin)
                        {
                            start = target.ColliderGrid.YMax - paintDepth;
                            end = target.ColliderGrid.YMax;
                            target.DoPaint(start, end, EDirectionType.Left, _epaintType, maskRandom, false);
                        }
                        if (end >= target.ColliderGrid.XMax)
                        {
                            start = target.ColliderGrid.YMax - paintDepth;
                            end = target.ColliderGrid.YMax;
                            target.DoPaint(start, end, EDirectionType.Right, _epaintType, maskRandom, false);
                        }
                    }
                    break;
                case EDirectionType.Right:
                    {
                        var start = centerPos.y - _radius;
                        var end = centerPos.y + _radius;
                        target.DoPaint(start, end, EDirectionType.Right, _epaintType, maskRandom);
                        if (start <= target.ColliderGrid.YMin)
                        {
                            start = target.ColliderGrid.XMax - paintDepth;
                            end = target.ColliderGrid.XMax;
                            target.DoPaint(start, end, EDirectionType.Down, _epaintType, maskRandom, false);
                        }
                        if (end >= target.ColliderGrid.YMax)
                        {
                            start = target.ColliderGrid.XMax - paintDepth;
                            end = target.ColliderGrid.XMax;
                            target.DoPaint(start, end, EDirectionType.Up, _epaintType, maskRandom, false);
                        }
                    }
                    break;
                case EDirectionType.Left:
                    {
                        var start = centerPos.y - _radius;
                        var end = centerPos.y + _radius;
                        target.DoPaint(start, end, EDirectionType.Left, _epaintType, maskRandom);
                        if (start <= target.ColliderGrid.YMin)
                        {
                            start = target.ColliderGrid.XMin;
                            end = target.ColliderGrid.XMin + paintDepth;
                            target.DoPaint(start, end, EDirectionType.Down, _epaintType, maskRandom, false);
                        }
                        if (end >= target.ColliderGrid.YMax)
                        {
                            start = target.ColliderGrid.XMin;
                            end = target.ColliderGrid.XMin + paintDepth;
                            target.DoPaint(start, end, EDirectionType.Up, _epaintType, maskRandom, false);
                        }
                    }
                    break;
            }
        }
    }
}
