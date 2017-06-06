/********************************************************************
** Filename : UICtrlCreate
** Author : Dong
** Date : 2015/7/29 星期三 下午 4:00:51
** Summary : UICtrlCreate
***********************************************************************/


using SoyEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Create)]
    public class UICtrlCreate : UICtrlInGameBase<UIViewCreate>
    {
	    private ushort _curSelectId = 0;
	    private UIDragMapItemController _dragController;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _curSelectId = 0;
            UpdateCurSelectItemShow();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Create.onClick.AddListener(OnCreate);
            _cachedView.CurSelectItemButton.onClick.AddListener(OnClickCurSelectItem);
			_dragController = UIDragMapItemController.AddEventListenerTo(_cachedView.CurSelectItemButton.gameObject);
				
//			Messenger<ushort>.AddListener(EMessengerType.OnSelectedItemChanged, OnSelectItemChanged);
		}

	    protected override void InitEventListener()
	    {
		    base.InitEventListener();
			RegisterEvent(EMessengerType.OnEditorLayerChanged, OnEditorLayerChanged);
	    }

	    protected override void OnDestroy()
        {
            base.OnDestroy();
            _cachedView.Create.onClick.RemoveListener(OnCreate);
            _cachedView.CurSelectItemButton.onClick.RemoveListener(OnClickCurSelectItem);
//            Messenger<ushort>.RemoveListener(EMessengerType.OnSelectedItemChanged, OnSelectItemChanged);
		}

		private void OnCreate()
        {
            //GM2DGUIManager.Instance.CloseUI<UICtrlCreate>();
            SocialGUIManager.Instance.OpenUI<UICtrlItem>();
        }

        private void OnClickCurSelectItem()
        {
            if (_curSelectId > 0)
            {
                Messenger<ushort>.Broadcast(EMessengerType.OnSelectedItemChanged, _curSelectId);
            }
        }


		#region event

		public void OnSelectItemChanged(ushort id)
		{
			_curSelectId = id;
			_dragController.SetCurSelectId(_curSelectId);
			UpdateCurSelectItemShow();
		}

        private void OnEditorLayerChanged()
	    {
//		    _curSelectId = 0;
//		    UpdateCurSelectItemShow();
	    }


        #endregion


        #region private

        private void UpdateCurSelectItemShow()
        {
            if (_curSelectId <= 0)
            {
                _cachedView.ShowCurSelectIcon.SetActiveEx(false);
                _cachedView.SpriteImage.sprite = null;
                _cachedView.TextureImage.texture = null;
            }
            else
            {
                Table_Unit table = UnitManager.Instance.GetTableUnit(_curSelectId);
                if (table  == null)
                {
                    return;
                }
                _cachedView.ShowCurSelectIcon.SetActiveEx(true);
                _cachedView.TextureImage.SetActiveEx(false);
                Sprite texture;
                if (GameResourceManager.Instance.TryGetSpriteByName(table.Icon, out texture))
                {
                    _cachedView.SpriteImage.sprite = texture;
                }
                else
                {
                    LogHelper.Error("tableUnit {0} icon {1} invalid! tableUnit.EGeneratedType is {2}", table.Id,
                        table.Icon, table.EGeneratedType);
                }
            }
        }

        #endregion
    }
}