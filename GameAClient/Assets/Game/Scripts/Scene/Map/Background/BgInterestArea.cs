///********************************************************************
//** Filename : BgInterestArea
//** Author : Dong
//** Date : 2016/10/25 星期二 上午 10:38:06
//** Summary : BgInterestArea
//***********************************************************************/

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using SoyEngine;
//using UnityEngine;

//namespace GameA.Game
//{
//    public class BgInterestArea
//    {
//        private IntVec2 _pos;
//        private IntVec2 _region;

//        /// <summary>
//        /// 内圈的半径
//        /// </summary>
//        private IntVec2 _viewInner;

//        /// <summary>
//        /// 外圈半径
//        /// </summary>
//        private IntVec2 _viewOuter;

//        /// <summary>
//        /// 玩家可视区域
//        /// </summary>
//        private Grid2D _lastInnerGrid;

//        /// <summary>
//        /// 已经下载的区域
//        /// </summary>
//        private Grid2D _lastOuterGrid;

//        private Dictionary<IntVec3, SceneNode> _cachedDynamicNodes = new Dictionary<IntVec3, SceneNode>();

//        private Scene2D _scene2D;

//        public BgInterestArea(IntVec2 viewInner, IntVec2 viewOuter, Scene2D scene)
//        {
//            _viewInner = viewInner;
//            _viewOuter = viewOuter;
//            _scene2D = scene;
//            LogHelper.Debug("{0} | {1}", _viewInner, _viewOuter);
//        }

//        public void Update(IntVec2 pos)
//        {
//            if (_pos.Equals(pos))
//            {
//                return;
//            }
//            _pos = pos;
//            var validMapRect = DataScene2D.Instance.ValidMapRect;
//            var validMapGrid = new Grid2D(validMapRect.Min, validMapRect.Max);

//            var innerGrid = new Grid2D(_pos - _viewInner, _pos + _viewInner);
//            innerGrid = _scene2D.GetRegionAlignedGrid(innerGrid);

//            innerGrid = innerGrid.IntersectWith(validMapGrid);
//            if (!innerGrid.Equals(_lastInnerGrid))
//            {
//                if (innerGrid.IsValid())
//                {
//                    Subscribe(innerGrid.Cut(_lastInnerGrid), innerGrid);
//                }
//                _lastInnerGrid = innerGrid;
//            }
//            var outerGrid = new Grid2D(_pos - _viewOuter, _pos + _viewOuter);
//            outerGrid = _scene2D.GetRegionAlignedGrid(outerGrid);
//            outerGrid = outerGrid.IntersectWith(validMapGrid);
//            if (!outerGrid.Equals(_lastOuterGrid))
//            {
//                Unsubscribe(_lastOuterGrid.Cut(outerGrid), outerGrid);
//                _lastOuterGrid = outerGrid;
//            }
//        }

//        /// <summary>
//        /// 注册相应区域17776638
//        /// </summary>
//        /// <param name="grids"></param>
//        /// <param name="curGrid"></param>
//        private void Subscribe(List<Grid2D> grids, Grid2D curGrid)
//        {
//            if (grids == null)
//            {
//                return;
//            }
//            _cachedDynamicNodes.Clear();
//            for (int i = 0; i < grids.Count; i++)
//            {
//                Grid2D grid2D = grids[i];
//                var nodes = SceneQuery2D.GridCastAll(ref grid2D, JoyPhysics2D.LayMaskAll, _scene2D, (int)EBgDepth.Static, (int)EBgDepth.Static);
//                if (nodes.Count > 0)
//                {
//                    for (int j = 0; j < nodes.Count; j++)
//                    {
//                        var node = nodes[j];
//                        if (node.IsDynamic())
//                        {
//                            if (!_cachedDynamicNodes.ContainsKey(node.Guid))
//                            {
//                                _cachedDynamicNodes.Add(node.Guid, node);
//                            }
//                        }
//                    }
//                }
//            }
//            if (_cachedDynamicNodes.Count > 0)
//            {
//                Messenger<SceneNode[], Grid2D>.Broadcast(EMessengerType.OnBgDynamicSubscribe, _cachedDynamicNodes.Values.ToArray(), curGrid);
//            }
//        }

//        /// <summary>
//        /// 撤销注册相应区域
//        /// </summary>
//        /// <param name="grids"></param>
//        /// <param name="curGrid"></param>
//        private void Unsubscribe(List<Grid2D> grids, Grid2D curGrid)
//        {
//            if (grids == null)
//            {
//                return;
//            }
//            _cachedDynamicNodes.Clear();
//            for (int i = 0; i < grids.Count; i++)
//            {
//                Grid2D grid2D = grids[i];
//                var nodes = SceneQuery2D.GridCastAll(ref grid2D, JoyPhysics2D.LayMaskAll, _scene2D, (int)EBgDepth.Static, (int)EBgDepth.Static);
//                if (nodes.Count > 0)
//                {
//                    for (int j = 0; j < nodes.Count; j++)
//                    {
//                        var node = nodes[j];
//                        if (node.IsDynamic())
//                        {
//                            if (!_cachedDynamicNodes.ContainsKey(node.Guid))
//                            {
//                                _cachedDynamicNodes.Add(node.Guid, node);
//                            }
//                        }
//                    }
//                }
//            }
//            if (_cachedDynamicNodes.Count > 0)
//            {
//                Messenger<SceneNode[], Grid2D>.Broadcast(EMessengerType.OnBgDynamicUnsubscribe, _cachedDynamicNodes.Values.ToArray(), curGrid);
//            }
//        }

//        /// <summary>
//        /// 限制Node的Grid范围
//        /// </summary>
//        /// <param name="node"></param>
//        /// <param name="limit"></param>
//        private NodeData CalculateNodeData(SceneNode node, Grid2D limit)
//        {
//            var nodeData = new NodeData(node.Id, node.ColliderGrid, node.Depth, node.Rotation);
//            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(node.Id);
//            if (tableUnit == null)
//            {
//                LogHelper.Error("ProcessAOI Failed,GetTableUnit:{0}", node);
//                return nodeData;
//            }
//            nodeData.ColliderGrid = GM2DTools.IntersectWith(limit, node.ColliderGrid, tableUnit, node.Rotation);
//            //LogHelper.Debug("Limit:{0}, Origin: {1}, NewGrid {2}, {3}", limit, node.ColliderGrid, nodeData.ColliderGrid, node.Id);
//            return nodeData;
//        }

//        public void OnDrawGizmos()
//        {
//            Gizmos.color = Color.red;
//            GM2DTools.Draw(_lastInnerGrid);
//            GM2DTools.Draw(_lastOuterGrid);
//        }
//    }
//}
