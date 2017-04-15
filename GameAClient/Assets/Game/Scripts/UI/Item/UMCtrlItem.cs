﻿/********************************************************************
** Filename : UMCtrlItem
** Author : Dong
** Date : 2015/7/29 星期三 下午 6:45:32
** Summary : UMCtrlItem
***********************************************************************/


using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 1, PreferedPoolSize = 10, MaxPoolSize = 1000)]
    public class UMCtrlItem : UMCtrlBase<UMViewItem>, IPoolableObject
    {
        private ushort _id;

        public void OnGet()
        {
        }

        public void OnFree()
        {
            _cachedView.Trans.SetParent(GM2DGUIManager.Instance.UIRoot.Trans, false);
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
            //这儿需要进行Item数量的判断。
            //if (!MapManager.Instance.MapEditor.CanLay(_id))
            //{
            //    //TODO 提示不可摆放
            //    LogHelper.Warning("Can Not Lay.{0}",_id);
            //    return;
            //}
            GM2DGUIManager.Instance.CloseUI<UICtrlItem>();
            Messenger<ushort>.Broadcast(EMessengerType.OnSelectedItemChanged, (ushort)PairUnitManager.Instance.GetCurrentId(_id));
        }

        internal void Set(Table_Unit tableUnit)
        {
            _id = (ushort) tableUnit.Id;
            if (!_isViewCreated)
            {
                return;
            }
            _cachedView.SpriteIcon.sprite = null;
            DictionaryTools.SetContentText(_cachedView.Name, tableUnit.Name);
            //DictionaryTools.SetContentText(_cachedView.Summary, tableUnit.Summary);
            //DictionaryTools.SetContentText(_cachedView.Count, "1 / 1");
            _cachedView.SpriteIcon.SetActiveEx(true);
            _cachedView.TextureIcon.SetActiveEx(false);
            Sprite texture;
            if (GameResourceManager.Instance.TryGetSpriteByName(tableUnit.Icon, out texture))
            {
                _cachedView.SpriteIcon.sprite = texture;
            }
            else
            {
                LogHelper.Error("tableUnit {0} icon {1} invalid! tableUnit.EGeneratedType is {2}", tableUnit.Id,
                    tableUnit.Icon, tableUnit.EGeneratedType);
            }
		}
    }
}