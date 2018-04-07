using SoyEngine;

namespace GameA.Game
{
    public class ColliderComponent : UnitComponent
    {
        public IntVec2 ColliderPos;
        public Grid2D ColliderGrid;
        public Grid2D ColliderGridInner;
        public Grid2D LastColliderGrid;

        public int ColliderPosY
        {
            get { return ColliderPos.y; }
            set { ColliderPos.y = value; }
        }

        public int ColliderPosX
        {
            get { return ColliderPos.x; }
            set { ColliderPos.x = value; }
        }

        public override void Clear()
        {
            base.Clear();
            ColliderPos = Unit.GetColliderPos(Unit.CurPos);
            var unitDesc = Unit.UnitDesc;
            LastColliderGrid = ColliderGrid = Unit.TableUnit.GetColliderGrid(ref unitDesc);
        }

        public void UpdateCollider(IntVec2 min)
        {
            if (ColliderPos.Equals(min))
            {
                return;
            }

            ColliderPos = min;
            ColliderGrid = GetColliderGrid(ColliderPos);
            if (!LastColliderGrid.Equals(ColliderGrid))
            {
                Unit.DynamicCollider.Grid = ColliderGrid;
                ColliderScene2D.CurScene.UpdateDynamicNode(Unit.DynamicCollider);
                LastColliderGrid = ColliderGrid;
            }
        }

        public Grid2D GetColliderGrid(IntVec2 min)
        {
            return new Grid2D(min.x, min.y, min.x + ColliderGrid.XMax - ColliderGrid.XMin,
                min.y + ColliderGrid.YMax - ColliderGrid.YMin);
        }

        public bool UpdateDynamicUnit()
        {
            if (!LastColliderGrid.Equals(ColliderGrid))
            {
                Unit.DynamicCollider.Grid = ColliderGrid;
                ColliderScene2D.CurScene.UpdateDynamicUnit(Unit, LastColliderGrid);
                LastColliderGrid = ColliderGrid;
                return true;
            }

            return false;
        }

        public int GetUpHitMin()
        {
            return ColliderGrid.YMax + 1;
        }

        public int GetDownHitMin(UnitBase other)
        {
            return ColliderGrid.YMin - 1 - (other.ColliderGrid.YMax - other.ColliderGrid.YMin);
        }

        public int GetLeftHitMin(UnitBase other)
        {
            return ColliderGrid.XMin - 1 - (other.ColliderGrid.XMax - other.ColliderGrid.XMin);
        }

        public int GetRightHitMin()
        {
            return Unit.ColliderGrid.XMax + 1;
        }
    }
}