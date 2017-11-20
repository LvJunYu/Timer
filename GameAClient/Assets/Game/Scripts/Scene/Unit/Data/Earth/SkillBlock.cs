using SoyEngine;

namespace GameA.Game
{
    public class SkillBlock : BlockBase
    {
        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.IsActor)
            {
                var min = new IntVec2(_colliderGrid.XMin, other.CenterPos.y);
                var grid = new Grid2D(min.x, min.y, min.x + ConstDefineGM2D.ServerTileScale, min.y);
                CheckSkillHit(other, grid, EDirectionType.Left);
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.IsActor)
            {
                var min = new IntVec2(_colliderGrid.XMin, other.CenterPos.y);
                var grid = new Grid2D(min.x, min.y, min.x + ConstDefineGM2D.ServerTileScale, min.y);
                CheckSkillHit(other, grid, EDirectionType.Right);
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsActor)
            {
                var min = new IntVec2(other.CenterPos.x, _colliderGrid.YMin);
                var grid = new Grid2D(min.x, min.y, min.x, min.y + ConstDefineGM2D.ServerTileScale);
                CheckSkillHit(other, grid, EDirectionType.Down);
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsActor || other.Id == UnitDefine.BoxId)
            {
                var min = new IntVec2(other.CenterPos.x, _colliderGrid.YMin);
                var grid = new Grid2D(min.x, min.y, min.x, min.y + ConstDefineGM2D.ServerTileScale);
                CheckSkillHit(other, grid, EDirectionType.Up);
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        protected override void Hit(UnitBase other, EDirectionType eDirectionType)
        {
            base.Hit(other, eDirectionType);
            switch (eDirectionType)
            {
                case EDirectionType.Left:
                case EDirectionType.Right:
                    if (other.IsActor)
                    {
                        var min = new IntVec2(_colliderGrid.XMin, other.CenterPos.y);
                        var grid = new Grid2D(min.x, min.y, min.x + ConstDefineGM2D.ServerTileScale, min.y);
                        CheckSkillHit(other, grid, eDirectionType);
                    }
                    break;
                case EDirectionType.Up:
                    if (other.IsActor || other.Id == UnitDefine.BoxId)
                    {
                        var min = new IntVec2(other.CenterPos.x, _colliderGrid.YMin);
                        var grid = new Grid2D(min.x, min.y, min.x, min.y + ConstDefineGM2D.ServerTileScale);
                        CheckSkillHit(other, grid, eDirectionType);
                    }
                    break;
                case EDirectionType.Down:
                    if (other.IsActor)
                    {
                        var min = new IntVec2(other.CenterPos.x, _colliderGrid.YMin);
                        var grid = new Grid2D(min.x, min.y, min.x, min.y + ConstDefineGM2D.ServerTileScale);
                        CheckSkillHit(other, grid, eDirectionType);
                    }
                    break;
            }
        }

        protected virtual void CheckSkillHit(UnitBase other, Grid2D grid, EDirectionType eDirectionType)
        {
        }
    }
}