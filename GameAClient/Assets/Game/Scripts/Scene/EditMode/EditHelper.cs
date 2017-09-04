/********************************************************************
** Filename : EditHelper
** Author : Dong
** Date : 2017/6/27 星期二 下午 3:11:47
** Summary : EditHelper
***********************************************************************/

using System;
using System.Collections.Generic;
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
        private static readonly Dictionary<int, UnitDesc> _replaceUnits = new Dictionary<int, UnitDesc>();
        private static readonly Dictionary<int, int> _unitIndexCount = new Dictionary<int, int>();
        private static readonly List<byte> _directionList = new List<byte>
        {
            (byte)EDirectionType.Up,
            (byte)EDirectionType.Right,
            (byte)EDirectionType.Down,
            (byte)EDirectionType.Left
        };
        
        private static readonly ushort[] _weaponTypes = {101, 102, 103, 201, 202, 203};
        private static readonly ushort[] _weaponJetTypes = {101, 102, 103};
        
        /// <summary>
        /// 每个物体的初始旋转/移动方向，编辑状态下点击物品栏里的物体可以改变初始旋转/移动方向
        /// </summary>
        private static readonly Dictionary<int, int> _unitOrigDirOrRot = new Dictionary<int, int>();

        public static Dictionary<int, int> UnitIndexCount
        {
            get { return _unitIndexCount; }
        }

        public static void GetMinMaxDepth(EEditorLayer editorLayer, out float minDepth, out float maxDepth)
        {
            minDepth = float.MinValue;
            maxDepth = float.MaxValue;
            switch (editorLayer)
            {
                case EEditorLayer.Normal:
                    minDepth = (int) EUnitDepth.Earth;
                    maxDepth = (int) EUnitDepth.Dynamic;
                    break;
                case EEditorLayer.Effect:
                    minDepth = (int) EUnitDepth.Effect;
                    maxDepth = (int) EUnitDepth.Effect;
                    break;
            }
        }
        
        public static bool TryGetUnitDesc(Vector2 mouseWorldPos, EEditorLayer editorLayer, out UnitDesc unitDesc)
        {
            unitDesc = new UnitDesc();
            IntVec2 mouseTile = GM2DTools.WorldToTile(mouseWorldPos);
            if (!DataScene2D.Instance.IsInTileMap(mouseTile))
            {
                return false;
            }
            if (!GM2DTools.TryGetUnitObject(mouseWorldPos, editorLayer, out unitDesc))
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
            if (tableUnit.CanEdit(EEditType.Direction))
            {
                unitDesc.Rotation = (byte)GetUnitOrigDirOrRot(tableUnit);
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
            if (table.CanEdit(EEditType.MoveDirection))
            {
                return table.MoveDirection;
            }
            if (table.CanEdit(EEditType.Direction))
            {
                return table.Direction - 1;
            }
            return 0;
        }

        public static void ChangeUnitOrigDirOrRot(Table_Unit table)
        {
            int current;
            if (!_unitOrigDirOrRot.TryGetValue(table.Id, out current))
            {
                if (table.MoveDirection != -1)
                {
                    _unitOrigDirOrRot[table.Id] = table.MoveDirection;
                }
                else if (table.Direction != -1)
                {
                    _unitOrigDirOrRot[table.Id] = table.Direction;
                }
                else
                {
                    return;
                }
                current = _unitOrigDirOrRot[table.Id];
            }
            byte newDir;
            if (table.MoveDirection != -1)
            {
                if (CalculateNextDir((byte) (current - 1), 15, out newDir))
                {
                    _unitOrigDirOrRot[table.Id] = newDir + 1;
                }
            }
            else if (table.Direction != -1)
            {
                if (CalculateNextDir((byte) (current), table.DirectionMask, out newDir))
                {
                    _unitOrigDirOrRot[table.Id] = newDir;
                }
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
            return true;
        }

        public static bool CheckMask(byte rotation,int mask)
        {
            return (mask & (byte)(1 << rotation)) != 0;
        }
        
        public static void Clear()
        {
            _replaceUnits.Clear();
            _unitIndexCount.Clear();
        }

        public static bool CheckCanAdd(UnitDesc unitDesc, out Table_Unit tableUnit)
        {
            tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("CheckCanAdd failed,{0}", unitDesc.ToString());
                return false;
            }
            //怪物同屏数量不可过多
            {
                if (tableUnit.EUnitType == EUnitType.Monster || UnitDefine.BoxId == tableUnit.Id)
                {
                    IntVec2 size = new IntVec2(15, 10) * ConstDefineGM2D.ServerTileScale;
                    IntVec2 mapSize = ConstDefineGM2D.MapTileSize;
                    var min = new IntVec2(unitDesc.Guid.x / size.x * size.x, unitDesc.Guid.y / size.y * size.y);
                    var grid = new Grid2D(min.x, min.y, Mathf.Min(mapSize.x, min.x + size.x - 1), Mathf.Min(mapSize.y, min.y + size.y - 1));
                    var units = DataScene2D.GridCastAllReturnUnits(grid, EnvManager.MonsterLayer);
                    if (units.Count >= ConstDefineGM2D.MaxPhysicsUnitCount)
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "同屏不能放置太多的动态物体喔~");
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
                int count;
                _unitIndexCount.TryGetValue(unitDesc.Id, out count);
                if (tableUnit.Count > 0 && count >= tableUnit.Count)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, string.Format("不可放置，{0}最多可放置{1}个~", tableUnit.Name, count));
                    return false;
                }
                if (count >= LocalUser.Instance.UserWorkshopUnitData.GetUnitLimt(unitDesc.Id))
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, string.Format("不可放置，目前剩余{0}个", count));
                    return false;
                }
            }
            return true;
        }

        public static void BeforeAddUnit(Table_Unit tableUnit)
        {
            //只有一个的自动删除
            if (tableUnit.Count == 1)
            {
                UnitDesc desc;
                if (_replaceUnits.TryGetValue(tableUnit.Id, out desc))
                {
                    if (desc.Id != 0)
                    {
                        EditMode.Instance.DeleteUnitWithCheck(desc);
                    }
                }
            }
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
            EditMode.Instance.MapStatistics.AddOrDeleteUnit(tableUnit, true, isInit);
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
            EditMode.Instance.MapStatistics.AddOrDeleteUnit(tableUnit, false);
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
                    DataScene2D.Instance.ProcessUnitExtra(unitDesc, unitExtra);
                }
            }
            else if (UnitDefine.IsWeaponPool(tableUnit.Id))
            {
                UnitExtra unitExtra;
                if (!DataScene2D.Instance.TryGetUnitExtra(unitDesc.Guid, out unitExtra))
                {
                    unitExtra.UnitValue = _weaponTypes[0];
                    DataScene2D.Instance.ProcessUnitExtra(unitDesc, unitExtra);
                }
            }
            else if (UnitDefine.IsJet(tableUnit.Id))
            {
                UnitExtra unitExtra;
                if (!DataScene2D.Instance.TryGetUnitExtra(unitDesc.Guid, out unitExtra))
                {
                    unitExtra.UnitValue = _weaponJetTypes[0];
                    DataScene2D.Instance.ProcessUnitExtra(unitDesc, unitExtra);
                }
            }
        }

        private struct ProcessClickUnitOperationParam
        {
            public UnitDesc UnitDesc;
            public Table_Unit TableUnit;
            public UnitExtra UnitExtra;
        }

        public static bool ProcessClickUnitOperation(UnitDesc unitDesc)
        {
            var context = new ProcessClickUnitOperationParam();
            context.UnitDesc = unitDesc;
            context.TableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            context.UnitExtra = DataScene2D.Instance.GetUnitExtra(unitDesc.Guid);
            if (context.TableUnit.CanEdit(EEditType.MoveDirection))
            {
                if (context.UnitExtra.MoveDirection != 0)
                {
                    return DoMove(ref context);
                }
            }
            if (UnitDefine.IsWeaponPool(context.TableUnit.Id))
            {
                return DoWeapon(ref context);
            }
            if (UnitDefine.IsJet(context.TableUnit.Id))
            {
                return DoJet(ref context);
            }
            if (context.TableUnit.CanEdit(EEditType.Direction))
            {
                return DoRotate(ref context);
            }
            if (UnitDefine.IsHasText(context.UnitDesc.Id))
            {
                return DoAddMsg(ref context);
            }
            if (context.UnitDesc.Id == UnitDefine.RollerId)
            {
                return DoRoller(ref context);
            }
            if (UnitDefine.IsEarth(context.UnitDesc.Id))
            {
                return DoEarth(ref context);
            }
            return false;
        }
        
        private static bool DoWeapon(ref ProcessClickUnitOperationParam processClickUnitOperationParam)
        {
            FindNextWeapon(ref processClickUnitOperationParam.UnitExtra.UnitValue, _weaponTypes);
            DataScene2D.Instance.ProcessUnitExtra(processClickUnitOperationParam.UnitDesc, processClickUnitOperationParam.UnitExtra);
            return true;
        }

        private static bool DoJet(ref ProcessClickUnitOperationParam processClickUnitOperationParam)
        {
            FindNextWeapon(ref processClickUnitOperationParam.UnitExtra.UnitValue, _weaponJetTypes);
            DataScene2D.Instance.ProcessUnitExtra(processClickUnitOperationParam.UnitDesc, processClickUnitOperationParam.UnitExtra);
            return true;
        }

        private static bool DoAddMsg(ref ProcessClickUnitOperationParam processClickUnitOperationParam)
        {
            SocialGUIManager.Instance.OpenUI<UICtrlGameItemAddMessage>(processClickUnitOperationParam.UnitDesc);
            return false;
        }

        private static bool DoRotate(ref ProcessClickUnitOperationParam processClickUnitOperationParam)
        {
            byte dir;
            if (!CalculateNextDir(processClickUnitOperationParam.UnitDesc.Rotation,
                processClickUnitOperationParam.TableUnit.DirectionMask, out dir))
            {
                return false;
            }
            processClickUnitOperationParam.UnitDesc.Rotation = dir;
            DataScene2D.Instance.ProcessUnitExtra(processClickUnitOperationParam.UnitDesc,
                processClickUnitOperationParam.UnitExtra);
            return false;
        }

        private static bool DoRoller(ref ProcessClickUnitOperationParam processClickUnitOperationParam)
        {
            byte dir;
            if (!CalculateNextDir((byte) (processClickUnitOperationParam.UnitExtra.RollerDirection - 1), 10,
                out dir))
            {
                return false;
            }
            processClickUnitOperationParam.UnitExtra.RollerDirection = (EMoveDirection) (dir + 1);
            DataScene2D.Instance.ProcessUnitExtra(processClickUnitOperationParam.UnitDesc,
                processClickUnitOperationParam.UnitExtra);
            return true;
        }

        private static bool DoMove(ref ProcessClickUnitOperationParam processClickUnitOperationParam)
        {
            byte dir;
            if (!CalculateNextDir((byte) (processClickUnitOperationParam.UnitExtra.MoveDirection - 1),15, out dir))
            {
                return false;
            }
            processClickUnitOperationParam.UnitExtra.MoveDirection = (EMoveDirection) (dir + 1);
            DataScene2D.Instance.ProcessUnitExtra(processClickUnitOperationParam.UnitDesc,
                processClickUnitOperationParam.UnitExtra);
            return true;
        }

        private static bool DoEarth(ref ProcessClickUnitOperationParam processClickUnitOperationParam)
        {
            processClickUnitOperationParam.UnitExtra.UnitValue++;
            if (processClickUnitOperationParam.UnitExtra.UnitValue > 2)
            {
                processClickUnitOperationParam.UnitExtra.UnitValue = 0;
            }
            DataScene2D.Instance.ProcessUnitExtra(processClickUnitOperationParam.UnitDesc,
                processClickUnitOperationParam.UnitExtra);
            return true;
        }
        
        private static void FindNextWeapon(ref ushort id, ushort[] weapons)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] == id)
                {
                    if (i + 1 < weapons.Length)
                    {
                        id = weapons[i + 1];
                        return;
                    }
                    id = weapons[0];
                    return;
                }
            }
            LogHelper.Error("FindNextWeapon Failed, {0}", id);
        }
        
        public static GameObject CreateDragRoot(Vector3 pos, int unitId, EDirectionType rotate, out UnitBase unitBase)
        {
            Table_Unit tableUnit = TableManager.Instance.GetUnit(unitId);

            unitBase = UnitManager.Instance.GetUnit(tableUnit, rotate);
            CollectionBase collectUnit = unitBase as CollectionBase;
            if (null != collectUnit)
            {
                collectUnit.StopTwenner();
            }
            var helperParentObj = new GameObject("DragHelperParent");
            var tran = helperParentObj.transform;
            pos.z = -50;
            tran.position = pos;
            unitBase.Trans.parent = tran;
            unitBase.Trans.localPosition = GM2DTools.GetUnitDragingOffset(unitId);
            unitBase.Trans.localScale = Vector3.one;
            return helperParentObj;
        }
    }
}
