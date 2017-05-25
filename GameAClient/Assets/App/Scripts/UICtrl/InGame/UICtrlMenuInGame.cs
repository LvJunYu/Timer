   /********************************************************************
   ** Filename : UICtrlMenuInGame.cs
   ** Author : quan
   ** Date : 2015/7/11 20:35
   ** Summary : 游戏内菜单
   ***********************************************************************/

using SoyEngine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlMenuInGame : UICtrlGenericBase<UIViewMenuInGame>
    {
        #region 常量与字段
        private bool _hasDrag = false;
        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.InGameStart;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            //RegisterEvent(EMessengerType.OnGameStartComplete, OnGameStartComplete);
            //RegisterEvent(GameA.Game.EMessengerType.OnCloseGameSetting, OnCloseGameSetting);
            //RegisterEvent(EMessengerType.OnEscapeClick, OnEscapeClick);
		}

		private void OnGameStartComplete()
        {
            //SocialGUIManager.Instance.OpenUI<UICtrlMenuInGame>();
        }

	    private void OnCloseGameSetting()
	    {
		    //ShowMenu(false);
	    }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ReturnToAppBtn.onClick.AddListener(OnReturnToAppBtnClick);
            _cachedView.ReturnToGameBtn.onClick.AddListener(OnReturnToGameBtnClick);
            _cachedView.BgButton.onClick.AddListener(OnReturnToGameBtnClick);
            _cachedView.FloatBtn.Button.onClick.AddListener(OnFloatBtnClick);
            _cachedView.FloatBtn.OnBeginDragEvent.AddListener(OnFloatBtnDrag);
            _cachedView.FloatBtn.OnDragEvent.AddListener(OnFloatBtnDrag);
            _cachedView.FloatBtn.OnEndDragEvent.AddListener(OnFloatBtnEndDrag);
        }

        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            ShowMenu(false);
        }

        private void OnReturnToAppBtnClick()
        {
            ShowMenu(false);
            SocialApp.Instance.ReturnToApp();
        }

        private void OnReturnToGameBtnClick()
        {
            ShowMenu(false);
        }

        private void OnFloatBtnClick()
        {
            if (_hasDrag) {
                return;
            }
            ShowMenu(true);
        }
        
        private void OnFloatBtnEndDrag(PointerEventData eventData)
        {
            _hasDrag = false;
            FloatButtonMove(eventData.delta);
        }

        private void OnFloatBtnDrag(PointerEventData eventData)
        {
            _hasDrag = true;
            FloatButtonMove(eventData.delta);
        }

        private void FloatButtonMove(Vector2 eventDelta)
        {
            Vector2 pSize = _cachedView.Trans.rect.size;
            if(Mathf.Min(pSize.x, pSize.y)<1)
            {
                return;
            }
            float scaleFactor = _cachedView.FloatBtn.Button.targetGraphic.canvas.transform.localScale.x;
            Vector2 delta = eventDelta/scaleFactor;
            delta.Set(delta.x/pSize.x, delta.y/pSize.y);
            RectTransform tran = _cachedView.FloatBtn.RectTransform;
            Vector2 anchor = tran.anchorMax;
            anchor += delta;
            anchor = Vector2.Max(Vector2.zero, Vector2.Min(Vector2.one, anchor));
            tran.anchorMax = anchor;
            tran.anchorMin = anchor;
            tran.anchoredPosition = Vector2.zero;
        }

        private void ShowMenu(bool show)
        {
			_cachedView.FloatBtn.gameObject.SetActive(!show);
			if (show)
	        {
				Messenger.Broadcast(EMessengerType.OpenGameSetting);
			}
			else
	        {
				_cachedView.BgButton.gameObject.SetActive(show);
				_cachedView.ListDock.gameObject.SetActive(show);
			}
		}

        private void OnEscapeClick()
        {
            if(!_isOpen)
            {
                return;
            }
            if(!_cachedView.FloatBtn.gameObject.activeInHierarchy)
            {
                return;
            }
            OnFloatBtnClick();
        }
        #endregion
    }
}