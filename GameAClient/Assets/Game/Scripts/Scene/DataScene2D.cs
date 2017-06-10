/********************************************************************
** Filename : DataScene2D
** Author : Dong
** Date : 2016/10/3 星期一 下午 8:16:34
** Summary : DataScene2D
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class DataScene2D : Scene2D
    {
        #region 常量与字段

        private static DataScene2D _instance;
        private SceneNode _mainPlayer;
        [SerializeField]
        private static Vector3 _startPos;
        [SerializeField]
        private Grid2D _mapGrid;
        [SerializeField]
        private IntRect _validMapRect;
        protected Dictionary<IntVec3, UnitExtra> _unitExtras = new Dictionary<IntVec3, UnitExtra>();
        protected Dictionary<IntVec3, List<IntVec3>> _switchedUnits = new Dictionary<IntVec3, List<IntVec3>>();
        private static List<UnitBase> _cachedUnits = new List<UnitBase>();

		/// <summary>
		/// 删除修改的物体堆栈
		/// </summary>
		private List<ModifyData> _removedUnits = new List<ModifyData>();
		/// <summary>
		/// 改动修改的物体堆栈
		/// </summary>
		private List<ModifyData> _modifiedUnits = new List<ModifyData> ();
		/// <summary>
		/// 添加修改的物体堆栈
		/// </summary>
		private List<ModifyData> _addedUnits = new List<ModifyData>();

        #endregion

        #region 属性

        public static DataScene2D Instance
        {
            get { return _instance ?? (_instance = new DataScene2D()); }
        }

        public SceneNode MainPlayer
        {
            get { return _mainPlayer; }
        }

        public Vector3 StartPos
        {
            get { return _startPos; }
        }

        public IntRect ValidMapRect
        {
            get { return _validMapRect; }
        }

        public Grid2D MapGrid
        {
            get { return _mapGrid; }
        }

        public Dictionary<IntVec3, UnitExtra> UnitExtras
        {
            get { return _unitExtras; }
        }

		public List<ModifyData> RemovedUnits {
			get {
				return this._removedUnits;
			}
		}

		public List<ModifyData> ModifiedUnits {
			get {
				return this._modifiedUnits;
			}
		}

		public List<ModifyData> AddedUnits {
			get {
				return this._addedUnits;
			}
		}

        public Dictionary<IntVec3, List<IntVec3>> SwitchedUnits {
            get {
                return this._switchedUnits;
            }
        }
        #endregion

        #region 方法

        protected override void OnInit()
        {
            base.OnInit();
            _mapGrid = new Grid2D(0, 0, ConstDefineGM2D.MapTileSize.x - 1, ConstDefineGM2D.MapTileSize.y - 1);
            var worldPos = GM2DTools.TileToWorld(new IntVec2(_mapGrid.XMin, _mapGrid.YMin));
            _startPos = new Vector2(worldPos.x, worldPos.y);
            _validMapRect = new IntRect(ConstDefineGM2D.MapStartPos, ConstDefineGM2D.DefaultValidMapRectSize + ConstDefineGM2D.MapStartPos - IntVec2.one);
        }

        public override void Dispose()
        {
            base.Dispose();
            _instance = null;
        }

        internal void ChangeMapRect(IntRect changedTileSize)
        {
            _validMapRect += changedTileSize;
        }

        public void InitPlay(IntRect mapRect)
        {
            _validMapRect = mapRect;
        }

        public bool IsInTileMap(IntVec2 tile)
        {
            return (tile.x >= _validMapRect.Min.x && tile.x <= _validMapRect.Max.x && tile.y >= _validMapRect.Min.y && tile.y <= _validMapRect.Max.y);
        }

        public bool IsInTileMap(IntVec3 tile)
        {
            return (tile.x >= _validMapRect.Min.x && tile.x <= _validMapRect.Max.x && tile.y >= _validMapRect.Min.y && tile.y <= _validMapRect.Max.y);
        }

        public bool IsInTileMap(Grid2D grid)
        {
            return (grid.XMin >= _validMapRect.Min.x && grid.XMax <= _validMapRect.Max.x && grid.YMin >= _validMapRect.Min.y && grid.YMax<= _validMapRect.Max.y);
        }

        internal IntVec3 GetTileIndex(Vector3 worldPos, int id, byte rotation = 0)
        {
            var tableUnit = UnitManager.Instance.GetTableUnit(id);
            if (tableUnit == null)
            {
                LogHelper.Error("WorldPosToTileIndex failed,{0}", id);
                return IntVec3.zero;
            }
            var tile = GM2DTools.WorldToTile(worldPos - GM2DTools.TileToWorld(tableUnit.GetDataSize(0, Vector2.one)) * 0.5f - _startPos);
            //>4的按照一个大格子（4小格）对其摆放。
            var size = tableUnit.GetDataSize(rotation, Vector2.one);
            int x = size.x > ConstDefineGM2D.ServerTileScale
                ? (int)(tile.x * ConstDefineGM2D.ClientTileScale + 0.5f) * ConstDefineGM2D.ServerTileScale
                : (int)((float)tile.x / size.x + 0.5f) * size.x;
            int y = size.y > ConstDefineGM2D.ServerTileScale
                ? (int)(tile.y * ConstDefineGM2D.ClientTileScale + 0.5f) * ConstDefineGM2D.ServerTileScale
                : (int)((float)tile.y / size.y + 0.5f) * size.y;

            return new IntVec3(x, y, UnitManager.GetDepth(tableUnit));
        }

        #endregion

        public bool AddData(UnitDesc unitDesc, Table_Unit tableUnit, bool isInitAdd = false)
        {
            var grid = tableUnit.GetBaseDataGrid(unitDesc.Guid);
            SceneNode hit;
            //数据检测
            if (GridCast(grid, out hit, JoyPhysics2D.LayMaskAll, unitDesc.Guid.z, unitDesc.Guid.z))
            {
                LogHelper.Error("AddData Failed,{0}", unitDesc.ToString());
                return false;
            }
            var dataNode = NodeFactory.GetDataNode(unitDesc, tableUnit);
            if (!AddNode(dataNode))
            {
                return false;
            }
            if (!isInitAdd)
            {
                SetUnitExtra(unitDesc, tableUnit);
            }
            if (tableUnit.EUnitType == EUnitType.MainPlayer)
            {
                _mainPlayer = dataNode;
            }
            return true;
        }

        private void SetUnitExtra(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            if (tableUnit.CanMove)
            {
                UnitExtra unitExtra;
                if (!TryGetUnitExtra(unitDesc.Guid, out unitExtra))
                {
                    unitExtra.MoveDirection = (EMoveDirection)tableUnit.OriginMoveDirection;
                    ProcessUnitExtra(unitDesc.Guid, unitExtra);
                }
            }
            if (tableUnit.Id == UnitDefine.RollerId)
            {
                UnitExtra unitExtra;
                if (!TryGetUnitExtra(unitDesc.Guid, out unitExtra))
                {
                    unitExtra.RollerDirection = EMoveDirection.Right;
                    ProcessUnitExtra(unitDesc.Guid, unitExtra);
                }
            }
        }

        public bool DeleteData(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            var dataNode = NodeFactory.GetDataNode(unitDesc, tableUnit);
            if (!DeleteNode(dataNode))
            {
                return false;
            }
            DeleteUnitExtra(unitDesc.Guid);
            if (tableUnit.EUnitType == EUnitType.MainPlayer)
            {
                _mainPlayer = null;
            }
            return true;
        }

        #region ExtraData

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

        public void ProcessUnitExtra(IntVec3 guid, UnitExtra unitExtra)
        {
            if (null != EditMode.Instance) {
                EditMode.Instance.MapStatistics.NeedSave = true;
            }
            if (unitExtra.Equals(UnitExtra.zero))
            {
                DeleteUnitExtra(guid);
            }
            else
            {
                _unitExtras.AddOrReplace(guid, unitExtra);
            }
            // 更新unit
            UnitBase unit;
            if (ColliderScene2D.Instance.TryGetUnit(guid, out unit))
            {
                unit.UpdateExtraData ();
            }
        }

        public void ProcessUnitChild(IntVec3 guid, UnitChild unitChild)
        {
            UnitExtra unitExtra = GetUnitExtra(guid);
            unitExtra.Child = unitChild;
            ProcessUnitExtra(guid, unitExtra);
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
                if (ColliderScene2D.Instance.TryGetUnit(unitsGuid[i], out unit))
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
            Dictionary<IntVec3, List<IntVec3>>.Enumerator itor = _switchedUnits.GetEnumerator();
            while (itor.MoveNext())
            {
                List<IntVec3> units = itor.Current.Value;
                if (units.Contains(guid))
                {
                    result.Add(itor.Current.Key);
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
            return true;
        }

        #endregion

        #region Physics

        private static readonly List<UnitDesc> _cachedUnitObjects = new List<UnitDesc>();

        internal static bool PointCast(IntVec2 point, out SceneNode sceneNode,int layerMask = JoyPhysics2D.LayMaskAll, float minDepth = float.MinValue, float maxDepth = float.MaxValue)
        {
            return SceneQuery2D.PointCast(point, out sceneNode, layerMask, Instance, minDepth, maxDepth);
        }

        internal static bool GridCast(Grid2D grid2D, out SceneNode node, int layerMask = JoyPhysics2D.LayMaskAll, float minDepth = float.MinValue, float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        {
            return SceneQuery2D.GridCast(ref grid2D, out node, layerMask, Instance, minDepth, maxDepth, excludeNode);
        }

        internal static List<SceneNode> GridCastAll(Grid2D grid2D, int layerMask = JoyPhysics2D.LayMaskAll, float minDepth = float.MinValue, float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        {
            return SceneQuery2D.GridCastAll(ref grid2D, layerMask, Instance, minDepth, maxDepth, excludeNode);
        }

        internal static List<UnitDesc> GridCastAllReturnUnits(UnitDesc unitDesc, int layerMask = JoyPhysics2D.LayMaskAll)
        {
            Grid2D outValue;
            if (!TryGetGridByUnitObject(unitDesc, out outValue))
            {
                LogHelper.Error("TryGetGridByUnitObject falied! UnitDesc is {0}", unitDesc);
                return null;
            }
            return GridCastAllReturnUnits(outValue, layerMask);
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

        public static List<UnitDesc> GridCastAllReturnUnits(Grid2D grid2D, int layerMask = JoyPhysics2D.LayMaskAll, float minDepth = float.MinValue, float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        {
            _cachedUnitObjects.Clear();
            var nodes = GridCastAll(grid2D, layerMask, minDepth, maxDepth, excludeNode);
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
    }

	public struct ModifyData {
		public UnitEditData OrigUnit;
		public UnitEditData ModifiedUnit;
		public ModifyData (UnitEditData orig, UnitEditData modified) {
			OrigUnit = orig;
			ModifiedUnit = modified;
		}
        // 从地图文件方序列化出来的构造函数
        public ModifyData (SoyEngine.Proto.ModifyItemData modifyItemData) {
            OrigUnit = new UnitEditData ();
            ModifiedUnit = new UnitEditData ();
            if (DataScene2D.Instance == null) {
                LogHelper.Error ("Instantiate modifyData failed, datascene2d not exist");
                return;
            }
            Table_Unit table = TableManager.Instance.GetUnit (modifyItemData.OrigData.Id);
            if (null == table) {
                LogHelper.Error ("ParseModifyData error, unit with invalid id {0}", modifyItemData.OrigData.Id);
                return;
            }
            int depth = UnitManager.GetDepth (table);
            UnitDesc origDesc = new UnitDesc (
                modifyItemData.OrigData.Id,
                new IntVec3(modifyItemData.OrigData.XMin, modifyItemData.OrigData.YMin, depth),
                (byte)modifyItemData.OrigData.Rotation,
                new Vector2(modifyItemData.OrigData.Scale== null ? 1 : modifyItemData.OrigData.Scale.X,
                    modifyItemData.OrigData.Scale==null ? 1 : modifyItemData.OrigData.Scale.Y)
            );
            UnitExtra origExtra = new UnitExtra ();
            origExtra.Child = new UnitChild (
                (ushort)modifyItemData.OrigExtra.UnitChild.Id,
                (byte)modifyItemData.OrigExtra.UnitChild.Rotation,
                (EMoveDirection)modifyItemData.OrigExtra.UnitChild.MoveDirection
            );
            origExtra.MoveDirection = (EMoveDirection)modifyItemData.OrigExtra.MoveDirection;
            origExtra.RollerDirection = (EMoveDirection)modifyItemData.OrigExtra.RollerDirection;
            OrigUnit = new UnitEditData (origDesc, origExtra);
            UnitDesc modifiedDesc = new UnitDesc (
                modifyItemData.ModifiedData.Id,
                new IntVec3(modifyItemData.ModifiedData.XMin, modifyItemData.ModifiedData.YMin, depth),
                (byte)modifyItemData.ModifiedData.Rotation,
                new Vector2(modifyItemData.ModifiedData.Scale== null ? 1 : modifyItemData.ModifiedData.Scale.X,
                    modifyItemData.ModifiedData.Scale==null ? 1 : modifyItemData.ModifiedData.Scale.Y)
            );
            UnitExtra modifiedExtra;
            DataScene2D.Instance.TryGetUnitExtra (modifiedDesc.Guid, out modifiedExtra);
            ModifiedUnit = new UnitEditData (modifiedDesc, modifiedExtra);
        }

        public SoyEngine.Proto.ModifyItemData ToModifyItemData () {
            SoyEngine.Proto.ModifyItemData mid = new SoyEngine.Proto.ModifyItemData ();
            mid.OrigData = new SoyEngine.Proto.MapRect2D ();
            mid.OrigData.Id = OrigUnit.UnitDesc.Id;
            mid.OrigData.Rotation = (int)OrigUnit.UnitDesc.Rotation;
            mid.OrigData.Scale = new SoyEngine.Proto.Vec2Proto ();
            mid.OrigData.Scale.X = OrigUnit.UnitDesc.Scale.x;
            mid.OrigData.Scale.Y = OrigUnit.UnitDesc.Scale.y;
            mid.OrigData.XMin = OrigUnit.UnitDesc.Guid.x;
            mid.OrigData.XMax = OrigUnit.UnitDesc.Guid.x + ConstDefineGM2D.ServerTileScale;
            mid.OrigData.YMin = OrigUnit.UnitDesc.Guid.y;
            mid.OrigData.YMax = OrigUnit.UnitDesc.Guid.y + ConstDefineGM2D.ServerTileScale;
            mid.OrigExtra = GM2DTools.ToProto (OrigUnit.UnitDesc.Guid, OrigUnit.UnitExtra);

            mid.ModifiedData = new SoyEngine.Proto.MapRect2D ();
            mid.ModifiedData.Id = ModifiedUnit.UnitDesc.Id;
            mid.ModifiedData.Rotation = (int)ModifiedUnit.UnitDesc.Rotation;
            mid.ModifiedData.Scale = new SoyEngine.Proto.Vec2Proto ();
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
