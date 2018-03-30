/********************************************************************
** Filename : GM2DTools
** Author : Dong
** Date : 2016/5/27 星期五 下午 1:42:16
** Summary : GM2DTools
***********************************************************************/

using System;
using SoyEngine;
using SoyEngine.Proto;
using Spine;
using Spine.Unity;
using UnityEngine;
using Animation = Spine.Animation;

namespace GameA.Game
{
    public static class GM2DTools
    {
        private static int _runtimeCreatedUnitDepth = (int) EUnitDepth.Max;

        public static float PixelsPerTile
        {
            get
            {
                return (1f * GM2DGame.Instance.GameScreenHeight / ConstDefineGM2D.MaxHeightTileCount *
                        ConstDefineGM2D.ClientTileScale);
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
            var validRect = DataScene2D.CurScene.ValidMapRect;
            switch (direction)
            {
                case 0:
                    return validRect.Max.y - point.y + 1;
                case 1:
                    return validRect.Max.x - point.x + 1;
                case 2:
                    return point.y - validRect.Min.y + 1;
                case 3:
                    return point.x - validRect.Min.x + 1;
            }

            return 1;
        }

        public static void GetBorderPoint(Grid2D grid, EDirectionType direction, ref IntVec2 pointA, ref IntVec2 pointB,
            int num = 0)
        {
            switch (direction)
            {
                case EDirectionType.Up:
                    pointA = new IntVec2(grid.XMin - num, grid.YMax + 1);
                    pointB = new IntVec2(grid.XMax + num, grid.YMax + 1);
                    break;
                case EDirectionType.Down:
                    pointA = new IntVec2(grid.XMin - num, grid.YMin - 1);
                    pointB = new IntVec2(grid.XMax + num, grid.YMin - 1);
                    break;
                case EDirectionType.Left:
                    pointA = new IntVec2(grid.XMin - 1, grid.YMin - num);
                    pointB = new IntVec2(grid.XMin - 1, grid.YMax + num);
                    break;
                case EDirectionType.Right:
                    pointA = new IntVec2(grid.XMax + 1, grid.YMin - num);
                    pointB = new IntVec2(grid.XMax + 1, grid.YMax + num);
                    break;
            }
        }

        public static void GetBorderPoint(Grid2D grid, EMoveDirection eMoveDirection, ref IntVec2 pointA,
            ref IntVec2 pointB)
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
            var centerPos = new IntVec2(colliderGrid.XMax + colliderGrid.XMin + 1,
                                colliderGrid.YMax + colliderGrid.YMin + 1) / 2;
            switch ((EDirectionType) direction)
            {
                case EDirectionType.Right:
                    return new Grid2D(colliderGrid.XMax + 1, centerPos.y - size.y / 2, colliderGrid.XMax + size.x,
                        centerPos.y + size.y / 2 - 1);
                case EDirectionType.Left:
                    return new Grid2D(colliderGrid.XMin - size.x, centerPos.y - size.y / 2, colliderGrid.XMin - 1,
                        centerPos.y + size.y / 2 - 1);
                case EDirectionType.Up:
                    return new Grid2D(centerPos.x - size.x / 2, colliderGrid.YMax + 1, centerPos.x + size.x / 2 - 1,
                        colliderGrid.YMax + size.y);
                case EDirectionType.Down:
                    return new Grid2D(centerPos.x - size.x / 2, colliderGrid.YMin - size.y,
                        centerPos.x + size.x / 2 - 1, colliderGrid.YMin - 1);
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
            return TileToWorld(pos) - Vector3.forward * z;
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

        public static Animation GetAnimation(SkeletonAnimation skeleton, string aniName)
        {
            if (skeleton == null)
            {
                return null;
            }

            return skeleton.state.Data.skeletonData.FindAnimation(aniName);
        }

        public static TrackEntry SetAnimation(SkeletonAnimation skeleton, int trackIndex, String animationName,
            bool loop)
        {
            var animation = GetAnimation(skeleton, animationName);
            if (animation == null)
            {
                LogHelper.Error("GetAnimation Failed,{0} | {1}", skeleton, animationName);
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
            int hCount = (int) mapSize.x;
            int vCount = (int) mapSize.y;

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
            var x = Mathf.FloorToInt(pixel.x / (PixelsPerTile * ConstDefineGM2D.ServerTileScale)) *
                    ConstDefineGM2D.ServerTileScale;
            var y = Mathf.FloorToInt(pixel.y / (PixelsPerTile * ConstDefineGM2D.ServerTileScale)) *
                    ConstDefineGM2D.ServerTileScale;
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

        public static IntRect WorldRectToTileRect(Rect rect)
        {
            return new IntRect(WorldToTile(rect.min), WorldToTile(rect.max));
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
            var mapRect2D = new MapRect2D
            {
                XMin = node.Grid.XMin,
                YMin = node.Grid.YMin,
                XMax = node.Grid.XMax,
                YMax = node.Grid.YMax,
                Id = node.Id,
                Rotation = node.Rotation
            };
            if (node.Scale != Vector2.one)
            {
                mapRect2D.Scale = ToProto(node.Scale);
            }

            return mapRect2D;
        }

        private static Vec2Proto ToProto(Vector2 nodeScale)
        {
            return new Vec2Proto
            {
                X = nodeScale.x,
                Y = nodeScale.y
            };
        }

        public static Vector2 ToEngine(Vec2Proto vec2Proto)
        {
            return new Vector2
            {
                x = vec2Proto.X,
                y = vec2Proto.Y
            };
        }

        public static UnitExtraKeyValuePair ToProto(IntVec3 index, UnitExtraDynamic data)
        {
            var res = new UnitExtraKeyValuePair();
            if (data == null)
            {
                res.IsNull = true;
                return res;
            }

            res.Guid = ToProto(index);
            res.MoveDirection = (byte) data.MoveDirection;
            res.Active = data.Active;
            res.ChildId = data.ChildId;
            res.ChildRotation = data.ChildRotation;
            res.RotateMode = data.RotateMode;
            res.RotateValue = data.RotateValue;
            res.TimeDelay = data.TimeDelay;
            res.TimeInterval = data.TimeInterval;
            res.Msg = data.Msg;
            res.JumpAbility = data.JumpAbility;
            res.TeamId = data.TeamId;
            res.Life = data.MaxHp;
            res.AttackPower = data.Damage;
            res.MoveSpeed = data.MaxSpeedX;
            res.EffectRange = data.EffectRange;
            res.CastRange = data.CastRange;
            res.ViewRange = data.ViewRange;
            res.BulletCount = data.BulletCount;
            res.CastSpeed = data.BulletSpeed;
            res.ChargeTime = data.ChargeTime;
            res.InjuredReduce = data.InjuredReduce;
            res.CureIncrease = data.CureIncrease;
            res.MonsterIntervalTime = data.MonsterIntervalTime;
            res.MonsterId = data.MonsterId;
            res.MaxAliveMonster = data.MaxAliveMonster;
            res.MaxCreatedMonster = data.MaxCreatedMonster;
            res.TimerSecond = data.TimerSecond;
            res.TimerMinSecond = data.TimerMinSecond;
            res.TimerMaxSecond = data.TimerMaxSecond;
            res.IsRandom = data.IsRandom;
            res.TimerCirculation = data.TimerCirculation;
            res.SurpriseBoxInterval = data.SurpriseBoxInterval;
            res.SurpriseBoxCountLimit = data.SurpriseBoxCountLimit;
            res.SurpriseBoxMaxCount = data.SurpriseBoxMaxCount;
            res.CommonValue = data.CommonValue;
            res.CycleInterval = data.CycleInterval;
            var drops = data.Drops;
            for (int i = 0, count = drops.Count; i < count; i++)
            {
                ushort val = drops.Get<ushort>(i);
                if (val != 0)
                {
                    res.Drops.Add(val);
                }
            }

            var knockbackForces = data.KnockbackForces;
            for (int i = 0, count = knockbackForces.Count; i < count; i++)
            {
                ushort val = knockbackForces.Get<ushort>(i);
                if (val != 0)
                {
                    res.KnockbackForces.Add(val);
                }
            }

            var addStates = data.AddStates;
            for (int i = 0, count = addStates.Count; i < count; i++)
            {
                ushort val = addStates.Get<ushort>(i);
                if (val != 0)
                {
                    res.AddStates.Add(val);
                }
            }

            var surpriseBoxItems = data.SurpriseBoxItems;
            for (int i = 0, count = surpriseBoxItems.Count; i < count; i++)
            {
                ushort val = surpriseBoxItems.Get<ushort>(i);
                if (val != 0)
                {
                    res.SurpriseBoxItems.Add(val);
                }
            }

            var internalUnitExtras = data.InternalUnitExtras;
            for (int i = 0, count = internalUnitExtras.Count; i < count; i++)
            {
                res.InternalUnitExtras.Add(ToProto(IntVec3.zero, internalUnitExtras.Get<UnitExtraDynamic>(i)));
            }

            //Npc相关数据
            res.NpcType = data.NpcType;
            res.NpcName = data.NpcName;
            res.NpcDialog = data.NpcDialog;
            res.NpcSerialNumber = data.NpcSerialNumber;
            res.NpcShowType = data.NpcShowType;
            res.NpcShowInterval = data.NpcShowInterval;
            var npcTask = data.NpcTask;
            for (int i = 0, count = npcTask.Count; i < count; i++)
            {
                UnitExtraNpcTaskData val = npcTask.Get<NpcTaskDynamic>(i).ToUnitExtraNpcTaskData();
                if (val != null)
                {
                    res.NpcTask.Add(val);
                }
            }

            return res;
        }

        public static UnitExtraDynamic ToEngine(UnitExtraKeyValuePair data, UnitExtraDynamic unitExtra = null)
        {
            if (null == data || data.IsNull)
            {
                return null;
            }

            if (unitExtra == null)
            {
                unitExtra = new UnitExtraDynamic();
            }
            else
            {
                unitExtra.Clear();
            }

            unitExtra.MoveDirection = (EMoveDirection) data.MoveDirection;
            unitExtra.Active = (byte) data.Active;
            unitExtra.ChildId = (ushort) data.ChildId;
            unitExtra.ChildRotation = (byte) data.ChildRotation;
            unitExtra.RotateMode = (byte) data.RotateMode;
            unitExtra.RotateValue = (byte) data.RotateValue;
            unitExtra.TimeDelay = (ushort) data.TimeDelay;
            unitExtra.TimeInterval = (ushort) data.TimeInterval;
            unitExtra.Msg = data.Msg;
            unitExtra.JumpAbility = (ushort) data.JumpAbility;
            unitExtra.TeamId = (byte) data.TeamId;
            unitExtra.MaxHp = (ushort) data.Life;
            unitExtra.Damage = (ushort) data.AttackPower;
            unitExtra.MaxSpeedX = (ushort) data.MoveSpeed;
            for (int i = 0; i < data.Drops.Count; i++)
            {
                unitExtra.Set(data.Drops[i], UnitExtraDynamic.FieldTag.Drops, i);
            }

            unitExtra.EffectRange = (ushort) data.EffectRange;
            unitExtra.CastRange = (ushort) data.CastRange;
            unitExtra.ViewRange = (ushort) data.ViewRange;
            unitExtra.BulletCount = (ushort) data.BulletCount;
            unitExtra.BulletSpeed = (ushort) data.CastSpeed;
            unitExtra.ChargeTime = (ushort) data.ChargeTime;
            unitExtra.InjuredReduce = (byte) data.InjuredReduce;
            unitExtra.CureIncrease = (ushort) data.CureIncrease;
            unitExtra.MonsterIntervalTime = (ushort) data.MonsterIntervalTime;
            unitExtra.MonsterId = (ushort) data.MonsterId;
            unitExtra.MaxAliveMonster = (byte) data.MaxAliveMonster;
            unitExtra.TimerSecond = (byte) data.TimerSecond;
            unitExtra.TimerMinSecond = (byte) data.TimerMinSecond;
            unitExtra.TimerMaxSecond = (byte) data.TimerMaxSecond;
            unitExtra.IsRandom = data.IsRandom;
            unitExtra.TimerCirculation = data.TimerCirculation;
            unitExtra.SurpriseBoxInterval = (byte) data.SurpriseBoxInterval;
            unitExtra.SurpriseBoxCountLimit = data.SurpriseBoxCountLimit;
            unitExtra.SurpriseBoxMaxCount = (byte) data.SurpriseBoxMaxCount;
            unitExtra.MaxCreatedMonster = (ushort) data.MaxCreatedMonster;
            unitExtra.CommonValue = (ushort) data.CommonValue;
            unitExtra.CycleInterval = (byte) data.CycleInterval;
            for (int i = 0; i < data.KnockbackForces.Count; i++)
            {
                unitExtra.Set((ushort) data.KnockbackForces[i], UnitExtraDynamic.FieldTag.KnockbackForces, i);
            }

            for (int i = 0; i < data.AddStates.Count; i++)
            {
                unitExtra.Set((ushort) data.AddStates[i], UnitExtraDynamic.FieldTag.AddStates, i);
            }

            for (int i = 0; i < data.SurpriseBoxItems.Count; i++)
            {
                unitExtra.Set((ushort) data.SurpriseBoxItems[i], UnitExtraDynamic.FieldTag.SurpriseBoxItems, i);
            }

            for (int i = 0; i < TeamManager.MaxTeamCount; i++)
            {
                if (i < data.InternalUnitExtras.Count)
                {
                    unitExtra.Set(ToEngine(data.InternalUnitExtras[i]), UnitExtraDynamic.FieldTag.InternalUnitExtras,
                        i);
                }
                else
                {
                    unitExtra.Set<UnitExtraDynamic>(null, UnitExtraDynamic.FieldTag.InternalUnitExtras, i);
                }
            }

            //Npc相关数据
            unitExtra.NpcType = (byte) data.NpcType;
            unitExtra.NpcName = data.NpcName;
            unitExtra.NpcDialog = data.NpcDialog;
            unitExtra.NpcSerialNumber = (ushort) data.NpcSerialNumber;
            unitExtra.NpcShowType = (byte) data.NpcShowType;
            unitExtra.NpcShowInterval = (ushort) data.NpcShowInterval;
            for (int i = 0; i < data.NpcTask.Count; i++)
            {
                var npctaskDynamic = new NpcTaskDynamic();
                npctaskDynamic.Set(data.NpcTask[i]);
                unitExtra.Set(npctaskDynamic, UnitExtraDynamic.FieldTag.NpcTask, i);
            }

            return unitExtra;
        }

        public static IntVec3Proto ToProto(IntVec3 value)
        {
            var guid = new IntVec3Proto();
            guid.X = value.x;
            guid.Y = value.y;
            guid.Z = value.z;
            return guid;
        }

        public static IntVec2Proto ToProto(IntVec2 value)
        {
            var v2 = new IntVec2Proto();
            v2.X = value.x;
            v2.Y = value.y;
            return v2;
        }

        public static RecAnimData ToProto(ShadowData.AnimRec value)
        {
            var r = new RecAnimData();
            r.FrameIdx = value.FrameIdx;
            r.NameIdx = value.NameIdx;
            r.TimeScale = value.TimeScale;
            r.TrackIdx = value.TrackIdx;
            r.Loop = value.Loop;
            return r;
        }

        public static PairUnitData ToProto(PairUnit pairUnit)
        {
            var data = new PairUnitData();
            data.UnitA = ToProto(pairUnit.UnitA.Guid);
            data.UnitB = ToProto(pairUnit.UnitB.Guid);
            data.UnitAScene = pairUnit.UnitAScene;
            data.UnitBScene = pairUnit.UnitBScene;
            data.Num = pairUnit.Num;
            return data;
        }

        public static IntVec3 ToEngine(IntVec3Proto guid)
        {
            return new IntVec3(guid.X, guid.Y, guid.Z);
        }

        public static IntVec2 ToEngine(IntVec2Proto guid)
        {
            return new IntVec2(guid.X, guid.Y);
        }

        public static ShadowData.AnimRec ToEngine(RecAnimData value)
        {
            var r = new ShadowData.AnimRec();
            r.FrameIdx = value.FrameIdx;
            r.NameIdx = (byte) value.NameIdx;
            r.TimeScale = (byte) value.TimeScale;
            r.TrackIdx = (byte) value.TrackIdx;
            r.Loop = value.Loop;
            return r;
        }

        internal static IntRect ToEngine(IntRectProto intRect)
        {
            return new IntRect(intRect.Min.X, intRect.Min.Y, intRect.Max.X, intRect.Max.Y);
        }

        internal static IntRectProto ToProto(IntRect intRect)
        {
            return new IntRectProto
            {
                Min = new IntVec2Proto {X = intRect.Min.x, Y = intRect.Min.y},
                Max = new IntVec2Proto {X = intRect.Max.x, Y = intRect.Max.y}
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
            IntVec2 size = byData
                ? tableUnit.GetDataSize(node.Rotation, node.Scale)
                : tableUnit.GetColliderSize(node.Rotation, node.Scale);
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
            IntVec2 size = byData
                ? tableUnit.GetDataSize(node.Rotation, node.Scale)
                : tableUnit.GetColliderSize(node.Rotation, node.Scale);
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
            float minDepth, maxDepth;
            EditHelper.GetMinMaxDepth(editorLayer, out minDepth, out maxDepth);
            if (!DataScene2D.PointCast(tile, out targetNode, JoyPhysics2D.LayMaskAll, minDepth, maxDepth))
            {
                return false;
            }

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
                unitDesc.Guid = tableUnit.ColliderToRenderer(new IntVec3(grid.XMin, grid.YMin, colliderNode.Depth),
                    colliderNode.Rotation);
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

            return -res;
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

            if (tableUnit.EGeneratedType == EGeneratedType.Spine && !UnitDefine.IsBullet(tableUnit.Id))
            {
                offsetInWorld.y -= modelSizeInWorld.y * 0.5f;
            }

            return offsetInWorld;
        }

        public static IntRect ToIntRect(Grid2D grid2D)
        {
            return new IntRect(grid2D.XMin, grid2D.YMin, grid2D.XMax, grid2D.YMax);
        }

        public static Grid2D ToGrid2D(IntRect rect)
        {
            return new Grid2D(rect.Min.x, rect.Min.y, rect.Max.x, rect.Max.y);
        }

        public static Vector2 GetDirection(float angle)
        {
            var rad = angle * Mathf.Deg2Rad;
            return new Vector2((float) Math.Sin(rad), (float) Math.Cos(rad));
        }

        public static float GetAngle(int rotation)
        {
            float euler = rotation >= (int) EDirectionType.RightUp ? (rotation - 3) * 90 - 45 : rotation * 90;
            return euler;
        }

        public static bool GetRotation8(int angle, out byte rotation)
        {
            if (angle % 90 == 0)
            {
                rotation = (byte) (angle / 90);
                return true;
            }

            if ((angle + 45) % 90 == 0)
            {
                rotation = (byte) ((angle + 45) / 90 + 3);
                return true;
            }

            rotation = 0;
            return false;
        }

        public static bool GetRotation4(int angle, out byte rotation)
        {
            if (angle % 90 == 0)
            {
                rotation = (byte) (angle / 90);
                return true;
            }

            rotation = 0;
            return false;
        }
    }
}