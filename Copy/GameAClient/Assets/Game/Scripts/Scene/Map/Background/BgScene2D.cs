/********************************************************************
** Filename : BgScene2D
** Author : Dong
** Date : 2016/11/28 星期一 下午 8:19:32
** Summary : BgScene2D
***********************************************************************/

using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class BgScene2D : Scene2D
    {
        private static BgScene2D _instance;
        private bool _run;
        private int _curSeed;
        private Vector3 _centerPos;
        private Vector3 _downCenterPos;
        private readonly Dictionary<IntVec3, BgItem> _items = new Dictionary<IntVec3, BgItem>();
        private Grid2D _followTileRect;
        private Rect _followRect;
        private Grid2D _validTileRect;
        private Rect _cloudRect;
        private Grid2D _cloudTileRect;
        private Rect _starRect;
        private Grid2D _starTileRect;
        private Rect _ghostRect;
        private Grid2D _ghostTileRect;
        private Transform[] _parents;
        private Transform _parent;
        private int _curBgGroup = 1;
        private readonly int[] _maxDepthCount = new int[(int) EBgDepth.Max];
        private readonly float[] _moveRatio = new float[(int) EBgDepth.Max];
        private readonly Dictionary<string, Sprite> _spriteDic = new Dictionary<string, Sprite>();

        private readonly Dictionary<int, List<Table_Background>> _tableBgs =
            new Dictionary<int, List<Table_Background>>();

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
            _spriteDic.Clear();
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
            return _moveRatio[depth - 1];
        }

        private int GetMaxDepthCount(int depth)
        {
            int count = _maxDepthCount[depth - 1];
            if (count == 1)
            {
                return 1;
            }
            int increase;
            if (_curBgGroup == 2 && depth <= 3)
            {
                increase = (int) (_followRect.width / 60);
            }
            else
            {
                increase = (int) (_followRect.width / 60 + _followRect.height / 30);
            }
            if (increase == 0)
            {
                increase = 1;
            }
            return count * increase;
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
            CaculateRect();
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
                if (bg.Group != _curBgGroup)
                {
                    continue;
                }

                Sprite sprite;
                if (!JoyResManager.Instance.TryGetSprite(bg.Model, out sprite))
                {
                    LogHelper.Error("TryGetSpriteByName failed,{0}", bg.Model);
                    continue;
                }

                if (!_spriteDic.ContainsKey(bg.Model))
                {
                    _spriteDic.Add(bg.Model, sprite);
                }

                List<Table_Background> tables;
                if (!_tableBgs.TryGetValue(bg.Depth, out tables))
                {
                    tables = new List<Table_Background>();
                    _tableBgs.Add(bg.Depth, tables);
                }

                _maxDepthCount[bg.Depth - 1] = bg.MaxCount;
                _moveRatio[bg.Depth - 1] = bg.FollowMoveRatio;
                tables.Add(bg);
            }
        }

        private void CaculateRect()
        {
            var validMapTileRect = DataScene2D.CurScene.ValidMapRect;
            _validTileRect = GM2DTools.ToGrid2D(validMapTileRect);
            var validMapRect = GM2DTools.TileRectToWorldRect(validMapTileRect);
            _centerPos = validMapRect.center;
            _downCenterPos = new Vector2(validMapRect.x + validMapRect.width / 2f, validMapRect.y);
            _followRect = validMapRect;
//            _followRect.width *= Mathf.Max(1, 1.6f * validMapRect.height / validMapRect.width); //横向拉伸，防止宽高比太小左右走光
            _followRect.width += 10;
            _followRect.height += 4; //地图编辑黑边有渐变 防止走光
            _followRect.center = _centerPos;
            _followTileRect = GM2DTools.ToGrid2D(GM2DTools.WorldRectToTileRect(_followRect));
            _cloudRect = _followRect;
            _cloudRect.size += new Vector2(20, 0);
            _cloudRect.center = _centerPos;
            _cloudTileRect = GM2DTools.ToGrid2D(GM2DTools.WorldRectToTileRect(_cloudRect));

            if (validMapRect.size.y <= ConstDefineGM2D.MinStarY)
            {
                _starRect = Rect.zero;
            }
            else
            {
                _starRect = new Rect(validMapRect.min.x - 10, validMapRect.min.y + ConstDefineGM2D.MinStarY,
                    validMapRect.size.x + 20, validMapRect.size.y - ConstDefineGM2D.MinStarY);
            }

            _starTileRect = GM2DTools.ToGrid2D(GM2DTools.WorldRectToTileRect(_starRect));
            if (validMapRect.size.y <= ConstDefineGM2D.MinGhostY)
            {
                _ghostRect = Rect.zero;
            }
            else
            {
                _ghostRect = new Rect(validMapRect.min.x - 10, validMapRect.min.y + ConstDefineGM2D.MinGhostY,
                    validMapRect.size.x + 20,
                    Mathf.Min(validMapRect.size.y - ConstDefineGM2D.MinGhostY, ConstDefineGM2D.MaxGhostHeight));
            }

            _ghostTileRect = GM2DTools.ToGrid2D(GM2DTools.WorldRectToTileRect(_ghostRect));
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
//                    if (GameRun.Instance.LogicFrameCnt == 0)
//                    {
//                        bgItem.SetBaseFollowPos(pos);
//                    }

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
            if (!CanOverlap(tableBg) && SceneQuery2D.GridCast(ref grid, out node, JoyPhysics2D.LayMaskAll, this,
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
            if (_curBgGroup == 1)
            {
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
            }
            else if (_curBgGroup == 2)
            {
                switch ((EBgDepth) tableBg.Depth)
                {
                    //前面的草
                    case EBgDepth.Depth1:
                    case EBgDepth.Depth2:
                    case EBgDepth.Depth3:
                        min = new IntVec2(Random.Range(_followTileRect.XMin, _followTileRect.XMax + size.x),
                            _followTileRect.YMin + GM2DTools.WorldToTile(-1.6f));
                        break;
                    //前面的地面
                    case EBgDepth.Depth4:
                        if (_followTileRect.XMin + (num - 1) * size.x > _followTileRect.XMax)
                        {
                            grid = Grid2D.zero;
                            return false;
                        }

                        min = new IntVec2(_followTileRect.XMin + (num - 1) * size.x,
                            _followTileRect.YMin - GM2DTools.WorldToTile(1.13f));
                        break;
                    //左右柱子
                    case EBgDepth.Depth5:
                        if (_validTileRect.YMin - GM2DTools.WorldToTile(3f) + (num - 1) / 2 * size.y >
                            _followTileRect.YMax)
                        {
                            grid = Grid2D.zero;
                            return false;
                        }

                        //左柱子
                        if (num % 2 == 1)
                        {
                            min = new IntVec2(_validTileRect.XMin - GM2DTools.WorldToTile(7.67f),
                                _followTileRect.YMin - GM2DTools.WorldToTile(0.66f) + (num - 1) / 2 * size.y);
                        }
                        //右柱子
                        else
                        {
                            scale.x = -scale.x;
                            min = new IntVec2(_validTileRect.XMax - GM2DTools.WorldToTile(1.34f),
                                _followTileRect.YMin - GM2DTools.WorldToTile(0.66f) + (num - 1) / 2 * size.y);
                        }

                        break;
                    //房顶
                    case EBgDepth.Depth6:
                        if (_followTileRect.XMin + (num - 1) * size.x > _followTileRect.XMax)
                        {
                            grid = Grid2D.zero;
                            return false;
                        }

                        min = new IntVec2(_followTileRect.XMin + (num - 1) * size.x,
                            _validTileRect.YMax - GM2DTools.WorldToTile(0.7f));
                        break;
                    case EBgDepth.Depth22:
                        if (_followTileRect.XMin + (num - 1) * size.x > _followTileRect.XMax)
                        {
                            grid = Grid2D.zero;
                            return false;
                        }

                        min = new IntVec2(_followTileRect.XMin + (num - 1) * size.x,
                            _validTileRect.YMax - GM2DTools.WorldToTile(4.7f));
                        break;
                    //后面的地面
                    case EBgDepth.Depth7:
                        if (_followTileRect.XMin + (num - 1) * size.x > _followTileRect.XMax)
                        {
                            grid = Grid2D.zero;
                            return false;
                        }

                        min = new IntVec2(_followTileRect.XMin + (num - 1) * size.x,
                            _followTileRect.YMin + GM2DTools.WorldToTile(1.85f));
                        break;
                    //前面不动的树    
                    case EBgDepth.Depth11:
                        min = new IntVec2(Random.Range(_followTileRect.XMin, _followTileRect.XMax + size.x),
                            _followTileRect.YMin + GM2DTools.WorldToTile(2.27f));
                        break;
                    //后面的树
                    case EBgDepth.Depth12:
                    case EBgDepth.Depth13:
                        min = new IntVec2(Random.Range(_followTileRect.XMin, _followTileRect.XMax + size.x),
                            _followTileRect.YMin + GM2DTools.WorldToTile(1.77f));
                        break;
                    //山
                    case EBgDepth.Depth14:
                    case EBgDepth.Depth16:
                    case EBgDepth.Depth17:
                        min = new IntVec2(Random.Range(_followTileRect.XMin, _followTileRect.XMax + size.x),
                            _followTileRect.YMin);
                        break;
                    //泡泡
                    case EBgDepth.Depth8:
                    case EBgDepth.Depth9:
                    case EBgDepth.Depth10:
                    //云
                    case EBgDepth.Depth15:
                    case EBgDepth.Depth20:
                        min = new IntVec2(Random.Range(_cloudTileRect.XMin, _cloudTileRect.XMax + size.x),
                            Random.Range(_cloudTileRect.YMin, _cloudTileRect.YMax + size.y));
                        break;
                    //鬼魂
                    case EBgDepth.Depth18:
                    case EBgDepth.Depth19:
                        min = new IntVec2(Random.Range(_ghostTileRect.XMin, _ghostTileRect.XMax + size.x),
                            Random.Range(_ghostTileRect.YMin, _ghostTileRect.YMax + size.y));
                        break;
                    //星星
                    case EBgDepth.Depth21:
                        min = new IntVec2(Random.Range(_starTileRect.XMin, _starTileRect.XMax + size.x),
                            Random.Range(_starTileRect.YMin, _starTileRect.YMax + size.y));
                        break;
                    //背景
                    case EBgDepth.Depth23:
                        min = new IntVec2(_followTileRect.XMin, _followTileRect.YMin - GM2DTools.WorldToTile(2f));
                        break;
                }
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
            var sprite = GetModelSprite(tableBg.Model);
            if (sprite == null)
            {
                return IntVec2.zero;
            }

            //1米 = 640计算单位 = 128像素，650 / 128 = 5，所以每像素占5个计算单位
            return new IntVec2((int) (sprite.rect.width * 5 * scale.x), (int) (sprite.rect.height * 5 * scale.y));
        }

        public Sprite GetModelSprite(string model)
        {
            Sprite sprite;
            if (_spriteDic.TryGetValue(model, out sprite))
            {
                return sprite;
            }

            LogHelper.Error("GetModelSprite failed,{0}", model);
            return null;
        }

        private void SetChirldFollowBasePos()
        {
            using (var iter = _items.GetEnumerator())
            {
                while (iter.MoveNext())
                {
                    var bgItem = iter.Current.Value;
                    bgItem.SetBaseFollowPos(_downCenterPos);
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
            if (bgItem == null ||
                !bgItem.Init(tableBg, node, ShowBeforeScene(tableBg.Depth), IsBackGround(tableBg.Model)))
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
                if (bgItem.TableBg.Model == "BJ_tree17")
                {
                    bgItem.Trans.gameObject.SetActive(value);
                }
            }
        }

        private bool IsBackGround(string model)
        {
            return _curBgGroup == 1 && model == "BJ" || _curBgGroup == 2 && model == "Night_BJ";
        }

        private bool ShowBeforeScene(int depth)
        {
            return _curBgGroup == 1 && depth <= 4 || _curBgGroup == 2 && depth <= 6;
        }

        private bool CanOverlap(Table_Background tableBackground)
        {
            //藤蔓可以重叠
            return _curBgGroup == 1 && tableBackground.Model == "BJ_tree17";
        }

        private bool DeleteView(SceneNode node)
        {
            BgItem bgItem;
            if (!_items.TryGetValue(node.Guid, out bgItem))
            {
                return false;
            }

            FreeItem(bgItem);
            return true;
        }

        private void FreeItem(BgItem bgItem)
        {
            PoolFactory<BgItem>.Free(bgItem);
        }

        public void OnMapChanged()
        {
            CaculateRect();
            ReGenerateBackground();
        }

        private void ReGenerateBackground()
        {
            foreach (var bgItem in _items.Values)
            {
                var bgNode = bgItem.Node;
                DeleteView(bgNode);
                DeleteNode(bgNode);
            }

            _items.Clear();
            GenerateBackground(_curSeed);
        }

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
            Depth15,
            Depth16,
            Depth17,
            Depth18,
            Depth19,
            Depth20,
            Depth21,
            Depth22,
            Depth23,
            Max
        }
    }
}