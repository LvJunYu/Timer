using GameA.Game;
using UnityEngine;

namespace GameA
{
    /// <summary>
    /// 聊天页面
    /// </summary>
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlShopPay : UICtrlAnimationBase<UIViewShopPay>
    {
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.OpenBlueVipBtn.onClick.AddListener(OnOpenBlueVipBtn);
            var data = TableManager.Instance.Table_DiamondShopDic;
            foreach (var value in data.Values)
            {
                var item = new UMCtrlShopPayItem();
                item.Init(_cachedView.ContentTft, ResScenary);
                item.Set(value);
            }
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromUp);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.FrontUI2;
        }

        private void OnOpenBlueVipBtn()
        {
            WWebViewManager.Instance.Open(ERequestType.OpenBlueVip);
//            Application.OpenURL(
//                "http://pay.qq.com/ipay/index.shtml?n=3&c=xxzxgj,xxqgame&aid=VIP.APP*****.PLATqqgame&ch=qdqb,kj");
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlShopPay>();
        }
    }
}