using SoyEngine;
using GameA.Game;

namespace GameA
{
    public class USCtrlBoostItem : USCtrlBase<USViewBoostItem>
    {
        private int _price;
        private int _number;
        private bool _checked;

        public bool Checked
        {
            get { return _checked; }
        }

        public int BoostItemType
        {
            get { return _cachedView.BoostItemType; }
        }

        /// <summary>
        /// 如果要用这个道具的话需要花费多少钻石
        /// </summary>
        /// <value>The price.</value>
        public int Price
        {
            get
            {
                if (_number > 0)
                {
                    return 0;
                }
                else
                {
                    return _price;
                }
            }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Btn.onClick.AddListener(OnBtn);
        }

        public void SetItem(PropItem item)
        {
            if (item.Type != _cachedView.BoostItemType)
                return;
            _number = item.Count;
            if (0 == _number)
            {
                _cachedView.DiamondIcon.SetActive(true);
                var tableBoostItem = TableManager.Instance.GetBoostItem(_cachedView.BoostItemType);
                if (null != tableBoostItem)
                {
                    _cachedView.DiamondIcon.SetActive(true);
                    _price = tableBoostItem.PriceDiamond;
                    _cachedView.Number.text = _price.ToString();
                }
                else
                {
                    LogHelper.Error("Get boostItem table failed, id:D {0}", item.Type);
                    _price = -1;
                    _cachedView.Number.text = "--";
                }
            }
            else
            {
                _cachedView.DiamondIcon.SetActive(false);
                _cachedView.Number.text = item.Count.ToString();
            }
            _checked = false;
            _cachedView.CheckImg.SetActive(false);
        }

        public void SetEmpty()
        {
            _number = 0;
            _cachedView.DiamondIcon.SetActive(true);
            var tableBoostItem = TableManager.Instance.GetBoostItem(_cachedView.BoostItemType);
            if (null != tableBoostItem)
            {
                _cachedView.DiamondIcon.SetActive(true);
                _price = tableBoostItem.PriceDiamond;
                _cachedView.Number.text = _price.ToString();
            }
            else
            {
                LogHelper.Error("Get boostItem table failed, id:D {0}", _cachedView.BoostItemType);
                _price = -1;
                _cachedView.Number.text = "--";
            }
            _checked = false;
            _cachedView.CheckImg.SetActive(false);
        }

        private void OnBtn()
        {
            if (_number <= 0)
            {
                if (_price <= 0) return;
                _checked = !_checked;
            }
            else
            {
                _checked = !_checked;
            }
            _cachedView.CheckImg.SetActive(_checked);
        }
    }
}