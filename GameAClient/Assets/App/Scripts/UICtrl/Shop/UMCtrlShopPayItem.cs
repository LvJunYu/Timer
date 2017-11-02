
using UnityEngine;

namespace GameA
{
    public class UMCtrlShopPayItem : UMCtrlBase<UMViewShopPayItem>
    {
        private PayItem _data;

        public void Set(PayItem data)
        {
            _data = data;
            _cachedView.BasicNumTxt.text = string.Format("x{0}", _data._basicNum);
            _cachedView.ExtraNumTxt.text = _data._extraNum.ToString();
            _cachedView.CostNumTxt.text = string.Format("￥ {0}", _data._costNum);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Btn.onClick.AddListener(OnBtn);
        }

        private void OnBtn()
        {
        }
    }
}