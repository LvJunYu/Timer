﻿using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UICommon)]
    public class UICtrlWWebView : UICtrlResManagedBase<UIViewWWebView>
    {
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
//            var curResolution = ScreenResolutionManager.Instance.CurResolution;
//            var center =
//                SocialGUIManager.ScreenToRectLocal(new Vector2(curResolution.width / 2, curResolution.height / 2),
//                    _cachedView.Trans);
//            var leftTop = SocialGUIManager.ScreenToRectLocal(new Vector2(
//                curResolution.width / 2 - WWebViewManager.Instance.Width / 2,
//                curResolution.height / 2 - WWebViewManager.Instance.Height / 2), _cachedView.Trans);
//            _cachedView.ImgRtf.localPosition = new Vector2(0, 15);
//            var width = Mathf.Abs((leftTop - center).x) * 2;
//            var height = Mathf.Abs((leftTop - center).y) * 2 + 30;
//            _cachedView.ImgRtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
//            _cachedView.ImgRtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpDialog;
        }

        private void OnCloseBtn()
        {
            WWebViewManager.Instance.Destroy();
            SocialGUIManager.Instance.CloseUI<UICtrlWWebView>();
        }
    }
}