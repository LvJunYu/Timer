using SoyEngine;

namespace GameA.Game
{
    public class PaintSkill : SkillBase
    {
        public override void OnProjectileHit(ProjectileBase projectile)
        {
            //检查地块范围
            var checkGrid = projectile.ColliderGrid.Enlarge(_radius);
            var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, JoyPhysics2D.GetColliderLayerMask(projectile.DynamicCollider.Layer));
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (unit.IsAlive)
                {
                    if (unit.CanPainted)
                    {
                        OnPaintHit(projectile, unit);
                    }
                    else if(unit.IsActor)
                    {
                        OnHeroHit(projectile, unit);
                    }
                }
            }
        }

        protected void OnPaintHit(ProjectileBase projectile, UnitBase target)
        {
            int length = ConstDefineGM2D.ServerTileScale;
            var guid = target.Guid;
            UnitBase neighborUnit;
            var curPos = projectile.CurPos;
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
            var colliderGrid = projectile.ColliderGrid;
            var maskRandom = projectile.MaskRandom;
            switch (eDirectionType)
            {
                case EDirectionType.Down:
                    {
                        int centerPoint = (colliderGrid.XMax + 1 + colliderGrid.XMin) / 2;
                        var start = centerPoint - _radius;
                        var end = centerPoint + _radius;
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
                        int centerPoint = (colliderGrid.XMax + 1 + colliderGrid.XMin) / 2;
                        var start = centerPoint - _radius;
                        var end = centerPoint + _radius;
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
                        int centerPoint = (colliderGrid.YMax + 1 + colliderGrid.YMin) / 2;
                        var start = centerPoint - _radius;
                        var end = centerPoint + _radius;
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
                        int centerPoint = (colliderGrid.YMax + 1 + colliderGrid.YMin) / 2;
                        var start = centerPoint - _radius;
                        var end = centerPoint + _radius;
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

        protected virtual void OnHeroHit(ProjectileBase projectile, UnitBase target)
        {
        }

        public PaintSkill(int id, UnitBase ower) : base(id, ower)
        {
        }
    }
}