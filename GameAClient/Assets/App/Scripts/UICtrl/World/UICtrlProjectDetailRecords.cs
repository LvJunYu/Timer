using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlProjectDetailRecords : UICtrlAnimationBase<UIViewProjectDetailRecords>
    {
        private const int _pageSize = 10;
        private Project _project;
        private WorldProjectRecordRankList _data;
        private List<RecordRankHolder> _dataList;

        private List<CardDataRendererWrapper<RecordRankHolder>> _contentList =
            new List<CardDataRendererWrapper<RecordRankHolder>>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.GridDataScroller.Set(OnItemRefresh, GetItemRenderer);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _project = parameter as Project;
            if (_project == null)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlProjectDetailRecords>();
                return;
            }
            RequestData();
            RefreshView();
        }

        protected override void OnClose()
        {
            _dataList = null;
            _contentList.Clear();
            base.OnClose();
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.Overlay;
        }

        private void RequestData(bool append = false)
        {
            if (_project == null) return;
            _data = _project.ProjectRecordRankList;
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }
            _data.Request(_project.ProjectId, startInx, _pageSize, () =>
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
            if (_project == null || _dataList == null)
            {
                _cachedView.GridDataScroller.SetEmpty();
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
            _cachedView.GridDataScroller.SetItemCount(_contentList.Count);
        }

        private void OnItemClick(CardDataRendererWrapper<RecordRankHolder> item)
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请求播放录像");
            _project.PrepareRes(() =>
            {
                item.Content.Record.RequestPlay(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    GameManager.Instance.RequestPlayRecord(_project, item.Content.Record);
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
            item.Init(parent, ResScenary);
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

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlProjectDetailRecords>();
        }
    }
}