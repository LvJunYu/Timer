using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlProjectRecentRecord : UPCtrlProjectDetailBase
    {
        private WorldProjectRecentRecordList _data;
        private List<Record> _dataList;
        private List<CardDataRendererWrapper<Record>> _contentList = new List<CardDataRendererWrapper<Record>>();

        protected override void RequestData(bool append = false)
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

        protected override void RefreshView()
        {
            if (_mainCtrl.Project == null)
            {
                DictionaryTools.SetContentText(_cachedView.Desc, UICtrlProjectDetail.EmptyStr);
                _cachedView.GridDataScrollers[(int) _menu].SetEmpty();
                return;
            }
            DictionaryTools.SetContentText(_cachedView.Desc, _mainCtrl.Project.Summary);
            if (_dataList == null)
            {
                _cachedView.GridDataScrollers[(int) _menu].SetEmpty();
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
                _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_contentList.Count);
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

        protected override IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlWorldRecentRecord();
            item.Init(parent, _resScenary);
            return item;
        }

        protected override void OnItemRefresh(IDataItemRenderer item, int inx)
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
        }
    }
}