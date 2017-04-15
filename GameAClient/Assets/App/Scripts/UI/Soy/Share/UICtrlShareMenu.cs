  /********************************************************************
  ** Filename : UICtrlShareMenu.cs
  ** Author : quan
  ** Date : 11/15/2016 1:39 PM
  ** Summary : UICtrlShareMenu.cs
  ***********************************************************************/

using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using System;
using DG.Tweening;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlShareMenu : UICtrlGenericBase<UIViewShareMenu>
    {
        #region 常量与字段
        private Color _bgTargetColor;
        private Color _bgAnimationStartColor = new Color(0f, 0f, 0f, 0f);
        private Vector2 _contentTargetPos = Vector2.zero;
        private Vector2 _contentAnimationStartPos;
        private bool _isTweening = false;
        private object _shareObj;
        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _bgTargetColor = _cachedView.BackgroundImage.color;
            _contentAnimationStartPos = new Vector2(0, -_cachedView.ContentDock.rect.size.y);
            _cachedView.CanvasGroup.blocksRaycasts = true;
            _isTweening = false;

            _cachedView.CancelBtn.onClick.AddListener(OnCancelBtnClick);
            _cachedView.BackgroundBtn.onClick.AddListener(OnBgBtnClick);
            _cachedView.ShareToWechatFriendsBtn.onClick.AddListener(OnWechatFriendsClick);
            _cachedView.ShareToWechatMomentsBtn.onClick.AddListener(OnWechatMomentsClick);
            _cachedView.ShareToQQFriendsBtn.onClick.AddListener(OnQQFriendsClick);
            _cachedView.ShareToQZoneBtn.onClick.AddListener(OnQZoneClick);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            StartOpenAnimation();
            _shareObj = parameter;
        }

        private void StartOpenAnimation()
        {
            if(_isTweening)
            {
                return;
            }
            _isTweening = true;
            _cachedView.CanvasGroup.interactable = false;
            _cachedView.BackgroundImage.color = _bgAnimationStartColor;
            _cachedView.ContentDock.anchoredPosition = _contentAnimationStartPos;
            _cachedView.BackgroundImage.DOColor(_bgTargetColor, 0.3f);
            _cachedView.ContentDock.DOAnchorPos(_contentTargetPos, 0.3f)
                .OnComplete(()=>{
                    _isTweening = false;
                    _cachedView.CanvasGroup.interactable = true;
                });
        }

        private void StartCloseAnimation()
        {
            if(_isTweening)
            {
                return;
            }
            _isTweening = true;
            _cachedView.CanvasGroup.interactable = false;
            _cachedView.BackgroundImage.color = _bgTargetColor;
            _cachedView.ContentDock.anchoredPosition = _contentTargetPos;
            _cachedView.BackgroundImage.DOColor(_bgAnimationStartColor, 0.3f);
            _cachedView.ContentDock.DOAnchorPos(_contentAnimationStartPos, 0.3f)
                .OnComplete(()=>{
                    _isTweening = false;
                    SocialGUIManager.Instance.CloseUI<UICtrlShareMenu>();
                });
        }

        private void OnCancelBtnClick()
        {
            StartCloseAnimation();
        }

        private void OnBgBtnClick()
        {
            StartCloseAnimation();
        }

        private void OnWechatFriendsClick()
        {
            ShareUtil.ShareWechatFriends(_shareObj);
            StartCloseAnimation();
        }

        private void OnWechatMomentsClick()
        {
            ShareUtil.ShareToWechatMoments(_shareObj);
            StartCloseAnimation();
        }

        private void OnQQFriendsClick()
        {
            ShareUtil.ShareToQQFriends(_shareObj);
            StartCloseAnimation();
        }

        private void OnQZoneClick()
        {
            ShareUtil.ShareToQZone(_shareObj);
            StartCloseAnimation();
        }
        #endregion
    }
}