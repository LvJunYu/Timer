/********************************************************************
** Filename : UMCtrlProjectCard
** Author : Dong
** Date : 2015/4/30 22:30:30
** Summary : UMCtrlProjectCard
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA
{
    public class UMCtrlProjectCard : UMCtrlCardBase<UMViewProjectCard>
    {
        private Project _content;

        public override object Data
        {
            get { return _content; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Card.onClick.AddListener(OnCardClick);
        }

        protected override void OnDestroy()
        {
            _cachedView.Card.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        private void OnCardClick()
        {
            ProjectParams param = new ProjectParams(){
                Type = EProjectParamType.Project,
                Project = _content
            };
            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(param);
        }

        public override void Set(object obj)
        {
            _content = obj as Project;
            RefreshView();
        }

        public void RefreshView()
        {
            if(null == _content)
            {
                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Icon, _cachedView.DefaultCoverTexture);
                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon, _cachedView.DefaultUserTexture);
                return;
            }
            if(_content.ProjectStatus == SoyEngine.Proto.EProjectStatus.PS_Public && !_content.IsValid)
            {
                DictionaryTools.SetContentText(_cachedView.Name, "[已删除]");
            }
            else
            {
                DictionaryTools.SetContentText(_cachedView.Name, _content.Name);
            }
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Icon, _content.IconPath, _cachedView.DefaultCoverTexture);
            User u = _content.UserLegacy;
            DictionaryTools.SetContentText(_cachedView.AuthorName, u.NickName);
            DictionaryTools.SetContentText(_cachedView.Summary, _content.Summary);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, _content.UserLegacy.HeadImgUrl, _cachedView.DefaultUserTexture);
//            DictionaryTools.SetContentText(_cachedView.ProjectCategoryText, EnumStringDefine.GetProjectCategoryString(_content.ProjectCategory));
        }

        public override void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Icon, _cachedView.DefaultCoverTexture);
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon, _cachedView.DefaultUserTexture);
        }
    }
}
