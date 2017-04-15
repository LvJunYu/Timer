/********************************************************************
** Filename : BgScene2D
** Author : Dong
** Date : 2016/11/28 星期一 下午 8:19:32
** Summary : BgScene2D
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SoyEngine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameA.Game
{
    public enum EBgDepth
    {
        Bg,
        Far,
        Middle,
        Near,
        Nearest,
        Max
    }

    public class BgScene2D : Scene2D
    {
        public static BgScene2D _instance;
        private bool _run;
        private int _curSeed;
        private IntVec2 _focusPos;
        private Dictionary<IntVec3, BgItem> _items = new Dictionary<IntVec3, BgItem>();
        private Grid2D _followRect;
        private Grid2D _staticRect;

        private Transform[] _parents;
        private Dictionary<int, List<Table_Background>> _tableBgs = new Dictionary<int, List<Table_Background>>();
        private static readonly int[] MaxDepthCount = new int[5] { 1, 50, 100, 40, 80 };
        private static readonly float[] MoveRatio = new float[5] { 0, 0.2f, 0.5f, 0.8f, 1 };
        private static IntVec2 RectSize = new IntVec2(40, 30) * ConstDefineGM2D.ServerTileScale;

        public static BgScene2D Instance
        {
            get { return _instance ?? (_instance = new BgScene2D()); }
        }

        public Grid2D FollowRect
        {
            get { return _followRect; }
        }

        public int CurSeed
        {
            get { return _curSeed; }
        }

        public override void Dispose()
        {
            base.Dispose();
            _items.Clear();
            _tableBgs.Clear();
            _instance = null;
        }

        public float GetMoveRatio(int depth)
        {
            return MoveRatio[depth];
        }

        public int GetMaxDepthCount(int depth)
        {
            return MaxDepthCount[depth];
        }

        public Grid2D GetRect(int depth)
        {
            switch ((EBgDepth)depth)
            {
                case EBgDepth.Bg:
                case EBgDepth.Far:
                case EBgDepth.Middle:
                case EBgDepth.Near:
                    return _followRect;
                case EBgDepth.Nearest:
                    return _staticRect;
            }
            return _followRect;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _followRect = new Grid2D(0, 0, RectSize.x, RectSize.y);
            _staticRect = new Grid2D(_followRect.XMin, _followRect.YMin + 11 * ConstDefineGM2D.ServerTileScale, _followRect.XMax, _followRect.YMax - 12 * ConstDefineGM2D.ServerTileScale);
            var parent = new GameObject("Background").transform;
            _parents = new Transform[(int)EBgDepth.Max];
            for (int i = 0; i < (int)EBgDepth.Max; i++)
            {
                _parents[i] = new GameObject(((EBgDepth)i).ToString()).transform;
                _parents[i].parent = parent;
            }
            _focusPos = GM2DTools.WorldToTile(CameraManager.Instance.RendererCamaraTrans.position);

            var bgs = TableManager.Instance.Table_BackgroundDic;
            foreach (Table_Background bg in bgs.Values)
            {
                List<Table_Background> tables;
                if (!_tableBgs.TryGetValue(bg.Depth, out tables))
                {
                    tables = new List<Table_Background>();
                    _tableBgs.Add(bg.Depth, tables);
                }
                tables.Add(bg);
            }
        }

        public Transform GetParent(int eBgDepth)
        {
            return _parents[eBgDepth];
        }

        public void UpdateLogic(IntVec2 pos)
        {
            if (!_run)
            {
                return;
            }
            RefreshFollowRect(pos);
            var delPos = pos - _focusPos;
            _focusPos = pos;
            var iter = _items.GetEnumerator();
            while (iter.MoveNext())
            {
                var bgItem = iter.Current.Value;
                bgItem.Update(delPos);
            }
        }

        private void RefreshFollowRect(IntVec2 center)
        {
            var min = center - RectSize / 2;
            _followRect = new Grid2D(min.x, min.y, min.x + RectSize.x - 1, min.y + RectSize.y - 1);
            _staticRect = new Grid2D(_followRect.XMin, _followRect.YMin + 11 * ConstDefineGM2D.ServerTileScale, _followRect.XMax, _followRect.YMax - 12 * ConstDefineGM2D.ServerTileScale);
        }

        public void GenerateBackground(int seed = 0)
        {
            _run = false;
            _curSeed = seed == 0 ? Time.frameCount : seed;
            Random.seed = _curSeed;
            foreach (var pair in _tableBgs)
            {
                GenerateItems(pair.Value, GetMaxDepthCount(pair.Key));
            }
            _run = true;
        }

        private void GenerateItems(List<Table_Background> tableBgs, int count)
        {
            int num = 0;
            for (int j = 0; j < count / tableBgs.Count; j++)
            {
                for (int i = 0; i < tableBgs.Count; i++)
                {
                    if (GenerateItem(tableBgs[i]))
                    {
                        num++;
                        if (num >= tableBgs.Count)
                        {
                            num = 0;
                        }
                    }
                }
            }
        }

        private bool GenerateItem(Table_Background tableBg)
        {
            SceneNode node;
            if (!TryAddNode(tableBg, out node))
            {
                return false;
            }
            return AddView(node, tableBg);
        }

        private bool TryAddNode(Table_Background tableBg, out SceneNode bgNode)
        {
            bgNode = null;
            //Grid2D grid;
            //float scale;
            //if (!TryGetRandomGrid(tableBg, out grid, out scale))
            //{
            //    return false;
            //}
            //bgNode = new BgNode();
            //bgNode.Init((ushort)tableBg.Id, grid, tableBg.Depth, (byte)(scale * 10), Vector2.one, 0);
            //SceneNode node;
            //if (SceneQuery2D.GridCast(ref grid, out node, JoyPhysics2D.LayMaskAll, this, tableBg.Depth, tableBg.Depth))
            //{
            //    return false;
            //}
            //if (!AddNode(bgNode))
            //{
            //    return false;
            //}
            return true;
        }

        private bool TryGetRandomGrid(Table_Background tableBg, out Grid2D grid, out float scale)
        {
            IntVec2 min = IntVec2.zero;
            var size = GetSize(tableBg, out scale);
            switch ((EBgDepth)tableBg.Depth)
            {
                case EBgDepth.Bg:
                    min = IntVec2.zero;
                    break;
                case EBgDepth.Far:
                case EBgDepth.Middle:
                case EBgDepth.Near:
                    min = new IntVec2(Random.Range(_followRect.XMin, _followRect.XMax - size.x),
                        Random.Range(_followRect.YMin, _followRect.YMax - size.y));
                    break;
                case EBgDepth.Nearest:
                    min = new IntVec2(Random.Range(_staticRect.XMin, _staticRect.XMax - size.x),
                        Random.Range(_staticRect.YMin, _staticRect.YMax - size.y));
                    break;
            }
            grid = new Grid2D(min.x, min.y, min.x + size.x - 1, min.y + size.y - 1);
            return true;
        }

        private IntVec2 GetSize(Table_Background tableBg, out float scale)
        {
            scale = Random.Range(tableBg.MinScale, tableBg.MaxScale);
            return new IntVec2(tableBg.Width, tableBg.Height) * 20 * scale;
        }

        private bool AddView(SceneNode node, Table_Background tableBg)
        {
            if (_items.ContainsKey(node.Guid))
            {
                return false;
            }
            var bgItem = GetItem(tableBg);
            if (bgItem == null || !bgItem.Init(tableBg, node))
            {
                return false;
            }
            _items.Add(node.Guid, bgItem);
            return true;
        }

        private bool DeleteView(SceneNode node)
        {
            BgItem bgItem;
            if (!_items.TryGetValue(node.Guid, out bgItem))
            {
                return false;
            }
            FreeItem(bgItem);
            return _items.Remove(node.Guid);
        }

        private BgItem GetItem(Table_Background tableBackground)
        {
            BgItem bgItem;
            if (tableBackground.Depth == (int)EBgDepth.Bg)
            {
                bgItem = PoolFactory<BgRoot>.Get();
            }
            else
            {
                bgItem = PoolFactory<BgItem>.Get();
            }
            return bgItem;
        }

        private void FreeItem(BgItem bgItem)
        {
            var root = bgItem as BgRoot;
            if (root != null)
            {
                PoolFactory<BgRoot>.Free(root);
                return;
            }
            PoolFactory<BgItem>.Free(bgItem);
        }

        //public void OnDrawGizmos()
        //{
        //    return;
        //    if (_root != null)
        //    {
        //        if (_root.Nodes != null)
        //        {
        //            for (int i = 0; i < _root.Nodes.Count; i++)
        //            {
        //                Gizmos.color = Color.black;
        //                for (int j = 0; j < _root.Nodes.Count; j++)
        //                {
        //                    _root.Nodes[j].OnDrawGizmos();
        //                }
        //            }
        //        }
        //        DrawQuadTree(_root.Children);
        //    }
        //}

        //public void DrawQuadTree(Quadtree[] children)
        //{
        //    if (children != null && children.Length > 0)
        //    {
        //        for (int i = 0; i < children.Length; i++)
        //        {
        //            var quadtree = children[i];
        //            if (quadtree != null)
        //            {
        //                if (quadtree.Nodes != null)
        //                {
        //                    Gizmos.color = Color.black;
        //                    for (int j = 0; j < quadtree.Nodes.Count; j++)
        //                    {
        //                        quadtree.Nodes[j].OnDrawGizmos();
        //                    }
        //                }
        //                //Gizmos.color = Color.white;
        //                //quadtree.OnDrawGizmos();
        //                DrawQuadTree(quadtree.Children);
        //            }
        //        }
        //    }
        //}

        //private void ProcessDynamicAOI(SceneNode[] nodes, Grid2D grid, bool isSubscribe)
        //{
        //    for (int i = 0; i < nodes.Length; i++)
        //    {
        //        var node = nodes[i];
        //        Table_Background tableBg = TableManager.Instance.GetBackground(node.Id);
        //        if (tableBg == null)
        //        {
        //            LogHelper.Error("ProcessDynamicAOI Failed,GetBackground:{0}", node);
        //            return;
        //        }
        //        if (isSubscribe)
        //        {
        //            if (!grid.Contains(node.ColliderGrid))
        //            {
        //                continue;
        //            }
        //            AddView(node, tableBg);
        //        }
        //        else
        //        {
        //            if (grid.Contains(node.ColliderGrid))
        //            {
        //                continue;
        //            }
        //            DestroyView(node);
        //        }
        //    }
        //}
    }
}
