using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameA
{
    public class UPCtrlWorldCandidate : UPCtrlWorldProjectBase
    {
        private WorldOfficialRecommendCandidateProjectList _data;

        protected new readonly List<CardDataRendererWrapper<WorldOfficialRecommendPrepareProject>>
            _contentList = new List<CardDataRendererWrapper<WorldOfficialRecommendPrepareProject>>();

        protected new Dictionary<long, CardDataRendererWrapper<WorldOfficialRecommendPrepareProject>> _dict =
            new Dictionary<long, CardDataRendererWrapper<WorldOfficialRecommendPrepareProject>>();

        protected new List<WorldOfficialRecommendPrepareProject> _projectList;
        public List<long> OpsrateProjectList = new List<long>();

        protected override void OnViewCreated()
        {
            _eCurUi = GetUMCurUI(_menu);
            _cachedView.CandidateGridData.Set(OnItemRefresh, GetItemRenderer);
            _cachedView.CandidateRect.OnEndDragEvent.AddListener(OnEndDragEvent);
            _cachedView.CancelBtn.onClick.AddListener(Close);
            _cachedView.MoveOutCandidateBtn.onClick.AddListener(OnMoveOutCandidateBtn);
            _cachedView.RecommendBtn.onClick.AddListener(OnRecommendBtn);
        }

        public override void Open()
        {
            _isOpen = true;
            _cachedView.CandidatePanel.SetActiveEx(true);
            OpsrateProjectList.Clear();
            RequestData();
        }

        public override void RequestData(bool append = false)
        {
            base.RequestData(append);
            _data = AppData.Instance.WorldData.WorldOfficialCandidateProjectList;
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }

            _isRequesting = true;
            _data.Request(startInx, _pageSize, () =>
            {
                _projectList = _data.AllList;
                if (_isOpen)
                {
                    RefreshView();
                }

                _isRequesting = false;
            }, code =>
            {
                _isRequesting = false;
                LogHelper.Error("WorldNewestProjectList Request fail, code = {0}", code);
            });
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

        protected override IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlProjectGMCanditate();
            item.Init(parent, _resScenary);
            return item;
        }

        public override void RefreshView()
        {
            _cachedView.EmptyObj.SetActiveEx(_projectList == null || _projectList.Count == 0);
            _contentList.Clear();
            _dict.Clear();
            if (_projectList == null)
            {
                _cachedView.CandidateGridData.SetEmpty();
                return;
            }

            _contentList.Capacity = Mathf.Max(_contentList.Capacity, _projectList.Count);
            for (int i = 0; i < _projectList.Count; i++)
            {
                if (!_dict.ContainsKey(_projectList[i].Id))
                {
                    CardDataRendererWrapper<WorldOfficialRecommendPrepareProject> w =
                        new CardDataRendererWrapper<WorldOfficialRecommendPrepareProject>(_projectList[i], OnItemClick);
                    _contentList.Add(w);
                    if (_dict.ContainsKey(_projectList[i].Id))
                    {
                    }
                    else
                    {
                        _dict.Add(_projectList[i].Id, w);
                    }
                }
            }

            _cachedView.CandidateGridData.SetItemCount(_contentList.Count);
        }

        protected void OnItemClick(CardDataRendererWrapper<WorldOfficialRecommendPrepareProject> item)
        {
            if (item == null || item.Content == null)
            {
                return;
            }

            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(item.Content.ProjectData);
            SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>().RefreshGMProject(item.Content.Status);
        }

        protected override void OnEndDragEvent(PointerEventData arg0)
        {
        }

        private void OnMoveOutCandidateBtn()
        {
            RemoteCommands.GmUpdateProjectOfficialRecommendStatus(OpsrateProjectList,
                EOfficialRecommendProjectStatus.EORPS_None,
                ret =>
                {
                    if (ret.ResultCode == (int) ENetResultCode.NR_Success)
                    {
                        SocialGUIManager.ShowPopupDialog("移除待选成功！");
                        RequestData();
                    }
                },
                code => { SocialGUIManager.ShowPopupDialog("移除待选失败"); });
            OpsrateProjectList.Clear();
        }

        private void OnRecommendBtn()
        {
            RemoteCommands.GmUpdateProjectOfficialRecommendStatus(OpsrateProjectList,
                EOfficialRecommendProjectStatus.EORPS_Prepare,
                ret =>
                {
                    if (ret.ResultCode == (int) ENetResultCode.NR_Success)
                    {
                        SocialGUIManager.ShowPopupDialog("移除待选成功！");
                        RequestData();
                    }
                },
                code => { SocialGUIManager.ShowPopupDialog("推荐失败"); });
            OpsrateProjectList.Clear();
        }

        public override void Close()
        {
            _cachedView.CandidatePanel.SetActiveEx(false);
            _isOpen = false;
        }
    }
}