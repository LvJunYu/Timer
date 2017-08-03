/********************************************************************
** Filename : EditHelper
** Author : Dong
** Date : 2017/6/27 星期二 下午 3:11:47
** Summary : EditHelper
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using SoyEngine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameA.Game
{
    public class EditHelper
    {
        /// <summary>
        /// 关卡中只能存在一个的物体的索引
        /// </summary>
        private static Dictionary<int, UnitDesc> _replaceUnits = new Dictionary<int, UnitDesc>();
        private static Dictionary<int, int> _unitIndexCount = new Dictionary<int, int>();
        private static List<UnitDesc> _cacheUnitDescs = new List<UnitDesc>();
        private static List<byte> _directionList = new List<byte>()
        {
            (byte)EDirectionType.Up,
            (byte)EDirectionType.Right,
            (byte)EDirectionType.Down,
            (byte)EDirectionType.Left,
        };
        
        /// <summary>
        /// 每个物体的初始旋转/移动方向，编辑状态下点击物品栏里的物体可以改变初始旋转/移动方向
        /// </summary>
        private static Dictionary<int, int> _unitOrigDirOrRot = new Dictionary<int, int>();

        public static Dictionary<int, int> UnitIndexCount
        {
            get { return _unitIndexCount; }
        }
        
        
        public static bool TryGetUnitDesc(Vector2 mouseWorldPos, out UnitDesc unitDesc)
        {
            if (!GM2DTools.TryGetUnitObject(mouseWorldPos, EEditorLayer.None, out unitDesc))
            {
                return false;
            }
            return true;
        }
        
        public static bool TryGetCreateKey(Vector2 mouseWorldPos, int unitId, out UnitDesc unitDesc)
        {
            unitDesc = new UnitDesc();
            IntVec2 mouseTile = GM2DTools.WorldToTile(mouseWorldPos);
            if (!DataScene2D.Instance.IsInTileMap(mouseTile))
            {
                return false;
            }
            IntVec3 tileIndex = DataScene2D.Instance.GetTileIndex(mouseWorldPos, unitId);
            unitDesc.Id = (ushort)unitId;
            unitDesc.Guid = tileIndex;
            var tableUnit = UnitManager.Instance.GetTableUnit(unitId);
            if (tableUnit == null)
            {
                return false;
            }
            if (tableUnit.CanRotate)
            {
                unitDesc.Rotation = (byte)EditHelper.GetUnitOrigDirOrRot(tableUnit);
            }
            unitDesc.Scale = Vector2.one;
            return true;
        }
        

        public static int GetUnitCnt(int unitId)
        {
            int cnt;
            if (_unitIndexCount.TryGetValue(unitId, out cnt))
            {
                return cnt;
            }
            return 0;
        }

        public static int GetUnitOrigDirOrRot(Table_Unit table)
        {
            int origDirOrRot;
            if (_unitOrigDirOrRot.TryGetValue(table.Id, out origDirOrRot))
            {
                return origDirOrRot;
            }
            if (table.Id == UnitDefine.RollerId)
            {
                return (int)EMoveDirection.Right;
            }
            if (table.CanMove)
            {
                return table.OriginMoveDirection;
            }
            if (table.CanRotate)
            {
                return (int)EDirectionType.Right;
            }
            return 0;
        }

        public static void ChangeUnitOrigDirOrRot(Table_Unit table)
        {
            int current;
            if (!_unitOrigDirOrRot.TryGetValue(table.Id, out current))
            {
                if (table.CanMove)
                {
                    _unitOrigDirOrRot[table.Id] = table.OriginMoveDirection;
                }
                else if (table.CanRotate)
                {
                    _unitOrigDirOrRot[table.Id] = (int)EDirectionType.Right;
                }
                else if (table.Id == UnitDefine.RollerId)
                {
                    _unitOrigDirOrRot[table.Id] = (int)EMoveDirection.Right;
                }
                else
                {
                    return;
                }
                current = _unitOrigDirOrRot[table.Id];
            }
            byte newDir;
            if (table.CanMove)
            {
                if (CalculateNextDir((byte) (current - 1), table.MoveDirectionMask, out newDir))
                {
                    _unitOrigDirOrRot[table.Id] = newDir + 1;
                }
                return;
            }
            if (table.CanRotate)
            {
                if (CalculateNextDir((byte) (current), table.RotationMask, out newDir))
                {
                    _unitOrigDirOrRot[table.Id] = newDir;
                }
                return;
            }
            if (table.Id == UnitDefine.RollerId)
            {
                if (CalculateNextDir((byte) (current - 1), 10, out newDir))
                {
                    _unitOrigDirOrRot[table.Id] = newDir + 1;
                }
                return;
            }
        }
        
        public static bool CalculateNextDir(byte curValue, int mask, out byte dir)
        {
            if (!CheckDirectionValid(curValue))
            {
                dir = 0;
                return false;
            }
            dir = 0;
            int index = 0;
            bool hasFind = false;
            bool res = false;
            while (index < 8)
            {
                dir = GetRepeatDirByIndex(index);
                if (hasFind)
                {
                    if (dir == curValue)
                    {
                        break;
                    }
                    if (CheckMask(dir, mask))
                    {
                        res = true;
                        break;
                    }
                }
                else
                {
                    if (dir == curValue)
                    {
                        hasFind = true;
                    }
                }
                index++;
            }
            return res;
        }
        
        protected static bool CheckDirectionValid(byte value)
        {
            return value == (byte)EDirectionType.Up ||
                   value == (byte)EDirectionType.Right ||
                   value == (byte)EDirectionType.Down ||
                   value == (byte)EDirectionType.Left;
        }
        
        protected static byte GetRepeatDirByIndex(int index)
        {
            int realIndex = index % 4;
            return _directionList[realIndex];
        }
        
        public static bool CheckCanAddChild(Table_Unit child, UnitDesc parent)
        {
            if (child == null || parent == UnitDesc.zero)
            {
                return false;
            }
            Table_Unit tableParent = UnitManager.Instance.GetTableUnit(parent.Id);
            if (tableParent == null)
            {
                return false;
            }
            // check if parent and child in same group
            if ((child.ChildType & tableParent.ParentType) == 0)
            {
                return false;
            }
            return true;
        }

        public static bool CheckCanBindMagic(Table_Unit child, UnitDesc parent)
        {
            if (child == null || parent == UnitDesc.zero)
            {
                return false;
            }
            Table_Unit tableParent = UnitManager.Instance.GetTableUnit(parent.Id);
            if (tableParent == null)
            {
                return false;
            }
            if (child.Id == UnitDefine.BlueStoneId && tableParent.OriginMagicDirection != 0)
            {
                return true;
            }
            return false;
        }
        
        public static bool CheckMask(byte rotation,int mask)
        {
            return (mask & (byte)(1 << rotation)) != 0;
        }
        
        public static void Clear()
        {
            _replaceUnits.Clear();
            _unitIndexCount.Clear();
            _cacheUnitDescs.Clear();
        }

        public static bool CheckCanAdd(UnitDesc unitDesc, out Table_Unit tableUnit)
        {
            tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("CheckCanAdd failed,{0}", unitDesc.ToString());
                return false;
            }
                
            //不可超出地图范围
            {
                var dataGrid = tableUnit.GetDataGrid(unitDesc.Guid.x, unitDesc.Guid.y, unitDesc.Rotation, unitDesc.Scale);
                var validMapRect = DataScene2D.Instance.ValidMapRect;
                var mapGrid = new Grid2D(validMapRect.Min.x, validMapRect.Min.y, validMapRect.Max.x, validMapRect.Max.y);
                if (!dataGrid.Intersects(mapGrid))
                {
                    return false;
                }
            }
            //怪物同屏数量不可过多
            {
                if (tableUnit.EUnitType == EUnitType.Monster)
                {
                    IntVec2 size = new IntVec2(15, 10) * ConstDefineGM2D.ServerTileScale;
                    IntVec2 mapSize = ConstDefineGM2D.MapTileSize;
                    var min = new IntVec2(unitDesc.Guid.x / size.x * size.x, unitDesc.Guid.y / size.y * size.y);
                    var grid = new Grid2D(min.x, min.y, Mathf.Min(mapSize.x, min.x + size.x - 1), Mathf.Min(mapSize.y, min.y + size.y - 1));
                    var units = DataScene2D.GridCastAllReturnUnits(grid, EnvManager.MonsterLayer);
                    if (units.Count >= ConstDefineGM2D.MaxPhysicsUnitCount)
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "同屏不能放置太多的怪物喔~");
                        return false;
                    }
                }
            }
            //成对机关个数不能超过
            {
                if (tableUnit.EPairType > 0)
                {
                    PairUnit pairUnit;
                    if (!PairUnitManager.Instance.TryGetNotFullPairUnit(tableUnit.EPairType, out pairUnit))
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, string.Format("超过{0}的最大数量，不可放置喔~", tableUnit.Name));
                        return false;
                    }
                }
            }
            //花草树只能放在泥土上。
            {
                if (UnitDefine.IsPlant(tableUnit.Id))
                {
                    var downGuid = unitDesc.GetDownPos((int)EUnitDepth.Earth);
                    UnitBase downUnit;
                    if (!TryGetUnit(downGuid, out downUnit) || !UnitDefine.IsEarth(downUnit.Id))
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, string.Format("{0}只可种植在泥土上~", tableUnit.Name));
                        return false;
                    }
                }
            }
            //数量不能超过限额
            {
                int count = 0;
                _unitIndexCount.TryGetValue(unitDesc.Id, out count);
                if (tableUnit.Count > 0 && count >= tableUnit.Count)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, string.Format("不可放置，{0}最多可放置{1}个~", tableUnit.Name, count));
                    return false;
                }
//                if (count >= LocalUser.Instance.UserWorkshopUnitData.GetUnitLimt(unitDesc.Id))
//                {
//                    Messenger<string>.Broadcast(EMessengerType.GameLog, string.Format("不可放置，目前上限{0}个", count));
//                    return false;
//                }
            }
            return true;
        }

        public static List<UnitDesc> BeforeAddUnit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            _cacheUnitDescs.Clear();
            _cacheUnitDescs.Add(unitDesc);
            //只有一个的自动删除
            if (tableUnit.Count == 1)
            {
                UnitDesc desc;
                if (_replaceUnits.TryGetValue(unitDesc.Id, out desc))
                {
                    if (desc.Id != 0)
                    {
                        EditMode2.Instance.DeleteUnit(desc);
                    }
                }
            }
            //地块上的树草自动生成
            if (UnitDefine.IsEarth(tableUnit.Id))
            {
                int random = Random.Range(0, 6);
                if (random == 0)
                {
                    IntVec3 up = unitDesc.GetUpPos((int) EUnitDepth.Earth);
                    if (IsEmpty(up))
                    {
                        _cacheUnitDescs.Add(new UnitDesc(UnitDefine.GetRandomPlantId(Random.Range(0, 3)), up, 0, Vector2.one));
                    }
                }
            }
            return _cacheUnitDescs;
        }

        public static void AfterAddUnit(UnitDesc unitDesc, Table_Unit tableUnit, bool isInit = false)
        {
            if (tableUnit.Count == 1)
            {
                _replaceUnits.Add(unitDesc.Id, unitDesc);
            }
//            if (tableUnit.Count > 1)
            {
                if (!_unitIndexCount.ContainsKey(unitDesc.Id))
                {
                    _unitIndexCount.Add(unitDesc.Id, new int());
                }
                _unitIndexCount[unitDesc.Id] += 1;
                Messenger<int>.Broadcast(EMessengerType.OnUnitAddedInEditMode, unitDesc.Id);
            }
            EditMode2.Instance.MapStatistics.AddOrDeleteUnit(tableUnit, true, isInit);
        }

        public static List<UnitDesc> BeforeDeleteUnit(UnitDesc unitDesc)
        {
            _cacheUnitDescs.Clear();
            _cacheUnitDescs.Add(unitDesc);
            //地块上的植被自动删除
            if (UnitDefine.IsEarth(unitDesc.Id))
            {
                var up = unitDesc.GetUpPos((int) EUnitDepth.Earth);
                UnitBase unit;
                if (TryGetUnit(up, out unit) && UnitDefine.IsPlant(unit.Id))
                {
                    _cacheUnitDescs.Add(new UnitDesc(unit.Id, up, 0, Vector3.one));
                }
            }
            return _cacheUnitDescs;
        }

        public static void AfterDeleteUnit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            if (_replaceUnits.ContainsKey(unitDesc.Id) && _replaceUnits[unitDesc.Id] == unitDesc)
            {
                _replaceUnits.Remove(unitDesc.Id);
            }
//            if (tableUnit.Count > 1)
            {
                if (_unitIndexCount.ContainsKey(unitDesc.Id))
                {
                    _unitIndexCount[unitDesc.Id] -= 1;
                }
                Messenger<int>.Broadcast(EMessengerType.OnUnitAddedInEditMode, unitDesc.Id);
            }
            EditMode2.Instance.MapStatistics.AddOrDeleteUnit(tableUnit, false);
        }

        public static bool TryGetReplaceUnit(int id, out UnitDesc outUnitDesc)
        {
            if (!_replaceUnits.TryGetValue(id, out outUnitDesc))
            {
                return false;
            }
            return outUnitDesc.Id != 0;
        }

        public static bool TryGetUnit(IntVec3 pos, out UnitBase unit)
        {
            return ColliderScene2D.Instance.TryGetUnit(pos, out unit);
        }

        public static bool IsEmpty(IntVec3 pos)
        {
            UnitBase unit;
            return !ColliderScene2D.Instance.TryGetUnit(pos, out unit);
        }
        
        public static void InitUnitExtraEdit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
//            if (tableUnit.CanMove)
//            {
//                UnitExtra unitExtra;
//                if (!TryGetUnitExtra(unitDesc.Guid, out unitExtra))
//                {
//                    unitExtra.MoveDirection = (EMoveDirection)tableUnit.OriginMoveDirection;
//                    ProcessUnitExtra(unitDesc.Guid, unitExtra);
//                }
//            }
//            if (tableUnit.Id == UnitDefine.RollerId)
//            {
//                UnitExtra unitExtra;
//                if (!TryGetUnitExtra(unitDesc.Guid, out unitExtra))
//                {
//                    unitExtra.RollerDirection = EMoveDirection.Right;
//                    ProcessUnitExtra(unitDesc.Guid, unitExtra);
//                }
//            }
            if (UnitDefine.IsEarth(tableUnit.Id))
            {
                UnitExtra unitExtra;
                if (!DataScene2D.Instance.TryGetUnitExtra(unitDesc.Guid, out unitExtra))
                {
                    unitExtra.UnitValue = (byte) Random.Range(1, 3);
                    DataScene2D.Instance.ProcessUnitExtra(unitDesc.Guid, unitExtra);
                }
            }
            else if (UnitDefine.IsWeaponPool(tableUnit.Id))
            {
                UnitExtra unitExtra;
                if (!DataScene2D.Instance.TryGetUnitExtra(unitDesc.Guid, out unitExtra))
                {
                    unitExtra.UnitValue = 2;
                    DataScene2D.Instance.ProcessUnitExtra(unitDesc.Guid, unitExtra);
                }
            }
            else if (UnitDefine.IsJet(tableUnit.Id))
            {
                UnitExtra unitExtra;
                if (!DataScene2D.Instance.TryGetUnitExtra(unitDesc.Guid, out unitExtra))
                {
                    unitExtra.UnitValue = 1;
                    DataScene2D.Instance.ProcessUnitExtra(unitDesc.Guid, unitExtra);
                }
            }
        }
    }
}
