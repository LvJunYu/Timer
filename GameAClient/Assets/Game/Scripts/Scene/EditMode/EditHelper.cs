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
                    var units = DataScene2D.GridCastAllReturnUnits(grid, EnvManager.HeroLayer);
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
                int count;
                if (_unitIndexCount.TryGetValue(unitDesc.Id, out count) && count >= tableUnit.Count)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, string.Format("不可放置，{0}最多可放置{1}个~", tableUnit.Name, count));
                    return false;
                }
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
                        EditMode.Instance.DeleteUnit(desc);
                    }
                }
            }
            //地块上的树草自动生成
            if (UnitDefine.IsEarth(tableUnit.Id))
            {
                int random = Random.Range(0, 5);
                if (random == 0)
                {
                    IntVec3 up = unitDesc.GetUpPos((int) EUnitDepth.Earth);
                    if (IsEmpty(up))
                    {
                        var newDesc = new UnitDesc(UnitDefine.GetRandomPlantId(Random.Range(0,3)),up,0,Vector2.one);
                        _cacheUnitDescs.Add(newDesc);
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
            if (tableUnit.Count > 1)
            {
                if (!_unitIndexCount.ContainsKey(unitDesc.Id))
                {
                    _unitIndexCount.Add(unitDesc.Id, new int());
                }
                _unitIndexCount[unitDesc.Id] += 1;
            }
            EditMode.Instance.MapStatistics.AddOrDeleteUnit(tableUnit, true, isInit);
        }

        public static void AfterDeleteUnit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            if (_replaceUnits.ContainsKey(unitDesc.Id) && _replaceUnits[unitDesc.Id] == unitDesc)
            {
                _replaceUnits.Remove(unitDesc.Id);
            }
            if (tableUnit.Count > 1)
            {
                if (_unitIndexCount.ContainsKey(unitDesc.Id))
                {
                    _unitIndexCount[unitDesc.Id] -= 1;
                }
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
    }
}
