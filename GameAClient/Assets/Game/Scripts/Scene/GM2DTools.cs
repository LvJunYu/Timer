/********************************************************************
** Filename : GM2DTools
** Author : Dong
** Date : 2016/5/27 星期五 下午 1:42:16
** Summary : GM2DTools
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace GameA.Game
{
    public class GM2DTools
    {
        private static int _runtimeCreatedUnitDepth = (int)EUnitDepth.Max;

	    public static float PixelsPerTile
	    {
		    get
		    {
			    return (GM2DGame.Instance.GameScreenHeight / ConstDefineGM2D.MaxHeightTileCount * ConstDefineGM2D.ClientTileScale);
			}
	    }
		
		public static int GetRuntimeCreatedUnitDepth()
        {
            return _runtimeCreatedUnitDepth++;
        }

        public static bool OnDirectionHit(UnitBase unit, UnitBase other, EMoveDirection moveDirection)
        {
            if (unit != null && unit.IsAlive)
            {
                int value = 0;
                switch (moveDirection)
                {
                    case EMoveDirection.Up:
                        return unit.OnDownHit(other, ref value, true);
                    case EMoveDirection.Down:
                        return unit.OnUpHit(other, ref value, true);
                    case EMoveDirection.Left:
                        return unit.OnRightHit(other, ref value, true);
                    case EMoveDirection.Right:
                        return unit.OnLeftHit(other, ref value, true);
                }
            }
            return false;
        }

        public static int GetDistanceToBorder(IntVec2 point, byte direction)
        {
            var validRect = DataScene2D.Instance.ValidMapRect;
            switch (direction)
            {
                case 0:
                    return validRect.Max.y - point.y + 1;
                case 1:
                    return validRect.Max.x - point.x + 1;
                case 2:
                    return point.y - validRect.Min.y +1;
                case 3:
                    return point.x - validRect.Min.x + 1;
            }
            return 1;
        }

        public static void GetBorderPoint(Grid2D grid, ERotationType rotation, ref IntVec2 pointA, ref IntVec2 pointB)
        {
            switch (rotation)
            {
                case ERotationType.Up:
                    pointA = new IntVec2(grid.XMin, grid.YMax + 1);
                    pointB = new IntVec2(grid.XMax, grid.YMax + 1);
                    break;
                case ERotationType.Down:
                    pointA = new IntVec2(grid.XMin, grid.YMin - 1);
                    pointB = new IntVec2(grid.XMax, grid.YMin - 1);
                    break;
                case ERotationType.Left:
                    pointA = new IntVec2(grid.XMin - 1, grid.YMin);
                    pointB = new IntVec2(grid.XMin - 1, grid.YMax);
                    break;
                case ERotationType.Right:
                    pointA = new IntVec2(grid.XMax + 1, grid.YMin);
                    pointB = new IntVec2(grid.XMax + 1, grid.YMax);
                    break;
            }
        }

        public static void GetBorderPoint(Grid2D grid, EMoveDirection eMoveDirection, ref IntVec2 pointA, ref IntVec2 pointB)
        {
            switch (eMoveDirection)
            {
                case EMoveDirection.Up:
                    pointA = new IntVec2(grid.XMin, grid.YMax + 1);
                    pointB = new IntVec2(grid.XMax, grid.YMax + 1);
                    break;
                case EMoveDirection.Down:
                    pointA = new IntVec2(grid.XMin, grid.YMin - 1);
                    pointB = new IntVec2(grid.XMax, grid.YMin - 1);
                    break;
                case EMoveDirection.Left:
                    pointA = new IntVec2(grid.XMin - 1, grid.YMin);
                    pointB = new IntVec2(grid.XMin - 1, grid.YMax);
                    break;
                case EMoveDirection.Right:
                    pointA = new IntVec2(grid.XMax + 1, grid.YMin);
                    pointB = new IntVec2(grid.XMax + 1, grid.YMax);
                    break;
            }
        }

        public static Grid2D CalculateFireColliderGrid(int id, Grid2D colliderGrid, byte direction)
        {
            var tableUnit = UnitManager.Instance.GetTableUnit(id);
            var size = tableUnit.GetColliderSize(0, Vector2.one);
            var centerPos = new IntVec2(colliderGrid.XMax + colliderGrid.XMin + 1, colliderGrid.YMax + colliderGrid.YMin + 1)/2;
            switch ((ERotationType)direction)
            {
                case ERotationType.Right:
                    return new Grid2D(colliderGrid.XMax + 1, centerPos.y - size.y / 2, colliderGrid.XMax + size.x, centerPos.y + size.y / 2 - 1);
                case ERotationType.Left:
                    return new Grid2D(colliderGrid.XMin - size.x, centerPos.y - size.y / 2, colliderGrid.XMin - 1, centerPos.y + size.y / 2 - 1);
                case ERotationType.Up:
                    return new Grid2D(centerPos.x - size.x / 2, colliderGrid.YMax + 1, centerPos.x + size.x / 2 - 1, colliderGrid.YMax + size.y);
                case ERotationType.Down:
                    return new Grid2D(centerPos.x - size.x / 2, colliderGrid.YMin - size.y, centerPos.x + size.x / 2 - 1, colliderGrid.YMin - 1);
            }
            return Grid2D.zero;
        }

		/// <summary>
		/// 从匠游世界坐标转为Unity世界坐标
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="pos">Position.</param>
        public static Vector3 GetPos(IntVec2 pos)
        {
//            var mapWidth = (int)(MapConfig.PermitMapSize.x * ConstDefineGM2D.ClientTileScale);
			float z = (pos.x + pos.y) * 0.00078125f;
            return TileToWorld(pos)  - Vector3.forward * z;
        }

		/// <summary>
		/// 从世界坐标转为带z轴排序的世界坐标
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="pos">Position.</param>
        public static Vector3 GetPos(Vector2 pos)
        {
//            var mapWidth = (int)(MapConfig.PermitMapSize.x * ConstDefineGM2D.ClientTileScale);
			float z = (pos.x + pos.y) * 0.00078125f;
            return new Vector3(pos.x, pos.y, -z);
        }

        public static bool TryGetSlotTrans(Transform owner, Slot slot, out Vector3 pos, out Quaternion qua)
        {
            pos = Vector3.zero;
            qua = Quaternion.identity;
            if (owner == null || slot == null)
            {
                return false;
            }
            qua = owner.rotation * Quaternion.Euler(0, 0, slot.Bone.WorldRotationX);
            pos = owner.position +
                  owner.rotation *
                  new Vector3(slot.Bone.WorldX * owner.localScale.x, slot.Bone.WorldY * owner.localScale.y, 0);
            return true;
        }

        public static bool FindSlot(SkeletonAnimation ani, ESlotType eSlotType, out Slot slot)
        {
            slot = null;
            if (ani == null)
            {
                LogHelper.Error("FindSlot failed ani == null");
                return false;
            }
            if (ani.Skeleton == null)
            {
                LogHelper.Error("FindSlot failed Skeleton == null,{0}", ani.ToString());
                return false;
            }
            slot = ani.Skeleton.FindSlot(eSlotType.ToString());
            if (slot == null)
            {
                LogHelper.Warning("FindSlot failed,{0}|{1}", ani, eSlotType.ToString());
                return false;
            }
            return true;
        }

        public static Spine.Animation GetAnimation(SkeletonAnimation skeleton, string aniName)
        {
            if (skeleton == null)
            {
                return null;
            }
            return skeleton.state.Data.skeletonData.FindAnimation(aniName);
        }

        public static TrackEntry SetAnimation(SkeletonAnimation skeleton, int trackIndex, String animationName, bool loop)
        {
            var animation = GetAnimation(skeleton, animationName);
            if (animation == null)
            {
                LogHelper.Error("GetAnimation Failed,{0} | {1}",skeleton, animationName);
                return null;
            }
            return skeleton.state.SetAnimation(trackIndex, animation, loop);
        }

        public static float GetAnimationDuration(SkeletonAnimation skeleton, string aniName)
        {
            var ani = GetAnimation(skeleton, aniName);
            if (ani == null)
            {
                LogHelper.Error("GetAnimationDuration Failed,{0}", aniName);
                return 0;
            }
            return ani.Duration;
        }

        public static int GetDirection(Vector2 direction)
        {
            int dir = 0;
            dir += direction.y > 0 ? 1 : 0;
            dir += direction.x > 0 ? 2 : 0;
            dir += direction.y < 0 ? 4 : 0;
            dir += direction.x < 0 ? 8 : 0;
            return dir;
        }

        public static int GetlDirection(IntVec2 direction)
        {
            int dir = 0;
            dir += direction.y > 0 ? 1 : 0;
            dir += direction.x > 0 ? 2 : 0;
            dir += direction.y < 0 ? 4 : 0;
            dir += direction.x < 0 ? 8 : 0;
            return dir;
        }

        public static void DrawGrids(Vector3 origin, Vector2 mapSize, Vector2 tileSize, Color gridColor)
        {
            //Handles.color = gridColor;
            int hCount = (int)mapSize.x;
            int vCount = (int)mapSize.y;

            float hLineLength = hCount * tileSize.x;
            float vLineLength = vCount * tileSize.y;
            for (var y = 0; y <= vCount; ++y)
            {
                Vector3 p1 = new Vector3(0, y * tileSize.y) + origin;
                Vector3 p2 = new Vector3(hLineLength, y * tileSize.y) + origin;
                Debug.DrawLine(p1, p2);
            }
            for (var x = 0; x <= hCount; ++x)
            {
                Vector3 p1 = new Vector3(x * tileSize.x, 0) + origin;
                Vector3 p2 = new Vector3(x * tileSize.x, vLineLength) + origin;
                Debug.DrawLine(p1, p2);
            }
        }

        public static Vector3 ScreenToWorldPoint(Vector2 screenPosition)
        {
            if (CameraManager.Instance == null)
            {
                return Vector3.zero;
            }
            return CameraManager.Instance.RendererCamera.ScreenToWorldPoint(screenPosition);
        }

        public static Vector2 ScreenToWorldSize(Vector2 screenSize)
        {
            var start = ScreenToWorldPoint(Vector2.zero);
            var end = ScreenToWorldPoint(screenSize);
            return end - start;
        }

	    public static IntVec2 ScreenToIntVec2(Vector2 mousePos)
	    {
            Vector2 mouseWorldPos = ScreenToWorldPoint(mousePos);
            IntVec2 mouseTile = WorldToTile(mouseWorldPos);
		    return mouseTile;
	    }

		public static Vector2 WorldToScreenPoint(Vector2 worldPosition)
        {
            return CameraManager.Instance.RendererCamera.WorldToScreenPoint(worldPosition);
        }

        public static Vector2 WorldToScreenSize(Vector2 worldSize)
        {
            var start = WorldToScreenPoint(Vector2.zero);
            var end = WorldToScreenPoint(worldSize);
            return end - start;
        }

        public static IntVec2 ScreenToTileByServerTile(Vector2 pixel)
        {
            pixel -= Vector2.one * ConstDefineGM2D.ClientTileScale * 0.5f;
            var x = Mathf.FloorToInt(pixel.x / (PixelsPerTile * ConstDefineGM2D.ServerTileScale)) * ConstDefineGM2D.ServerTileScale;
            var y = Mathf.FloorToInt(pixel.y / (PixelsPerTile * ConstDefineGM2D.ServerTileScale)) * ConstDefineGM2D.ServerTileScale;
            return new IntVec2(x, y);
        }

        public static IntVec2 WorldToTile(Vector2 worldPos)
        {
            var tile = worldPos * ConstDefineGM2D.ServerTileScale;
            return new IntVec2(Mathf.FloorToInt(tile.x), Mathf.FloorToInt(tile.y));
        }

        public static int WorldToTile(float pos)
        {
            return Mathf.FloorToInt(pos * ConstDefineGM2D.ServerTileScale);
        }

        public static Vector3 TileToWorld(IntVec3 tile)
        {
            Vector2 t = new Vector2(tile.x, tile.y) * ConstDefineGM2D.ClientTileScale;
            return new Vector3(t.x, t.y, 0);
        }

        public static Vector3 TileToWorld(IntVec2 tile)
        {
            Vector2 t = new Vector2(tile.x, tile.y) * ConstDefineGM2D.ClientTileScale;
            return new Vector3(t.x, t.y, 0);
        }

        public static Vector3 TileToWorld(IntVec2 tile, float z)
        {
            Vector2 t = new Vector2(tile.x, tile.y) * ConstDefineGM2D.ClientTileScale;
            return new Vector3(t.x, t.y, z);
        }

        public static float TileToWorld(int tile)
        {
            return tile * ConstDefineGM2D.ClientTileScale;
        }

        public static Rect TileRectToWorldRect(IntRect tileRect)
        {
            Vector2 min = TileToWorld(tileRect.Min);
            Vector2 max = TileToWorld(tileRect.Max + IntVec2.one);
            return new Rect(min, max - min);
        }

        public static Vector2 TileToScreen(IntVec2 tile)
        {
            return new Vector2(tile.x, tile.y) * PixelsPerTile;
        }

        public static bool IsContain(Grid2D outer, IntRect inner)
        {
            if (outer.Max.x < inner.Max.x || outer.Max.y < inner.Max.y)
            {
                return false;
            }
            if (outer.Min.x > inner.Min.x || outer.Min.y > inner.Min.y)
            {
                return false;
            }
            return true;
        }

        public static bool CheckIsDeleteScreenOperator(EScreenOperator type)
        {
            if (type == EScreenOperator.LeftDelete ||
                type == EScreenOperator.RightDelete ||
                type == EScreenOperator.UpDelete)
            {
                return true;
            }
            return false;
        }

        public static MapRect2D ToProto(SceneNode node)
        {
            return new MapRect2D
            {
                XMin = node.Grid.XMin,
                YMin = node.Grid.YMin,
                XMax = node.Grid.XMax,
                YMax = node.Grid.YMax,
                Id = node.Id,
                Rotation = node.Rotation,
            };
        }

        public static UnitExtraKeyValuePair ToProto(IntVec3 index, UnitExtra data)
        {
            var res = new UnitExtraKeyValuePair();
            res.Guid = ToProto(index); ;
            res.MoveDirection = (byte)data.MoveDirection;
            res.RollerDirection = (byte)data.RollerDirection;
            res.Msg = data.Msg;
            res.UnitChild = ToProto(data.Child); 
            return res;
        }

        public static IntVec3Proto ToProto(IntVec3 value)
        {
            var guid = new IntVec3Proto();
            guid.X = value.x;
            guid.Y = value.y;
            guid.Z = value.z;
            return guid;
        }

        public static UnitChildProto ToProto(UnitChild value)
        {
            var unitChild = new UnitChildProto();
            unitChild.Id = value.Id;
            unitChild.Rotation = value.Rotation;
            unitChild.MoveDirection = (byte)value.MoveDirection;
            return unitChild;
        }

        public static PairUnitData ToProto(PairUnit pairUnit)
        {
            var data = new PairUnitData();
            data.UnitA = ToProto(pairUnit.UnitA.Guid);
            data.UnitB = ToProto(pairUnit.UnitB.Guid);
            data.Num = pairUnit.Num;
            return data;
        }

        public static IntVec3 ToEngine(IntVec3Proto guid)
	    {
		    return new IntVec3(guid.X, guid.Y, guid.Z);
	    }

        internal static IntRect ToEngine(IntRectProto intRect)
        {
            return new IntRect(intRect.Min.X, intRect.Min.Y, intRect.Max.X, intRect.Max.Y);
        }

        internal static IntRectProto ToProto(IntRect intRect)
        {
            return new IntRectProto
            {
                Min = new IntVec2Proto{ X = intRect.Min.x, Y = intRect.Min.y },
                Max = new IntVec2Proto { X = intRect.Max.x, Y = intRect.Max.y }
            };
        }

        public static void Draw(Grid2D grid)
        {
            Vector3 center = new Vector3(grid.XMax + 1 + grid.XMin, grid.YMax + 1 + grid.YMin, 1) * 0.5f;
            var size = new Vector3(grid.XMax + 1 - grid.XMin, grid.YMax + 1 - grid.YMin, 1);
            Gizmos.DrawWireCube(center * ConstDefineGM2D.ClientTileScale, size * ConstDefineGM2D.ClientTileScale);
        }

        public static Grid2D IntersectWith(IntVec2 one, SceneNode node, Table_Unit tableUnit, bool byData = true)
        {
            var other = node.Grid;
            IntVec2 size = byData ? tableUnit.GetDataSize(node.Rotation, node.Scale) : tableUnit.GetColliderSize(node.Rotation, node.Scale);
            Grid2D grid;
            grid.XMin = other.XMin + (one.x - other.XMin) / size.x * size.x;
            grid.YMin = other.YMin + (one.y - other.YMin) / size.y * size.y;
            grid.XMax = other.XMax - (other.XMax - one.x) / size.x * size.x;
            grid.YMax = other.YMax - (other.YMax - one.y) / size.y * size.y;
            return grid;
        }

        public static Grid2D IntersectWith(Grid2D one, SceneNode node, Table_Unit tableUnit, bool byData = true)
        {
            var other = node.Grid;
            IntVec2 size = byData ? tableUnit.GetDataSize(node.Rotation, node.Scale) : tableUnit.GetColliderSize(node.Rotation, node.Scale);
            Grid2D grid = one.IntersectWith(other);
            if (grid.Equals(other))
            {
                return other;
            }
            int xMinLength = grid.XMin - other.XMin;
            int yMinLength = grid.YMin - other.YMin;
            int xMaxLength = other.XMax - grid.XMax;
            int yMaxLength = other.YMax - grid.YMax;
            if (xMinLength % size.x != 0)
            {
                grid.XMin = other.XMin + xMinLength / size.x * size.x;
            }
            if (yMinLength % size.y != 0)
            {
                grid.YMin = other.YMin + yMinLength / size.y * size.y;
            }
            if (xMaxLength % size.x != 0)
            {
                grid.XMax = other.XMax - xMaxLength / size.x * size.x;
            }
            if (yMaxLength % size.y != 0)
            {
                grid.YMax = other.YMax - yMaxLength / size.y * size.y;
            }
            return grid;
        }

	    public static bool TryGetUnitObject(Vector3 mouseWorldPos, EEditorLayer editorLayer, out UnitDesc unitDesc)
	    {
			unitDesc = new UnitDesc();
			var tile = WorldToTile(mouseWorldPos);
			SceneNode targetNode;
		    int layerMask = editorLayer == EEditorLayer.Effect ? EnvManager.MainPlayerAndEffectLayer : EnvManager.UnitLayerWithoutEffect;
			if (!DataScene2D.PointCast(tile, out targetNode, layerMask))
			{
				return false;
			}
			unitDesc = new UnitDesc();
			unitDesc.Id = targetNode.Id;
            unitDesc.Rotation = targetNode.Rotation;
	        unitDesc.Scale = targetNode.Scale;
			var tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
			if (tableUnit == null)
			{
				LogHelper.Error("WorldPosToTileIndex failed,{0}", unitDesc.Id);
				return false;
			}
			var grid = IntersectWith(tile, targetNode, tableUnit);
			unitDesc.Guid = new IntVec3(grid.XMin, grid.YMin, targetNode.Depth);
			return true;
		}

        /// <summary>
        ///     编辑关卡时候调用
        /// </summary>
        /// <param name="mouseWorldPos"></param>
        /// <param name="unitDesc"></param>
        /// <returns></returns>
        internal static bool TryGetUnitObject(Vector3 mouseWorldPos, out UnitDesc unitDesc)
        {
            unitDesc = new UnitDesc();
            var tile = WorldToTile(mouseWorldPos);
            SceneNode targetNode;
            if (!DataScene2D.PointCast(tile, out targetNode))
            {
                return false;
            }
            unitDesc = new UnitDesc();
            unitDesc.Id = targetNode.Id;
            unitDesc.Rotation = targetNode.Rotation;
            unitDesc.Scale = targetNode.Scale;
            var tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("WorldPosToTileIndex failed,{0}", unitDesc.Id);
                return false;
            }
            var grid = IntersectWith(tile, targetNode, tableUnit);
            unitDesc.Guid = new IntVec3(grid.XMin, grid.YMin, targetNode.Depth);
            return true;
        }

        /// <summary>
        ///     运行时候调用
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="unitDesc"></param>
        /// <returns></returns>
        internal static bool TryGetUnitObject(RayHit2D hit, out UnitDesc unitDesc)
        {
            unitDesc = new UnitDesc();
            if (hit.node == null)
            {
                LogHelper.Error("TryGetUnitObject failed,{0}", hit);
                return false;
            }
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(hit.node.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("TryGetUnitObject failed,{0}", hit);
                return false;
            }
            unitDesc.Id = hit.node.Id;
            unitDesc.Rotation = hit.node.Rotation;
            unitDesc.Scale = hit.node.Scale;
            if (hit.node.IsDynamic())
            {
                unitDesc.Guid = tableUnit.ColliderToRenderer(hit.node.Guid, hit.node.Rotation);
            }
            else
            {
                var tile = WorldToTile(hit.point - hit.normal * ConstDefineGM2D.ClientTileScale * 0.5f);
                var grid = IntersectWith(tile, hit.node, tableUnit, false);
                unitDesc.Guid = tableUnit.ColliderToRenderer(new IntVec3(grid.XMin, grid.YMin, hit.node.Depth), hit.node.Rotation);
            }
            return true;
        }
        
        internal static bool TryGetUnitObject(IntVec2 tile, SceneNode colliderNode, out UnitDesc unitDesc)
        {
            unitDesc = new UnitDesc();
            if (colliderNode == null)
            {
                LogHelper.Error("TryGetUnitObject failed,{0}", tile);
                return false;
            }
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(colliderNode.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("TryGetUnitObject failed,{0}", colliderNode);
                return false;
            }
            unitDesc.Id = colliderNode.Id;
            unitDesc.Rotation = colliderNode.Rotation;
            unitDesc.Scale = colliderNode.Scale;
            if (colliderNode.IsDynamic())
            {
                unitDesc.Guid = tableUnit.ColliderToRenderer(colliderNode.Guid, colliderNode.Rotation);
            }
            else
            {
                var grid = IntersectWith(tile, colliderNode, tableUnit, false);
                unitDesc.Guid = tableUnit.ColliderToRenderer(new IntVec3(grid.XMin, grid.YMin, colliderNode.Depth), colliderNode.Rotation);
            }
            return true;
        }

        public static Vector3 GetUnitDragingOffset(int id)
        {
            Vector3 res = Vector3.zero;
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(id);
            if (tableUnit == null)
            {
                LogHelper.Error("GetUnitDragingOffset called but id is invalid {0}!", id);
                return res;
            }
            if (tableUnit.EGeneratedType == EGeneratedType.Spine)
            {
                IntVec2 size = tableUnit.GetDataSize(0, Vector2.one);
                Vector3 offset = TileToWorld(size) * 0.5f;
                offset.x = 0;
                res = offset;
            }
            return res;
        }

        public static Vector3 GetModelOffsetInWorldPos(IntVec2 dataSize, IntVec2 modelSize, Table_Unit tableUnit)
        {
            Vector3 offsetInWorld = Vector3.zero;
            Vector3 modelSizeInWorld = TileToWorld(modelSize);
            // 这里只能假设物体都是一个方块大小，不然锚点没有参照物
            Vector3 dataSizeInWorld = TileToWorld(dataSize);
            EAnchore anchor = tableUnit.EModelAnchore;
            switch (anchor)
            {
                case EAnchore.UpLeft:
                    {
                        offsetInWorld.x = modelSizeInWorld.x * 0.5f;
                        offsetInWorld.y = dataSizeInWorld.y - modelSizeInWorld.y * 0.5f;
                        break;
                    }
                case EAnchore.Left:
                    {
                        offsetInWorld.x = modelSizeInWorld.x * 0.5f;
                        offsetInWorld.y = dataSizeInWorld.y * 0.5f;
                        break;
                    }
                case EAnchore.DownLeft:
                    {
                        offsetInWorld.x = modelSizeInWorld.x * 0.5f;
                        offsetInWorld.y = modelSizeInWorld.y * 0.5f;
                        break;
                    }
                case EAnchore.Up:
                    {
                        offsetInWorld.x = dataSizeInWorld.x * 0.5f;
                        offsetInWorld.y = dataSizeInWorld.y - modelSizeInWorld.y * 0.5f;
                        break;
                    }
                case EAnchore.Center:
                    {
                        offsetInWorld.x = dataSizeInWorld.x * 0.5f;
                        offsetInWorld.y = dataSizeInWorld.y * 0.5f;
                        break;
                    }
                case EAnchore.Down:
                    {
                        offsetInWorld.x = dataSizeInWorld.x * 0.5f;
                        offsetInWorld.y = modelSizeInWorld.y * 0.5f;
                        break;
                    }
                case EAnchore.UpRight:
                    {
                        offsetInWorld.x = dataSizeInWorld.x - modelSizeInWorld.x * 0.5f;
                        offsetInWorld.y = dataSizeInWorld.y - modelSizeInWorld.y * 0.5f;
                        break;
                    }
                case EAnchore.Right:
                    {
                        offsetInWorld.x = dataSizeInWorld.x - modelSizeInWorld.x * 0.5f;
                        offsetInWorld.y = dataSizeInWorld.y * 0.5f;
                        break;
                    }
                case EAnchore.DownRight:
                    {
                        offsetInWorld.x = dataSizeInWorld.x - modelSizeInWorld.x * 0.5f;
                        offsetInWorld.y = modelSizeInWorld.y * 0.5f;
                        break;
                    }
            }
			if (tableUnit.EGeneratedType == EGeneratedType.Spine)
            {
                offsetInWorld.y -= modelSizeInWorld.y * 0.5f;
            }
            return offsetInWorld;
        }
    }
}
