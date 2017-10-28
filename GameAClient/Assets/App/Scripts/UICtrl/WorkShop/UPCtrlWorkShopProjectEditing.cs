using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorkShopProjectEditing : UPCtrlWorkShopProjectBase
    {
        private PersonalProjectList _data;

        public override void RequestData(bool append = false)
        {
            _data = LocalUser.Instance.PersonalProjectList;
            if (_data.IsInited && !_data.IsDirty) return;
            _data.Request(() =>
            {
                _projectList = _data.ProjectList;
                if (_isOpen)
                {
                    RefreshView();
                }
            });
        }

        public override void RefreshView()
        {
            _projectList = _data.ProjectList;
            _cachedView.EmptyObj.SetActiveEx(_projectList == null || _projectList.Count == 0);
            _contentList.Clear();
            _dict.Clear();
            _contentList.Add(new CardDataRendererWrapper<Project>(Project.NewEditProject, OnNewItemClick));
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

        private void OnNewItemClick(CardDataRendererWrapper<Project> item)
        {
            SocialGUIManager.Instance.OpenUI<UICtrlSetProjectSize>();
        }

        protected override IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlProject();
            item.SetMode(UMCtrlProject.EFunc.Editing);
            item.Init(parent, _resScenary);
            return item;
        }
    }
}