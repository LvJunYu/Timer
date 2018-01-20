using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlProjectRankRecord : UPCtrlProjectDetailBase
    {
        private const int _pageSize = 10;
        private WorldProjectRecordRankList _data;
        private List<RecordRankHolder> _dataList;

        private List<CardDataRendererWrapper<RecordRankHolder>> _contentList =
            new List<CardDataRendererWrapper<RecordRankHolder>>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.RankGridDataScroller.Set(OnItemRefresh, GetItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            RequestData();
            RefreshView();
        }

        public override void Close()
        {
            _cachedView.RankGridDataScroller.ContentPosition = Vector2.zero;
            base.Close();
        }

        protected override void RequestData(bool append = false)
        {
            if (_mainCtrl.Project == null) return;
            _data = _mainCtrl.Project.ProjectRecordRankList;
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }
            _mainCtrl.Project.RequestRecordRankList(startInx, _pageSize, () =>
            {
                _dataList = _data.AllList;
                if (_isOpen)
                {
                    RefreshView();
                }
            }, code => { });
        }

        protected override void RefreshView()
        {
            if (_mainCtrl.Project == null || _dataList == null)
            {
                _cachedView.RankGridDataScroller.SetEmpty();
                return;
            }
            _contentList.Clear();
            _contentList.Capacity = Mathf.Max(_contentList.Capacity, _dataList.Count);
            for (int i = 0; i < _dataList.Count; i++)
            {
                RecordRankHolder r = _dataList[i];
                CardDataRendererWrapper<RecordRankHolder> w =
                    new CardDataRendererWrapper<RecordRankHolder>(r, OnItemClick);
                _contentList.Add(w);
            }
            _cachedView.RankGridDataScroller.SetItemCount(_contentList.Count);
        }

        private void OnItemClick(CardDataRendererWrapper<RecordRankHolder> item)
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请求播放录像");
            _mainCtrl.Project.PrepareRes(() =>
            {
                item.Content.Record.RequestPlay(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    GameManager.Instance.RequestPlayRecord(_mainCtrl.Project, item.Content.Record);
                    SocialApp.Instance.ChangeToGame();
                }, error =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("进入录像失败");
                });
            }, () =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.ShowPopupDialog("进入录像失败");
            });
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlWorldRecordRank();
            item.Init(parent, _resScenary);
            return item;
        }

        private void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (!_isOpen)
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
            _dataList = null;
            _contentList.Clear();
            _cachedView.RankGridDataScroller.ContentPosition = Vector2.zero;
        }
    }
}