/********************************************************************
** Filename : ColliderScene2D
** Author : Dong
** Date : 2016/10/4 星期二 下午 4:31:12
** Summary : ColliderScene2D
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JoyEngine;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Serializable]
    public class ColliderScene2D : Scene2D
    {
        private static ColliderScene2D _instance;
        private readonly Dictionary<IntVec3, UnitBase> _units = new Dictionary<IntVec3, UnitBase>();
        [SerializeField] private readonly List<UnitBase> _allUnits = new List<UnitBase>();
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

        public List<UnitBase> AllUnits
        {
            get { return _allUnits; }
        }


        public override void Dispose()
        {
            base.Dispose();
            foreach (var unit in _units.Values)
            {
                if (unit != null)
                {
                    unit.OnObjectDestroy();
                    unit.OnDispose();
                }
            }
            _units.Clear();
            _allUnits.Clear();
            Messenger<NodeData[], Grid2D>.RemoveListener(EMessengerType.OnAOISubscribe, OnAOISubscribe);
            Messenger<NodeData[], Grid2D>.RemoveListener(EMessengerType.OnAOIUnsubscribe, OnAOIUnsubscribe);
            Messenger<SceneNode[], Grid2D>.RemoveListener(EMessengerType.OnDynamicSubscribe, OnDynamicSubscribe);
            Messenger<SceneNode[], Grid2D>.RemoveListener(EMessengerType.OnDynamicUnsubscribe, OnDynamicUnsubscribe);
            _instance = null;
        }

        public void Init(IntVec2 regionTilesCount, int width, int height)
        {
            Init(width, height);
            InitRegions(regionTilesCount);
            _interestArea = new InterestArea(RegionTileSize * 1.5f, RegionTileSize * 2.5f, this);
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
            if (unit.IsMain)
            {
                PlayMode.Instance.MainUnit = (MainUnit)unit;
            }
            else
            {
                _pathGrid[unitDesc.Guid.x / ConstDefineGM2D.ServerTileScale, unitDesc.Guid.y / ConstDefineGM2D.ServerTileScale] = 0;
            }
            _allUnits.Add(unit);
            return true;
        }

        public bool DeleteUnit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            var colliderNode = NodeFactory.GetColliderNode(unitDesc, tableUnit);
            if (colliderNode == null)
            {
                LogHelper.Error("DeleteCollider Failed,{0}", unitDesc.ToString());
                return false;
            }
            if (!DeleteNode(colliderNode))
            {
                LogHelper.Error("DeleteCollider Failed,{0}", unitDesc.ToString());
                return false;
            }
            UnitBase unit;
            if (!_units.TryGetValue(unitDesc.Guid, out unit))
            {
                //LogHelper.Warning("DeleteUnit Failed, {0}", UnitDesc);
                return true;
            }
            _allUnits.Remove(unit);
            _pathGrid[unitDesc.Guid.x / ConstDefineGM2D.ServerTileScale, unitDesc.Guid.y / ConstDefineGM2D.ServerTileScale] = 1;
            UnitManager.Instance.FreeUnitView(unit);
            return _units.Remove(unitDesc.Guid);
        }

        public bool InstantiateView(UnitDesc unitDesc, Table_Unit tableUnit, SceneNode node = null)
        {
            UnitBase unit;
            if (!_units.TryGetValue(unitDesc.Guid, out unit))
            {
                //LogHelper.Warning("InstantiateView Failed, {0}", UnitDesc);
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
                //LogHelper.Warning("DeleteUnit Failed, {0}", UnitDesc);
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

        public void Reset()
        {
            //foreach (var dynamicNode in _dynamicNodes)
            //{
            //    dynamicNode.Value.Reset();
            //}
        }

        #region Comparison SortData

        public void SortData()
        {
            _allUnits.Sort(_comparisonMoving);
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
            if (tableUnit.EUnitType == EUnitType.MainPlayer || tableUnit.Id == 65535 || UnitDefine.Instance.IsSwitch(tableUnit.Id))
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Physics

        private static List<RayHit2D> _cachedRayHits = new List<RayHit2D>();
        private static readonly List<UnitDesc> _cachedUnitObjects = new List<UnitDesc>();
        private static readonly List<UnitBase> _cachedUnits = new List<UnitBase>();

        internal static bool PointCast(Vector2 point, out SceneNode sceneNode, int layerMask = JoyPhysics2D.LayMaskAll, float minDepth = float.MinValue, float maxDepth = float.MaxValue)
        {
            return SceneQuery2D.PointCast(GM2DTools.WorldToTile(point), out sceneNode, layerMask, _instance, minDepth, maxDepth);
        }

        internal static bool PointCast(IntVec2 point, out SceneNode sceneNode, int layerMask = JoyPhysics2D.LayMaskAll, float minDepth = float.MinValue, float maxDepth = float.MaxValue)
        {
            return SceneQuery2D.PointCast(point, out sceneNode, layerMask, _instance, minDepth, maxDepth);
        }

        //public static bool Raycast(Vector2 origin, Vector2 direction, out RayHit2D hit, float distance, int layerMask, Color color)
        //{
        //    Debug.DrawRay(origin, direction * distance, color);
        //    return RaycastInternal(origin, direction, out hit, distance, layerMask, false);
        //}

        //public static bool RaycastQueryInCollider(Vector2 origin, Vector2 direction, out RayHit2D hit, float distance, int layerMask = JoyPhysics2D.LayMaskAll)
        //{
        //    Debug.DrawRay(origin, direction * distance, Color.white);
        //    return RaycastInternal(origin, direction, out hit, distance, layerMask, true);
        //}

        //private static bool RaycastInternal(Vector2 orgin, Vector2 direction, out RayHit2D hit, float distance, int layerMask, bool inCollider)
        //{
        //    hit = new RayHit2D();
        //    List<RayHit2D> hits = inCollider ? RaycastAllInternalQueryInCollider(orgin, direction, distance, layerMask) : RaycastAllInternal(orgin, direction, distance, layerMask);
        //    var count = hits.Count;
        //    if (count == 0)
        //    {
        //        return false;
        //    }
        //    for (int i = 0; i < count; i++)
        //    {
        //        hit = hits[i];
        //        var tableUnit = UnitManager.Instance.GetTableUnit(hits[i].node.Id);
        //        if (tableUnit == null)
        //        {
        //            LogHelper.Error("Raycast Failed, GetTableUnit:{0}", hits[i].node.Id);
        //            continue;
        //        }
        //        //物体是单向的时候
        //        if (tableUnit.ColliderDirection != 15)
        //        {
        //            int dir = GM2DTools.GetDirection(hits[i].normal);
        //            if ((dir & GM2DTools.GetCurrentColliderDirection(tableUnit, hits[i].node.Rotation)) == 0)
        //            {
        //                continue;
        //            }
        //        }
        //        return true;
        //    }
        //    return false;
        //}

        //public static List<RayHit2D> RaycastAll(Vector2 orgin, Vector2 direction, float distance, int layerMask, bool inCollider = false)
        //{
        //    List<RayHit2D> hits = inCollider ? RaycastAllInternalQueryInCollider(orgin, direction, distance, layerMask) : RaycastAllInternal(orgin, direction, distance, layerMask);
        //    var count = hits.Count;
        //    if (count == 0)
        //    {
        //        return null;
        //    }
        //    _cachedRayHits.Clear();
        //    for (int i = 0; i < count; i++)
        //    {
        //        var tableUnit = UnitManager.Instance.GetTableUnit(hits[i].node.Id);
        //        if (tableUnit == null)
        //        {
        //            LogHelper.Error("Raycast Failed, GetTableUnit:{0}", hits[i].node.Id);
        //            continue;
        //        }
        //        //物体是单向的时候
        //        if (tableUnit.ColliderDirection != 15)
        //        {
        //            int dir = GM2DTools.GetDirection(hits[i].normal);
        //            if ((dir & GM2DTools.GetCurrentColliderDirection(tableUnit, hits[i].node.Rotation)) == 0)
        //            {
        //                continue;
        //            }
        //        }
        //        _cachedRayHits.Add(hits[i]);
        //    }
        //    return _cachedRayHits;
        //}

        //internal static List<RayHit2D> RaycastAllInternal(Vector2 origin, Vector2 direction, float distance,
        //    int layerMask = JoyPhysics2D.LayMaskAll,
        //    float minDepth = float.MinValue, float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        //{
        //    JoyPhysics2D.QueryInCollider = false;
        //    return SceneQuery2D.RaycastAll(origin * ConstDefineGM2D.ServerTileScale, direction,
        //        distance * ConstDefineGM2D.ServerTileScale, layerMask, Instance, minDepth, maxDepth,
        //        excludeNode);
        //}

        //internal static List<RayHit2D> RaycastAllInternalQueryInCollider(Vector2 origin, Vector2 direction, float distance,
        //    int layerMask = JoyPhysics2D.LayMaskAll,
        //    float minDepth = float.MinValue, float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        //{
        //    JoyPhysics2D.QueryInCollider = true;
        //    return SceneQuery2D.RaycastAll(origin * ConstDefineGM2D.ServerTileScale, direction,
        //        distance * ConstDefineGM2D.ServerTileScale, layerMask, Instance, minDepth, maxDepth,
        //        excludeNode);
        //}

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

        public static List<UnitBase> GridCastAllReturnUnits(Grid2D one, int layerMask = JoyPhysics2D.LayMaskAll,
            float minDepth = float.MinValue, float maxDepth = float.MaxValue, SceneNode excludeNode = null)
        {
            _cachedUnits.Clear();
            var nodes = SceneQuery2D.GridCastAll(ref one, layerMask, Instance, minDepth, maxDepth, excludeNode);
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(node.Id);
                if (tableUnit == null)
                {
                    LogHelper.Error("GetTableUnit failed.{0}", node.Id);
                    continue;
                }
                if (node.IsDynamic())
                {
                    var guid = tableUnit.ColliderToRenderer(node.Guid, node.Rotation);
                    UnitBase unit;
                    if (!_instance.TryGetUnit(guid, out unit))
                    {
                        //LogHelper.Warning("TryGetUnits failed,{0}", node);
                        continue;
                    }
                    _cachedUnits.Add(unit);
                }
                else
                {
                    var newGrid = GM2DTools.IntersectWith(one, node, tableUnit, false);
                    var size = tableUnit.GetColliderSize(node);
                    var count = tableUnit.GetColliderCount(newGrid, node.Rotation, node.Scale);
                    for (int j = 0; j < count.x; j++)
                    {
                        for (int k = 0; k < count.y; k++)
                        {
                            var guid = new IntVec3(newGrid.XMin + j * size.x, newGrid.YMin + k * size.y, node.Depth);
                            guid = tableUnit.ColliderToRenderer(guid, node.Rotation);
                            UnitBase unit;
                            if (!_instance.TryGetUnit(guid, out unit))
                            {
                                //LogHelper.Warning("TryGetUnit failed,{0}", node);
                                continue;
                            }
                            _cachedUnits.Add(unit);
                        }
                    }
                }
            }
            return _cachedUnits;
        }

        public static List<UnitBase> GetUnits(GridHit2D hit, Grid2D one)
        {
            _cachedUnits.Clear();
            SceneNode node = hit.node;
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(node.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("GetTableUnit failed.{0}", node.Id);
                return null;
            }
            if (node.IsDynamic())
            {
                IntVec3 guid = tableUnit.ColliderToRenderer(node.Guid, node.Rotation);
                UnitBase unit;
                if (!_instance.TryGetUnit(guid, out unit))
                {
                    //LogHelper.Warning("TryGetUnits failed,{0}", node);
                    return null;
                }
                _cachedUnits.Add(unit);
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
                        _cachedUnits.Add(unit);
                    }
                }
            }
            return _cachedUnits;
        }

        public static List<SceneNode> CircleCastAll(Vector2 center, float radius, int layerMask = JoyPhysics2D.LayMaskAll,
            float minDepth = float.MinValue, float maxDepth = float.MaxValue,
            SceneNode excludeNode = null)
        {
            return SceneQuery2D.CircleCastAll(center * ConstDefineGM2D.ServerTileScale,
                radius * ConstDefineGM2D.ServerTileScale, layerMask, Instance, minDepth, maxDepth,
                excludeNode);
        }

        public static List<UnitDesc> CircleCastAllReturnUnits(Vector2 center, float radius, int layerMask = JoyPhysics2D.LayMaskAll,
            float minDepth = float.MinValue, float maxDepth = float.MaxValue,
            SceneNode excludeNode = null)
        {
            _cachedUnitObjects.Clear();
            var grid = new Grid2D(GM2DTools.WorldToTile(center.x - radius), GM2DTools.WorldToTile(center.y - radius),
                GM2DTools.WorldToTile(center.x + radius) - 1, GM2DTools.WorldToTile(center.y + radius) - 1);
            grid = grid.IntersectWith(DataScene2D.Instance.RootGrid);
            List<SceneNode> nodes = CircleCastAll(center, radius, layerMask, minDepth, maxDepth, excludeNode);
            center *= ConstDefineGM2D.ServerTileScale;
            radius *= ConstDefineGM2D.ServerTileScale;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (!SplitSceneNodeCacheToUnitObject(center, radius, grid, nodes[i]))
                {
                    LogHelper.Error("SplitSceneNodeCacheToUnitObject failed.{0}, {1}", nodes[i], grid);
                }
            }
            return _cachedUnitObjects;
        }

        private static bool SplitSceneNodeCacheToUnitObject(Vector2 center, float radius, Grid2D grid, SceneNode node)
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
                _cachedUnitObjects.Add(new UnitDesc(node.Id, tableUnit.ColliderToRenderer(node.Guid, node.Rotation), node.Rotation, node.Scale));
                return true;
            }
            Grid2D newGrid = GM2DTools.IntersectWith(grid, node, tableUnit, false);
            var colliderSize = tableUnit.GetColliderSize(node);
            var count = tableUnit.GetColliderCount(newGrid, node.Rotation, node.Scale);
            for (int j = 0; j < count.x; j++)
            {
                for (int k = 0; k < count.y; k++)
                {
                    var rectIndex = new IntVec3(newGrid.XMin + j * colliderSize.x, newGrid.YMin + k * colliderSize.y, node.Depth);
                    var splitedGrid = tableUnit.GetBaseColliderGrid(rectIndex);
                    if (splitedGrid.Intersect(center, radius))
                    {
                        _cachedUnitObjects.Add(new UnitDesc(node.Id, tableUnit.ColliderToRenderer(rectIndex, node.Rotation), node.Rotation, node.Scale));
                    }
                }
            }
            return true;
        }

        #endregion

        #region path

        public List<IntVec2> FindPath(UnitBase unit, UnitBase target, short maxCharacterJumpHeight)
        {
            var start = unit.CurPos / ConstDefineGM2D.ServerTileScale;
            var end = target.CurPos / ConstDefineGM2D.ServerTileScale;
            var size = unit.GetColliderSize() / ConstDefineGM2D.ServerTileScale;
            return FindPath(start, end, Math.Max(1, size.x), Math.Max(1, size.y), maxCharacterJumpHeight);
        }

        public override bool IsOnewayPlatform(int x, int y)
        {
            return base.IsOnewayPlatform(x, y);
        }

        public override bool IsGround(int x, int y)
        {
            if (_pathGrid[x, y] == 0)
            {
                return true;
            }
            return false;
        }

        #endregion

        internal bool AnySolidBlockInStripe(int x, int y0, int y1)
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
