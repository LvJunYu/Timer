/********************************************************************
** Filename : DataScene2D
** Author : Dong
** Date : 2016/10/3 星期一 下午 8:16:34
** Summary : DataScene2D
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    public class DataScene2D : Scene2D
    {
        [SerializeField] private IntRect _validMapRect;
        private int _sceneIndex;
        protected Dictionary<IntVec3, UnitExtra> _unitExtras = new Dictionary<IntVec3, UnitExtra>();
        private UnitExtra _playerExtra;
        private IntVec2 _size = ConstDefineGM2D.DefaultValidMapRectSize;
        protected Dictionary<IntVec3, List<IntVec3>> _switchedUnits = new Dictionary<IntVec3, List<IntVec3>>();
        private static List<UnitBase> _cachedUnits = new List<UnitBase>();
        private List<UnitDesc> _spawnDatas = new List<UnitDesc>();

        /// <summary>
        /// 删除修改的物体堆栈
        /// </summary>
        private List<ModifyData> _removedUnits = new List<ModifyData>();

        /// <summary>
        /// 改动修改的物体堆栈
        /// </summary>
        private List<ModifyData> _modifiedUnits = new List<ModifyData>();

        /// <summary>
        /// 添加修改的物体堆栈
        /// </summary>
        private List<ModifyData> _addedUnits = new List<ModifyData>();

        public static DataScene2D CurScene
        {
            get { return Scene2DManager.Instance.CurDataScene2D; }
        }

        public static DataScene2D MainDataScene2D
        {
            get { return Scene2DManager.Instance.MainDataScene2D; }
        }

        public List<UnitDesc> SpawnDatas
        {
            get { return _spawnDatas; }
        }

        public IntRect ValidMapRect
        {
            get { return _validMapRect; }
        }

        public Dictionary<IntVec3, UnitExtra> UnitExtras
        {
            get { return _unitExtras; }
        }

        public List<ModifyData> RemovedUnits
        {
            get { return _removedUnits; }
        }

        public List<ModifyData> ModifiedUnits
        {
            get { return _modifiedUnits; }
        }

        public List<ModifyData> AddedUnits
        {
            get { return _addedUnits; }
        }

        public UnitExtra PlayerExtra
        {
            get { return _playerExtra; }
        }

        public int SceneIndex
        {
            get { return _sceneIndex; }
        }

        public void Init(IntVec2 size, int index = 0)
        {
            _sceneIndex = index;
            _size = size;
            Init(ConstDefineGM2D.MapTileSize.x, ConstDefineGM2D.MapTileSize.y);
        }

        protected override void OnInit()
        {
            base.OnInit();
            var startPos = ConstDefineGM2D.MapStartPos;
            _validMapRect = new IntRect(startPos, startPos + _size - IntVec2.one);
        }

        /// <summary>
        /// 这个方法只能被MapManager访问，只能是创建地图并设置初始大小时访问
        /// </summary>
        /// <param name="size"></param>
        public void SetMapSize(IntVec2 size)
        {
            if (_size == size) return;
            _size = size;
            var startPos = ConstDefineGM2D.MapStartPos;
            _validMapRect = new IntRect(startPos, startPos + _size - IntVec2.one);
        }

        private void ChangeMapRect(IntRect changedTileSize)
        {
            _validMapRect += changedTileSize;
        }

        public void ChangeMapRect(bool add, bool horizontal, bool record = true)
        {
            var changedTileSize = new IntRect(IntVec2.zero, IntVec2.zero);
            if (add)
            {
                if (horizontal)
                {
                    changedTileSize.Max = IntVec2.right * ConstDefineGM2D.MapChangeTitle;
                }
                else
                {
                    changedTileSize.Max = IntVec2.up * ConstDefineGM2D.MapChangeTitle;
                }
            }
            else
            {
                if (horizontal)
                {
                    changedTileSize.Max = IntVec2.left * ConstDefineGM2D.MapChangeTitle;
                }
                else
                {
                    changedTileSize.Max = IntVec2.down * ConstDefineGM2D.MapChangeTitle;
                }
            }

            if (record)
            {
                //记录
                var recordBatch = new EditRecordBatch();
                recordBatch.RecordChangeMapRect(add, horizontal);
                Scene2DManager.Instance.CommitRecordBatch(recordBatch);
            }
            ChangeMapRect(changedTileSize);
            Scene2DManager.Instance.OnMapChanged(horizontal ? EChangeMapRectType.Right : EChangeMapRectType.Top);
            var gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (gameModeEdit != null)
            {
                gameModeEdit.NeedSave = true;
            }
        }

        public void InitPlay(IntRect mapRect)
        {
            _validMapRect = mapRect;
        }

        public bool IsInTileMap(IntVec2 tile)
        {
            return tile.x >= _validMapRect.Min.x && tile.x <= _validMapRect.Max.x && tile.y >= _validMapRect.Min.y &&
                   tile.y <= _validMapRect.Max.y;
        }

        public bool IsInTileMap(IntVec3 tile)
        {
            return tile.x >= _validMapRect.Min.x && tile.x <= _validMapRect.Max.x && tile.y >= _validMapRect.Min.y &&
                   tile.y <= _validMapRect.Max.y;
        }

        public bool IsInTileMap(Grid2D grid)
        {
            return grid.XMin >= _validMapRect.Min.x && grid.XMax <= _validMapRect.Max.x &&
                   grid.YMin >= _validMapRect.Min.y && grid.YMax <= _validMapRect.Max.y;
        }

        internal IntVec3 GetTileIndex(Vector3 worldPos, int id, byte rotation = 0)
        {
            var tableUnit = UnitManager.Instance.GetTableUnit(id);
            if (tableUnit == null)
            {
                LogHelper.Error("WorldPosToTileIndex failed,{0}", id);
                return IntVec3.zero;
            }

            var tile = GM2DTools.WorldToTile(
                worldPos - GM2DTools.TileToWorld(tableUnit.GetDataSize(0, Vector2.one)) * 0.5f);
            //>4的按照一个大格子（4小格）对其摆放。
            var size = tableUnit.GetDataSize(rotation, Vector2.one);
            int x = size.x > ConstDefineGM2D.ServerTileScale
                ? (int) (tile.x * ConstDefineGM2D.ClientTileScale + 0.5f) * ConstDefineGM2D.ServerTileScale
                : (int) ((float) tile.x / size.x + 0.5f) * size.x;
            int y = size.y > ConstDefineGM2D.ServerTileScale
                ? (int) (tile.y * ConstDefineGM2D.ClientTileScale + 0.5f) * ConstDefineGM2D.ServerTileScale
                : (int) ((float) tile.y / size.y + 0.5f) * size.y;
            return new IntVec3(x, y, UnitManager.GetDepth(tableUnit));
        }

        public bool AddData(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            var grid = tableUnit.GetDataGrid(unitDesc.Guid.x, unitDesc.Guid.y, 0, unitDesc.Scale);
            SceneNode hit;
            //数据检测
            if (GridCast(grid, out hit, _sceneIndex, JoyPhysics2D.LayMaskAll, unitDesc.Guid.z, unitDesc.Guid.z))
            {
                LogHelper.Error("AddData Failed,{0}", unitDesc.ToString());
                return false;
            }

            var dataNode = NodeFactory.GetDataNode(unitDesc, tableUnit);
            if (!AddNode(dataNode))
            {
                return false;
            }

            if (UnitDefine.IsSpawn(tableUnit.Id))
            {
                _spawnDatas.Add(unitDesc);
            }

            return true;
        }

        public bool DeleteData(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            var dataNode = NodeFactory.GetDataNode(unitDesc, tableUnit);
            if (!DeleteNode(dataNode))
            {
                return false;
            }

            DeleteUnitExtra(unitDesc.Guid);
            if (UnitDefine.IsSpawn(tableUnit.Id))
            {
                _spawnDatas.Remove(unitDesc);
            }

            return true;
        }

        #region ExtraData&Advanced

        public bool TryGetUnitExtra(IntVec3 guid, out UnitExtra unitExtra)
        {
            return _unitExtras.TryGetValue(guid, out unitExtra);
        }

        public UnitExtra GetUnitExtra(IntVec3 guid)
        {
            UnitExtra unitExtra;
            TryGetUnitExtra(guid, out unitExtra);
            return unitExtra;
        }

        public void ProcessUnitExtra(UnitDesc unitDesc, UnitExtra unitExtra, EditRecordBatch editRecordBatch = null)
        {
            UnitBase unit;
            bool canSwitch = false;
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                EditMode.Instance.MapStatistics.NeedSave = true;
                bool needCreate = false;
                if (ColliderScene2D.CurScene.TryGetUnit(unitDesc.Guid, out unit))
                {
                    var oldUnitDesc = unit.UnitDesc;
                    EditMode.Instance.DeleteUnit(oldUnitDesc);
                    canSwitch = unit.CanControlledBySwitch;
                    needCreate = true;
                }

                if (unitExtra.Equals(UnitExtra.zero))
                {
                    DeleteUnitExtra(unitDesc.Guid);
                }
                else
                {
                    _unitExtras.AddOrReplace(unitDesc.Guid, unitExtra);
                }

                if (needCreate)
                {
                    EditMode.Instance.AddUnit(unitDesc);
                }

                if (canSwitch)
                {
                    if (ColliderScene2D.CurScene.TryGetUnit(unitDesc.Guid, out unit))
                    {
                        if (!unit.CanControlledBySwitch)
                        {
                            OnUnitDeleteUpdateSwitchData(unitDesc, editRecordBatch);
                        }
                    }
                    else
                    {
                        OnUnitDeleteUpdateSwitchData(unitDesc, editRecordBatch);
                        LogHelper.Error("ProcessUnitExtra UnitBase missing");
                    }
                }
            }
            else
            {
                _unitExtras.AddOrReplace(unitDesc.Guid, unitExtra);
                // 更新unit
                if (ColliderScene2D.CurScene.TryGetUnit(unitDesc.Guid, out unit))
                {
                    unit.UpdateExtraData();
                }
            }
        }

        public void SetPlayerExtra(UnitExtra unitExtra)
        {
            _playerExtra = unitExtra;
        }

        public void DeleteUnitExtra(IntVec3 guid)
        {
            if (_unitExtras.ContainsKey(guid))
            {
                _unitExtras.Remove(guid);
            }
        }

        #endregion

        #region Switch

        /// <summary>
        /// 查找开关控制的Unit
        /// </summary>
        /// <returns>The switched units.</returns>
        /// <param name="guid">开关id.</param>
        public List<UnitBase> GetControlledUnits(IntVec3 guid)
        {
            List<IntVec3> unitsGuid;
            if (!_switchedUnits.TryGetValue(guid, out unitsGuid))
            {
                return null;
            }

            _cachedUnits.Clear();
            for (int i = 0; i < unitsGuid.Count; i++)
            {
                UnitBase unit;
                if (ColliderScene2D.CurScene.TryGetUnit(unitsGuid[i], out unit))
                {
                    _cachedUnits.Add(unit);
                }
            }

            return _cachedUnits.ToList();
        }

        /// <summary>
        /// 查找控制unit的开关
        /// </summary>
        /// <returns>The switch units connected.</returns>
        /// <param name="guid">GUID.</param>
        public List<IntVec3> GetSwitchUnitsConnected(IntVec3 guid)
        {
            var result = new List<IntVec3>();
            if (_switchedUnits != null)
            {
                using (var itor = _switchedUnits.GetEnumerator())
                {
                    while (itor.MoveNext())
                    {
                        List<IntVec3> units = itor.Current.Value;
                        if (units.Contains(guid))
                        {
                            result.Add(itor.Current.Key);
                        }
                    }
                }
            }

            return result;
        }

        public bool BindSwitch(IntVec3 switchGuid, IntVec3 unitGuid)
        {
            if (!_switchedUnits.ContainsKey(switchGuid))
            {
                _switchedUnits.Add(switchGuid, new List<IntVec3>());
            }

            if (_switchedUnits[switchGuid].Contains(unitGuid))
            {
                return false;
            }

            _switchedUnits[switchGuid].Add(unitGuid);
            Messenger<IntVec3, IntVec3, bool>.Broadcast(EMessengerType.OnSwitchConnectionChanged,
                switchGuid, unitGuid, true);
            return true;
        }

        public bool UnbindSwitch(IntVec3 switchGuid, IntVec3 unitGuid)
        {
            List<IntVec3> unitsGuid;
            if (!_switchedUnits.TryGetValue(switchGuid, out unitsGuid))
            {
                LogHelper.Error("UnbindSwitch Failed, {0}, {1}", switchGuid, unitGuid);
                return false;
            }

            unitsGuid.Remove(unitGuid);
            if (unitsGuid.Count == 0)
            {
                _switchedUnits.Remove(switchGuid);
            }

            Messenger<IntVec3, IntVec3, bool>.Broadcast(EMessengerType.OnSwitchConnectionChanged,
                switchGuid, unitGuid, false);
            return true;
        }

        public void OnUnitDeleteUpdateSwitchData(UnitDesc unitDesc, EditRecordBatch recordBatch = null)
        {
            if (UnitDefine.IsSwitch(unitDesc.Id))
            {
                List<IntVec3> list;
                if (_switchedUnits.TryGetValue(unitDesc.Guid, out list))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        IntVec3 unitGuid = list[i];
                        Messenger<IntVec3, IntVec3, bool>.Broadcast(EMessengerType.OnSwitchConnectionChanged,
                            unitDesc.Guid, unitGuid, false);
                        if (null != recordBatch)
                        {
                            recordBatch.RecordRemoveSwitchConnection(unitDesc.Guid, unitGuid);
                        }
                    }

                    _switchedUnits.Remove(unitDesc.Guid);
                }
            }
            else
            {
                using (var itor = _switchedUnits.GetEnumerator())
                {
                    while (itor.MoveNext())
                    {
                        var switchGuid = itor.Current.Key;
                        List<IntVec3> units = itor.Current.Value;
                        int unitInx = units.IndexOf(unitDesc.Guid);
                        if (unitInx < 0)
                        {
                            continue;
                        }

                        units.RemoveAt(unitInx);
                        Messenger<IntVec3, IntVec3, bool>.Broadcast(EMessengerType.OnSwitchConnectionChanged,
                            switchGuid, unitDesc.Guid, true);
                        if (null != recordBatch)
                        {
                            recordBatch.RecordRemoveSwitchConnection(switchGuid, unitDesc.Guid);
                        }
                    }
                }
            }
        }

        public void OnUnitMoveUpdateSwitchData(UnitDesc oldUnitDesc, UnitDesc newUnitDesc,
            EditRecordBatch recordBatch = null)
        {
            if (UnitDefine.IsSwitch(newUnitDesc.Id))
            {
                List<IntVec3> list;
                if (_switchedUnits.TryGetValue(oldUnitDesc.Guid, out list))
                {
                    _switchedUnits.Remove(oldUnitDesc.Guid);
                    _switchedUnits.Add(newUnitDesc.Guid, list);
                    for (int i = 0; i < list.Count; i++)
                    {
                        Messenger<IntVec3, IntVec3, bool>.Broadcast(EMessengerType.OnSwitchConnectionChanged,
                            oldUnitDesc.Guid, list[i], false);
                        Messenger<IntVec3, IntVec3, bool>.Broadcast(EMessengerType.OnSwitchConnectionChanged,
                            newUnitDesc.Guid, list[i], true);
                        if (null != recordBatch)
                        {
                            recordBatch.RecordRemoveSwitchConnection(oldUnitDesc.Guid, list[i]);
                            recordBatch.RecordAddSwitchConnection(newUnitDesc.Guid, list[i]);
                        }
                    }
                }
            }
            else
            {
                using (var itor = _switchedUnits.GetEnumerator())
                {
                    while (itor.MoveNext())
                    {
                        var switchGuid = itor.Current.Key;
                        List<IntVec3> units = itor.Current.Value;
                        for (int i = 0; i < units.Count; i++)
                        {
                            if (units[i] == oldUnitDesc.Guid)
                            {
                                units[i] = newUnitDesc.Guid;
                                Messenger<IntVec3, IntVec3, bool>.Broadcast(EMessengerType.OnSwitchConnectionChanged,
                                    switchGuid, oldUnitDesc.Guid, false);
                                Messenger<IntVec3, IntVec3, bool>.Broadcast(EMessengerType.OnSwitchConnectionChanged,
                                    switchGuid, newUnitDesc.Guid, true);
                                if (null != recordBatch)
                                {
                                    recordBatch.RecordRemoveSwitchConnection(switchGuid, oldUnitDesc.Guid);
                                    recordBatch.RecordAddSwitchConnection(switchGuid, newUnitDesc.Guid);
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }

        public void SaveSwitchUnitData(List<SwitchUnitData> list)
        {
            using (var switchUnitItor = _switchedUnits.GetEnumerator())
            {
                while (switchUnitItor.MoveNext())
                {
                    SwitchUnitData newData = new SwitchUnitData();
                    newData.SwitchGUID = GM2DTools.ToProto(switchUnitItor.Current.Key);
                    for (int i = 0; i < switchUnitItor.Current.Value.Count; i++)
                    {
                        newData.ControlledGUIDs.Add(GM2DTools.ToProto(switchUnitItor.Current.Value[i]));
                    }

                    list.Add(newData);
                }
            }
        }

        #endregion

        #region Physics

        private static readonly List<UnitDesc> _cachedUnitObjects = new List<UnitDesc>();

        internal static bool PointCast(IntVec2 point, out SceneNode sceneNode, int layerMask = JoyPhysics2D.LayMaskAll,
            float minDepth = float.MinValue, float maxDepth = float.MaxValue)
        {
            return SceneQuery2D.PointCast(point, out sceneNode, layerMask, CurScene, minDepth, maxDepth);
        }

        internal static bool GridCast(Grid2D grid2D, out SceneNode node, int sceneIndex,
            int layerMask = JoyPhysics2D.LayMaskAll, float minDepth = float.MinValue, float maxDepth = float.MaxValue,
            SceneNode excludeNode = null)
        {
            return SceneQuery2D.GridCast(ref grid2D, out node, layerMask,
                Scene2DManager.Instance.GetDataScene2D(sceneIndex), minDepth, maxDepth, excludeNode);
        }

        internal static List<SceneNode> GridCastAll(Grid2D grid2D, int layerMask = JoyPhysics2D.LayMaskAll,
            float minDepth = float.MinValue, float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        {
            return SceneQuery2D.GridCastAll(ref grid2D, layerMask, CurScene, minDepth, maxDepth, excludeNode);
        }

        internal static List<UnitDesc> GridCastAllReturnUnits(UnitDesc unitDesc,
            int layerMask = JoyPhysics2D.LayMaskAll, float minDepth = float.MinValue, float maxDepth = float.MaxValue)
        {
            Grid2D outValue;
            if (!TryGetGridByUnitObject(unitDesc, out outValue))
            {
                LogHelper.Error("TryGetGridByUnitObject falied! UnitDesc is {0}", unitDesc);
                return null;
            }

            return GridCastAllReturnUnits(outValue, layerMask, minDepth, maxDepth);
        }

        internal static bool TryGetGridByUnitObject(UnitDesc unitDesc, out Grid2D outValue)
        {
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("AddData failed.{0}", unitDesc.Id);
                outValue = new Grid2D();
                return false;
            }

            outValue = tableUnit.GetDataGrid(ref unitDesc);
            return true;
        }

        public static List<UnitDesc> GridCastAllReturnUnits(Grid2D grid2D, int layerMask = JoyPhysics2D.LayMaskAll,
            float minDepth = float.MinValue, float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        {
            _cachedUnitObjects.Clear();
            var nodes = GridCastAll(grid2D, layerMask, minDepth, maxDepth, excludeNode);
            return GetUnits(grid2D, nodes);
        }

        public static List<UnitDesc> GetUnits(Grid2D grid2D, List<SceneNode> nodes)
        {
            _cachedUnitObjects.Clear();
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(node.Id);
                if (tableUnit == null)
                {
                    LogHelper.Error("GetTableUnit failed.{0}", node.Id);
                    continue;
                }

                var newGrid = GM2DTools.IntersectWith(grid2D, node, tableUnit);
                var size = tableUnit.GetDataSize(node.Rotation, node.Scale);
                var count = tableUnit.GetDataCount(newGrid, node.Rotation, node.Scale);
                for (int j = 0; j < count.x; j++)
                {
                    for (int k = 0; k < count.y; k++)
                    {
                        var rectIndex = new IntVec3(newGrid.XMin + j * size.x,
                            newGrid.YMin + k * size.y, node.Depth);
                        _cachedUnitObjects.Add(new UnitDesc(node.Id, rectIndex, node.Rotation, node.Scale));
                    }
                }
            }

            return _cachedUnitObjects;
        }

        #endregion

        public void SetPlayerCommonValue<T>(string fieldName, T value) where T : IEquatable<T>
        {
            var field = typeof(UnitExtra).GetField(fieldName);
            T original = (T) field.GetValue(_playerExtra);
            if (original.Equals(value)) return;
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                EditMode.Instance.MapStatistics.NeedSave = true;
                for (int i = 0; i < _spawnDatas.Count; i++)
                {
                    var unitExtra = GetUnitExtra(_spawnDatas[i].Guid);
                    if (((T) field.GetValue(unitExtra)).Equals(original))
                    {
                        object unitExtraObj = unitExtra;
                        field.SetValue(unitExtraObj, value);
                        _unitExtras.AddOrReplace(_spawnDatas[i].Guid, (UnitExtra) unitExtraObj);
                    }
                }
            }

            object playerExtraObj = _playerExtra;
            field.SetValue(playerExtraObj, value);
            _playerExtra = (UnitExtra) playerExtraObj;
        }

        public void SetPlayerMaxHp(int value)
        {
            if (_playerExtra.MaxHp == value) return;
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                EditMode.Instance.MapStatistics.NeedSave = true;
                for (int i = 0; i < _spawnDatas.Count; i++)
                {
                    var unitExtra = GetUnitExtra(_spawnDatas[i].Guid);
                    if (unitExtra.MaxHp == _playerExtra.MaxHp)
                    {
                        unitExtra.MaxHp = (ushort) value;
                        _unitExtras.AddOrReplace(_spawnDatas[i].Guid, unitExtra);
                    }
                }
            }

            _playerExtra.MaxHp = (ushort) value;
        }

        public void SetPlayerJumpAbility(int value)
        {
            if (_playerExtra.JumpAbility == value) return;
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                EditMode.Instance.MapStatistics.NeedSave = true;
                for (int i = 0; i < _spawnDatas.Count; i++)
                {
                    var unitExtra = GetUnitExtra(_spawnDatas[i].Guid);
                    if (unitExtra.JumpAbility == _playerExtra.JumpAbility)
                    {
                        unitExtra.JumpAbility = (ushort) value;
                        _unitExtras.AddOrReplace(_spawnDatas[i].Guid, unitExtra);
                    }
                }
            }

            _playerExtra.JumpAbility = (ushort) value;
        }

        public void SetPlayerMaxSpeedX(int value)
        {
            if (_playerExtra.MaxSpeedX == value) return;
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                EditMode.Instance.MapStatistics.NeedSave = true;
                for (int i = 0; i < _spawnDatas.Count; i++)
                {
                    var unitExtra = GetUnitExtra(_spawnDatas[i].Guid);
                    if (unitExtra.MaxSpeedX == _playerExtra.MaxSpeedX)
                    {
                        unitExtra.MaxSpeedX = (ushort) value;
                        _unitExtras.AddOrReplace(_spawnDatas[i].Guid, unitExtra);
                    }
                }
            }

            _playerExtra.MaxSpeedX = (ushort) value;
        }

        public void SetPlayerInjuredReduce(int value)
        {
            if (_playerExtra.InjuredReduce == value) return;
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                EditMode.Instance.MapStatistics.NeedSave = true;
                for (int i = 0; i < _spawnDatas.Count; i++)
                {
                    var unitExtra = GetUnitExtra(_spawnDatas[i].Guid);
                    if (unitExtra.InjuredReduce == _playerExtra.InjuredReduce)
                    {
                        unitExtra.InjuredReduce = (byte) value;
                        _unitExtras.AddOrReplace(_spawnDatas[i].Guid, unitExtra);
                    }
                }
            }

            _playerExtra.InjuredReduce = (byte) value;
        }

        public void SetPlayerCureIncrease(int value)
        {
            if (_playerExtra.CureIncrease == value) return;
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                EditMode.Instance.MapStatistics.NeedSave = true;
                for (int i = 0; i < _spawnDatas.Count; i++)
                {
                    var unitExtra = GetUnitExtra(_spawnDatas[i].Guid);
                    if (unitExtra.CureIncrease == _playerExtra.CureIncrease)
                    {
                        unitExtra.CureIncrease = (ushort) value;
                        _unitExtras.AddOrReplace(_spawnDatas[i].Guid, unitExtra);
                    }
                }
            }

            _playerExtra.CureIncrease = (ushort) value;
        }

        public void InitDefaultPlayerUnitExtra()
        {
            var table = TableManager.Instance.GetUnit(UnitDefine.MainPlayerId);
            var unitExtra = new UnitExtra();
            unitExtra.MaxHp = (ushort) table.Hp;
            unitExtra.MaxSpeedX = (ushort) table.MaxSpeed;
            unitExtra.JumpAbility = (ushort) table.JumpAbility;
            unitExtra.InjuredReduce = 0;
            unitExtra.CureIncrease = 0;
            SetPlayerExtra(unitExtra);
        }
    }

    public enum EChangeMapRectType
    {
        None,
        Right,
        Top
    }

#pragma warning disable 0660 0661
    public struct UnitEditData : IEquatable<UnitEditData>
    {
        public UnitDesc UnitDesc;
        public UnitExtra UnitExtra;

        public UnitEditData(UnitDesc unitDesc, UnitExtra unitExtra)
        {
            UnitDesc = unitDesc;
            UnitExtra = unitExtra;
        }

        public static bool operator ==(UnitEditData a, UnitEditData other)
        {
            return (a.UnitDesc == other.UnitDesc) && (a.UnitExtra == other.UnitExtra);
        }

        public static bool operator !=(UnitEditData a, UnitEditData other)
        {
            return !(a == other);
        }

        public bool Equals(UnitEditData other)
        {
            return (UnitDesc == other.UnitDesc) && (UnitExtra == other.UnitExtra);
        }
    }

    public struct ModifyData
    {
        public UnitEditData OrigUnit;
        public UnitEditData ModifiedUnit;

        public ModifyData(UnitEditData orig, UnitEditData modified)
        {
            OrigUnit = orig;
            ModifiedUnit = modified;
        }

        // 从地图文件方序列化出来的构造函数
        public ModifyData(ModifyItemData modifyItemData)
        {
            OrigUnit = new UnitEditData();
            ModifiedUnit = new UnitEditData();
            if (DataScene2D.CurScene == null)
            {
                LogHelper.Error("Instantiate modifyData failed, datascene2d not exist");
                return;
            }

            Table_Unit table = TableManager.Instance.GetUnit(modifyItemData.OrigData.Id);
            if (null == table)
            {
                LogHelper.Error("ParseModifyData error, unit with invalid id {0}", modifyItemData.OrigData.Id);
                return;
            }

            int depth = UnitManager.GetDepth(table);
            UnitDesc origDesc = new UnitDesc(
                modifyItemData.OrigData.Id,
                new IntVec3(modifyItemData.OrigData.XMin, modifyItemData.OrigData.YMin, depth),
                (byte) modifyItemData.OrigData.Rotation,
                new Vector2(modifyItemData.OrigData.Scale == null ? 1 : modifyItemData.OrigData.Scale.X,
                    modifyItemData.OrigData.Scale == null ? 1 : modifyItemData.OrigData.Scale.Y)
            );
            UnitExtra origExtra = new UnitExtra();
            origExtra.MoveDirection = (EMoveDirection) modifyItemData.OrigExtra.MoveDirection;
            origExtra.Active = (byte) modifyItemData.OrigExtra.Active;
            origExtra.ChildId = (ushort) modifyItemData.OrigExtra.ChildId;
            origExtra.ChildRotation = (byte) modifyItemData.OrigExtra.ChildRotation;
            origExtra.RotateMode = (byte) modifyItemData.OrigExtra.RotateMode;
            origExtra.RotateValue = (byte) modifyItemData.OrigExtra.RotateValue;
            origExtra.TimeDelay = (ushort) modifyItemData.OrigExtra.TimeDelay;
            origExtra.TimeInterval = (ushort) modifyItemData.OrigExtra.TimeInterval;
            OrigUnit = new UnitEditData(origDesc, origExtra);
            UnitDesc modifiedDesc = new UnitDesc(
                modifyItemData.ModifiedData.Id,
                new IntVec3(modifyItemData.ModifiedData.XMin, modifyItemData.ModifiedData.YMin, depth),
                (byte) modifyItemData.ModifiedData.Rotation,
                new Vector2(modifyItemData.ModifiedData.Scale == null ? 1 : modifyItemData.ModifiedData.Scale.X,
                    modifyItemData.ModifiedData.Scale == null ? 1 : modifyItemData.ModifiedData.Scale.Y)
            );
            UnitExtra modifiedExtra;
            DataScene2D.CurScene.TryGetUnitExtra(modifiedDesc.Guid, out modifiedExtra);
            ModifiedUnit = new UnitEditData(modifiedDesc, modifiedExtra);
        }

        public ModifyItemData ToModifyItemData()
        {
            ModifyItemData mid = new ModifyItemData();
            mid.OrigData = new MapRect2D();
            mid.OrigData.Id = OrigUnit.UnitDesc.Id;
            mid.OrigData.Rotation = OrigUnit.UnitDesc.Rotation;
            mid.OrigData.Scale = new Vec2Proto();
            mid.OrigData.Scale.X = OrigUnit.UnitDesc.Scale.x;
            mid.OrigData.Scale.Y = OrigUnit.UnitDesc.Scale.y;
            mid.OrigData.XMin = OrigUnit.UnitDesc.Guid.x;
            mid.OrigData.XMax = OrigUnit.UnitDesc.Guid.x + ConstDefineGM2D.ServerTileScale;
            mid.OrigData.YMin = OrigUnit.UnitDesc.Guid.y;
            mid.OrigData.YMax = OrigUnit.UnitDesc.Guid.y + ConstDefineGM2D.ServerTileScale;
            mid.OrigExtra = GM2DTools.ToProto(OrigUnit.UnitDesc.Guid, OrigUnit.UnitExtra);
            mid.ModifiedData = new MapRect2D();
            mid.ModifiedData.Id = ModifiedUnit.UnitDesc.Id;
            mid.ModifiedData.Rotation = ModifiedUnit.UnitDesc.Rotation;
            mid.ModifiedData.Scale = new Vec2Proto();
            mid.ModifiedData.Scale.X = ModifiedUnit.UnitDesc.Scale.x;
            mid.ModifiedData.Scale.Y = ModifiedUnit.UnitDesc.Scale.y;
            mid.ModifiedData.XMin = ModifiedUnit.UnitDesc.Guid.x;
            mid.ModifiedData.XMax = ModifiedUnit.UnitDesc.Guid.x + ConstDefineGM2D.ServerTileScale;
            mid.ModifiedData.YMin = ModifiedUnit.UnitDesc.Guid.y;
            mid.ModifiedData.YMax = ModifiedUnit.UnitDesc.Guid.y + ConstDefineGM2D.ServerTileScale;
            return mid;
        }
    }
}