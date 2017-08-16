/********************************************************************
** Filename : UICtrlEdit
** Author : Dong
** Date : 2015/7/2 16:30:13
** Summary : UICtrlEdit
***********************************************************************/

using GameA.Game;
using SoyEngine;

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

            _cachedView.Erase.onClick.AddListener(OnEnterErase);
			_cachedView.EraseSelected.onClick.AddListener(OnExitErase);

			_cachedView.Home.onClick.AddListener(OnClickHome);
			_cachedView.Undo.onClick.AddListener(OnUndo);
			_cachedView.Redo.onClick.AddListener(OnRedo);

			_cachedView.Play.onClick.AddListener(OnPlay);
			_cachedView.Pause.onClick.AddListener(OnPause);

			_cachedView.ButtonFinishCondition.onClick.AddListener(OnClickFinishCondition);

            _cachedView.EnterSwitchMode.onClick.AddListener (OnClickEnterSwitchModeBtn);
            _cachedView.ExitSwitchMode.onClick.AddListener (OnClickExitSwitchModeBtn);
	        _cachedView.EnterCamCtrlModeBtn.onClick.AddListener(OnEnterCamCtrlMode);
	        _cachedView.ExitCamCtrlModeBtn.onClick.AddListener(OnExitCamCtrlMode);
	        _cachedView.Save.onClick.AddListener(OnSave);
        }


		protected override void InitEventListener()
	    {
		    base.InitEventListener();
			RegisterEvent(EMessengerType.AfterEditModeStateChange, AfterEditModeStateChange);
	    }

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			Messenger.RemoveListener(EMessengerType.AfterEditModeStateChange, AfterEditModeStateChange);
		}

        public void ChangeToEditTestMode()
        {
			SetButtonState(EMode.EditTest);
		}

        public void ChangeToEditMode()
        {
			_editMode = EMode.Edit;
			SetButtonState(EMode.Edit);
	        AfterEditModeStateChange();
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

		private void SetButtonState (EMode mode) {
			switch (mode) {
            case EMode.Edit:
	            _cachedView.Erase.gameObject.SetActive (false);
	            _cachedView.EraseSelected.gameObject.SetActive (false);
                _cachedView.Redo.gameObject.SetActive (true);
                _cachedView.Undo.gameObject.SetActive (true);
                _cachedView.ButtonFinishCondition.SetActiveEx (true);

	            _cachedView.EnterEffectMode.SetActiveEx (false);
	            _cachedView.ExitEffectMode.SetActiveEx (false);

	            _cachedView.EnterCamCtrlModeBtn.SetActiveEx (false);
	            _cachedView.ExitCamCtrlModeBtn.SetActiveEx (false);
	            
	            _cachedView.EnterSwitchMode.SetActiveEx (false);
	            _cachedView.ExitSwitchMode.SetActiveEx (false);

                _cachedView.Play.gameObject.SetActive (true);
                _cachedView.Pause.gameObject.SetActive (false);
	            _cachedView.Save.gameObject.SetActive (true);
                _cachedView.Home.gameObject.SetActive (true);
				break;
			case EMode.EditTest:
				_cachedView.Erase.gameObject.SetActive (false);
				_cachedView.EraseSelected.gameObject.SetActive (false);
				_cachedView.Redo.gameObject.SetActive (false);
				_cachedView.Undo.gameObject.SetActive (false);
				_cachedView.ButtonFinishCondition.SetActiveEx (false);

				_cachedView.EnterEffectMode.SetActiveEx (false);
				_cachedView.ExitEffectMode.SetActiveEx (false);

				_cachedView.EnterCamCtrlModeBtn.SetActiveEx (false);
				_cachedView.ExitCamCtrlModeBtn.SetActiveEx (false);
	            
				_cachedView.EnterSwitchMode.SetActiveEx (false);
				_cachedView.ExitSwitchMode.SetActiveEx (false);

				_cachedView.Play.gameObject.SetActive (false);
				_cachedView.Pause.gameObject.SetActive (true);
				_cachedView.Save.gameObject.SetActive (false);
				_cachedView.Home.gameObject.SetActive (true);
				break;
			case EMode.PlayRecord:
				_cachedView.Erase.gameObject.SetActive (false);
				_cachedView.EraseSelected.gameObject.SetActive (false);
				_cachedView.Redo.gameObject.SetActive (false);
				_cachedView.Undo.gameObject.SetActive (false);
				_cachedView.ButtonFinishCondition.SetActiveEx (false);

				_cachedView.EnterEffectMode.SetActiveEx (false);
				_cachedView.ExitEffectMode.SetActiveEx (false);

				_cachedView.EnterCamCtrlModeBtn.SetActiveEx (false);
				_cachedView.ExitCamCtrlModeBtn.SetActiveEx (false);
	            
				_cachedView.EnterSwitchMode.SetActiveEx (false);
				_cachedView.ExitSwitchMode.SetActiveEx (false);

				_cachedView.Play.gameObject.SetActive (false);
				_cachedView.Pause.gameObject.SetActive (false);
				_cachedView.Save.gameObject.SetActive (false);
				_cachedView.Home.gameObject.SetActive (true);
				break;
			case EMode.ModifyEdit:
				_cachedView.Erase.gameObject.SetActive (false);
				_cachedView.EraseSelected.gameObject.SetActive (false);
				_cachedView.Redo.gameObject.SetActive (false);
				_cachedView.Undo.gameObject.SetActive (false);
				_cachedView.ButtonFinishCondition.SetActiveEx (false);

				_cachedView.EnterEffectMode.SetActiveEx (false);
				_cachedView.ExitEffectMode.SetActiveEx (false);

				_cachedView.EnterCamCtrlModeBtn.SetActiveEx (false);
				_cachedView.ExitCamCtrlModeBtn.SetActiveEx (false);
	            
				_cachedView.EnterSwitchMode.SetActiveEx (false);
				_cachedView.ExitSwitchMode.SetActiveEx (false);

				_cachedView.Play.gameObject.SetActive (true);
				_cachedView.Pause.gameObject.SetActive (false);
				_cachedView.Save.gameObject.SetActive (false);
				_cachedView.Home.gameObject.SetActive (true);
				break;
            }
		}
		
		private void OnSave()
		{
			GameModeEdit gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
			if (null != gameModeEdit)
			{
				SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在保存");
				gameModeEdit.Save(() =>
				{
					SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
					SocialGUIManager.ShowPopupDialog("保存成功");
				}, result =>
				{
					SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
					SocialGUIManager.ShowPopupDialog("保存失败");
				});
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

		private void OnClickEnterSwitchModeBtn () {
	        EditMode.Instance.StartSwitch();
		}

		private void OnClickExitSwitchModeBtn () {
	        EditMode.Instance.StopSwitch();
		}

		private void OnUndo()
		{
			EditMode.Instance.Undo();
		}
		
		private void OnRedo()
		{
			EditMode.Instance.Redo();
		}

	    private void OnClickHome()
	    {
			Messenger.Broadcast(EMessengerType.OpenGameSetting);
            SocialGUIManager.Instance.GetUI<UICtrlGameSetting>().ChangeToSettingInGame();
        }

        private void OnEnterErase()
        {
			EditMode.Instance.StartRemove();
        }

		private void OnExitErase()
		{
			EditMode.Instance.StopRemove();
		}

        private void OnEnterCamCtrlMode ()
        {
	        EditMode.Instance.StartCamera();
        }

        private void OnExitCamCtrlMode ()
        {
	        EditMode.Instance.StopCamera();
        }


		private void UpdateEditModeBtnView()
		{
			bool inCamera = EditMode.Instance.IsInState(EditModeState.Camera.Instance);
			_cachedView.EnterCamCtrlModeBtn.gameObject.SetActive(!inCamera);
			_cachedView.ExitCamCtrlModeBtn.gameObject.SetActive(inCamera);

			bool inRemove = EditMode.Instance.IsInState(EditModeState.Remove.Instance);
			_cachedView.Erase.gameObject.SetActive(!inRemove);
			_cachedView.EraseSelected.gameObject.SetActive(inRemove);

			bool inSwitch = EditMode.Instance.IsInState(EditModeState.Switch.Instance);
			_cachedView.EnterSwitchMode.gameObject.SetActive(!inSwitch);
			_cachedView.ExitSwitchMode.gameObject.SetActive(inSwitch);
		}

        #region event 

	    private void AfterEditModeStateChange()
	    {
		    if (!_isViewCreated)
		    {
			    return;
		    }
			if (_editMode == EMode.Edit) {
				UpdateEditModeBtnView();
			}
		}

		#endregion

		#endregion

	}
}
