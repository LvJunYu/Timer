/********************************************************************
** Filename : UICtrlEdit
** Author : Dong
** Date : 2015/7/2 16:30:13
** Summary : UICtrlEdit
***********************************************************************/

using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup]
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
			_cachedView.Pause.onClick.AddListener(OnPause);


			_cachedView.ButtonFinishCondition.onClick.AddListener(OnClickFinishCondition);

	        _cachedView.EnterEffectMode.onClick.AddListener(OnClickEffectModeButton);
	        _cachedView.ExitEffectMode.onClick.AddListener(OnClickEffectModeButton);

            _cachedView.EnterSwitchMode.onClick.AddListener (OnClickSwitchModeButton);
            _cachedView.ExitSwitchMode.onClick.AddListener (OnClickSwitchModeButton);
	        //_cachedView.EnterCamCtrlModeBtn.onClick.AddListener(OnEnterCamCtrlMode);
	        //_cachedView.ExitCamCtrlModeBtn.onClick.AddListener(OnExitCamCtrlMode);
	        //_cachedView.EnterCamCtrlModeBtn.gameObject.SetActive(true);
	        //_cachedView.ExitCamCtrlModeBtn.gameObject.SetActive(false);
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
            UpdateModeButtonState ();
//            _moveBtnOrigPos = _cachedView.MoveBtn.RectTrans.localPosition;
            //_moveBtnOrigPos = new Vector2 (60, -50);
        }

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			Messenger.RemoveListener(EMessengerType.AfterCommandChanged, AfterCommandChanged);
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

        private void OnPlay()
        {
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
            UpdateModeButtonState();
        }

        private void OnUndo()
        {
            Broadcast(ECommandType.Undo);
        }

	    private void OnClickHome()
	    {
			Messenger.Broadcast(EMessengerType.OpenGameSetting);
            SocialGUIManager.Instance.GetUI<UICtrlGameSetting>().ChangeToWorkShop();
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

        private void OnEnterCamCtrlMode ()
        {
            if (EditMode.Instance.CurCommandType != ECommandType.Camera) {
	            _cachedView.EnterCamCtrlModeBtn.gameObject.SetActive(false);
	            _cachedView.ExitCamCtrlModeBtn.gameObject.SetActive(true);
                Broadcast (ECommandType.Camera);
            }
        }

        private void OnExitCamCtrlMode ()
        {
            if (EditMode.Instance.CurCommandType == ECommandType.Camera) {
	            _cachedView.EnterCamCtrlModeBtn.gameObject.SetActive(true);
	            _cachedView.ExitCamCtrlModeBtn.gameObject.SetActive(false);
                CameraManager.Instance.CameraCtrlEdit.AdjustOrthoSizeEnd(0f);
                CameraManager.Instance.CameraCtrlEdit.MovePosEnd(Vector2.zero);
                Broadcast (ECommandType.Create);
            }
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

        private void UpdateModeButtonState () {
            if (null == EditMode.Instance || EditMode.Instance.CurCommandType == ECommandType.Play) {
                _cachedView.EnterSwitchMode.SetActiveEx (false);
                _cachedView.ExitSwitchMode.SetActiveEx (false);
	            
	            _cachedView.EnterCamCtrlModeBtn.SetActiveEx(false);
	            _cachedView.ExitCamCtrlModeBtn.SetActiveEx(false);
                return;
            }
            bool isSwitchSelect = EditMode.Instance.CurCommandType == ECommandType.Switch;
            _cachedView.EnterSwitchMode.SetActiveEx (!isSwitchSelect);
            _cachedView.ExitSwitchMode.SetActiveEx (isSwitchSelect);
	        
	        bool isCameraSelect = EditMode.Instance.CurCommandType == ECommandType.Camera;
	        _cachedView.EnterCamCtrlModeBtn.SetActiveEx(!isCameraSelect);
	        _cachedView.ExitCamCtrlModeBtn.SetActiveEx(isCameraSelect);
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
                UpdateModeButtonState ();
			} else if (_editMode == EMode.ModifyEdit) {
			}
		}

		#endregion

		#endregion

	}
}
