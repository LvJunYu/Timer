using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorldRanklistPanel : UPCtrlWorldPanelBase
    {
        private const string _advLv = "冒险等级";
        private const string _createLv = "创造等级";
        private const string _advCount = "通关个数";
        private const string _createCount = "被点赞数";
        private WorldRankList _data;
        private const int _pageSize = 20;

        protected List<CardDataRendererWrapper<WorldRankItem.WorldRankHolder>> _contentList =
            new List<CardDataRendererWrapper<WorldRankItem.WorldRankHolder>>();

        protected List<WorldRankItem.WorldRankHolder> _projectList;
        private EWorldRankType _curType = EWorldRankType.WRT_Player;
        private ERankTimeBucket _curBucket;
        private int _timeBucketCount;
        private bool _isRequesting;

        public override void Open()
        {
            base.Open();
            _cachedView.RankTimeTapGroup.SelectIndex(_timeBucketCount - 1 - (int) ERankTimeBucket.RTB_Total, true);
        }

        public override void Close()
        {
            _cachedView.GridDataScrollers[(int) _menu].RefreshCurrent();
            base.Close();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScrollers[(int) _menu].Set(OnItemRefresh, GetItemRenderer);
            _timeBucketCount = _cachedView.RankTimeBtnAry.Length;
            for (int i = 0; i < _timeBucketCount; i++)
            {
                var inx = i;
                _cachedView.RankTimeTapGroup.AddButton(_cachedView.RankTimeBtnAry[i],
                    _cachedView.RankTimeBtnSelectAry[i],
                    b => ClickTimeBucket(inx, b));
            }
            for (int i = 0; i < _cachedView.RankTypeBtnAry.Length; i++)
            {
                var inx = i;
                _cachedView.RankTypeTapGroup.AddButton(_cachedView.RankTypeBtnAry[i],
                    _cachedView.RankTypeBtnSelectAry[i],
                    b => ClickType(inx, b));
            }
        }

        public override void RequestData(bool append = false)
        {
            if (_isRequesting) return;
            _isRequesting = true;
            _data = AppData.Instance.WorldData.RankList;
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }
            _data.Request(_curType, _curBucket, startInx, _pageSize,
                () =>
                {
                    _isRequesting = false;
                    _projectList = _data.AllList;
                    if (_isOpen)
                    {
                        if (!append)
                        {
                            _cachedView.GridDataScrollers[(int) _menu].ContentPosition = Vector2.zero;
                        }
                        RefreshView();
                    }
                }, code =>
                {
                    _isRequesting = false;
                    SocialGUIManager.ShowPopupDialog("服务器繁忙，请稍后再试！");
                });
        }

        protected override void RefreshView()
        {
            _cachedView.LevelTex.text = _curType == EWorldRankType.WRT_Player ? _advLv : _createLv;
            _cachedView.CountTex.text = _curType == EWorldRankType.WRT_Player ? _advCount : _createCount;
            _cachedView.EmptyObj.SetActiveEx(_projectList == null || _projectList.Count == 0);
            if (_projectList == null)
            {
                _contentList.Clear();
                _cachedView.GridDataScrollers[(int) _menu].SetEmpty();
                return;
            }
            _contentList.Clear();
            _contentList.Capacity = Mathf.Max(_contentList.Capacity, _projectList.Count);
            for (int i = 0; i < _projectList.Count; i++)
            {
                CardDataRendererWrapper<WorldRankItem.WorldRankHolder> w =
                    new CardDataRendererWrapper<WorldRankItem.WorldRankHolder>(_projectList[i], null);
                _contentList.Add(w);
            }
            _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_contentList.Count);
        }

        protected IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlWorldRank();
            item.Init(parent, _resScenary);
            return item;
        }

        protected override void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (_unload)
            {
                item.Set(null);
            }
            else
            {
                if (inx >= _contentList.Count)
                {
                    LogHelper.Error("OnItemRefresh Error Inx > count");
                    return;
                }
                if (item is UMCtrlWorldRank)
                {
                    ((UMCtrlWorldRank) item).SetType(_curType);
                }
                item.Set(_contentList[inx]);
                if (!_data.IsEnd)
                {
                    if (inx > _contentList.Count - 2)
                    {
                        RequestData(true);
                    }
                }
            }
        }

        public override void Clear()
        {
            base.Clear();
            _unload = true;
            _contentList.Clear();
            _projectList = null;
            _cachedView.GridDataScrollers[(int) _menu].ContentPosition = Vector2.zero;
        }

        private void ClickTimeBucket(int selectInx, bool open)
        {
            if (open)
            {
                _isRequesting = false;
                _curBucket = (ERankTimeBucket) (_timeBucketCount - 1 - selectInx);
                RequestData();
            }
        }

        private void ClickType(int selectInx, bool open)
        {
            if (open)
            {
                _isRequesting = false;
                _curType = (EWorldRankType) selectInx;
                RequestData();
            }
        }
    }
}