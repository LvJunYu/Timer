using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlProjectDetailInfo : UISocialCtrlBase<UIViewProjectDetailInfo>
    {
        #region Fields
        private Project _project;
        #endregion

        #region Properties
        #endregion

        #region Methods
        protected override void OnOpen (object parameter)
        {
            base.OnOpen(parameter);

            if (null == parameter) {
                SocialGUIManager.Instance.CloseUI<UICtrlProjectDetailInfo> ();
            }
            _project = parameter as Project;
            if (null == _project) {
                SocialGUIManager.Instance.CloseUI<UICtrlProjectDetailInfo> ();
            }

            RefreshView ();
        }
        
        protected override void OnClose() {
            
            base.OnClose ();
        }
        
        protected override void InitEventListener() {
            base.InitEventListener ();
        }
        
        protected override void OnViewCreated() {
            base.OnViewCreated ();

            _cachedView.CloseBtn.onClick.AddListener (OnClose);
            _cachedView.PlayBtn.onClick.AddListener (OnPlayBtn);
            _cachedView.AddTagBtn.onClick.AddListener (OnAddTagBtn);
        }
        
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        private void RefreshView () {
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _project.IconPath, _cachedView.DefaultCover);
            _cachedView.ProjectTitle.text = _project.Name;
            _cachedView.ProjectDesc.text = _project.Summary;
            _cachedView.AuthorName.text = _project.UserInfo.NickName;
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.AuthorImg, 
                _project.UserInfo.HeadImgUrl,
                _cachedView.DefaultHeadImg);

            _cachedView.ServedPlayerCnt.text = _project.ExtendData.PlayCount.ToString();
            _cachedView.LikedPlayerCnt.text = _project.ExtendData.LikeCount.ToString();
            _cachedView.PassRate.text = string.Format ("{0:F1}%", (float)_project.ExtendData.CompleteCount / _project.ExtendData.PlayCount);
        }

        private void OnPlayBtn () {

        }

        private void OnCloseBtn () {
            SocialGUIManager.Instance.CloseUI<UICtrlPublishProject> ();
        }


        private void OnAddTagBtn () {
        }
        #endregion
    }
}
