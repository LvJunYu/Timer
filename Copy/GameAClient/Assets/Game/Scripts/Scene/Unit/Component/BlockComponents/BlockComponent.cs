using UnityEngine;

namespace GameA.Game
{
    public class BlockComponent : UnitComponent
    {
        public virtual bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnUpClampSpeed(other);
                y = GetUpHitMin();
            }

            return true;
        }

        public virtual bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnDownClampSpeed(other);
                y = GetDownHitMin(other);
            }

            return true;
        }

        public virtual bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnLeftClampSpeed(other);
                x = GetLeftHitMin(other);
            }

            return true;
        }

        public virtual bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnRightClampSpeed(other);
                x = GetRightHitMin();
            }

            return true;
        }

        protected int GetUpHitMin()
        {
            return Unit.ColliderGrid.YMax + 1;
        }

        protected int GetDownHitMin(UnitBase other)
        {
            return Unit.ColliderGrid.YMin - 1 - (other.ColliderGrid.YMax - other.ColliderGrid.YMin);
        }

        protected int GetLeftHitMin(UnitBase other)
        {
            return Unit.ColliderGrid.XMin - 1 - (other.ColliderGrid.XMax - other.ColliderGrid.XMin);
        }

        protected int GetRightHitMin()
        {
            return Unit.ColliderGrid.XMax + 1;
        }

        protected void OnUpClampSpeed(UnitBase other)
        {
            if (other.SpeedY < 0)
            {
                other.SpeedY = 0;
            }
        }

        protected void OnDownClampSpeed(UnitBase other)
        {
            if (other.SpeedY > 0)
            {
                other.SpeedY = 0;
            }
        }

        protected void OnLeftClampSpeed(UnitBase other)
        {
            if (other.SpeedX > 0)
            {
                other.SpeedX = 0;
            }
        }

        protected void OnRightClampSpeed(UnitBase other)
        {
            if (other.SpeedX < 0)
            {
                other.SpeedX = 0;
            }
        }

        protected Vector3 GetHitEffectPos(UnitBase other, EDirectionType hitDirectionType)
        {
            if (other.Trans == null)
            {
                return Vector3.zero;
            }

            var otherCenterPos = other.GetColliderPos(other.CurPos) + other.GetColliderSize() / 2;
            var otherPos = GM2DTools.TileToWorld(otherCenterPos, other.Trans.localPosition.z);
            switch (hitDirectionType)
            {
                case EDirectionType.Up:
                    return new Vector3(otherPos.x, GM2DTools.TileToWorld(GetUpHitMin()), otherPos.z);
                case EDirectionType.Down:
                    return new Vector3(otherPos.x, GM2DTools.TileToWorld(GetDownHitMin(other)), otherPos.z);
                case EDirectionType.Left:
                    return new Vector3(GM2DTools.TileToWorld(GetLeftHitMin(other)), otherPos.y, otherPos.z);
                case EDirectionType.Right:
                    return new Vector3(GM2DTools.TileToWorld(GetRightHitMin()), otherPos.y, otherPos.z);
            }

            return Vector3.zero;
        }
    }
}