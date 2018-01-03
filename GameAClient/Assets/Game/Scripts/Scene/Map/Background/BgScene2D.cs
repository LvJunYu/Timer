/********************************************************************
** Filename : BgScene2D
** Author : Dong
** Date : 2016/11/28 星期一 下午 8:19:32
** Summary : BgScene2D
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

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
        Depth10,
        Depth11,
        Depth12,
        Depth13,
        Depth14,
        Max
    }

    public class BgScene2D : Scene2D
    {
        private static BgScene2D _instance;
        private bool _run;
        private int _curSeed;
        private int _curSceneIndex;
        private Vector3 _basePos;
        private readonly Dictionary<IntVec3, BgItem> _items = new Dictionary<IntVec3, BgItem>();
        private Grid2D _followTileRect;
        private Rect _followRect;
        private Grid2D _validTileRect;
        private Grid2D _cloudTileRect;
        private Rect _cloudRect;
        private Transform[] _parents;
        private Transform _parent;

        private readonly Dictionary<int, List<Table_Background>> _tableBgs =
            new Dictionary<int, List<Table_Background>>();

        private static readonly int[] MaxDepthCount = {20, 20, 20, 20, 50, 20, 50, 50, 50, 50, 50, 50, 50, 1};
        private static readonly float[] MoveRatio = {1, 1, 1, 1, 1, 1, 0.7f, 0.6f, 0.5f, 0.4f, 0.3f, 0.2f, 0.1f, 0f};

        public static BgScene2D Instance
        {
            get { return _instance ?? (_instance = new BgScene2D()); }
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
                    Object.Destroy(bgItem.Trans.gameObject);
                }
            }

            _items.Clear();
            if (_parent != null)
            {
                Object.Destroy(_parent.gameObject);
            }

            _tableBgs.Clear();
            _instance = null;
        }

        public float GetMoveRatio(int depth)
        {
            return MoveRatio[depth - 1];
        }

        private int GetMaxDepthCount(int depth)
        {
            int increase = (int) (_followRect.width / 60 + _followRect.height / 30);
            if (increase == 0)
            {
                increase = 1;
            }

            return MaxDepthCount[depth - 1] * increase;
        }

        public Rect GetRect(int depth)
        {
            switch (depth)
            {
                case (int) EBgDepth.Depth8:
                case (int) EBgDepth.Depth10:
                case (int) EBgDepth.Depth11:
                    return _cloudRect;
            }

            return _followRect;
        }

        protected override void OnInit()
        {
            base.OnInit();
            var validMapTileRect = DataScene2D.CurScene.ValidMapRect;
            _validTileRect = GM2DTools.ToGrid2D(validMapTileRect);
//            validMapTileRect.Max = new IntVec2(validMapTileRect.Max.x,
//                validMapTileRect.Min.y + ConstDefineGM2D.DefaultValidMapRectSize.y);
            var validMapRect = GM2DTools.TileRectToWorldRect(validMapTileRect);
            _basePos = validMapRect.center;
            _followRect = validMapRect;
//            _followRect.size = GM2DTools.TileToWorld(ConstDefineGM2D.DefaultValidMapRectSize);
            _followRect.width *= Mathf.Max(1, 1.6f * validMapRect.height / validMapRect.width); //横向拉伸，防止宽高比太小左右走光
            _followRect.width += 10;
            _followRect.height += 4; //地图编辑黑边有渐变 防止走光
            _followRect.center = _basePos;

            _followTileRect = GM2DTools.ToGrid2D(GM2DTools.WorldRectToTileRect(_followRect));
            _cloudRect = _followRect;
            _cloudRect.size += new Vector2(20, 0);
            _cloudRect.center = _basePos;
            _cloudTileRect = GM2DTools.ToGrid2D(GM2DTools.WorldRectToTileRect(_cloudRect));
            _parent = new GameObject("Background").transform;
            _parents = new Transform[(int) EBgDepth.Max];
            for (int i = 0; i < (int) EBgDepth.Max; i++)
            {
                _parents[i] = new GameObject(((EBgDepth) i).ToString()).transform;
                _parents[i].parent = _parent;
            }

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

        public void OnPlay()
        {
            _run = true;
        }

        public void OnStop()
        {
            _run = false;
        }

        public void Reset()
        {
            using (var iter = _items.GetEnumerator())
            {
                while (iter.MoveNext())
                {
                    var bgItem = iter.Current.Value;
                    bgItem.ResetPos();
                }
            }
        }

        public void ResetByFollowPos(Vector3 pos)
        {
            using (var iter = _items.GetEnumerator())
            {
                while (iter.MoveNext())
                {
                    var bgItem = iter.Current.Value;
                    bgItem.SetBaseFollowPos(pos);
                    bgItem.ResetPos();
                    bgItem.Update(pos);
                }
            }
        }

        public Transform GetParent(int eBgDepth)
        {
            return _parents[eBgDepth];
        }

        public void UpdateLogic(Vector3 pos)
        {
            if (!_run)
            {
                return;
            }

            using (var iter = _items.GetEnumerator())
            {
                while (iter.MoveNext())
                {
                    var bgItem = iter.Current.Value;
                    if (GameRun.Instance.LogicFrameCnt == 0)
                    {
                        bgItem.SetBaseFollowPos(pos);
                    }

                    bgItem.Update(pos);
                }
            }
        }

        public void GenerateBackground(int seed = 0)
        {
            _run = false;
            _curSeed = seed == 0 ? Time.frameCount : seed;
            Random.InitState(_curSeed);
            foreach (var pair in _tableBgs)
            {
                GenerateItems(pair.Value, GetMaxDepthCount(pair.Key));
            }

            SetChirldFollowBasePos();
        }

        private void GenerateItems(List<Table_Background> tableBgs, int count)
        {
            int num = 0;
            for (int j = 0; j < count / tableBgs.Count; j++)
            {
                for (int i = 0; i < tableBgs.Count; i++)
                {
                    //传物体重复个数，用于计算平地的位置
                    if (GenerateItem(tableBgs[i], j + 1))
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

        private bool GenerateItem(Table_Background tableBg, int num = 0)
        {
            SceneNode node;
            if (!TryAddNode(tableBg, out node, num))
            {
                return false;
            }

            return AddView(node, tableBg);
        }

        private bool TryAddNode(Table_Background tableBg, out SceneNode bgNode, int num = 0)
        {
            bgNode = null;
            Grid2D grid;
            Vector2 scale;
            if (!TryGetRandomGrid(tableBg, out grid, out scale, num))
            {
                return false;
            }

            bgNode = NodeFactory.GetBgNode((ushort) tableBg.Id, grid, tableBg.Depth, scale);
            SceneNode node;
            //藤蔓可以重叠
            if (!IsCirrus(tableBg.Depth) && SceneQuery2D.GridCast(ref grid, out node, JoyPhysics2D.LayMaskAll, this,
                    tableBg.Depth, tableBg.Depth))
            {
                return false;
            }

            if (!AddNode(bgNode))
            {
                return false;
            }

            return true;
        }

        private bool TryGetRandomGrid(Table_Background tableBg, out Grid2D grid, out Vector2 scale, int num = 0)
        {
            IntVec2 min = IntVec2.zero;
            var size = GetSize(tableBg, out scale);
            switch ((EBgDepth) tableBg.Depth)
            {
                //左右柱子
                case EBgDepth.Depth3:
                    if (_validTileRect.YMin - GM2DTools.WorldToTile(3f) + (num - 1) / 2 * size.y >
                        _followTileRect.YMax)
                    {
                        grid = Grid2D.zero;
                        return false;
                    }

                    //左柱子
                    if (num % 2 == 1)
                    {
                        min = new IntVec2(_validTileRect.XMin - GM2DTools.WorldToTile(6.6f),
                            _followTileRect.YMin - GM2DTools.WorldToTile(0.66f) + (num - 1) / 2 * size.y);
                    }
                    //右柱子
                    else
                    {
                        min = new IntVec2(_validTileRect.XMax - GM2DTools.WorldToTile(0.6f),
                            _followTileRect.YMin - GM2DTools.WorldToTile(0.66f) + (num - 1) / 2 * size.y);
                    }

                    break;
                //草
                case EBgDepth.Depth1:
                    if (_followTileRect.XMin + (num - 1) * size.x > _followTileRect.XMax)
                    {
                        grid = Grid2D.zero;
                        return false;
                    }

                    min = new IntVec2(_followTileRect.XMin + (num - 1) * size.x,
                        _followTileRect.YMin - GM2DTools.WorldToTile(1f));
                    break;
                //藤蔓
                case EBgDepth.Depth4:
                    if (_followTileRect.XMin + (num - 1) * size.x * 0.8f > _followTileRect.XMax)
                    {
                        grid = Grid2D.zero;
                        return false;
                    }

                    min = new IntVec2(_followTileRect.XMin + (int) ((num - 1) * size.x * 0.8f),
                        _validTileRect.YMax - GM2DTools.WorldToTile(0.7f));
                    break;
                //前面不动的树    
                case EBgDepth.Depth5:
                    min = new IntVec2(Random.Range(_followTileRect.XMin, _followTileRect.XMax + size.x),
                        _followTileRect.YMin + GM2DTools.WorldToTile(2.27f));
                    break;
                //前面的地面
                case EBgDepth.Depth2:
                    if (_followTileRect.XMin + (num - 1) * size.x > _followTileRect.XMax)
                    {
                        grid = Grid2D.zero;
                        return false;
                    }

                    min = new IntVec2(_followTileRect.XMin + (num - 1) * size.x,
                        _followTileRect.YMin - GM2DTools.WorldToTile(1.13f));
                    break;
                //后面的地面
                case EBgDepth.Depth6:
                    if (_followTileRect.XMin + (num - 1) * size.x > _followTileRect.XMax)
                    {
                        grid = Grid2D.zero;
                        return false;
                    }

                    min = new IntVec2(_followTileRect.XMin + (num - 1) * size.x,
                        _followTileRect.YMin + GM2DTools.WorldToTile(1.85f));
                    break;
                //后面的树
                case EBgDepth.Depth7:
                case EBgDepth.Depth9:
                case EBgDepth.Depth12:
                case EBgDepth.Depth13:
                    min = new IntVec2(Random.Range(_followTileRect.XMin, _followTileRect.XMax + size.x),
                        _followTileRect.YMin);
                    break;
                //云
                case EBgDepth.Depth8:
                case EBgDepth.Depth10:
                case EBgDepth.Depth11:
                    min = new IntVec2(Random.Range(_cloudTileRect.XMin, _cloudTileRect.XMax + size.x),
                        Random.Range(_cloudTileRect.YMin, _cloudTileRect.YMax + size.y));
                    break;
                //背景
                case EBgDepth.Depth14:
                    min = new IntVec2(_followTileRect.XMin, _followTileRect.YMin - GM2DTools.WorldToTile(2f));
                    break;
            }

            grid = new Grid2D(min.x, min.y, min.x + size.x - 1, min.y + size.y - 1);
            return true;
        }

        private IntVec2 GetSize(Table_Background tableBg, out Vector2 scale)
        {
            var x = Random.Range(tableBg.MinScaleX, tableBg.MaxScaleX);
            var y = Random.Range(tableBg.MinScaleY, tableBg.MaxScaleY);
            if (Util.IsFloatEqual(tableBg.MinScaleX, tableBg.MinScaleY) &&
                Util.IsFloatEqual(tableBg.MaxScaleX, tableBg.MaxScaleY))
            {
                x = y = Mathf.Max(x, y);
            }

            scale.x = x;
            scale.y = y;
            //1米 = 640计算单位 = 128像素，650 / 128 = 5，所以每像素占5个计算单位
            return new IntVec2((int) (tableBg.Width * 5 * scale.x), (int) (tableBg.Height * 5 * scale.y));
        }

        private void SetChirldFollowBasePos()
        {
            using (var iter = _items.GetEnumerator())
            {
                while (iter.MoveNext())
                {
                    var bgItem = iter.Current.Value;
                    bgItem.SetBaseFollowPos(_basePos);
                }
            }
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

        /// <summary>
        /// 显示/隐藏藤曼
        /// </summary>
        /// <param name="value"></param>
        public void SetCirrus(bool value)
        {
            foreach (var bgItem in _items.Values)
            {
                if (bgItem.Depth == 4)
                {
                    bgItem.Trans.gameObject.SetActive(value);
                }
            }
        }

        private bool IsCirrus(int depth)
        {
            return depth == 4;
        }

//        private bool DeleteView(SceneNode node)
//        {
//            BgItem bgItem;
//            if (!_items.TryGetValue(node.Guid, out bgItem))
//            {
//                return false;
//            }
//            FreeItem(bgItem);
//            return _items.Remove(node.Guid);
//        }

//        private void FreeItem(BgItem bgItem)
//        {
//            PoolFactory<BgItem>.Free(bgItem);
//        }
        public void ChangeScene(int index)
        {
            if (_curSceneIndex == index) return;
            if (index >= ConstDefineGM2D.MapStartPosArry.Length)
            {
                LogHelper.Error("index is out of range");
                return;
            }
            
            var validMapTileRect = DataScene2D.CurScene.ValidMapRect;
            _validTileRect = GM2DTools.ToGrid2D(validMapTileRect);
            var validMapRect = GM2DTools.TileRectToWorldRect(validMapTileRect);
            _basePos = validMapRect.center;
            _followRect = validMapRect;
            _followRect.width *= Mathf.Max(1, 1.6f * validMapRect.height / validMapRect.width); //横向拉伸，防止宽高比太小左右走光
            _followRect.width += 10;
            _followRect.height += 4; //地图编辑黑边有渐变 防止走光
            _followRect.center = _basePos;
            _followTileRect = GM2DTools.ToGrid2D(GM2DTools.WorldRectToTileRect(_followRect));
            _cloudRect = _followRect;
            _cloudRect.size += new Vector2(20, 0);
            _cloudRect.center = _basePos;
            _cloudTileRect = GM2DTools.ToGrid2D(GM2DTools.WorldRectToTileRect(_cloudRect));
            
            var delta = GM2DTools.TileToWorld(ConstDefineGM2D.MapStartPosArry[index] - ConstDefineGM2D.MapStartPosArry[_curSceneIndex]);
            using (var iter = _items.GetEnumerator())
            {
                while (iter.MoveNext())
                {
                    var bgItem = iter.Current.Value;
                    bgItem.ResetBasePos(delta);
                    bgItem.SetBaseFollowPos(CameraManager.Instance.MainCameraPos);
                    bgItem.ResetPos();
                }
            }

            _curSceneIndex = index;
        }
    }
}