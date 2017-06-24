/********************************************************************
** Filename : UICtrlEdit
** Author : Dong
** Date : 2015/7/2 16:30:13
** Summary : UICtrlEdit
***********************************************************************/

using SoyEngine;
using UnityEngine;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlEdit : UICtrlInGameBase<UIViewEdit>
	{
		public enum EMode
		{
			None,
			// 正常编辑
			Edit,
			// 编辑时测试
			EditTest,
			// 正常游戏
			Play,
			// 播放录像
			PlayRecord,
			// 改造编辑
			ModifyEdit,

		}

        #region 常量与字段
        //private Social.UIDraggableButton _moveBtn;
        //private bool _moveBtnDragged = false;
        private Vector2 _moveBtnOrigPos;
        private Vector2 _moveBtnDragOffset;
        private float _maxMoveY = 270f;

		// 编辑类型，是正常编辑还是改造编辑
		private EMode _editMode = EMode.Edit;
        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

            _cachedView.Erase.onClick.AddListener(OnErase);
			_cachedView.EraseSelected.onClick.AddListener(OnErase);
			_cachedView.Redo.onClick.AddListener(OnRedo);
            _cachedView.Undo.onClick.AddListener(OnUndo);

			_cachedView.Home.onClick.AddListener(OnClickHome);
            if(_cachedView.Capture != null)
            {
                _cachedView.Capture.onClick.AddListener(OnClickCapture);
            }


			_cachedView.Play.onClick.AddListener(OnPlay);
			_cachedView.Publish.onClick.AddListener(OnPublish);
			_cachedView.Pause.onClick.AddListener(OnPause);


			_cachedView.ButtonFinishCondition.onClick.AddListener(OnClickFinishCondition);

	        _cachedView.EnterEffectMode.onClick.AddListener(OnClickEffectModeButton);
	        _cachedView.ExitEffectMode.onClick.AddListener(OnClickEffectModeButton);

            _cachedView.EnterSwitchMode.onClick.AddListener (OnClickSwitchModeButton);
            _cachedView.ExitSwitchMode.onClick.AddListener (OnClickSwitchModeButton);
            //_moveBtn = _cachedView.MoveBtnObj.AddComponent<Social.UIDraggableButton> ();
            //_moveBtn.Button = _moveBtn.gameObject.GetComponent<Button> ();
            //_moveBtn.RectTransform = _moveBtn.GetComponent<RectTransform> ();
            //_moveBtn.Button.onClick.AddListener (OnMoveBtnClick);
            //_moveBtn.OnBeginDragEvent.AddListener (OnMoveBtnDrag);
            //_moveBtn.OnDragEvent.AddListener (OnMoveBtnDrag);
            //_moveBtn.OnEndDragEvent.AddListener (OnMoveBtnEndDrag);
            //_moveBtnOrigPos = _moveBtn.RectTransform.position;
//            _cachedView.MoveBtn.OnPress += OnEnterDragMode;
//            _cachedView.MoveBtn.OnRelease += OnExitDragMode;
//            _cachedView.MoveBtn.OnDragBegin += OnMoveBtnDragBegin;
//            _cachedView.MoveBtn.OnDragMove += OnMoveBtnMove;
//            _cachedView.MoveBtn.OnDragEnd += OnMoveBtnDragEnd;
        }

	    protected override void InitEventListener()
	    {
		    base.InitEventListener();
			RegisterEvent(EMessengerType.AfterCommandChanged, AfterCommandChanged);
	    }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
	        UpdateEraseButtonState();
//	        UpdateEffectModeButtonState();
            UpdateSwitchModeButtonState ();
//            _moveBtnOrigPos = _cachedView.MoveBtn.RectTrans.localPosition;
            //_moveBtnOrigPos = new Vector2 (60, -50);
        }

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			Messenger.RemoveListener(EMessengerType.AfterCommandChanged, AfterCommandChanged);
		}

        public override void OnUpdate ()
        {
            base.OnUpdate ();
//            if (_cachedView.MoveBtn.IsPressed) {
//                Vector2 pos = _cachedView.MoveBtn.RectTrans.localPosition;
//                if (pos.y - _moveBtnOrigPos.y > _maxMoveY * 0.4f) {
//                    Game.CameraManager.Instance.UpdateFadeCameraOrthoSizeOffset (0.12f);
//                } else if (pos.y - _moveBtnOrigPos.y > _maxMoveY * 0.15f) {
//                    Game.CameraManager.Instance.UpdateFadeCameraOrthoSizeOffset (0.05f);
//                } else if (pos.y - _moveBtnOrigPos.y > -_maxMoveY * 0.15f) {
//
//                } else if (pos.y - _moveBtnOrigPos.y > -_maxMoveY * 0.4f) {
//                    Game.CameraManager.Instance.UpdateFadeCameraOrthoSizeOffset (-0.05f);
//                } else {
//                    Game.CameraManager.Instance.UpdateFadeCameraOrthoSizeOffset (-0.12f);
//                }
//            }
        }

        public void ChangeToEditTestMode()
        {
			SetButtonState(EMode.EditTest);
            if(_cachedView.Capture != null)
            {
                _cachedView.Capture.gameObject.SetActive(true);
            }
			if (EditMode.Instance != null)
			{
				EditMode.Instance.SetEditorModeEffect(false);
			}
		}

        public void ChangeToEditMode()
        {
			_editMode = EMode.Edit;
			SetButtonState(EMode.Edit);
        }

        public void ChangeToPlayMode()
        {
			SetButtonState(EMode.Play);		
		}

		public void ChangeToModifyMode () {
			_editMode = EMode.ModifyEdit;
			SetButtonState (EMode.ModifyEdit);
		}

        public void ChangeToPlayRecordMode()
        {
			SetButtonState(EMode.PlayRecord);
        }

//        private void SetButtonState(bool value)
//        {
//            bool active = value;
//            _cachedView.Erase.gameObject.SetActive(active);
//            _cachedView.Redo.gameObject.SetActive(false);
//            _cachedView.Undo.gameObject.SetActive(active);
//            _cachedView.Publish.gameObject.SetActive(active);
//            _cachedView.Play.gameObject.SetActive(active);
//            _cachedView.ButtonFinishCondition.SetActiveEx(active);
//
//            //_cachedView.EnterDragMode.SetActiveEx(active);
//            //_cachedView.ExitDragMode.SetActiveEx(false);
//            _cachedView.MoveBtn.SetActiveEx (active);
//            _cachedView.MoveBtnBg.SetActive (active);
//
//			_cachedView.EnterEffectMode.SetActiveEx(active);
//			_cachedView.ExitEffectMode.SetActiveEx(false);
//
//            _cachedView.Pause.gameObject.SetActive(active);
//            if(_cachedView.Capture != null)
//            {
//                _cachedView.Capture.gameObject.SetActive(false);
//            }
//        }

		private void SetButtonState (EMode mode) {
			switch (mode) {
            case EMode.Edit:
                _cachedView.Erase.gameObject.SetActive (true);
                _cachedView.Redo.gameObject.SetActive (false);
                _cachedView.Undo.gameObject.SetActive (true);
                _cachedView.Publish.gameObject.SetActive (false);
                _cachedView.ButtonFinishCondition.SetActiveEx (true);
//				_cachedView.MoveBtn.SetActiveEx (true);
                _cachedView.MoveBtnBg.SetActive (true);

                _cachedView.EnterEffectMode.SetActiveEx (false);
                _cachedView.ExitEffectMode.SetActiveEx (false);

                _cachedView.Play.gameObject.SetActive (true);
                _cachedView.Pause.gameObject.SetActive (false);
                if (_cachedView.Capture != null) {
                    _cachedView.Capture.gameObject.SetActive (false);
                }
                _cachedView.Home.gameObject.SetActive (true);
				break;
			case EMode.EditTest:
				_cachedView.Erase.gameObject.SetActive(false);
				_cachedView.Redo.gameObject.SetActive(false);
				_cachedView.Undo.gameObject.SetActive(false);
				_cachedView.Publish.gameObject.SetActive(false);
				_cachedView.ButtonFinishCondition.SetActiveEx(false);

//				_cachedView.MoveBtn.SetActiveEx (false);
				_cachedView.MoveBtnBg.SetActive (false);

				_cachedView.EnterEffectMode.SetActiveEx(false);
				_cachedView.ExitEffectMode.SetActiveEx(false);

				_cachedView.Play.gameObject.SetActive(false);
				_cachedView.Pause.gameObject.SetActive(true);
				if(_cachedView.Capture != null)
				{
					_cachedView.Capture.gameObject.SetActive(false);
				}
                _cachedView.Home.gameObject.SetActive (false);
				break;
			case EMode.PlayRecord:
				_cachedView.Erase.gameObject.SetActive(false);
				_cachedView.Redo.gameObject.SetActive(false);
				_cachedView.Undo.gameObject.SetActive(false);
				_cachedView.Publish.gameObject.SetActive(false);
				_cachedView.ButtonFinishCondition.SetActiveEx(false);

//				_cachedView.MoveBtn.SetActiveEx (false);
				_cachedView.MoveBtnBg.SetActive (false);

				_cachedView.EnterEffectMode.SetActiveEx(false);
				_cachedView.ExitEffectMode.SetActiveEx(false);

				_cachedView.Play.gameObject.SetActive(false);
				_cachedView.Pause.gameObject.SetActive(true);
				if(_cachedView.Capture != null)
				{
					_cachedView.Capture.gameObject.SetActive(false);
				}
				break;
			case EMode.ModifyEdit:
				_cachedView.Erase.gameObject.SetActive(false);
				_cachedView.Redo.gameObject.SetActive(false);
				_cachedView.Undo.gameObject.SetActive(false);
				_cachedView.Publish.gameObject.SetActive(false);
				_cachedView.ButtonFinishCondition.SetActiveEx(false);

//				_cachedView.MoveBtn.SetActiveEx (true);
				_cachedView.MoveBtnBg.SetActive (true);

				_cachedView.EnterEffectMode.SetActiveEx(false);
				_cachedView.ExitEffectMode.SetActiveEx(false);

				_cachedView.Play.gameObject.SetActive(true);
				_cachedView.Pause.gameObject.SetActive(false);
				if(_cachedView.Capture != null)
				{
					_cachedView.Capture.gameObject.SetActive(false);
				}
                _cachedView.Home.gameObject.SetActive (true);
				break;
            }
		}

        private void OnEdit()
        {
            Broadcast(ECommandType.Edit);
        }

        private void OnPublish()
        {
//            Broadcast(ECommandType.Publish);
//            SocialGUIManager.Instance.OpenUI<UICtrlPublish>();
        }
        
        private void OnPlay()
        {
			if (DataScene2D.Instance.MainPlayer == null)
			{
                Messenger<string>.Broadcast (EMessengerType.GameErrorLog, "游戏无法开启，请先放置主角");
			}
            GameModeEdit gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (null != gameModeEdit)
            {
                gameModeEdit.ChangeMode(GameModeEdit.EMode.EditTest);
            }
        }

        private void OnPause()
        {
			GameModeEdit gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
			if (null != gameModeEdit)
			{
                gameModeEdit.ChangeMode(GameModeEdit.EMode.Edit);
			}
        }

        private void OnClickFinishCondition()
        {
            if (GM2DGame.Instance.GameMode.GameRunMode != EGameRunMode.Edit)
            {
                return;
            }
            SocialGUIManager.Instance.OpenUI<UICtrlGamePlay>();
        }

	    private void OnClickEffectModeButton()
	    {
		    bool value = EditMode.Instance.CurEditorLayer == EEditorLayer.None;
			EditMode.Instance.SetEditorModeEffect(value);
		    UpdateEffectModeButtonState();
            _cachedView.EnterSwitchMode.SetActiveEx (!value);
	    }

        private void OnClickSwitchModeButton () {
            if (EditMode.Instance.CurCommandType == ECommandType.Switch)
            {
                Broadcast(ECommandType.Create);
                _cachedView.Erase.SetActiveEx (true);
                _cachedView.EnterEffectMode.SetActiveEx (false);
            }
            else
            {
                Broadcast(ECommandType.Switch);
                _cachedView.Erase.SetActiveEx (false);
                _cachedView.EnterEffectMode.SetActiveEx (false);
            }
            UpdateSwitchModeButtonState();
        }

        private void OnUndo()
        {
            Broadcast(ECommandType.Undo);
        }

	    private void OnClickHome()
	    {
			Messenger.Broadcast(EMessengerType.OpenGameSetting);
		}

        private void OnClickCapture()
        {
            Messenger.Broadcast(EMessengerType.CaptureGameCover);
        }

        private void OnRedo()
        {
            Broadcast(ECommandType.Redo);
        }

        private void OnErase()
        {
            if (EditMode.Instance.CurCommandType == ECommandType.Erase)
            {
                Broadcast(ECommandType.Create);
                _cachedView.EnterSwitchMode.SetActiveEx (true);
            }
	        else
	        {
				Broadcast(ECommandType.Erase);
                _cachedView.EnterSwitchMode.SetActiveEx (false);
			}
	        UpdateEraseButtonState();
        }

	 //   private void OnDragMode()
	 //   {
		//    if (EditMode.Instance.CurCommandType == ECommandType.Move)
		//    {
		//	    Broadcast(ECommandType.Create);
		//    }
		//    else
		//    {
		//		Broadcast(ECommandType.Move);
		//	}
		//}

        private void OnEnterDragMode ()
        {
            if (EditMode.Instance.CurCommandType != ECommandType.Move) {
                Broadcast (ECommandType.Move);
            }
        }

        private void OnExitDragMode ()
        {
            if (EditMode.Instance.CurCommandType == ECommandType.Move) {
                Game.CameraManager.Instance.OnPinchEnd ();
                Game.CameraManager.Instance.OnDragEnd (Vector2.zero);
                Broadcast (ECommandType.Create);
                SetMoveBtnPos (_moveBtnOrigPos);
            }
        }

        private void OnMoveBtnDragBegin (UnityEngine.EventSystems.PointerEventData eventData) {
//            _moveBtnDragOffset = (Vector2)_cachedView.MoveBtn.RectTrans.localPosition - eventData.pressPosition;
//            SetMoveBtnPos (eventData.position + _moveBtnDragOffset);
        }

        private void OnMoveBtnDragEnd (UnityEngine.EventSystems.PointerEventData eventData) {
//            _cachedView.MoveBtn.RectTrans.localPosition = _moveBtnOrigPos;
        }

        private void OnMoveBtnMove (UnityEngine.EventSystems.PointerEventData eventData)
        {
            SetMoveBtnPos (eventData.position + _moveBtnDragOffset);
        }

        private void SetMoveBtnPos (Vector2 pos) {
            if (pos.y > _moveBtnOrigPos.y + _maxMoveY * 0.5f) {
                pos.y = _moveBtnOrigPos.y + _maxMoveY * 0.5f;
            }
            if (pos.y < _moveBtnOrigPos.y - _maxMoveY * 0.5f) {
                pos.y = _moveBtnOrigPos.y - _maxMoveY * 0.5f;
            }
            pos.x = _moveBtnOrigPos.x;
//            _cachedView.MoveBtn.RectTrans.localPosition = new Vector3(pos.x, pos.y, 0);
        }

	    private void UpdateEraseButtonState()
	    {
            if (EditMode.Instance == null || EditMode.Instance.CurCommandType == ECommandType.Play)
			{
				_cachedView.EraseSelected.SetActiveEx(false);
				_cachedView.Erase.SetActiveEx(false);
				return;
			}
            bool isSelect = EditMode.Instance.CurCommandType == ECommandType.Erase;
            _cachedView.EraseSelected.SetActiveEx(isSelect);
			_cachedView.Erase.SetActiveEx(!isSelect);
		}

        private void UpdateSwitchModeButtonState () {
            if (null == EditMode.Instance || EditMode.Instance.CurCommandType == ECommandType.Play) {
                _cachedView.EnterSwitchMode.SetActiveEx (false);
                _cachedView.ExitSwitchMode.SetActiveEx (false);
                return;
            }
            bool isSelect = EditMode.Instance.CurCommandType == ECommandType.Switch;
            _cachedView.EnterSwitchMode.SetActiveEx (!isSelect);
            _cachedView.ExitSwitchMode.SetActiveEx (isSelect);
        }

	    private void UpdateEffectModeButtonState()
	    {
			if (EditMode.Instance == null)
			{
                _cachedView.EnterEffectMode.SetActiveEx(false);
                _cachedView.ExitEffectMode.SetActiveEx(false);
				return;
			}
			bool isSelect = EditMode.Instance.CurEditorLayer == EEditorLayer.Effect;
			_cachedView.EnterEffectMode.SetActiveEx(!isSelect);
			_cachedView.ExitEffectMode.SetActiveEx(isSelect);
		}

	 //   private void UpdateMoveButtonState()
	 //   {
		//	bool isSelect = EditMode.Instance.CurCommandType == ECommandType.Move;

		//	_cachedView.EnterDragMode.SetActiveEx(!isSelect);
		//	_cachedView.ExitDragMode.SetActiveEx(isSelect);
		//}
        private void OnMoveBtnClick ()
        {
            //if (_moveBtnDragged) {
            //    return;
            //}
            //ShowMenu (true);
        }

		private void Broadcast(ECommandType type)
        {
            //关掉UICtrlItem
//            SocialGUIManager.Instance.CloseUI<UICtrlItem>();
            Messenger<ECommandType>.Broadcast(EMessengerType.OnCommandChanged, type);
        }

        #region event 

	    private void AfterCommandChanged()
	    {
		    //UpdateMoveButtonState();
			if (_editMode == EMode.Edit) {
				UpdateEraseButtonState ();
                UpdateSwitchModeButtonState ();
			} else if (_editMode == EMode.ModifyEdit) {
			}
		}

		#endregion

		#endregion

	}
}
