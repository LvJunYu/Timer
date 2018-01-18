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

        public void Set(DictionaryListObject data, EItemType eItemType)
        {
            _isSelecting = false;
            _cachedView.SelectDockBtn.SetActiveEx(false);
            _eItemType = eItemType;
            int count = data == null ? 0 : data.Count;
            for (int i = 0; i < _cachedView.ItemBtn.Length; i++)
            {
                if (i < count)
                {
                    Sprite sprite = null;
                    switch (_eItemType)
                    {
                        case EItemType.Drops:
                            if (data != null)
                            {
                                var table = TableManager.Instance.GetUnit(data.Get<int>(i));
                                if (table != null)
                                {
                                    sprite = JoyResManager.Instance.GetSprite(table.Icon);
                                }
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

        public void SetSprite(int index, Sprite sprite)
        {
            if (index < _cachedView.Imgs.Length)
            {
                _cachedView.Imgs[index].sprite = sprite;
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