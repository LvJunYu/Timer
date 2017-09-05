/********************************************************************
** Filename : Table_Unit
** Author : Dong
** Date : 2016/3/16 星期三 下午 11:57:30
** Summary : Table_Unit
***********************************************************************/

using SoyEngine;
using Spine.Unity;
using UnityEngine;

namespace GameA.Game
{
    public partial class Table_Unit
	{
        public virtual EColliderType EColliderType
        {
            get { return (EColliderType)ColliderType; }
        }

        public EGeneratedType EGeneratedType
        {
            get { return (EGeneratedType)GeneratedType; }
        }

        public EAnchore EAnchore
        {
            get { return (EAnchore)Anchore; }
        }

        public EAnchore EModelAnchore
        {
            get { return (EAnchore)ModelAnchore; }
        }

		public EUnitType EUnitType
		{
			get { return (EUnitType) UnitType; }
		}

	    public EPairType EPairType
	    {
	        get { return (EPairType) PairType; }
	    }

	    public EDirectionType DefaultDirection
	    {
	        get
	        {
	            return (EDirectionType) (Direction - 1);
	        }
	    }

	    public EMoveDirection DefaultMoveDirection
	    {
	        get
	        {
	            if (MoveDirection == 5)
	            {
	                return EMoveDirection.None;
	            }
	            return (EMoveDirection) MoveDirection;
	        }
	    }

	    public EActiveState DefaultActiveState
	    {
	        get
	        {
	            return (EActiveState) (ActiveState - 1);
	        }
	    }

	    public ERotateType DefaultRotateType
	    {
	        get
	        {
	            if (RotateState == 3)
	            {
	                return ERotateType.None;
	            }
		        return (ERotateType) (RotateState - 1);
	        }
	    }

		public bool HasDirection8
		{
			get { return DirectionMask == 127; }
		}

		public bool CanEdit(EEditType editType)
	    {
	        switch (editType)
	        {
	            case EEditType.Direction:
	                return Direction > 0;
	            case EEditType.MoveDirection:
	                return MoveDirection > 0;
	            case EEditType.Active:
	                return ActiveState > 0;
	            case EEditType.Child:
	                return ChildState != null;
	            case EEditType.Rotate:
	                return RotateState > 0;
	            case EEditType.Time:
	                return TimeState != null;
	            case EEditType.Text:
	                return TextState > 0;
	            case EEditType.Style:
	                return Id == 4001;
	        }
	        return false;
	    }

	    // 碰撞体偏移
		public IntVec2 Offset;
        // 模型偏移
        public Vector3 ModelOffset;

        public virtual IntVec2 GetDataSize(SceneNode node)
        {
            return GetDataSize(node.Rotation, node.Scale);
        }

        public virtual IntVec2 GetDataSize(ref UnitDesc unitDesc)
        {
            return GetDataSize(unitDesc.Rotation, unitDesc.Scale);
        }

        public virtual IntVec2 GetDataSize(int rotation, Vector2 scale)
	    {
            if (rotation == (byte)EDirectionType.Up || rotation == (byte)EDirectionType.Down)
            {
                return new IntVec2((int) (Width * scale.x), (int) (Height * scale.y));
            }
            return new IntVec2((int) (Height * scale.y), (int) (Width * scale.x));
	    }

        public virtual IntVec2 GetColliderSize(ref UnitDesc unitDesc)
        {
            return GetColliderSize(unitDesc.Rotation, unitDesc.Scale);
        }

	    public virtual IntVec2 GetColliderSize(SceneNode node)
	    {
            return GetColliderSize(node.Rotation, node.Scale);
	    }

	    public virtual IntVec2 GetColliderSize(int rotation, Vector2 scale)
        {
            if (rotation == (byte)EDirectionType.Up || rotation == (byte)EDirectionType.Down)
            {
                return new IntVec2((int) (CWidth * scale.x), (int) (CHeight * scale.y));
            }
            return new IntVec2((int) (CHeight * scale.y), (int) (CWidth * scale.x));
        }

        public Grid2D GetBaseDataGrid(int xmin, int ymin)
        {
            return GetDataGrid(xmin, ymin, 0, Vector2.one);
        }

	    public Grid2D GetBaseDataGrid(IntVec3 rectIndex)
	    {
            return GetDataGrid(rectIndex.x, rectIndex.y, 0, Vector2.one);
	    }

        public Grid2D GetDataGrid(ref UnitDesc unitDesc)
        {
            return GetDataGrid(unitDesc.Guid.x, unitDesc.Guid.y, unitDesc.Rotation, unitDesc.Scale);
        }

        public Grid2D GetDataGrid(int xmin, int ymin, byte rotation, Vector2 scale)
	    {
	        var size = GetDataSize(rotation, scale);
            return new Grid2D(xmin, ymin, xmin + size.x - 1,
                ymin + size.y - 1);
	    }

        public Grid2D GetBaseColliderGrid(IntVec3 colliderRectIndex)
        {
            return GetColliderGrid(colliderRectIndex.x, colliderRectIndex.y, 0, Vector2.one);
        }

        public Grid2D GetColliderGrid(int xmin, int ymin, byte rotation, Vector2 scale)
        {
            var size = GetColliderSize(rotation, scale);
            return new Grid2D(xmin, ymin, xmin + size.x - 1,
                ymin + size.y - 1);
        }

        public Grid2D GetColliderGrid(ref UnitDesc unitDesc)
        {
            var colliderGuid = RendererToCollider(ref unitDesc);
            var size = GetColliderSize(unitDesc.Rotation, unitDesc.Scale);
            return new Grid2D(colliderGuid.x, colliderGuid.y, colliderGuid.x + size.x - 1,
                colliderGuid.y + size.y - 1);
        }

        public IntVec2 GetDataCount(NodeData node)
        {
            return GetDataCount(node.Grid, node.Direction, node.Scale);
        }

        public IntVec2 GetColliderCount(NodeData colliderNode)
        {
            return GetColliderCount(colliderNode.Grid, colliderNode.Direction, colliderNode.Scale);
        }

        public IntVec2 GetColliderCount(Grid2D grid, byte rotation, Vector2 scale)
        {
            var size = GetColliderSize(rotation, scale);
            return new IntVec2((grid.XMax + 1 - grid.XMin) / size.x, (grid.YMax + 1 - grid.YMin) / size.y);
        }

	    public IntVec2 GetDataCount(Grid2D grid, byte rotation, Vector2 scale)
	    {
            var size = GetDataSize(rotation, scale);
            return new IntVec2((grid.XMax + 1 - grid.XMin) / size.x, (grid.YMax + 1 - grid.YMin) / size.y);
	    }

	    public virtual void Init()
	    {
            Offset.x = 0;
            Offset.y = 0;
            switch (EAnchore)
            {
                case EAnchore.Up: {
                    Offset.x = (Width - CWidth) / 2;
                    Offset.y = Height - CHeight;
                    break;
                }
                case EAnchore.Center: {
                    Offset.x = (Width - CWidth) / 2;
                    Offset.y = (Height - CHeight) / 2;
                    break;
                }
                case EAnchore.Down: {
                    Offset.x = (Width - CWidth) / 2;
                    Offset.y = 0;
                    break;
                }
                case EAnchore.UpLeft: {
                    Offset.x = 0;
                    Offset.y = Height - CHeight;
                    break;
                }
                case EAnchore.Left: {
                    Offset.x = 0;
                    Offset.y = (Height - CHeight) / 2;
                    break;
                }
                case EAnchore.DownLeft: {
                    Offset.x = 0;
                    Offset.y = 0;
                    break;
                }
                case EAnchore.UpRight: {
                    Offset.x = Width - CWidth;
                    Offset.y = Height - CHeight;
                    break;
                }
                case EAnchore.Right: {
                    Offset.x = Width - CWidth;
                    Offset.y = (Height - CHeight) / 2;
                    break;
                }
                case EAnchore.DownRight: {
                    Offset.x = Width - CWidth;
                    Offset.y = 0;
                    break;
                }
            }
            // 先置为一个无效值，运行时初始化第一个物体的时候再赋有效值，因为图片大小在载入前不知道
            ModelOffset.x = float.MaxValue;
            ModelOffset.y = float.MaxValue;
        }

	    public IntVec3 RendererToCollider(ref UnitDesc unitDesc)
	    {
	        return RendererToCollider(unitDesc.Guid, unitDesc.Rotation);
	    }

	    public IntVec3 RendererToCollider(IntVec3 renderIndex, int rotation)
        {
            if (rotation == (byte)EDirectionType.Up)
            {
                return new IntVec3(renderIndex.x + Offset.x, renderIndex.y + Offset.y,
                    renderIndex.z);
            }
            if (rotation == (byte)EDirectionType.Right)
            {
                return new IntVec3(renderIndex.x + Offset.y,
                    renderIndex.y + Width - Offset.x - CWidth, renderIndex.z);
            }
            if (rotation == (byte)EDirectionType.Down)
            {
                return new IntVec3(renderIndex.x + Width - Offset.x - CWidth,
                    renderIndex.y + Height - Offset.y - CHeight, renderIndex.z);
            }
            return new IntVec3(renderIndex.x + Height - Offset.y - CHeight,
                renderIndex.y + Width - Offset.x - CWidth, renderIndex.z);
        }

        public IntVec2 RendererToCollider(IntVec2 renderIndex, int rotation)
        {
            if (rotation == (byte)EDirectionType.Up)
            {
                return new IntVec2(renderIndex.x + Offset.x, renderIndex.y + Offset.y);
            }
            if (rotation == (byte)EDirectionType.Right)
            {
                return new IntVec2(renderIndex.x + Offset.y,
                    renderIndex.y + Width - Offset.x - CWidth);
            }
            if (rotation == (byte)EDirectionType.Down)
            {
                return new IntVec2(renderIndex.x + Width - Offset.x - CWidth,
                    renderIndex.y + Height - Offset.y - CHeight);
            }
            return new IntVec2(renderIndex.x + Height - Offset.y - CHeight,
                renderIndex.y + Width - Offset.x - CWidth);
        }

	    public IntVec3 ColliderToRenderer(UnitDesc unitDesc)
	    {
	        return ColliderToRenderer(unitDesc.Guid, unitDesc.Rotation);
	    }

	    public IntVec3 ColliderToRenderer(IntVec3 colliderIndex, int rotation)
        {
            if (rotation == (byte)EDirectionType.Up)
            {
                return new IntVec3(colliderIndex.x - Offset.x, colliderIndex.y - Offset.y,
                    colliderIndex.z);
            }
            if (rotation == (byte)EDirectionType.Right)
            {
                return new IntVec3(colliderIndex.x - Offset.y,
                    colliderIndex.y - Width + Offset.x + CWidth, colliderIndex.z);
            }
            if (rotation == (byte)EDirectionType.Down)
            {
                return new IntVec3(colliderIndex.x - Width + Offset.x + CWidth,
                    colliderIndex.y - Height + Offset.y + CHeight, colliderIndex.z);
            }
            return new IntVec3(colliderIndex.x - Height + Offset.y + CHeight,
                colliderIndex.y - Width + Offset.x + CWidth, colliderIndex.z);
        }

        public IntVec2 ColliderToRenderer(IntVec2 colliderIndex, int rotation)
        {
            if (rotation == (byte)EDirectionType.Up)
            {
                return new IntVec2(colliderIndex.x - Offset.x, colliderIndex.y - Offset.y);
            }
            if (rotation == (byte)EDirectionType.Right)
            {
                return new IntVec2(colliderIndex.x - Offset.y,
                    colliderIndex.y - Width + Offset.x + CWidth);
            }
            if (rotation == (byte)EDirectionType.Down)
            {
                return new IntVec2(colliderIndex.x - Width + Offset.x + CWidth,
                    colliderIndex.y - Height + Offset.y + CHeight);
            }
            return new IntVec2(colliderIndex.x - Height + Offset.y + CHeight,
                colliderIndex.y - Width + Offset.x + CWidth);
        }
    }

    public enum EGeneratedType
    {
        None,
        Spine,
        Tiling,
        Morph,
        Empty,
    }
}