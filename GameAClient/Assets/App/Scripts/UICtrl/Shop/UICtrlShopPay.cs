using UnityEngine;

namespace GameA
{
    /// <summary>
    /// 聊天页面
    /// </summary>
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlShopPay : UICtrlAnimationBase<UIViewShopPay>
    {
        public static PayItem[] PayItems =
        {
            new PayItem(6, 60, 30),
            new PayItem(30, 300, 150),
            new PayItem(98, 980, 490),
            new PayItem(198, 1980, 990),
            new PayItem(328, 3280, 1640),
            new PayItem(648, 6480, 3240)
        };

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.OpenBlueVipBtn.onClick.AddListener(OnOpenBlueVipBtn);
            for (int i = 0; i < PayItems.Length; i++)
            {
                var item = new UMCtrlShopPayItem();
                item.Init(_cachedView.ContentTft, ResScenary);
                item.Set(PayItems[i]);
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
            Application.OpenURL(
                "http://pay.qq.com/ipay/index.shtml?n=3&c=xxzxgj,xxqgame&aid=VIP.APP*****.PLATqqgame&ch=qdqb,kj");
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlShopPay>();
        }
    }

    public struct PayItem
    {
        public PayItem(int costNum, int basicNum, int extraNum)
        {
            _costNum = costNum;
            _basicNum = basicNum;
            _extraNum = extraNum;
        }

        public int _costNum;
        public int _basicNum;
        public int _extraNum;
    }
}