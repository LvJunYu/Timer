using GameA.Game;

namespace GameA
{
    public class UMCtrlShopPayItem : UMCtrlBase<UMViewShopPayItem>
    {
        private Table_DiamondShop _data;

        public void Set(Table_DiamondShop data)
        {
            _data = data;
            _cachedView.BasicNumTxt.text = string.Format("x{0}", _data.Count);
            _cachedView.ExtraNumTxt.text = _data.AdditionalCount.ToString();
            _cachedView.CostNumTxt.text = string.Format("￥ {0}", _data.Price / 10);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Btn.onClick.AddListener(OnBtn);
        }

        private void OnBtn()
        {
            WWebViewManager.Instance.Open(ERequestType.BuyItem, _data.Id, 1);
        }
    }
}