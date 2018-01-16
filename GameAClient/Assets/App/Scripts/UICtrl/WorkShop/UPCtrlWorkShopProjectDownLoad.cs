using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorkShopProjectDownload : UPCtrlWorkShopProjectBase
    {
        private bool _isRequesting;
        private PersonalProjectList _data = LocalUser.Instance.PersonalProjectList;

        public override void RequestData(bool append = false)
        {
            if (_data.IsInited && !_data.IsDirty || _isRequesting) return;
            _isRequesting = true;
            int startInx = 0;
            if (append)
            {
                startInx = _data.AllDownloadList.Count;
            }

            _data.Request(EWorkShopProjectType.WSPT_Download, startInx, _pageSize, () =>
            {
                _projectList = _data.AllDownloadList;
                if (_isOpen)
                {
                    RefreshView();
                }

                _isRequesting = false;
            }, code => _isRequesting = false);
        }

        public override void RefreshView()
        {
            _projectList = _data.AllDownloadList;
            _cachedView.EmptyObj.SetActiveEx(_projectList == null || _projectList.Count == 0);
            _contentList.Clear();
            _dict.Clear();
            if (_projectList != null)
            {
                _contentList.Capacity = Mathf.Max(_contentList.Capacity, _projectList.Count + 1);
                for (int i = 0; i < _projectList.Count; i++)
                {
                    CardDataRendererWrapper<Project> w =
                        new CardDataRendererWrapper<Project>(_projectList[i], OnItemClick);
                    _contentList.Add(w);
                    _dict.Add(_projectList[i].ProjectId, w);
                }
            }
            _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_contentList.Count);
        }

        protected override void OnItemClick(CardDataRendererWrapper<Project> item)
        {
            if (null != item && null != item.Content)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlWorkShopEdit>(item.Content);
            }
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
                if (!_data.DownloadIsEnd)
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
            var item = new UMCtrlProject();
            item.SetCurUI(UMCtrlProject.ECurUI.Download);
            item.Init(parent, _resScenary);
            return item;
        }
    }
}