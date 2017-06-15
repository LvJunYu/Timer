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
        Depth1 = 1,
        Depth2,
        Depth3,
        Depth4,
        Depth5,
        Depth6,
        Depth7,
        Depth8,
        Depth9,
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
        private Grid2D _cloudRect;
        private Transform[] _parents;
        private Transform _parent;
        private Dictionary<int, List<Table_Background>> _tableBgs = new Dictionary<int, List<Table_Background>>();
        private static readonly int[] MaxDepthCount = new int[9] { 50, 50, 50, 50, 50, 50, 50, 50, 1 };
        private static readonly float[] MoveRatio = new float[9] { 1, 1f, 0.8f, 1f, 0.5f, 0.5f, 1, 1, 1 };

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
            foreach (var bgItem in _items.Values)
            {
                if (bgItem != null && bgItem.Trans != null)
                {
                    UnityEngine.Object.Destroy(bgItem.Trans.gameObject);
                }
            }
            _items.Clear();
            if (_parent != null)
            {
                UnityEngine.Object.Destroy(_parent.gameObject);
            }
            _tableBgs.Clear();
            _instance = null;
        }

        public float GetMoveRatio(int depth)
        {
            return MoveRatio[depth - 1];
        }

        public int GetMaxDepthCount(int depth)
        {
            return MaxDepthCount[depth - 1];
        }

        public Grid2D GetRect(int depth)
        {
            switch (depth)
            {
                case (int)EBgDepth.Depth3:
                case (int)EBgDepth.Depth5:
                case (int)EBgDepth.Depth6:
                    return _cloudRect;
            }
            return _followRect;
        }

        protected override void OnInit()
        {
            base.OnInit();
            var validMapRect = DataScene2D.Instance.ValidMapRect;
            _followRect = new Grid2D(validMapRect.Min.x, validMapRect.Min.y, validMapRect.Max.x, validMapRect.Max.y);
            _cloudRect = new Grid2D(validMapRect.Min.x - 15 * ConstDefineGM2D.ServerTileScale, validMapRect.Min.y, validMapRect.Max.x + 15 * ConstDefineGM2D.ServerTileScale, validMapRect.Max.y);
            _parent = new GameObject("Background").transform;
            _parents = new Transform[(int)EBgDepth.Max];
            for (int i = 0; i < (int)EBgDepth.Max; i++)
            {
                _parents[i] = new GameObject(((EBgDepth)i).ToString()).transform;
                _parents[i].parent = _parent;
            }
            _focusPos = GM2DTools.WorldToTile(CameraManager.Instance.MainCamaraTrans.position);

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
            //RefreshFollowRect(pos);
            var delPos = pos - _focusPos;
            _focusPos = pos;
            var iter = _items.GetEnumerator();
            while (iter.MoveNext())
            {
                var bgItem = iter.Current.Value;
                bgItem.Update(delPos);
            }
        }

        //private void RefreshFollowRect(IntVec2 center)
        //{
        //    var min = center - RectSize / 2;
        //    _followRect = new Grid2D(min.x, min.y, min.x + RectSize.x - 1, min.y + RectSize.y - 1);
        //    _staticRect = new Grid2D(_followRect.XMin, _followRect.YMin + 11 * ConstDefineGM2D.ServerTileScale, _followRect.XMax, _followRect.YMax - 12 * ConstDefineGM2D.ServerTileScale);
        //}

        public void GenerateBackground(int seed = 0)
        {
            _run = false;
            _curSeed = seed == 0 ? Time.frameCount : seed;
            Random.InitState(_curSeed);
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
            Grid2D grid;
            Vector2 scale;
            if (!TryGetRandomGrid(tableBg, out grid, out scale))
            {
                return false;
            }
            bgNode = NodeFactory.GetBgNode((ushort)tableBg.Id, grid, tableBg.Depth, scale);
            SceneNode node;
            if (SceneQuery2D.GridCast(ref grid, out node, JoyPhysics2D.LayMaskAll, this, tableBg.Depth, tableBg.Depth))
            {
                return false;
            }
            if (!AddNode(bgNode))
            {
                return false;
            }
            return true;
        }

        private bool TryGetRandomGrid(Table_Background tableBg, out Grid2D grid, out Vector2 scale)
        {
            IntVec2 min = IntVec2.zero;
            var size = GetSize(tableBg, out scale);
            switch ((EBgDepth)tableBg.Depth)
            {
                case EBgDepth.Depth1:
                case EBgDepth.Depth2:
                case EBgDepth.Depth4:
                case EBgDepth.Depth7:
                case EBgDepth.Depth8:
                    min = new IntVec2(Random.Range(_followRect.XMin, _followRect.XMax - size.x), _followRect.YMin);
                    break;
                case EBgDepth.Depth3:
                case EBgDepth.Depth5:
                case EBgDepth.Depth6:
                    min = new IntVec2(Random.Range(_cloudRect.XMin, _cloudRect.XMax - size.x),
                        Random.Range(_cloudRect.YMin, _cloudRect.YMax - size.y));
                    break;
                case EBgDepth.Depth9:
                    min = new IntVec2(_followRect.XMin, _followRect.YMin);
                    break;
            }
            grid = new Grid2D(min.x, min.y, min.x + size.x - 1, min.y + size.y - 1);
            return true;
        }

        private IntVec2 GetSize(Table_Background tableBg, out Vector2 scale)
        {
            var x = Random.Range(tableBg.MinScaleX, tableBg.MaxScaleX);
            var y = Random.Range(tableBg.MinScaleY, tableBg.MaxScaleY);
            if (Util.IsFloatEqual(tableBg.MinScaleX, tableBg.MinScaleY) && Util.IsFloatEqual(tableBg.MaxScaleX, tableBg.MaxScaleY))
            {
                x = y = Mathf.Max(x, y);
            }
            scale.x = x;
            scale.y = y;
            return new IntVec2((int) (tableBg.Width * 5 * scale.x), (int) (tableBg.Height * 5 * scale.y));
        }

        private bool AddView(SceneNode node, Table_Background tableBg)
        {
            if (_items.ContainsKey(node.Guid))
            {
                return false;
            }
            var bgItem = PoolFactory<BgItem>.Get();
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

        private void FreeItem(BgItem bgItem)
        {
            PoolFactory<BgItem>.Free(bgItem);
        }
    }
}
