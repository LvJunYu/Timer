/********************************************************************
** Filename : ColliderScene2D
** Author : Dong
** Date : 2016/10/4 星期二 下午 4:31:12
** Summary : ColliderScene2D
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    [Serializable]
    public class ColliderScene2D : Scene2D
    {
        private static ColliderScene2D _instance;
        private readonly Dictionary<IntVec3, UnitBase> _units = new Dictionary<IntVec3, UnitBase>();
        
        [SerializeField] private readonly List<UnitBase> _allSwitchUnits = new List<UnitBase>();
        [SerializeField] private readonly List<UnitBase> _allMagicUnits = new List<UnitBase>();
        [SerializeField] private readonly List<UnitBase> _allBulletUnits = new List<UnitBase>();
        [SerializeField] private readonly List<UnitBase> _allOtherUnits = new List<UnitBase>();

        [SerializeField] private readonly List<ColliderDesc> _allColliderDescs = new List<ColliderDesc>();
        private Comparison<UnitBase> _comparisonMoving = SortRectIndex;
        private InterestArea _interestArea;
        private byte[,] _pathGrid;

        public static ColliderScene2D Instance
        {
            get { return _instance ?? (_instance = new ColliderScene2D()); }
        }

        public Dictionary<IntVec3, UnitBase> Units
        {
            get { return _units; }
        }

        public List<UnitBase> AllOtherUnits
        {
            get { return _allOtherUnits; }
        }

        public List<UnitBase> AllSwitchUnits
        {
            get { return _allSwitchUnits; }
        }

        public List<UnitBase> AllMagicUnits
        {
            get { return _allMagicUnits; }
        }

        public List<UnitBase> AllBulletUnits
        {
            get { return _allBulletUnits; }
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var unit in _units.Values)
            {
                if (unit != null)
                {
                    if (unit.View != null)
                    {
                        Object.Destroy(unit.View.Trans.gameObject);
                    }
                    unit.OnObjectDestroy();
                    unit.OnDispose();
                }
            }
            _units.Clear();
            _allSwitchUnits.Clear();
            _allMagicUnits.Clear();
            _allBulletUnits.Clear();
            _allOtherUnits.Clear();
            Messenger<NodeData[], Grid2D>.RemoveListener(EMessengerType.OnAOISubscribe, OnAOISubscribe);
            Messenger<NodeData[], Grid2D>.RemoveListener(EMessengerType.OnAOIUnsubscribe, OnAOIUnsubscribe);
            Messenger<SceneNode[], Grid2D>.RemoveListener(EMessengerType.OnDynamicSubscribe, OnDynamicSubscribe);
            Messenger<SceneNode[], Grid2D>.RemoveListener(EMessengerType.OnDynamicUnsubscribe, OnDynamicUnsubscribe);
            _instance = null;
        }

        public void Init(IntVec2 regionTilesCount, int width, int height)
        {
            Init(width, height);
            if (MapConfig.UseAOI)
            {
                InitRegions(regionTilesCount);
                _interestArea = new InterestArea(ConstDefineGM2D.RegionTileSize * 1.5f ,
                    ConstDefineGM2D.RegionTileSize * 2.5f, this);
//                _interestArea = new InterestArea(RegionTileSize * 1.5f, RegionTileSize * 2.5f, this);
            }
            _pathGrid = new byte[Mathf.NextPowerOfTwo(width / JoyConfig.ServerTileScale), Mathf.NextPowerOfTwo(height / JoyConfig.ServerTileScale)];
            for (int i = 0; i < _pathGrid.GetLength(0); i++)
            {
                for (int j = 0; j < _pathGrid.GetLength(1); j++)
                {
                    _pathGrid[i, j] = 1;
                }
            }
            InitPathFinder(_pathGrid);
        }

        protected override void OnInit()
        {
            Messenger<NodeData[], Grid2D>.AddListener(EMessengerType.OnAOISubscribe, OnAOISubscribe);
            Messenger<NodeData[], Grid2D>.AddListener(EMessengerType.OnAOIUnsubscribe, OnAOIUnsubscribe);
            Messenger<SceneNode[], Grid2D>.AddListener(EMessengerType.OnDynamicSubscribe, OnDynamicSubscribe);
            Messenger<SceneNode[], Grid2D>.AddListener(EMessengerType.OnDynamicUnsubscribe, OnDynamicUnsubscribe);
        }

        public void UpdateLogic(IntVec2 pos)
        {
            if (_interestArea != null)
            {
                _interestArea.Update(pos);
            }
        }

        public void InitCreateArea(IntVec2 pos)
        {
            if (_interestArea != null)
            {
                _interestArea.InitCreate(pos);
            }
        }

        public bool AddCollider(ColliderDesc colliderDesc)
        {
            if (_allColliderDescs.Contains(colliderDesc))
            {
                LogHelper.Warning("AddCollider Failed, {0}", colliderDesc);
                return false;
            }
            var colliderNode = NodeFactory.GetColliderNode(colliderDesc);
            if (!AddNode(colliderNode))
            {
                LogHelper.Error("AddCollider Failed,{0}", colliderDesc);
                return false;
            }
            _allColliderDescs.Add(colliderDesc);
            return true;
        }

        public bool DeleteCollider(ColliderDesc colliderDesc)
        {
            if (!_allColliderDescs.Contains(colliderDesc))
            {
                return false;
            }
            var colliderNode = NodeFactory.GetColliderNode(colliderDesc);
            if (!DeleteNode(colliderNode))
            {
                LogHelper.Error("DeleteCollider Failed,{0}", colliderDesc);
                return false;
            }
            _allColliderDescs.Remove(colliderDesc);
            return true;
        }
        
        public bool AddUnit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            if (_units.ContainsKey(unitDesc.Guid))
            {
                //LogHelper.Warning("AddUnit Failed, {0}", UnitDesc);
                return false;
            }
            var colliderNode = NodeFactory.GetColliderNode(unitDesc, tableUnit);
            if (colliderNode == null)
            { 
                return false;
            }
            if (!AddNode(colliderNode))
            {
                LogHelper.Error("AddUnit Failed,{0}", unitDesc.ToString());
                return false;
            }
            UnitBase unit = UnitManager.Instance.GetUnit(unitDesc.Id);
            if (!unit.Init(tableUnit, unitDesc, colliderNode.IsDynamic() ? colliderNode : null))
            {
                DeleteNode(colliderNode);
                return false;
            }
            _units.Add(unitDesc.Guid, unit);
            if (tableUnit.IsGround == 1)
            {
                _pathGrid[unitDesc.Guid.x / ConstDefineGM2D.ServerTileScale, unitDesc.Guid.y / ConstDefineGM2D.ServerTileScale] = 0;
            }
            if (UnitDefine.IsSwitchTrigger(unit.Id))
            {
                _allSwitchUnits.Add(unit);
            }
            else if (unit.UseMagic())
            {
                _allMagicUnits.Add(unit);
            }
            else if(UnitDefine.IsBullet(unit.Id))
            {
                _allBulletUnits.Add(unit);
            }
            else
            {
                _allOtherUnits.Add(unit);
            }
            return true;
        }

        public void SetGround(IntVec3 guid, bool flag)
        {
            _pathGrid[guid.x / ConstDefineGM2D.ServerTileScale, guid.y / ConstDefineGM2D.ServerTileScale] =
                (byte) (flag ? 0 : 1);
        }

        public bool DeleteUnit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            UnitBase unit;
            if (!_units.TryGetValue(unitDesc.Guid, out unit))
            {
                LogHelper.Warning("DeleteUnit Failed, {0}", unitDesc);
                return true;
            }
            var colliderNode = NodeFactory.GetColliderNode(unitDesc, tableUnit);
            if (colliderNode == null)
            {
                LogHelper.Error("GetColliderNode Failed,{0}", unitDesc.ToString());
                return false;
            }
            if (!DeleteNode(colliderNode))
            {
                LogHelper.Error("DeleteNode Failed,{0}", unitDesc.ToString());
                return false;
            }
            unit.OnDispose();
            if (tableUnit.IsGround == 1)
            {
                _pathGrid[unitDesc.Guid.x / ConstDefineGM2D.ServerTileScale, unitDesc.Guid.y / ConstDefineGM2D.ServerTileScale] = 1;
            }
            if (UnitDefine.IsSwitchTrigger(unit.Id))
            {
                _allSwitchUnits.Remove(unit);
            }
            else if (unit.UseMagic())
            {
                _allMagicUnits.Remove(unit);
            }
            else if(UnitDefine.IsBullet(unit.Id))
            {
                _allBulletUnits.Remove(unit);
            }
            else
            {
                _allOtherUnits.Remove(unit);
            }
            return _units.Remove(unitDesc.Guid);
        }

        public bool InstantiateView(UnitDesc unitDesc, Table_Unit tableUnit, SceneNode node = null)
        {
            UnitBase unit;
            if (!_units.TryGetValue(unitDesc.Guid, out unit))
            {
                LogHelper.Warning("InstantiateView Failed, {0}", unitDesc);
                return false;
            }
            if (unit.View !=null)
            {
                return false;
            }
            if (!unit.InstantiateView())
            {
                return false;
            }
            if (unit.TableUnit.EGeneratedType == EGeneratedType.Morph)
            {
                unit.DoProcessMorph(true);
            }
            return true;
        }

        public bool DestroyView(UnitDesc unitDesc)
        {
            UnitBase unit;
            if (!_units.TryGetValue(unitDesc.Guid, out unit))
            {
                LogHelper.Warning("DestroyView Failed, {0}", unitDesc);
                return false;
            }
            if (unit.TableUnit.EGeneratedType == EGeneratedType.Morph)
            {
                unit.DoProcessMorph(false);
            }
            UnitManager.Instance.FreeUnitView(unit);
            return true;
        }

        public bool TryGetUnit(IntVec3 guid, out UnitBase unit)
        {
            if (!_units.TryGetValue(guid, out unit))
            {
                return false;
            }
            return !unit.IsFreezed;
        }

        public bool TryGetUnit(SceneNode colliderNode, out UnitBase unit)
        {
            var tableUnit = UnitManager.Instance.GetTableUnit(colliderNode.Id);
            if (tableUnit == null)
            {
                unit = null;
                return false;
            }
            return _units.TryGetValue(tableUnit.ColliderToRenderer(colliderNode.Guid, colliderNode.Rotation), out unit);
        }

        #region AOI

        public bool UpdateDynamicUnit(UnitBase unit, Grid2D lastGrid)
        {
            if (UpdateDynamicNode(unit.DynamicCollider))
            {
                if (unit.TableUnit.IsGround == 1)
                {
                    _pathGrid[lastGrid.XMin / ConstDefineGM2D.ServerTileScale, lastGrid.YMin / ConstDefineGM2D.ServerTileScale] = 1;
                    _pathGrid[unit.ColliderGrid.XMin / ConstDefineGM2D.ServerTileScale, unit.ColliderGrid.YMin / ConstDefineGM2D.ServerTileScale] = 0;
                }
                return true;
            }
            return false;
        }

        public void Reset()
        {
            for (int i = 0; i < _allSwitchUnits.Count; i++)
            {
                _allSwitchUnits[i].Reset();
            }
            for (int i = 0; i < _allMagicUnits.Count; i++)
            {
                _allMagicUnits[i].Reset();
            }
            for (int i = 0; i < _allOtherUnits.Count; i++)
            {
                _allOtherUnits[i].Reset();
            }
            for (int i = 0; i < _allColliderDescs.Count; i++)
            {
                DeleteCollider(_allColliderDescs[i]);
            }
            _allColliderDescs.Clear();
        }

        #region Comparison SortData

        public void SortData()
        {
            _allSwitchUnits.Sort(_comparisonMoving);
            _allMagicUnits.Sort(_comparisonMoving);
            _allOtherUnits.Sort(_comparisonMoving);
            Sort();
        }

        private static int SortRectIndex(UnitBase one, UnitBase other)
        {
            int v = one.Guid.x.CompareTo(other.Guid.x);
            if (v == 0)
            {
                v = one.Guid.y.CompareTo(other.Guid.y);
                if (v == 0)
                {
                    v = one.Guid.z.CompareTo(other.Guid.z);
                }
            }
            return v;
        }
   
        #endregion

        private void OnAOISubscribe(NodeData[] nodes, Grid2D interGrid)
        {
            //LogHelper.Debug("OnAOISubscribe : {0}", nodes.Length);
            ProcessAOI(nodes, interGrid, true);
        }

        private void OnAOIUnsubscribe(NodeData[] nodes, Grid2D outerGrid)
        {
            //LogHelper.Debug("OnAOIUnsubscribe : {0}", nodes.Length);
            ProcessAOI(nodes, outerGrid, false);
        }

        private void OnDynamicSubscribe(SceneNode[] nodes, Grid2D interGrid)
        {
            //LogHelper.Debug("OnDynamicSubscribe:{0}", nodes.Length);
            ProcessDynamicAOI(nodes, interGrid, true);
        }

        private void OnDynamicUnsubscribe(SceneNode[] nodes, Grid2D outerGrid)
        {
            //LogHelper.Debug("OnDynamicUnsubscribe:{0}", nodes.Length);
            ProcessDynamicAOI(nodes, outerGrid, false);
        }

        private void ProcessAOI(NodeData[] nodes, Grid2D grid, bool isSubscribe)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(node.Id);
                if (tableUnit == null)
                {
                    LogHelper.Error("ProcessAOI Failed,GetTableUnit:{0}", node);
                    return;
                }
                if (!isSubscribe && !CheckCanDelete(tableUnit))
                {
                    continue;
                }
                var unitObject = new UnitDesc();
                unitObject.Id = node.Id;
                unitObject.Rotation = node.Direction;
                unitObject.Scale = node.Scale;
                unitObject.Guid.z = node.Depth;
                var count = tableUnit.GetColliderCount(node);
                var size = tableUnit.GetColliderSize(node.Direction, node.Scale);
                for (int j = 0; j < count.x; j++)
                {
                    for (int k = 0; k < count.y; k++)
                    {
                        unitObject.Guid.x = node.Grid.XMin + j * size.x;
                        unitObject.Guid.y = node.Grid.YMin + k * size.y;
                        unitObject.Guid = tableUnit.ColliderToRenderer(unitObject.Guid, unitObject.Rotation);
                        if (isSubscribe)
                        {
                            if (!grid.Contains(tableUnit.GetBaseDataGrid(unitObject.Guid)))
                            {
                                continue;
                            }
                            InstantiateView(unitObject, tableUnit);
                        }
                        else
                        {
                            if (grid.Contains(tableUnit.GetBaseDataGrid(unitObject.Guid)))
                            {
                                continue;
                            }
                            DestroyView(unitObject);
                        }
                    }
                }
            }
        }

        private void ProcessDynamicAOI(SceneNode[] nodes, Grid2D grid, bool isSubscribe)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(node.Id);
                if (tableUnit == null)
                {
                    LogHelper.Error("ProcessDynamicAOI Failed,GetTableUnit:{0}", node);
                    return;
                }
                if (!isSubscribe && !CheckCanDelete(tableUnit))
                {
                    continue;
                }
                var unitObject = new UnitDesc();
                unitObject.Id = node.Id;
                unitObject.Rotation = node.Rotation;
                unitObject.Scale = node.Scale;
                unitObject.Guid = tableUnit.ColliderToRenderer(node.Guid, node.Rotation);
                if (isSubscribe)
                {
                    if (!grid.Contains(node.Grid))
                    {
                        continue;
                    }
                    InstantiateView(unitObject, tableUnit, node);
                }
                else
                {
                    if (grid.Contains(node.Grid))
                    {
                        continue;
                    }
                    DestroyView(unitObject);
                }
            }
        }

        private bool CheckCanDelete(Table_Unit tableUnit)
        {
            //if (tableUnit.EUnitType == EUnitType.Monster || tableUnit.Id == 65535 || UnitDefine.IsSwitch(tableUnit.Id))
            //{
            //    return false;
            //}
            return true;
        }

        #endregion

        #region Physics

        private static readonly List<UnitBase> _cachedUnits = new List<UnitBase>();

        internal static bool PointCast(IntVec2 point, out SceneNode sceneNode, int layerMask = JoyPhysics2D.LayMaskAll, float minDepth = float.MinValue, float maxDepth = float.MaxValue)
        {
            return SceneQuery2D.PointCast(point, out sceneNode, layerMask, _instance, minDepth, maxDepth);
        }

        internal static bool Raycast(IntVec2 origin, Vector2 direction, out RayHit2D hit, float distance = ConstDefineGM2D.MaxMapDistance,
            int layerMask = JoyPhysics2D.LayMaskAll, SceneNode excludeNode = null)
        {
            return SceneQuery2D.Raycast(new Vector2(origin.x, origin.y), direction, out hit, distance, layerMask,
                Instance, float.MinValue, float.MaxValue, excludeNode);
        }

        internal static List<RayHit2D> RaycastAll(IntVec2 origin, Vector2 direction, float distance = ConstDefineGM2D.MaxMapDistance,
            int layerMask = JoyPhysics2D.LayMaskAll, float minDepth = float.MinValue, float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        {
            return SceneQuery2D.RaycastAll(new Vector2(origin.x, origin.y), direction, distance, layerMask, Instance,
                float.MinValue, float.MaxValue, excludeNode);
        }
        
        public static List<UnitBase>  RaycastAllReturnUnits(IntVec2 origin, Vector2 direction, float distance = ConstDefineGM2D.MaxMapDistance, int layerMask = JoyPhysics2D.LayMaskAll,
            float minDepth = float.MinValue, float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        {
            _cachedUnits.Clear();
            var hits = RaycastAll(origin, direction, distance , layerMask, minDepth, maxDepth, excludeNode);
            for (int i = 0; i < hits.Count; i++)
            {
                var hit = hits[i];
                var tile = hit.point - new IntVec2(hit.normal.x > 0 ? 1 : hit.normal.x < 0 ? -1 : 0, hit.normal.y > 0 ? 1 : hit.normal.y < 0 ? -1 : 0);
                GetUnits(hit.node, new Grid2D(tile.x, tile.y, tile.x, tile.y), _cachedUnits);
            }
            return _cachedUnits;
        }

        internal static bool GridCast(Grid2D grid2D, out SceneNode node, int layerMask = JoyPhysics2D.LayMaskAll,
            float minDepth = float.MinValue, float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        {
            return SceneQuery2D.GridCast(ref grid2D, out node, layerMask, Instance, minDepth, maxDepth,
                excludeNode);
        }

        internal static bool GridCast(IntVec2 pointA, IntVec2 pointB, byte direction, out GridHit2D hit, int distance = ConstDefineGM2D.MaxMapDistance,
            int layerMask = JoyPhysics2D.LayMaskAll, float minDepth = float.MinValue,
            float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        {
            return SceneQuery2D.GridCast(pointA, pointB, direction, out hit, distance, layerMask, Instance, minDepth, maxDepth, excludeNode);
        }

        internal static List<SceneNode> GridCastAll(Grid2D grid2D, int layerMask = JoyPhysics2D.LayMaskAll, float minDepth = float.MinValue,
            float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        {
            return SceneQuery2D.GridCastAll(ref grid2D, layerMask, Instance, minDepth, maxDepth,
                excludeNode);
        }

        internal static List<GridHit2D> GridCastAll(IntVec2 pointA, IntVec2 pointB, byte direction, int distance = ConstDefineGM2D.MaxMapDistance, int layerMask = JoyPhysics2D.LayMaskAll, float minDepth = float.MinValue,
            float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        {
            return SceneQuery2D.GridCastAll(pointA, pointB, direction, distance, layerMask, Instance, minDepth, maxDepth,
                excludeNode);
        }

        internal static List<GridHit2D> GridCastAll(Grid2D grid, byte direction, int layerMask = JoyPhysics2D.LayMaskAll, float minDepth = float.MinValue,
            float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        {
            return SceneQuery2D.GridCastAll(ref grid, direction, layerMask, Instance, minDepth, maxDepth,excludeNode);
        }

        public static List<UnitBase> GridCastAllReturnUnits(Grid2D one, int layerMask = JoyPhysics2D.LayMaskAll,
            float minDepth = float.MinValue, float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        {
            _cachedUnits.Clear();
            var nodes = SceneQuery2D.GridCastAll(ref one, layerMask, Instance, minDepth, maxDepth, excludeNode);
            for (int i = 0; i < nodes.Count; i++)
            {
                GetUnits(nodes[i], one, _cachedUnits);
            }
            return _cachedUnits;
        }

        public static List<UnitBase> GetUnits(RayHit2D hit)
        {
            _cachedUnits.Clear();
            var tile = hit.point - new IntVec2(hit.normal.x > 0 ? 1 : hit.normal.x < 0 ? -1 : 0, hit.normal.y > 0 ? 1 : hit.normal.y < 0 ? -1 : 0);
            GetUnits(hit.node, new Grid2D(tile.x, tile.y, tile.x, tile.y), _cachedUnits);
            return _cachedUnits;
        }

        public static List<UnitBase> GetUnits(GridHit2D hit, Grid2D one)
        {
            _cachedUnits.Clear();
            GetUnits(hit.node, one, _cachedUnits);
            return _cachedUnits;
        }

        private static bool GetUnits(SceneNode node, Grid2D one, List<UnitBase> units)
        {
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(node.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("GetTableUnit failed.{0}", node.Id);
                return false;
            }
            if (node.IsDynamic())
            {
                IntVec3 guid = tableUnit.ColliderToRenderer(node.Guid, node.Rotation);
                UnitBase unit;
                if (!_instance.TryGetUnit(guid, out unit))
                {
                    //LogHelper.Warning("TryGetUnits failed,{0}", node);
                    return false;
                }
                units.Add(unit);
            }
            else
            {
                Grid2D newGrid = GM2DTools.IntersectWith(one, node, tableUnit, false);
                IntVec2 size = tableUnit.GetColliderSize(node);
                IntVec2 count = tableUnit.GetColliderCount(newGrid, node.Rotation, node.Scale);
                for (int j = 0; j < count.x; j++)
                {
                    for (int k = 0; k < count.y; k++)
                    {
                        var guid = new IntVec3(newGrid.XMin + j * size.x, newGrid.YMin + k * size.y, node.Depth);
                        guid = tableUnit.ColliderToRenderer(guid, node.Rotation);
                        UnitBase unit;
                        if (!_instance.TryGetUnit(guid, out unit))
                        {
                            continue;
                        }
                        units.Add(unit);
                    }
                }
            }
            return true;
        }
        
        public static List<SceneNode> CircleCastAll(IntVec2 center, int radius, int layerMask = JoyPhysics2D.LayMaskAll,
            float minDepth = float.MinValue, float maxDepth = float.MaxValue,
            SceneNode excludeNode = null)
        {
            return SceneQuery2D.CircleCastAll(center, radius, layerMask, Instance, minDepth, maxDepth,
                excludeNode);
        }

        public static List<UnitBase> CircleCastAllReturnUnits(IntVec2 center, int radius, int layerMask = JoyPhysics2D.LayMaskAll, float minDepth = float.MinValue, float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        {
            _cachedUnits.Clear();
            var grid = new Grid2D(center.x - radius, center.y - radius, center.x + radius - 1, center.y + radius - 1);
            grid = grid.IntersectWith(DataScene2D.Instance.RootGrid);
            List<SceneNode> nodes = SceneQuery2D.CircleCastAll(center, radius, layerMask, Instance, minDepth, maxDepth, excludeNode);
            for (int i = 0; i < nodes.Count; i++)
            {
                if (!SplitNode(center, radius, grid, nodes[i]))
                {
                    LogHelper.Error("SplitNode failed.{0}, {1}", nodes[i], grid);
                }
            }
            return _cachedUnits;
        }

        private static bool SplitNode(IntVec2 center, int radius, Grid2D grid, SceneNode node)
        {
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(node.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("GetTableUnit failed.{0}", node.Id);
                return false;
            }
            //动态物体
            if (node.IsDynamic())
            {
                var guid = tableUnit.ColliderToRenderer(node.Guid, node.Rotation);
                UnitBase unit;
                if (!_instance.TryGetUnit(guid, out unit))
                {
                    return false;
                }
                _cachedUnits.Add(unit);
                return true;
            }

            Grid2D newGrid = GM2DTools.IntersectWith(grid, node, tableUnit, false);
            var colliderSize = tableUnit.GetColliderSize(node);
            var count = tableUnit.GetColliderCount(newGrid, node.Rotation, node.Scale);
            for (int j = 0; j < count.x; j++)
            {
                for (int k = 0; k < count.y; k++)
                {
                    var guid = new IntVec3(newGrid.XMin + j * colliderSize.x, newGrid.YMin + k * colliderSize.y, node.Depth);
                    guid = tableUnit.ColliderToRenderer(guid, node.Rotation);
                    var splitedGrid = tableUnit.GetColliderGrid(guid.x, guid.y, node.Rotation, node.Scale);
                    if (splitedGrid.Intersect(center, radius))
                    {
                        UnitBase unit;
                        if (!_instance.TryGetUnit(guid, out unit))
                        {
                            continue;
                        }
                        _cachedUnits.Add(unit);
                    }
                }
            }
            return true;
        }

        #endregion

        #region path

        public List<IntVec2> FindPath(UnitBase unit, UnitBase target, short maxCharacterJumpHeight)
        {
            var start = unit.CenterDownPos ;
            var end = target.DownUnit == null ? target.CenterDownPos : target.DownUnit.CenterUpFloorPos;
            //不在空中的时候
            if (!target.Grounded)
            {
                var hits = RaycastAll(target.CenterDownPos, Vector2.down, GM2DTools.WorldToTile(30f),
                    EnvManager.UnitLayer, float.MinValue, float.MaxValue, unit.DynamicCollider);
                for (int i = 0; i < hits.Count; i++)
                {
                    var hit = hits[i];
                    var tableUnit = UnitManager.Instance.GetTableUnit(hit.node.Id);
                    if (tableUnit != null && tableUnit.IsGround == 1)
                    {
                        end.y -= hit.distance;
                        break;
                    }
                }
            }
            var size = unit.GetColliderSize() / ConstDefineGM2D.ServerTileScale;
            return FindPath(start / ConstDefineGM2D.ServerTileScale, end / ConstDefineGM2D.ServerTileScale, Math.Max(1, size.x), Math.Max(1, size.y), maxCharacterJumpHeight);
        }

        public override bool IsGround(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return false;
            }
            if (_pathGrid[x, y] == 0)
            {
                return true;
            }
            return false;
        }

        #endregion

        internal bool HasBlockInLine(int x, int y0, int y1)
        {
            int startY, endY;
            if (y0 <= y1)
            {
                startY = y0;
                endY = y1;
            }
            else
            {
                startY = y1;
                endY = y0;
            }
            for (int y = startY; y <= endY; ++y)
            {
                if (IsGround(x, y))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
