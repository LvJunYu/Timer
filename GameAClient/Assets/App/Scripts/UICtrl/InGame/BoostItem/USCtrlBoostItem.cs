using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using SoyEngine;
using GameA.Game;

namespace GameA
{
    public class USCtrlBoostItem : USCtrlBase<USViewBoostItem>
    {
        #region 常量与字段
        private int _price;
        private int _number;
        private bool _checked = false;
        #endregion

        #region 属性
        public bool Checked
        {
            get {
                return _checked;
            }
        }

        public int BoostItemType
        {
            get
            {
                return _cachedView.BoostItemType;
            }
        }

        /// <summary>
        /// 如果要用这个道具的话需要花费多少钻石
        /// </summary>
        /// <value>The price.</value>
        public int Price
        {
            get
            {
                if (_number >0)
                {
                    return 0;
                } else
                {
                    return _price;
                }
            }
        }

        #endregion

        #region 方法
        public override void Init (USViewBoostItem view)
        {
            base.Init (view);
        }

        protected override void OnViewCreated ()
        {
            base.OnViewCreated ();
            _cachedView.Btn.onClick.AddListener (OnBtn);
        }
        public void SetItem (PropItem item)
        {
            if (item.Type != _cachedView.BoostItemType)
                return;
            _number = item.Count;
            if (0 == _number)
            {
                _cachedView.DiamondIcon.SetActive (true);
                var tableBoostItem = TableManager.Instance.GetBoostItem (item.Type);
                if (null != tableBoostItem)
                {
                    _price = tableBoostItem.PriceDiamond;
                    _cachedView.Number.text = _price.ToString ();
                }
                else
                {
                    LogHelper.Error ("Get boostItem table failed, id:D {0}", item.Type);
                    _price = -1;
                    _cachedView.Number.text = "--";
                }
            } else
            {
                _cachedView.DiamondIcon.SetActive (false);
                _cachedView.Number.text = item.Count.ToString ();
            }
        }

        private void OnBtn ()
        {
            if (_number <= 0) {
                if (_price <= 0) return;
                _checked = true;
            }
            else
            {
                _checked = true;
            }
        }

        #endregion
    }
}