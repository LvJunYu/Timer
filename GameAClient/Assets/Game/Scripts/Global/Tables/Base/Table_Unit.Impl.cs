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
        public bool IsHero
        {
            get { return EUnitType == EUnitType.MainPlayer || EUnitType == EUnitType.Monster; }
        }

        public bool IsMorphUnit
        {
            get { return EGeneratedType == EGeneratedType.Morph; }
        }

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

	    public bool CanRotate
	    {
	        get { return RotationMask != 0; }
	    }

        public bool CanMove
        {
            get { return OriginMoveDirection != 0; }
        }

	    public int _width;
	    public int _height;
        public int _cWidth;
        public int _cHeight;

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
            if (rotation == (byte)ERotationType.Up || rotation == (byte)ERotationType.Down)
            {
                return new IntVec2((int) (_width * scale.x), (int) (_height * scale.y));
            }
            return new IntVec2((int) (_height * scale.y), (int) (_width * scale.x));
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
            if (rotation == (byte)ERotationType.Up || rotation == (byte)ERotationType.Down)
            {
                return new IntVec2((int) (_cWidth * scale.x), (int) (_cHeight * scale.y));
            }
            return new IntVec2((int) (_cHeight * scale.y), (int) (_cWidth * scale.x));
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
	        _width = Width*20;
	        _height = Height*20;
	        _cWidth = CWidth*20;
	        _cHeight = CHeight*20;

            Offset.x = 0;
            Offset.y = 0;
            switch (EAnchore)
            {
                case EAnchore.Up: {
                    Offset.x = (_width - _cWidth) / 2;
                    Offset.y = _height - _cHeight;
                    break;
                }
                case EAnchore.Center: {
                    Offset.x = (_width - _cWidth) / 2;
                    Offset.y = (_height - _cHeight) / 2;
                    break;
                }
                case EAnchore.Down: {
                    Offset.x = (_width - _cWidth) / 2;
                    Offset.y = 0;
                    break;
                }
                case EAnchore.UpLeft: {
                    Offset.x = 0;
                    Offset.y = _height - _cHeight;
                    break;
                }
                case EAnchore.Left: {
                    Offset.x = 0;
                    Offset.y = (_height - _cHeight) / 2;
                    break;
                }
                case EAnchore.DownLeft: {
                    Offset.x = 0;
                    Offset.y = 0;
                    break;
                }
                case EAnchore.UpRight: {
                    Offset.x = _width - _cWidth;
                    Offset.y = _height - _cHeight;
                    break;
                }
                case EAnchore.Right: {
                    Offset.x = _width - _cWidth;
                    Offset.y = (_height - _cHeight) / 2;
                    break;
                }
                case EAnchore.DownRight: {
                    Offset.x = _width - _cWidth;
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
            if (rotation == (byte)ERotationType.Up)
            {
                return new IntVec3(renderIndex.x + Offset.x, renderIndex.y + Offset.y,
                    renderIndex.z);
            }
            if (rotation == (byte)ERotationType.Right)
            {
                return new IntVec3(renderIndex.x + Offset.y,
                    renderIndex.y + _width - Offset.x - _cWidth, renderIndex.z);
            }
            if (rotation == (byte)ERotationType.Down)
            {
                return new IntVec3(renderIndex.x + _width - Offset.x - _cWidth,
                    renderIndex.y + _height - Offset.y - _cHeight, renderIndex.z);
            }
            return new IntVec3(renderIndex.x + _height - Offset.y - _cHeight,
                renderIndex.y + _width - Offset.x - _cWidth, renderIndex.z);
        }

        public IntVec2 RendererToCollider(IntVec2 renderIndex, int rotation)
        {
            if (rotation == (byte)ERotationType.Up)
            {
                return new IntVec2(renderIndex.x + Offset.x, renderIndex.y + Offset.y);
            }
            if (rotation == (byte)ERotationType.Right)
            {
                return new IntVec2(renderIndex.x + Offset.y,
                    renderIndex.y + _width - Offset.x - _cWidth);
            }
            if (rotation == (byte)ERotationType.Down)
            {
                return new IntVec2(renderIndex.x + _width - Offset.x - _cWidth,
                    renderIndex.y + _height - Offset.y - _cHeight);
            }
            return new IntVec2(renderIndex.x + _height - Offset.y - _cHeight,
                renderIndex.y + _width - Offset.x - _cWidth);
        }

	    public IntVec3 ColliderToRenderer(UnitDesc unitDesc)
	    {
	        return ColliderToRenderer(unitDesc.Guid, unitDesc.Rotation);
	    }

	    public IntVec3 ColliderToRenderer(IntVec3 colliderIndex, int rotation)
        {
            if (rotation == (byte)ERotationType.Up)
            {
                return new IntVec3(colliderIndex.x - Offset.x, colliderIndex.y - Offset.y,
                    colliderIndex.z);
            }
            if (rotation == (byte)ERotationType.Right)
            {
                return new IntVec3(colliderIndex.x - Offset.y,
                    colliderIndex.y - _width + Offset.x + _cWidth, colliderIndex.z);
            }
            if (rotation == (byte)ERotationType.Down)
            {
                return new IntVec3(colliderIndex.x - _width + Offset.x + _cWidth,
                    colliderIndex.y - _height + Offset.y + _cHeight, colliderIndex.z);
            }
            return new IntVec3(colliderIndex.x - _height + Offset.y + _cHeight,
                colliderIndex.y - _width + Offset.x + _cWidth, colliderIndex.z);
        }

        public IntVec2 ColliderToRenderer(IntVec2 colliderIndex, int rotation)
        {
            if (rotation == (byte)ERotationType.Up)
            {
                return new IntVec2(colliderIndex.x - Offset.x, colliderIndex.y - Offset.y);
            }
            if (rotation == (byte)ERotationType.Right)
            {
                return new IntVec2(colliderIndex.x - Offset.y,
                    colliderIndex.y - _width + Offset.x + _cWidth);
            }
            if (rotation == (byte)ERotationType.Down)
            {
                return new IntVec2(colliderIndex.x - _width + Offset.x + _cWidth,
                    colliderIndex.y - _height + Offset.y + _cHeight);
            }
            return new IntVec2(colliderIndex.x - _height + Offset.y + _cHeight,
                colliderIndex.y - _width + Offset.x + _cWidth);
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