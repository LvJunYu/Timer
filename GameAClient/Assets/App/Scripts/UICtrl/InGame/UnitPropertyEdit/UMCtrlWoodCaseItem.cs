using GameA.Game;
using NewResourceSolution;
using UnityEngine;

namespace GameA
{
    public class UMCtrlWoodCaseItem : UMCtrlBase<UMViewWoodCaseItem>
    {
        private const string SpriteNoneName = "img_box_prohibit";
        private USCtrlWoodCaseItemsSetting _mainCtrl;
        private int _id;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Btn.onClick.AddListener(OnBtn);
        }

        public void Set(int id, USCtrlWoodCaseItemsSetting ctrl)
        {
            _id = id;
            _mainCtrl = ctrl;
            _cachedView.Icon.sprite = GetSprite(id);
            if (id == 0)
            {
                _cachedView.Icon.SetNativeSize();
            }
        }

        private void OnBtn()
        {
            _mainCtrl.Select(_id);
        }

        public void SetSelected(int id)
        {
            _cachedView.SelectedObj.SetActive(id == _id);
        }

        public static Sprite GetSprite(int id)
        {
            if (id != 0)
            {
                var table = UnitManager.Instance.GetTableUnit(id);
                if (table != null)
                {
                    return JoyResManager.Instance.GetSprite(table.Icon);
                }
            }

            return JoyResManager.Instance.GetSprite(SpriteNoneName);
        }
    }
}