using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlSurpriseBoxItem : UMCtrlBase<UMViewSurpriseBoxItem>
    {
        private USCtrlItemsSetting _mainCtrl;
        private bool _add;
        private int _id;
        private int _index;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.AddBtn.onClick.AddListener(OnAddBtn);
            _cachedView.DeleteBtn.onClick.AddListener(OnDeleteBtn);
        }

        public void Set(int id, bool add, USCtrlItemsSetting ctrl, int index = 0)
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

            _cachedView.AddBtn.SetActiveEx(_add);
            _cachedView.DeleteBtn.SetActiveEx(!_add);
        }

        private void OnDeleteBtn()
        {
            _mainCtrl.DeleteItem(_index);
        }

        private void OnAddBtn()
        {
            _mainCtrl.AddItem(_id);
        }

        public void SetParent(RectTransform parent)
        {
            _cachedView.Trans.SetParent(parent);
            _cachedView.Trans.anchoredPosition = Vector2.zero;
        }
    }
}