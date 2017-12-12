using System.Collections.Generic;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class USCtrlAddItem : USCtrlBase<USViewAddItem>
    {
        private int _curIndex;
        private EItemType _eItemType;
        private bool _isSelecting;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.AddNewBtn.onClick.AddListener(OnAddBtn);
            for (int i = 0; i < _cachedView.ItemBtn.Length; i++)
            {
                _cachedView.ItemBtn[i].onClick.AddListener(OnAddBtn);
            }
        }

        private void OnAddBtn()
        {
            _isSelecting = !_isSelecting;
            _cachedView.SelectDockBtn.SetActiveEx(_isSelecting);
        }

        public void Set(MultiParam data, EItemType eItemType)
        {
            _isSelecting = false;
            _cachedView.SelectDockBtn.SetActiveEx(false);
            _eItemType = eItemType;
            List<int> dataList = data.ToList();
            for (int i = 0; i < _cachedView.ItemBtn.Length; i++)
            {
                if (i < dataList.Count)
                {
                    Sprite sprite = null;
                    switch (_eItemType)
                    {
                        case EItemType.Drops:
                            var table = TableManager.Instance.GetUnit(dataList[i]);
                            if (table != null)
                            {
                                sprite = JoyResManager.Instance.GetSprite(table.Icon);
                            }
                            break;
                        case EItemType.States:
                            //todo
                            break;
                    }
                    if (sprite != null)
                    {
                        _cachedView.ItemBtn[i].GetComponent<Image>().sprite = sprite;
                    }
                    //todo 默认图片
                    _cachedView.ItemBtn[i].SetActiveEx(true);
                }
                else
                {
                    _cachedView.ItemBtn[i].SetActiveEx(false);
                }
            }
        }

        public enum EItemType
        {
            Drops,
            States,
        }

        public void SetEnable(bool value)
        {
            _cachedView.SetActiveEx(value);
        }
    }
}