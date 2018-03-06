using GameA.Game;
using NewResourceSolution;
using UnityEngine;

namespace GameA
{
    public class UMCtrlSurpriseBoxItem : UMCtrlBase<UMViewSurpriseBoxItem>
    {
        private USCtrlSurpriseBoxItemsSetting _mainCtrl;
        private bool _add;
        private int _id;
        private int _index;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Btn.onClick.AddListener(OnBtn);
        }

        public void Set(int id, bool add, USCtrlSurpriseBoxItemsSetting ctrl, int index = 0)
        {
            _id = id;
            _add = add;
            _mainCtrl = ctrl;
            _index = index;
            RefreshView();
        }

        private void RefreshView()
        {
            var table = UnitManager.Instance.GetTableUnit(_id);
            if (table != null)
            {
                _cachedView.Icon.sprite = JoyResManager.Instance.GetSprite(table.Icon);
            }

            _cachedView.AddObj.SetActive(_add);
            _cachedView.DeleteObj.SetActive(!_add);
        }

        private void OnBtn()
        {
            if (_add)
            {
                _mainCtrl.AddItem(_id);
            }
            else
            {
                _mainCtrl.DeleteItem(_index);
            }
        }

        public void SetParent(RectTransform parent)
        {
            _cachedView.Trans.SetParent(parent);
            _cachedView.Trans.anchoredPosition = Vector2.zero;
        }
    }
}