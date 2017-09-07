/********************************************************************
** Filename : EditHelper
** Author : Dong
** Date : 2017/6/27 星期二 下午 3:11:47
** Summary : EditHelper
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class EditHelper
    {
        /// <summary>
        /// 关卡中只能存在一个的物体的索引
        /// </summary>
        private static readonly Dictionary<int, UnitDesc> _replaceUnits = new Dictionary<int, UnitDesc>();
        private static readonly Dictionary<int, int> _unitIndexCount = new Dictionary<int, int>();
        
        private static readonly Dictionary<int, UnitEditData> _unitDefaultDataDict = new Dictionary<int, UnitEditData>();
        private static readonly IntVec3 DefaultUnitGuid = new IntVec3(-1, -1, -1);

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
            unitDesc = GetUnitDefaultData(unitId).UnitDesc;
            IntVec2 mouseTile = GM2DTools.WorldToTile(mouseWorldPos);
            if (!DataScene2D.Instance.IsInTileMap(mouseTile))
            {
                return false;
            }
            IntVec3 tileIndex = DataScene2D.Instance.GetTileIndex(mouseWorldPos, unitId);
            unitDesc.Guid = tileIndex;
            var tableUnit = UnitManager.Instance.GetTableUnit(unitId);
            if (tableUnit == null)
            {
                return false;
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

        public static IntVec3 GetDefaultUnitGuid()
        {
            return DefaultUnitGuid;
        }

        public static bool TryEditUnitData(UnitDesc unitDesc)
        {
            if (!CheckCanEdit(unitDesc.Id))
            {
                return false;
            }
            UnitEditData unitEditData;
            if (unitDesc.Guid == DefaultUnitGuid)
            {
                unitEditData = GetUnitDefaultData(unitDesc.Id);
            }
            else
            {
                unitEditData = new UnitEditData(unitDesc, DataScene2D.Instance.GetUnitExtra(unitDesc.Guid));
            }
            SocialGUIManager.Instance.OpenUI<UICtrlUnitPropertyEdit>(unitEditData);
            return false;
        }

        public static void CompleteEditUnitData(UnitEditData origin, UnitEditData editData)
        {
            if (origin.UnitDesc.Guid == DefaultUnitGuid)
            {
                UpdateUnitDefaultData(editData);
            }
            else
            {
                EditModeState.Global.Instance.ModifyUnitData(origin.UnitDesc, origin.UnitExtra, editData.UnitDesc,
                    editData.UnitExtra);
            }
        }

        public static bool CheckCanEdit(int id)
        {
            var table = TableManager.Instance.GetUnit(id);
            if (null == table)
            {
                return false;
            }
            for (var type = EEditType.None + 1; type < EEditType.Max; type++)
            {
                if (table.CanEdit(type))
                {
                    return true;
                }
            }
            return false;
        }

        public static void UpdateUnitDefaultData(UnitEditData unitEditData)
        {
            _unitDefaultDataDict.AddOrReplace(unitEditData.UnitDesc.Id, unitEditData);
        }
        
        public static UnitEditData GetUnitDefaultData(int id)
        {
            UnitEditData data;
            if (!_unitDefaultDataDict.TryGetValue(id, out data))
            {
                data = InternalGetUnitDefaultData(id);
                _unitDefaultDataDict.Add(id, data);
            }
            return data;
        }

        private static UnitEditData InternalGetUnitDefaultData(int id)
        {
            UnitEditData unitEditData = new UnitEditData();
            unitEditData.UnitDesc.Guid = DefaultUnitGuid;
            unitEditData.UnitDesc.Id = id;
            unitEditData.UnitDesc.Scale = Vector2.one;
            var table = TableManager.Instance.GetUnit(id);
            if (null == table)
            {
                LogHelper.Error("InternalGetUnitDefaultData TableUnit is null, id: " + id);
                return unitEditData;
            }
            if (table.CanEdit(EEditType.Active))
            {
                unitEditData.UnitExtra.Active = (byte) table.DefaultActiveState;
            }
            if (table.CanEdit(EEditType.Child))
            {
                unitEditData.UnitExtra.ChildId = (ushort) table.ChildState[0];
            }
            if (table.CanEdit(EEditType.Direction))
            {
                unitEditData.UnitDesc.Rotation = (byte) table.DefaultDirection;
            }
            if (table.CanEdit(EEditType.MoveDirection))
            {
                unitEditData.UnitExtra.MoveDirection = table.DefaultMoveDirection;
            }
            if (table.CanEdit(EEditType.Rotate))
            {
                unitEditData.UnitExtra.RotateMode = (byte) table.DefaultRotateMode;
                unitEditData.UnitExtra.RotateValue = (byte) table.DefaultRotateEnd;
            }
            if (table.CanEdit(EEditType.Time))
            {
                unitEditData.UnitExtra.TimeDelay = (ushort) table.TimeState[0];
                unitEditData.UnitExtra.TimeInterval = (ushort) table.TimeState[1];
            }
        
            if (UnitDefine.IsMonster(table.Id))
            {
                unitEditData.UnitExtra.MoveDirection = (EMoveDirection) (unitEditData.UnitDesc.Rotation + 1);
                unitEditData.UnitDesc.Rotation = 0;
            }
            return unitEditData;
        }

        public static bool CheckMask(byte rotation,int mask)
        {
            return (mask & (byte)(1 << rotation)) != 0;
        }
        
        public static void Clear()
        {
            _replaceUnits.Clear();
            _unitIndexCount.Clear();
            _unitDefaultDataDict.Clear();
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
