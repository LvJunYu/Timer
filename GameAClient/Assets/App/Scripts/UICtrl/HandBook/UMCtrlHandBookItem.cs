using System;
using UnityEngine;
using System.Collections.Generic;
using NewResourceSolution;
using UnityEngine.UI;
using SoyEngine;
using GameA;
using GameA.Game;

namespace GameA
{
    public class UMCtrlHandBookItem : UMCtrlBase<UMViewHandBookItem>
    {
        #region 变量

        private string _unitIconName = null;
        private int _unitId;
        private Color _colorMask = new Color(0.125f, 0.125f, 0.125f, 1);
        private Sprite _unitIcon;
        private Table_Unit _uint;

        #endregion

        #region 属性

        #endregion

        #region 方法

        public void IintItem(int unitID, bool isUnlock)
        {
            _unitId = unitID;
            _uint = TableManager.Instance.GetUnit(unitID);
            _unitIconName = _uint.Icon;
            if (JoyResManager.Instance.TryGetSprite(_unitIconName, out _unitIcon))
            {
                _cachedView.Icon.sprite = _unitIcon;
            }
            _cachedView.ExplantionBtn.onClick.AddListener(OnBtn);
            if (!isUnlock)
            {
                _cachedView.Icon.color = _colorMask;
            }
            _cachedView.IsLock.gameObject.SetActive(!isUnlock);
        }

        public void OnBtn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlHandBook>().UpdateDesc(_unitId, this);
        }

        public void OnSelect()
        {
            _cachedView.Select.gameObject.SetActive(true);
        }

        public void OnSelectDisable()
        {
            _cachedView.Select.gameObject.SetActive(false);
        }

        #endregion
    }
}