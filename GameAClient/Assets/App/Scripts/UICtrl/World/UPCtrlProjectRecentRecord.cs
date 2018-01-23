using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlProjectRecentRecord : UPCtrlProjectDetailBase
    {
        private const int _pageSize = 10;
        private WorldProjectRecentRecordList _data;
        private List<Record> _dataList;
        private List<CardDataRendererWrapper<Record>> _contentList = new List<CardDataRendererWrapper<Record>>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.RecentGridDataScroller.Set(OnItemRefresh, GetItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            RequestData();
            RefreshView();
        }

        public override void Close()
        {
            _cachedView.RecentGridDataScroller.ContentPosition = Vector2.zero;
            base.Close();
        }

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
//                _cachedView.RecordGridDataScroller.OnViewportSizeChanged();
                _cachedView.RecentGridDataScroller.SetEmpty();
                return;
            }

//            _cachedView.DescTitle.SetActiveEx(!string.IsNullOrEmpty(_mainCtrl.Project.Summary));
//            _cachedView.RecentGridDataScroller.OnViewportSizeChanged();
            if (_dataList == null)
            {
                _cachedView.RecentGridDataScroller.SetEmpty();
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

                _cachedView.RecentGridDataScroller.SetItemCount(_contentList.Count);
            }
        }

        private void OnItemClick(CardDataRendererWrapper<Record> item)
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请求播放录像");
            if (item.Content.ProjectVersion == _mainCtrl.Project.ProjectVersion)
            {
                PlayRecord(_mainCtrl.Project, item.Content);
            }
            else
            {
                ProjectManager.Instance.GetDataOnAsync(item.Content.ProjectId, p => PlayRecord(p, item.Content), () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("进入录像失败");
                });
            }
        }

        private void PlayRecord(Project project, Record record)
        {
            project.PrepareRes(() =>
            {
                record.RequestPlay(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    GameManager.Instance.RequestPlayRecord(_mainCtrl.Project, record);
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

                var um = item as UMCtrlWorldRecentRecord;
                if (um != null)
                {
                    int newestVersion = _mainCtrl.Project.NewestProjectVersion;
                    um.SetVersionLineEnable(inx - 1 >= 0 &&
                                            _contentList[inx - 1].Content.ProjectVersion == newestVersion &&
                                            _contentList[inx].Content.ProjectVersion < newestVersion);
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
            _cachedView.RecentGridDataScroller.ContentPosition = Vector2.zero;
        }
    }
}