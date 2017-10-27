using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorkShopProjectEditing : UPCtrlWorkShopProjectBase
    {
        private PersonalProjectList _data;
        
        protected override void RequestData(bool append = false)
        {
            _data = LocalUser.Instance.PersonalProjectList;
            if (!_data.IsInited || _data.IsDirty) return;
            _data.Request(() =>
            {
                _projectList = _data.ProjectList;
                if (_isOpen)
                {
                    RefreshView();
                }
            });
        }

        private void OnNewProjectBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlSetProjectSize>();
        }

        protected override IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlWorldProject();
            item.Init(parent, _resScenary);
            item.SetEditMode();
            return item;
        }
    }
}