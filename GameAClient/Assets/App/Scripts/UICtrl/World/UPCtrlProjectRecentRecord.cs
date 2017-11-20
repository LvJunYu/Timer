using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlProjectRecentRecord : UPCtrlBase<UICtrlProjectDetail, UIViewProjectDetail>,
        IOnChangeHandler<long>
    {
        private const int _pageSize = 10;
        protected EResScenary _resScenary;
        private WorldProjectRecentRecordList _data;
        private List<Record> _dataList;
        private List<CardDataRendererWrapper<Record>> _contentList = new List<CardDataRendererWrapper<Record>>();

        public bool HasComment
        {
            get { return _contentList.Count > 0; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.RecordGridDataScroller.Set(OnItemRefresh, GetItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            RequestData();
            RefreshView();
        }

        private void RequestData(bool append = false)
        {
            if (_mainCtrl.Project == null) return;
            _data = _mainCtrl.Project.ProjectRecentRecordList;
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }
            _data.Request(_mainCtrl.Project.ProjectId, startInx, _pageSize, () =>
            {
                _dataList = _data.AllList;
                if (_isOpen)
                {
                    RefreshView();
                }
            }, code => { });
        }

        private void RefreshView()
        {
            if (_mainCtrl.Project == null)
            {
//                _cachedView.RecordGridDataScroller.OnViewportSizeChanged();
                _cachedView.RecordGridDataScroller.SetEmpty();
                return;
            }
//            _cachedView.DescTitle.SetActiveEx(!string.IsNullOrEmpty(_mainCtrl.Project.Summary));
            _cachedView.RecordGridDataScroller.OnViewportSizeChanged();
            if (_dataList == null)
            {
                _cachedView.RecordGridDataScroller.SetEmpty();
            }
            else
            {
                _contentList.Clear();
                _contentList.Capacity = Mathf.Max(_contentList.Capacity, _dataList.Count);
                for (int i = 0; i < _dataList.Count; i++)
                {
                    CardDataRendererWrapper<Record> w = new CardDataRendererWrapper<Record>(_dataList[i], OnItemClick);
                    _contentList.Add(w);
                }
                _cachedView.RecordGridDataScroller.SetItemCount(_contentList.Count);
            }
        }

        private void OnItemClick(CardDataRendererWrapper<Record> item)
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请求播放录像");
            _mainCtrl.Project.PrepareRes(() =>
            {
                item.Content.RequestPlay(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    GameManager.Instance.RequestPlayRecord(_mainCtrl.Project, item.Content);
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
            var item = new UMCtrlWorldRecentRecord();
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

        public void OnChangeHandler(long val)
        {
            if (_isOpen)
            {
                RefreshView();
            }
        }

        public void OnChangeToApp()
        {
            RequestData();
        }

        public void Set(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public void Clear()
        {
            _dataList = null;
            _contentList.Clear();
        }
    }
}