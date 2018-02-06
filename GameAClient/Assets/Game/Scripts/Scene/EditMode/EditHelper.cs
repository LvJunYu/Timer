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
        private static readonly Dictionary<int, SceneUnitDesc> _replaceUnits = new Dictionary<int, SceneUnitDesc>();

        private static readonly Dictionary<int, int> _unitIndexCount = new Dictionary<int, int>();

        private static readonly Dictionary<int, UnitEditData>
            _unitDefaultDataDict = new Dictionary<int, UnitEditData>();

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
            if (!DataScene2D.CurScene.IsInTileMap(mouseTile))
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
            if (!DataScene2D.CurScene.IsInTileMap(mouseTile))
            {
                return false;
            }

            IntVec3 tileIndex = DataScene2D.CurScene.GetTileIndex(mouseWorldPos, unitId);
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
            if (!UnitDefine.IsEarth(unitDesc.Id) && !CheckCanEdit(unitDesc.Id))
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
                unitEditData = new UnitEditData(unitDesc, DataScene2D.CurScene.GetUnitExtra(unitDesc.Guid));
            }

            //地块特殊处理一下
            if (UnitDefine.IsEarth(unitDesc.Id))
            {
                var origin = unitEditData;
                unitEditData.UnitExtra = origin.UnitExtra.Clone();
                unitEditData.UnitExtra.ChildId = (ushort) (Mathf.Clamp(unitEditData.UnitExtra.ChildId, 1, 2) % 2 + 1);
                EditModeState.Global.Instance.ModifyUnitData(origin.UnitDesc, origin.UnitExtra, unitEditData.UnitDesc,
                    unitEditData.UnitExtra);
            }
            else
            {
                SocialGUIManager.Instance.OpenUI<UICtrlUnitPropertyEdit>(unitEditData);
            }

            return false;
        }

        public static void CompleteEditUnitData(UnitEditData origin, UnitEditData editData)
        {
            if (origin.UnitDesc.Guid == DefaultUnitGuid)
            {
                UpdateUnitDefaultData(editData);
                //复活点阵营改变
                if (UnitDefine.IsSpawn(origin.UnitDesc.Id) && editData.UnitExtra.TeamId != origin.UnitExtra.TeamId)
                {
                    Messenger.Broadcast(EMessengerType.OnTeamChanged);
                }

                Messenger<int>.Broadcast(EMessengerType.OnEditUnitDefaultDataChange, origin.UnitDesc.Id);
            }
            else
            {
                EditModeState.Global.Instance.ModifyUnitData(origin.UnitDesc, origin.UnitExtra, editData.UnitDesc,
                    editData.UnitExtra);
                Messenger<IntVec3>.Broadcast(EMessengerType.OnEditUnitDataChange, origin.UnitDesc.Guid);
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
                if (type != EEditType.Style && table.CanEdit(type))
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
            UnitEditData defaultData;
            if (!_unitDefaultDataDict.TryGetValue(id, out defaultData))
            {
                defaultData = InternalGetUnitDefaultData(id);
                _unitDefaultDataDict.Add(id, defaultData);
            }
            UnitEditData data = new UnitEditData();
            data.UnitDesc = defaultData.UnitDesc;
            data.UnitExtra = defaultData.UnitExtra.Clone();
            //第一个出生点特殊处理
            if (UnitDefine.IsSpawn(id) &&
                Scene2DManager.Instance.GetDataScene2D(Scene2DManager.Instance.SqawnSceneIndex).SpawnDatas.Count == 0)
            {
                if (data.UnitExtra.InternalUnitExtras.Count == 0)
                {
                    data.UnitExtra.InternalUnitExtras.Add(UnitExtraDynamic.GetDefaultPlayerValue());
                }
            }
            //地块特殊处理
            if (UnitDefine.IsEarth(id))
            {
                data.UnitExtra.ChildId = (ushort) Random.Range(1, 3);
            }

            return data;
        }

        private static UnitEditData InternalGetUnitDefaultData(int id)
        {
            UnitEditData unitEditData = new UnitEditData();
            unitEditData.UnitDesc.Guid = DefaultUnitGuid;
            unitEditData.UnitDesc.Id = id;
            unitEditData.UnitDesc.Scale = Vector2.one;
            unitEditData.UnitExtra = new UnitExtraDynamic();
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
                unitEditData.UnitExtra.UpdateDefaultValueFromChildId();
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

            if (table.CanEdit(EEditType.TimeDelay))
            {
                unitEditData.UnitExtra.TimeDelay = (ushort) table.TimeState[0];
                unitEditData.UnitExtra.TimeInterval = (ushort) table.TimeState[1];
            }

            if (UnitDefine.IsMonster(table.Id))
//                || UnitDefine.IsNpc(table.Id))
            {
                unitEditData.UnitExtra.MoveDirection = (EMoveDirection) (unitEditData.UnitDesc.Rotation + 1);
                unitEditData.UnitDesc.Rotation = 0;
            }

            if (UnitDefine.IsMonster(table.Id) || UnitDefine.IsNpc(table.Id))
            {
                unitEditData.UnitExtra.MaxHp = (ushort) table.Hp;
                unitEditData.UnitExtra.MaxSpeedX = (ushort) table.MaxSpeed;
            }

            if (table.SkillId > 0)
            {
                var skill = TableManager.Instance.GetSkill(table.SkillId);
                if (skill.EffectValues != null && skill.EffectValues.Length > 0)
                {
                    unitEditData.UnitExtra.EffectRange = (ushort) skill.EffectValues[0];
                }
                unitEditData.UnitExtra.CastRange = (ushort) skill.CastRange;
                unitEditData.UnitExtra.TimeInterval = (ushort) skill.CDTime;
                unitEditData.UnitExtra.Damage = (ushort) skill.Damage;
                for (int i = 0; i < skill.KnockbackForces.Length; i++)
                {
                    unitEditData.UnitExtra.Set((ushort) skill.KnockbackForces[i],
                        UnitExtraDynamic.FieldTag.KnockbackForces, i);
                }
                for (int i = 0; i < skill.AddStates.Length; i++)
                {
                    unitEditData.UnitExtra.Set((ushort) skill.AddStates[i], UnitExtraDynamic.FieldTag.AddStates, i);
                }
            }

            if (table.CanEdit(EEditType.Spawn))
            {
//                UnitExtraDynamic.GetDefaultPlayerValue(unitEditData.UnitExtra);
//                if (Scene2DManager.Instance.GetDataScene2D(Scene2DManager.Instance.SqawnSceneIndex).SpawnDatas.Count ==
//                    0)
//                {
//                    unitEditData.UnitExtra.InternalUnitExtras.Add(UnitExtraDynamic.GetDefaultPlayerValue());
//                }
            }

            if (table.CanEdit(EEditType.MonsterCave))
            {
                unitEditData.UnitExtra.MonsterId = (ushort) UnitDefine.MonstersInCave[0];
                unitEditData.UnitExtra.UpdateFromMonsterId();
                unitEditData.UnitExtra.MonsterIntervalTime = 1000;
                unitEditData.UnitExtra.MaxCreatedMonster = 100;
                unitEditData.UnitExtra.MaxAliveMonster = 5;
            }

            return unitEditData;
        }

        public static bool CheckMask(int val, int mask)
        {
            return (mask & 1 << val) != 0;
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
            //检查在地图内
            var dataGrid = tableUnit.GetDataGrid(unitDesc.Guid.x, unitDesc.Guid.y, 0, unitDesc.Scale);
            if (!DataScene2D.CurScene.IsInTileMap(dataGrid))
            {
                return false;
            }
            //绳子必须绑在石头上
            if (UnitDefine.RopeId == tableUnit.Id)
            {
                var upUnit = RopeManager.Instance.GetUpFloorUnit(unitDesc, tableUnit);
                if (upUnit == null)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "绳子只能绑在稳固的物体上喔~");
                    return false;
                }

                if (upUnit.Id == UnitDefine.RopeId)
                {
                    Rope rope = upUnit as Rope;
                    if (rope != null && rope.SegmentIndex >= tableUnit.ValidRange - 1)
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "绳子最长就这么长~");
                        return false;
                    }
                }
            }
            //怪物同屏数量不可过多
            {
                if (tableUnit.EUnitType == EUnitType.Monster || UnitDefine.BoxId == tableUnit.Id)
                {
                    IntVec2 size = new IntVec2(15, 10) * ConstDefineGM2D.ServerTileScale;
                    IntVec2 mapSize = ConstDefineGM2D.MapTileSize;
                    var min = new IntVec2(unitDesc.Guid.x / size.x * size.x, unitDesc.Guid.y / size.y * size.y);
                    var grid = new Grid2D(min.x, min.y, Mathf.Min(mapSize.x, min.x + size.x - 1),
                        Mathf.Min(mapSize.y, min.y + size.y - 1));
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
                    if (PairUnitManager.Instance.TryGetNotFullPairUnit(tableUnit.EPairType, out pairUnit))
                    {
                        UnitDesc notEmptyUnitDesc;
                        int notEmptyScene;
                        if (pairUnit.UnitA != UnitDesc.zero)
                        {
                            notEmptyUnitDesc = pairUnit.UnitA;
                            notEmptyScene = pairUnit.UnitAScene;
                        }
                        else
                        {
                            notEmptyUnitDesc = pairUnit.UnitB;
                            notEmptyScene = pairUnit.UnitBScene;
                        }

                        if (tableUnit.EPairType == EPairType.PortalDoor)
                        {
                            if (notEmptyUnitDesc != UnitDesc.zero)
                            {
                                int validTileRange = tableUnit.ValidRange * ConstDefineGM2D.ServerTileScale;
                                if (!InValidRange(notEmptyUnitDesc, unitDesc, validTileRange))
                                {
                                    Messenger<string>.Broadcast(EMessengerType.GameLog, "超过传送门的最大间距，不可放置喔~");
                                    return false;
                                }

                                if (notEmptyScene != Scene2DManager.Instance.CurSceneIndex)
                                {
                                    Messenger<string>.Broadcast(EMessengerType.GameLog, "传送门不能放在不同场景哦~");
                                    return false;
                                }
                            }
                        }
                        else if (tableUnit.EPairType == EPairType.SpacetimeDoor)
                        {
                            if (notEmptyUnitDesc != UnitDesc.zero &&
                                notEmptyScene == Scene2DManager.Instance.CurSceneIndex)
                            {
                                Messenger<string>.Broadcast(EMessengerType.GameLog, "时空门不能放在同一个场景内哦~");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog,
                            string.Format("超过{0}的最大数量，不可放置喔~", tableUnit.Name));
                        return false;
                    }
                }
            }
            //花草树只能放在泥土上。
            {
                if (UnitDefine.IsPlant(tableUnit.Id))
                {
                    var downGuid = unitDesc.GetDownPos((int) EUnitDepth.Earth);
                    UnitBase downUnit;
                    if (!TryGetUnit(downGuid, out downUnit) || !UnitDefine.IsEarth(downUnit.Id))
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog,
                            string.Format("{0}只可种植在泥土上~", tableUnit.Name));
                        return false;
                    }
                }
            }
            //数量不能超过限额
            {
                int count;
                _unitIndexCount.TryGetValue(unitDesc.Id, out count);
                if (GetTableUnit_Count(tableUnit) > 0 && count >= GetTableUnit_Count(tableUnit))
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog,
                        string.Format("不可放置，{0}最多可放置{1}个~", tableUnit.Name, count));
                    return false;
                }

                if (count >= LocalUser.Instance.UserWorkshopUnitData.GetUnitLimt(unitDesc.Id))
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, string.Format("不可放置，目前剩余{0}个", count));
                    return false;
                }
            }
            //判断npc的总数
            {
                if (UnitDefine.IsNpc(unitDesc.Id) &&
                    NpcTaskDataTemp.Intance.GetNpcSerialNum() == NpcTaskDataTemp.NoneNumMark)
                {
                    return false;
                }
            }
            return true;
        }

        public static void BeforeAddUnit(Table_Unit tableUnit)
        {
            //只有一个的自动删除
            if (GetTableUnit_Count(tableUnit) == 1)
            {
                SceneUnitDesc sceneUnitDesc;
                if (TryGetReplaceUnit(tableUnit.Id, out sceneUnitDesc))
                {
                    Scene2DManager.Instance.ActionFromOtherScene(sceneUnitDesc.SceneIndex,
                        () => { EditMode.Instance.DeleteUnitWithCheck(sceneUnitDesc.UnitDesc); });
                }
            }
        }

        public static void AfterAddUnit(UnitDesc unitDesc, Table_Unit tableUnit, bool isInit = false)
        {
            if (GetTableUnit_Count(tableUnit) == 1)
            {
                _replaceUnits.Add(unitDesc.Id, new SceneUnitDesc(unitDesc, Scene2DManager.Instance.CurSceneIndex));
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
            NpcTaskDataTemp.Intance.AddNpc(unitDesc);

            EditMode.Instance.MapStatistics.AddOrDeleteUnit(tableUnit, true, isInit);
            //如果添加的是出生点则清空默认属性，因为出生点之间属性互斥
            if (unitDesc.Id == UnitDefine.SpawnId && _unitDefaultDataDict.ContainsKey(UnitDefine.SpawnId))
            {
                _unitDefaultDataDict.Remove(UnitDefine.SpawnId);
            }
        }

        public static void AfterDeleteUnit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            if (_replaceUnits.ContainsKey(unitDesc.Id) && _replaceUnits[unitDesc.Id].UnitDesc.Guid == unitDesc.Guid &&
                _replaceUnits[unitDesc.Id].SceneIndex == Scene2DManager.Instance.CurSceneIndex)
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

        public static bool TryGetReplaceUnit(int id, out SceneUnitDesc outUnitDesc)
        {
            if (!_replaceUnits.TryGetValue(id, out outUnitDesc))
            {
                return false;
            }

            return outUnitDesc.UnitDesc.Id != 0;
        }

        public static bool TryGetUnit(IntVec3 pos, out UnitBase unit)
        {
            return ColliderScene2D.CurScene.TryGetUnit(pos, out unit);
        }

        public static bool IsEmpty(IntVec3 pos)
        {
            UnitBase unit;
            return !ColliderScene2D.CurScene.TryGetUnit(pos, out unit);
        }

        public static int CalcDirectionVal(byte dir)
        {
            if (dir < 4)
            {
                return dir * 2;
            }
            else
            {
                return (dir - 4) * 2 + 1;
            }
        }

        public static int GetTableUnit_Count(Table_Unit tableUnit)
        {
            if (!GM2DGame.Instance.GameMode.IsMulti)
            {
                return tableUnit.Count;
            }

            if (UnitDefine.IsSpawn(tableUnit.Id))
            {
                return 6;
            }

            return tableUnit.Count;
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

        private static bool InValidRange(UnitDesc pairUnitUnitB, UnitDesc unitDesc, int validRange)
        {
            if (Mathf.Abs(pairUnitUnitB.Guid.x - unitDesc.Guid.x) > validRange) return false;
            if (Mathf.Abs(pairUnitUnitB.Guid.y - unitDesc.Guid.y) > validRange) return false;
            return true;
        }

        public static UnitExtraDynamic CheckUnitExtra(int id, UnitExtraDynamic unitExtra)
        {
            var defaultUnitExtra = GetUnitDefaultData(id).UnitExtra;
            if (unitExtra.Damage == 0)
            {
                unitExtra.Damage = defaultUnitExtra.Damage;
            }

            if (unitExtra.EffectRange == 0)
            {
                unitExtra.EffectRange = defaultUnitExtra.EffectRange;
            }

            if (unitExtra.CastRange == 0)
            {
                unitExtra.CastRange = defaultUnitExtra.CastRange;
            }

            if (unitExtra.ViewRange == 0)
            {
                unitExtra.ViewRange = defaultUnitExtra.ViewRange;
            }

            if (unitExtra.BulletCount == 0)
            {
                unitExtra.BulletCount = defaultUnitExtra.BulletCount;
            }

            if (unitExtra.BulletSpeed == 0)
            {
                unitExtra.BulletSpeed = defaultUnitExtra.BulletSpeed;
            }

            if (unitExtra.ChargeTime == 0)
            {
                unitExtra.ChargeTime = defaultUnitExtra.ChargeTime;
            }

            if (unitExtra.MaxHp == 0)
            {
                unitExtra.MaxHp = defaultUnitExtra.MaxHp;
            }

            if (unitExtra.MaxSpeedX == 0)
            {
                unitExtra.MaxSpeedX = defaultUnitExtra.MaxSpeedX;
            }

            if (unitExtra.JumpAbility == 0)
            {
                unitExtra.JumpAbility = defaultUnitExtra.JumpAbility;
            }

            return unitExtra;
        }
    }
}