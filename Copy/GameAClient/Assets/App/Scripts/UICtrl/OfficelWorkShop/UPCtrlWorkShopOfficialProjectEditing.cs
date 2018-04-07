using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorkShopOfficialProjectEditing : UPCtrlWorkShopOfficialProjectBase
    {
        private bool _isRequesting;
        private PersonalProjectList _data = LocalUser.Instance.PersonalProjectList;

        public override void RequestData(bool append = false)
        {
            if (_isRequesting) return;
            _isRequesting = true;
            int startInx = 0;
            if (append)
            {
                startInx = _data.AllEdittingList.Count;
            }

            _data.Request(EWorkShopProjectType.WSPT_Editting, startInx, _pageSize, () =>
            {
                _projectList = _data.AllEdittingList;
                if (_isOpen)
                {
                    RefreshView();
                }

                _isRequesting = false;
            }, code => _isRequesting = false, true);
        }

        public override void RefreshView()
        {
            _projectList = _data.AllEdittingList;
            _cachedView.EmptyObj.SetActiveEx(_projectList == null || _projectList.Count == 0);
            _contentList.Clear();
            _dict.Clear();
            _contentList.Add(new CardDataRendererWrapper<Project>(Project.EmptyProject, OnNewItemClick));
            if (_projectList != null)
            {
                _contentList.Capacity = Mathf.Max(_contentList.Capacity, _projectList.Count + 1);
                for (int i = 0; i < _projectList.Count; i++)
                {
                    CardDataRendererWrapper<Project> w =
                        new CardDataRendererWrapper<Project>(_projectList[i], OnItemClick);
                    _contentList.Add(w);
                    if (_dict.ContainsKey(_projectList[i].ProjectId))
                    {
                    }
                    else
                    {
                        _dict.Add(_projectList[i].ProjectId, w);
                    }
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

        private void OnNewItemClick(CardDataRendererWrapper<Project> item)
        {
            SocialGUIManager.Instance.OpenUI<UICtrlSetProjectSize>(true);
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
                if (!_data.EdittingIsEnd)
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
            item.SetCurUI(UMCtrlProject.ECurUI.Editing);
            item.Init(parent, _resScenary);
            return item;
        }

        public void OnWorkShopProjectPublished(long projectId)
        {
            var project = _data.AllEdittingList.Find(p => p.ProjectId == projectId);
            if (project != null)
            {
                _data.AllEdittingList.Remove(project);
            }

            RefreshView();
        }
    }
}