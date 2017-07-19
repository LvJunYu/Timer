/********************************************************************
** Filename : UMCtrlItem
** Author : Dong
** Date : 2015/7/29 星期三 下午 6:45:32
** Summary : UMCtrlItem
***********************************************************************/


using SoyEngine;
using UnityEngine;
using GameA.Game;
using NewResourceSolution;

namespace GameA
{
    [Poolable(MinPoolSize = 1, PreferedPoolSize = 10, MaxPoolSize = 1000)]
    public class UMCtrlItem : UMCtrlBase<UMViewItem>, IPoolableObject
    {
        private Table_Unit _table;
        private bool _selected;

        public void OnGet()
        {
            Messenger<ushort>.AddListener (EMessengerType.OnSelectedItemChanged, OnSelectedItemChanged);
            Messenger<int>.AddListener (EMessengerType.OnUnitAddedInEditMode, OnUnitAddedInEditMode);
            Messenger<int>.AddListener (EMessengerType.OnUnitDeletedInEditMode, OnUnitAddedInEditMode);
        }

        public void OnFree()
        {
            _cachedView.Trans.SetParent(SocialGUIManager.Instance.UIRoot.Trans, false);
            _cachedView.Trans.localPosition = Vector3.one * 65535;
            Messenger<ushort>.RemoveListener (EMessengerType.OnSelectedItemChanged, OnSelectedItemChanged);
            Messenger<int>.RemoveListener (EMessengerType.OnUnitAddedInEditMode, OnUnitAddedInEditMode);
            Messenger<int>.RemoveListener (EMessengerType.OnUnitDeletedInEditMode, OnUnitAddedInEditMode);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Item.onClick.AddListener(OnItem);
        }

        protected override void OnDestroy()
        {
            _cachedView.Item.onClick.RemoveListener(OnItem);
        }

	    public void OnDestroyObject()
	    {
		    
	    }


		private void OnItem()
        {
            if ((_table.CanRotate || _table.CanMove || _table.Id == UnitDefine.RollerId))// &&
//                !UnitDefine.IsHero(_table.Id))
            {
                if (_selected)
                {
                    EditHelper.ChangeUnitOrigDirOrRot(_table);
                    RefreshArrowRotation();
                }
            }
            
            
            //这儿需要进行Item数量的判断。
            //if (!MapManager.Instance.MapEditor.CanLay(_id))
            //{
            //    //TODO 提示不可摆放
            //    LogHelper.Warning("Can Not Lay.{0}",_id);
            //    return;
            //}
//            SocialGUIManager.Instance.CloseUI<UICtrlItem>();
//            if (!_selected) {
                Messenger<ushort>.Broadcast (EMessengerType.OnSelectedItemChanged, (ushort)PairUnitManager.Instance.GetCurrentId (_table.Id));
//            }
        }

        internal void Set(Table_Unit tableUnit, bool selected)
        {
            _table = tableUnit;
            _selected = selected;
            if (!_isViewCreated)
            {
                return;
            }
            _cachedView.SpriteIcon.sprite = null;
//            DictionaryTools.SetContentText(_cachedView.Name, tableUnit.Name);
            //DictionaryTools.SetContentText(_cachedView.Summary, tableUnit.Summary);
            //DictionaryTools.SetContentText(_cachedView.Count, "1 / 1");
            _cachedView.SpriteIcon.SetActiveEx(true);
//            _cachedView.TextureIcon.SetActiveEx(false);
            Sprite texture;
            if (ResourcesManager.Instance.TryGetSprite(tableUnit.Icon, out texture))
            {
                _cachedView.SpriteIcon.sprite = texture;
            }
            else
            {
                LogHelper.Error("tableUnit {0} icon {1} invalid! tableUnit.EGeneratedType is {2}", tableUnit.Id,
                    tableUnit.Icon, tableUnit.EGeneratedType);
            }
            if (_selected) {
                _cachedView.SpriteIcon.transform.transform.localPosition = Vector3.up * 15;
//                _cachedView.SpriteIcon.transform.transform.localScale = Vector3.one * 1.1f;
                
                // 除了主角，所有能旋转，能移动，还有传送带 都需要显示箭头
                if ((_table.CanRotate || _table.CanMove || _table.Id == UnitDefine.RollerId))// &&
//                    !UnitDefine.IsHero(_table.Id))
                {
                    _cachedView.Arrow.SetActive(true);
                    RefreshArrowRotation();
                }
            } else {
                _cachedView.SpriteIcon.transform.transform.localPosition = Vector3.zero;
//                _cachedView.SpriteIcon.transform.transform.localScale = Vector3.one;
            }

            int currentCnt = EditHelper.GetUnitCnt(_table.Id);
            int limit = LocalUser.Instance.UserWorkshopUnitData.GetUnitLimt(_table.Id);
            int number = limit - currentCnt;
            if (number < 0) number = 0;
            if (number > 1000)
            {
                _cachedView.Number.gameObject.SetActive(false);
                _cachedView.Unlimited.SetActive(true);
            }
            else
            {
                _cachedView.Number.gameObject.SetActive(true);
                _cachedView.Unlimited.SetActive(false);
                _cachedView.Number.text = number.ToString();
            }
        }

        private void OnSelectedItemChanged (ushort id)
        {
            if (id == _table.Id) {
                _selected = true;
                _cachedView.SpriteIcon.transform.transform.localPosition = Vector3.up * 15;
//                _cachedView.SpriteIcon.transform.transform.localScale = Vector3.one * 1.1f;
                _cachedView.ShadowTrans.localScale = Vector3.one * 0.7f;

                // 除了主角，所有能旋转，能移动，还有传送带 都需要显示箭头
                if ((_table.CanRotate || _table.CanMove || _table.Id == UnitDefine.RollerId))// &&
//                    !UnitDefine.IsHero(_table.Id))
                {
                    _cachedView.Arrow.SetActive(true);
                    RefreshArrowRotation();
                }
            } else {
                _selected = false;
                _cachedView.SpriteIcon.transform.transform.localPosition = Vector3.zero;
//                _cachedView.SpriteIcon.transform.transform.localScale = Vector3.one;
                _cachedView.ShadowTrans.localScale = Vector3.one;
                
                _cachedView.Arrow.SetActive(false);
            }
        }

        private void OnUnitAddedInEditMode(int id)
        {
            if (id == _table.Id)
            {
                int currentCnt = EditHelper.GetUnitCnt(_table.Id);
                int limit = LocalUser.Instance.UserWorkshopUnitData.GetUnitLimt(_table.Id);
                int number = limit - currentCnt;
                if (number < 0) number = 0;
                if (number > 1000)
                {
                    _cachedView.Number.gameObject.SetActive(false);
                    _cachedView.Unlimited.SetActive(true);
                }
                else
                {
                    _cachedView.Number.gameObject.SetActive(true);
                    _cachedView.Unlimited.SetActive(false);
                    _cachedView.Number.text = number.ToString();
                }
            }
        }

        private void RefreshArrowRotation()
        {
            EMoveDirection current = EditHelper.GetUnitOrigDirOrRot(_table);
            if (_table.CanMove || _table.Id == UnitDefine.RollerId)
            {
                _cachedView.Arrow.transform.localEulerAngles = new Vector3(0, 0, -90 * (byte)(current - 1));
            }
            else if (_table.CanRotate)
            {
                _cachedView.Arrow.transform.localEulerAngles = new Vector3(0, 0, -90 * (byte)(current));
            }
        }
    }
}