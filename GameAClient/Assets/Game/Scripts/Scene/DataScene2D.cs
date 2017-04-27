/********************************************************************
** Filename : DataScene2D
** Author : Dong
** Date : 2016/10/3 星期一 下午 8:16:34
** Summary : DataScene2D
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
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

            return new IntVec3(x, y, UnitManager.Instance.GetDepth(tableUnit));
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
			Debug.Log ("______________________________ dataScene2D.AddData");
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
            if (tableUnit.Id == ConstDefineGM2D.RollerId)
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
			Debug.Log ("______________________________ dataScene2D.DelData");
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
            if (unitExtra.Equals(UnitExtra.zero))
            {
                DeleteUnitExtra(guid);
                return;
            }
            _unitExtras.AddOrReplace(guid, unitExtra);
            // 更新unit
            UnitBase unit = null;
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

        public List<UnitBase> GetSwitchedUnits(IntVec3 guid)
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
            return _cachedUnits;
        }

        public void BindSwitch(IntVec3 switchGuid, IntVec3 unitGuid)
        {
            if (!_switchedUnits.ContainsKey(switchGuid))
            {
                _switchedUnits.Add(switchGuid, new List<IntVec3>());
            }
            _switchedUnits[switchGuid].Add(unitGuid);
        }

        public void UnbindSwitch(IntVec3 switchGuid, IntVec3 unitGuid)
        {
            List<IntVec3> unitsGuid;
            if (!_switchedUnits.TryGetValue(switchGuid, out unitsGuid))
            {
                LogHelper.Error("UnbindSwitch Failed, {0}, {1}", switchGuid, unitGuid);
                return;
            }
            unitsGuid.Remove(unitGuid);
            if (unitsGuid.Count == 0)
            {
                _switchedUnits.Remove(switchGuid);
            }
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
	}
}
