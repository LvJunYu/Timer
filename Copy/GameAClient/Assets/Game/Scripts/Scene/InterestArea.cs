/********************************************************************
** Filename : InterestArea
** Author : Dong
** Date : 2016/10/25 星期二 上午 10:38:06
** Summary : InterestArea
***********************************************************************/

using System.Collections.Generic;
using System.Linq;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class InterestArea
    {
        private IntVec2 _pos;
        private IntVec2 _region;

        /// <summary>
        /// 内圈的半径
        /// </summary>
        private IntVec2 _viewInner;

        /// <summary>
        /// 外圈半径
        /// </summary>
        private IntVec2 _viewOuter;

        /// <summary>
        /// 玩家可视区域
        /// </summary>
        private Grid2D _lastInnerGrid;

        /// <summary>
        /// 已经下载的区域
        /// </summary>
        private Grid2D _lastOuterGrid;

        private Dictionary<IntVec3, NodeData> _cachedNodes = new Dictionary<IntVec3, NodeData>();
        private Dictionary<IntVec3, SceneNode> _cachedDynamicNodes = new Dictionary<IntVec3, SceneNode>();

        private Scene2D _scene2D;

        public InterestArea(IntVec2 viewInner, IntVec2 viewOuter, Scene2D scene)
        {
            _viewInner = viewInner;
            _viewOuter = viewOuter;
            _scene2D = scene;
        }

        /// <summary>
        /// 重置View
        /// </summary>
        /// <param name="viewInner"></param>
        /// <param name="viewOuter"></param>
        public void ResetView(IntVec2 viewInner, IntVec2 viewOuter)
        {
            _viewInner = viewInner;
            _viewOuter = viewOuter;
        }

        public void InitCreate(IntVec2 pos)
        {
            _pos = pos;
//            var validMapRect = DataScene2D.CurScene.ValidMapRect;
//            var validMapGrid = new Grid2D(validMapRect.Min, validMapRect.Max);

            var innerGrid = new Grid2D(_pos - _viewInner, _pos + _viewInner);
            innerGrid = _scene2D.GetRegionAlignedGrid(innerGrid);
//            innerGrid = innerGrid.IntersectWith(validMapGrid);
            if (innerGrid.IsValid())
            {
                Subscribe(innerGrid.Cut(Grid2D.zero), innerGrid);
            }
            _lastInnerGrid = innerGrid;

            //这里的innerGrid = outerGrid,确保初始运行时候生成区域保持一致
            var outerGrid = new Grid2D(_pos - _viewInner, _pos + _viewInner);
            outerGrid = _scene2D.GetRegionAlignedGrid(outerGrid);
//            outerGrid = outerGrid.IntersectWith(validMapGrid);
            Unsubscribe(_lastOuterGrid.Cut(Grid2D.zero), outerGrid);
            _lastOuterGrid = outerGrid;
        }

        public void Update(IntVec2 pos)
        {
            if (_pos.Equals(pos))
            {
                return;
            }
            _pos = pos;
//            var validMapRect = DataScene2D.CurScene.ValidMapRect;
//            var validMapGrid = new Grid2D(validMapRect.Min, validMapRect.Max);

            var innerGrid = new Grid2D(_pos - _viewInner, _pos + _viewInner);
            innerGrid = _scene2D.GetRegionAlignedGrid(innerGrid);

//            innerGrid = innerGrid.IntersectWith(validMapGrid);
            if (!innerGrid.Equals(_lastInnerGrid))
            {
                if (innerGrid.IsValid())
                {
                    Subscribe(innerGrid.Cut(_lastInnerGrid), innerGrid);
                }
                _lastInnerGrid = innerGrid;
            }
            var outerGrid = new Grid2D(_pos - _viewOuter, _pos + _viewOuter);
            outerGrid = _scene2D.GetRegionAlignedGrid(outerGrid);
//            outerGrid = outerGrid.IntersectWith(validMapGrid);
            if (!outerGrid.Equals(_lastOuterGrid))
            {
                Unsubscribe(_lastOuterGrid.Cut(outerGrid), outerGrid);
                _lastOuterGrid = outerGrid;
            }
        }

        internal void UpdateDynamic(SceneNode node, IntVec2 previous)
        {
            var previousStatus = _lastOuterGrid.Contains(previous);
            var currentStatus = _lastOuterGrid.Contains(new IntVec2(node.Grid.XMin, node.Grid.YMin));
            if (previousStatus && currentStatus)
            {
            }
            else if (previousStatus)
            {
                Messenger<SceneNode>.Broadcast(EMessengerType.OnDynamicUnsubscribe, node);
            }
            else if (currentStatus)
            {
                Messenger<SceneNode>.Broadcast(EMessengerType.OnDynamicSubscribe, node);
            }
        }

        /// <summary>
        /// 注册相应区域17776638
        /// </summary>
        /// <param name="grids"></param>
        /// <param name="curGrid"></param>
        private void Subscribe(List<Grid2D> grids, Grid2D curGrid)
        {
            if (grids == null)
            {
                return;
            }
            _cachedNodes.Clear();
            _cachedDynamicNodes.Clear();
            for (int i = 0; i < grids.Count; i++)
            {
                Grid2D grid2D = grids[i];
                var nodes = SceneQuery2D.GridCastAll(ref grid2D, JoyPhysics2D.LayMaskAll, _scene2D);
                if (nodes.Count > 0)
                {
                    for (int j = 0; j < nodes.Count; j++)
                    {
                        var node = nodes[j];
                        if (node.IsDynamic())
                        {
                            if (!_cachedDynamicNodes.ContainsKey(node.Guid))
                            {
                                _cachedDynamicNodes.Add(node.Guid, node);
                            }
                        }
                        else
                        {
                            var nodeData = CalculateNodeData(nodes[j], grid2D);
                            var guid = new IntVec3(nodeData.Grid.XMin, nodeData.Grid.YMin, nodeData.Depth);
                            if (!_cachedNodes.ContainsKey(guid))
                            {
                                _cachedNodes.Add(guid, nodeData);
                            }
                        }
                    }
                }
            }
            if (_cachedNodes.Count > 0)
            {
                Messenger<NodeData[], Grid2D>.Broadcast(EMessengerType.OnAOISubscribe, _cachedNodes.Values.ToArray(), curGrid);
            }
            if (_cachedDynamicNodes.Count > 0)
            {
                Messenger<SceneNode[], Grid2D>.Broadcast(EMessengerType.OnDynamicSubscribe, _cachedDynamicNodes.Values.ToArray(), curGrid);
            }
        }

        /// <summary>
        /// 撤销注册相应区域
        /// </summary>
        /// <param name="grids"></param>
        /// <param name="curGrid"></param>
        private void Unsubscribe(List<Grid2D> grids, Grid2D curGrid)
        {
            if (grids == null)
            {
                return;
            }
            _cachedNodes.Clear();
            _cachedDynamicNodes.Clear();
            for (int i = 0; i < grids.Count; i++)
            {
                Grid2D grid2D = grids[i];
                var nodes = SceneQuery2D.GridCastAll(ref grid2D, JoyPhysics2D.LayMaskAll, _scene2D);
                if (nodes.Count > 0)
                {
                    for (int j = 0; j < nodes.Count; j++)
                    {
                        var node = nodes[j];
                        if (node.IsDynamic())
                        {
                            if (!_cachedDynamicNodes.ContainsKey(node.Guid))
                            {
                                _cachedDynamicNodes.Add(node.Guid, node);
                            }
                        }
                        else
                        {
                            var nodeData = CalculateNodeData(nodes[j], grid2D);
                            var guid = new IntVec3(nodeData.Grid.XMin, nodeData.Grid.YMin, nodeData.Depth);
                            if (!_cachedNodes.ContainsKey(guid))
                            {
                                _cachedNodes.Add(guid, nodeData);
                            }
                        }
                    }
                }
            }
            if (_cachedNodes.Count > 0)
            {
                Messenger<NodeData[], Grid2D>.Broadcast(EMessengerType.OnAOIUnsubscribe, _cachedNodes.Values.ToArray(), curGrid);
            }
            if (_cachedDynamicNodes.Count > 0)
            {
                Messenger<SceneNode[], Grid2D>.Broadcast(EMessengerType.OnDynamicUnsubscribe, _cachedDynamicNodes.Values.ToArray(), curGrid);
            }
        }

        /// <summary>
        /// 限制Node的Grid范围
        /// </summary>
        /// <param name="node"></param>
        /// <param name="limit"></param>
        private NodeData CalculateNodeData(SceneNode node, Grid2D limit)
        {
            var nodeData = new NodeData(node.Id, node.Grid, node.Depth, node.Rotation, node.Scale);
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(node.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("ProcessAOI Failed,GetTableUnit:{0}", node);
                return nodeData;
            }
            nodeData.Grid = GM2DTools.IntersectWith(limit, node, tableUnit);
            //LogHelper.Debug("Limit:{0}, Origin: {1}, NewGrid {2}, {3}", limit, node.ColliderGrid, nodeData.ColliderGrid, node.Id);
            return nodeData;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            GM2DTools.Draw(_lastInnerGrid);
            GM2DTools.Draw(_lastOuterGrid);
        }
    }

    public class Region
    {
        public IntVec2 Coordinate;

        public Region(IntVec2 coordinate)
        {
            Coordinate = coordinate;
        }
    }
}
