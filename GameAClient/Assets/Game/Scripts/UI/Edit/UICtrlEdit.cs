/********************************************************************
** Filename : UICtrlEdit
** Author : Dong
** Date : 2015/7/2 16:30:13
** Summary : UICtrlEdit
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA.Game
{
    [UIAutoSetup(EUIAutoSetupType.Create)]
    public class UICtrlEdit : UICtrlGenericBase<UIViewEdit>
    {
        #region 常量与字段
        //private Social.UIDraggableButton _moveBtn;
        //private bool _moveBtnDragged = false;
        private Vector2 _moveBtnOrigPos;
        private Vector2 _moveBtnDragOffset;
        private float _maxMoveY = 50f;

		// 编辑类型，是正常编辑还是改造编辑
		private EMode _editMode = EMode.Edit;
        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
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

			_cachedView.ModifyEraseBtn.onClick.AddListener (OnModifyEraseBtn);
			_cachedView.ModifyModifyBtn.onClick.AddListener (OnModifyModifyBtn);
			_cachedView.ModifyAddBtn.onClick.AddListener (OnModifyAddBtn);

			for (int i = 0; i < _cachedView.ModifyItems.Length; i++) {
				_cachedView.ModifyItems [i].id = i;
				_cachedView.ModifyItems[i].DelBtnCb = OnModifyItemDelBtn;
			}

            //_moveBtn = _cachedView.MoveBtnObj.AddComponent<Social.UIDraggableButton> ();
            //_moveBtn.Button = _moveBtn.gameObject.GetComponent<Button> ();
            //_moveBtn.RectTransform = _moveBtn.GetComponent<RectTransform> ();
            //_moveBtn.Button.onClick.AddListener (OnMoveBtnClick);
            //_moveBtn.OnBeginDragEvent.AddListener (OnMoveBtnDrag);
            //_moveBtn.OnDragEvent.AddListener (OnMoveBtnDrag);
            //_moveBtn.OnEndDragEvent.AddListener (OnMoveBtnEndDrag);
            //_moveBtnOrigPos = _moveBtn.RectTransform.position;
            _cachedView.MoveBtn.OnPress += OnEnterDragMode;
            _cachedView.MoveBtn.OnRelease += OnExitDragMode;
            _cachedView.MoveBtn.OnDragBegin += OnMoveBtnDragBegin;
            _cachedView.MoveBtn.OnDragMove += OnMoveBtnMove;
            _cachedView.MoveBtn.OnDragEnd += OnMoveBtnDragEnd;
	        if (SocialGUIManager.Instance.RunRecordInApp)
	        {
		        _cachedView.Home.SetActiveEx(false);
	        }
        }

	    protected override void InitEventListener()
	    {
		    base.InitEventListener();
			RegisterEvent(EMessengerType.AfterCommandChanged, AfterCommandChanged);
			RegisterEvent(EMessengerType.OnModifyUnitChanged, OnMapDataChanged);
	    }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
	        UpdateEraseButtonState();
	        UpdateEffectModeButtonState();
            _moveBtnOrigPos = _cachedView.MoveBtn.RectTrans.localPosition;
            //_moveBtnOrigPos = new Vector2 (60, -50);
        }

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			Messenger.RemoveListener(EMessengerType.AfterCommandChanged, AfterCommandChanged);
			Messenger.RemoveListener(EMessengerType.OnModifyUnitChanged, OnMapDataChanged);
		}

        public override void OnUpdate ()
        {
            base.OnUpdate ();
            if (_cachedView.MoveBtn.IsPressed) {
                Vector2 pos = _cachedView.MoveBtn.RectTrans.localPosition;
                if (pos.y - _moveBtnOrigPos.y > _maxMoveY * 0.4f) {
                    CameraManager.Instance.UpdateFadeCameraOrthoSizeOffset (0.12f);
                } else if (pos.y - _moveBtnOrigPos.y > _maxMoveY * 0.15f) {
                    CameraManager.Instance.UpdateFadeCameraOrthoSizeOffset (0.05f);
                } else if (pos.y - _moveBtnOrigPos.y > -_maxMoveY * 0.15f) {

                } else if (pos.y - _moveBtnOrigPos.y > -_maxMoveY * 0.4f) {
                    CameraManager.Instance.UpdateFadeCameraOrthoSizeOffset (-0.05f);
                } else {
                    CameraManager.Instance.UpdateFadeCameraOrthoSizeOffset (-0.12f);
                }
            }
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
				_cachedView.Publish.gameObject.SetActive (true);
				_cachedView.ButtonFinishCondition.SetActiveEx (true);
				_cachedView.MoveBtn.SetActiveEx (true);
				_cachedView.MoveBtnBg.SetActive (true);

				_cachedView.EnterEffectMode.SetActiveEx (true);
				_cachedView.ExitEffectMode.SetActiveEx (false);

				_cachedView.Play.gameObject.SetActive (true);
				_cachedView.Pause.gameObject.SetActive (false);
				if (_cachedView.Capture != null) {
					_cachedView.Capture.gameObject.SetActive (false);
				}

				_cachedView.ModifyPannel.SetActive (false);
				break;
			case EMode.EditTest:
				_cachedView.Erase.gameObject.SetActive(false);
				_cachedView.Redo.gameObject.SetActive(false);
				_cachedView.Undo.gameObject.SetActive(false);
				_cachedView.Publish.gameObject.SetActive(false);
				_cachedView.ButtonFinishCondition.SetActiveEx(false);

				_cachedView.MoveBtn.SetActiveEx (false);
				_cachedView.MoveBtnBg.SetActive (false);

				_cachedView.EnterEffectMode.SetActiveEx(false);
				_cachedView.ExitEffectMode.SetActiveEx(false);

				_cachedView.Play.gameObject.SetActive(false);
				_cachedView.Pause.gameObject.SetActive(true);
				if(_cachedView.Capture != null)
				{
					_cachedView.Capture.gameObject.SetActive(false);
				}

				_cachedView.ModifyPannel.SetActive (false);
				break;
			case EMode.PlayRecord:
				_cachedView.Erase.gameObject.SetActive(false);
				_cachedView.Redo.gameObject.SetActive(false);
				_cachedView.Undo.gameObject.SetActive(false);
				_cachedView.Publish.gameObject.SetActive(false);
				_cachedView.ButtonFinishCondition.SetActiveEx(false);

				_cachedView.MoveBtn.SetActiveEx (false);
				_cachedView.MoveBtnBg.SetActive (false);

				_cachedView.EnterEffectMode.SetActiveEx(false);
				_cachedView.ExitEffectMode.SetActiveEx(false);

				_cachedView.Play.gameObject.SetActive(false);
				_cachedView.Pause.gameObject.SetActive(true);
				if(_cachedView.Capture != null)
				{
					_cachedView.Capture.gameObject.SetActive(false);
				}

				_cachedView.ModifyPannel.SetActive (false);
				break;
			case EMode.ModifyEdit:
				_cachedView.Erase.gameObject.SetActive(false);
				_cachedView.Redo.gameObject.SetActive(false);
				_cachedView.Undo.gameObject.SetActive(false);
				_cachedView.Publish.gameObject.SetActive(false);
				_cachedView.ButtonFinishCondition.SetActiveEx(false);

				_cachedView.MoveBtn.SetActiveEx (true);
				_cachedView.MoveBtnBg.SetActive (true);

				_cachedView.EnterEffectMode.SetActiveEx(false);
				_cachedView.ExitEffectMode.SetActiveEx(false);

				_cachedView.Play.gameObject.SetActive(true);
				_cachedView.Pause.gameObject.SetActive(false);
				if(_cachedView.Capture != null)
				{
					_cachedView.Capture.gameObject.SetActive(false);
				}

				_cachedView.ModifyPannel.SetActive (true);
				break;
			}
		}

        private void OnEdit()
        {
            Broadcast(ECommandType.Edit);
        }

        private void OnPublish()
        {
            Broadcast(ECommandType.Publish);
            GM2DGUIManager.Instance.OpenUI<UICtrlPublish>();
        }
        
        private void OnPlay()
        {
			if (DataScene2D.Instance.MainPlayer == null || !PlayMode.Instance.CheckPlayerValid())
			{
				Messenger<string>.Broadcast(EMessengerType.GameErrorLog, LocaleManager.GameLocale("error_editor_test_no_main_player"));
			}
			Broadcast(ECommandType.Play);
            GM2DGame.Instance.ChangeToMode(EMode.EditTest);
        }

        private void OnPause()
        {
            Broadcast(ECommandType.Pause);
			if (_editMode == EMode.Edit) {
				GM2DGame.Instance.ChangeToMode (EMode.Edit);
			} else {
				GM2DGame.Instance.ChangeToMode (EMode.ModifyEdit);
			}
        }

        private void OnClickFinishCondition()
        {
            if (GM2DGame.Instance.CurrentMode != EMode.Edit)
            {
                return;
            }
            GM2DGUIManager.Instance.OpenUI<UICtrlGamePlay>();
        }

	    private void OnClickEffectModeButton()
	    {
		    bool value = EditMode.Instance.CurEditorLayer == EEditorLayer.None;
			EditMode.Instance.SetEditorModeEffect(value);
		    UpdateEffectModeButtonState();
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

		#region Modify
		private void OnModifyAddBtn () {
			if (EditMode.Instance.CurCommandType != ECommandType.Create) {
				SwitchModifyMode (ECommandType.Create);
			}
		}
		private void OnModifyEraseBtn () {
			if (EditMode.Instance.CurCommandType != ECommandType.Erase) {
				SwitchModifyMode (ECommandType.Erase);
			}
		}
		private void OnModifyModifyBtn () {
			if (EditMode.Instance.CurCommandType != ECommandType.Modify) {
				SwitchModifyMode (ECommandType.Modify);
			}
		}

		private void SwitchModifyMode (ECommandType type) {
			switch (type) {
			case ECommandType.Create:
				_cachedView.ModifyAddBtn.transform.localScale = Vector3.one * 1.2f;
				_cachedView.ModifyEraseBtn.transform.localScale = Vector3.one;
				_cachedView.ModifyModifyBtn.transform.localScale = Vector3.one;
				break;
			case ECommandType.Erase:
				_cachedView.ModifyAddBtn.transform.localScale = Vector3.one;
				_cachedView.ModifyEraseBtn.transform.localScale = Vector3.one * 1.2f;
				_cachedView.ModifyModifyBtn.transform.localScale = Vector3.one;
				break;
			case ECommandType.Modify:
				_cachedView.ModifyAddBtn.transform.localScale = Vector3.one;
				_cachedView.ModifyEraseBtn.transform.localScale = Vector3.one;
				_cachedView.ModifyModifyBtn.transform.localScale = Vector3.one * 1.2f;
				break;
			}
			Messenger<ECommandType>.Broadcast(EMessengerType.OnCommandChanged, type);

			// update modifyItemList
			UpdateModifyItemList();
		}

		/// <summary>
		/// 改造列表删除按钮响应函数
		/// </summary>
		/// <param name="idx">Index.</param>
		private void OnModifyItemDelBtn (int idx) {
			if (EditMode.Instance.CurCommandType == ECommandType.Create) {
				
			} else if (EditMode.Instance.CurCommandType == ECommandType.Erase) {
				((ModifyEditMode)EditMode.Instance).UndoModifyErase (idx);
			} else if (EditMode.Instance.CurCommandType == ECommandType.Modify) {
				((ModifyEditMode)EditMode.Instance).UndoModifModify (idx);
			}
		}
		/// <summary>
		/// 更新改造地块列表界面
		/// </summary>
		private void UpdateModifyItemList () {
			if (_editMode != EMode.ModifyEdit)
				return;
			ModifyEditMode modifyEditMode = EditMode.Instance as ModifyEditMode;
			if (null == modifyEditMode)
				return;
            List<ModifyData> descs = null;
			switch (EditMode.Instance.CurCommandType) {
			case ECommandType.Create:
				descs = modifyEditMode.AddedUnits;
				break;
			case ECommandType.Erase:
				descs = modifyEditMode.RemovedUnits;
				break;
			case ECommandType.Modify:
				descs = modifyEditMode.ModifiedUnits;
				break;
			}
			if (null == descs)
				return;
			int i = 0;
			for (; i < _cachedView.ModifyItems.Length && i < descs.Count; i++) {
                var tableUnit = TableManager.Instance.GetUnit (descs [i].OrigUnit.UnitDesc.Id);
				if (null == tableUnit) {
                    LogHelper.Error ("can't find tabledata of modifyItem id{0}", descs[i].OrigUnit.UnitDesc.Id);
				} else {
					Sprite texture;
					if (GameResourceManager.Instance.TryGetSpriteByName(tableUnit.Icon, out texture))
					{
						_cachedView.ModifyItems [i].SetItem (texture);
					}
					else
					{
						LogHelper.Error("tableUnit {0} icon {1} invalid! tableUnit.EGeneratedType is {2}", tableUnit.Id,
							tableUnit.Icon, tableUnit.EGeneratedType);
					}
				}
			}
			for (; i < _cachedView.ModifyItems.Length; i++) {
				_cachedView.ModifyItems [i].SetEmpty ();
			}
		}
		#endregion

        private void OnRedo()
        {
            Broadcast(ECommandType.Redo);
        }

        private void OnErase()
        {
            if (EditMode.Instance.CurCommandType == ECommandType.Erase)
            {
                Broadcast(ECommandType.Create);
            }
	        else
	        {
				Broadcast(ECommandType.Erase);
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
                CameraManager.Instance.OnPinchEnd ();
                CameraManager.Instance.OnDragEnd (Vector2.zero);
                Broadcast (ECommandType.Create);
                SetMoveBtnPos (_moveBtnOrigPos);
            }
        }

        private void OnMoveBtnDragBegin (UnityEngine.EventSystems.PointerEventData eventData) {
            _moveBtnDragOffset = (Vector2)_cachedView.MoveBtn.RectTrans.localPosition - eventData.pressPosition;
            SetMoveBtnPos (eventData.position + _moveBtnDragOffset);
        }

        private void OnMoveBtnDragEnd (UnityEngine.EventSystems.PointerEventData eventData) {
            _cachedView.MoveBtn.RectTrans.localPosition = _moveBtnOrigPos;
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
            _cachedView.MoveBtn.RectTrans.localPosition = new Vector3(pos.x, pos.y, 0);
        }

	    private void UpdateEraseButtonState()
	    {
			if (EditMode.Instance == null)
			{
				_cachedView.EraseSelected.SetActiveEx(false);
				_cachedView.Erase.SetActiveEx(false);
				return;
			}
            bool isSelect = EditMode.Instance.CurCommandType == ECommandType.Erase;
            _cachedView.EraseSelected.SetActiveEx(isSelect);
			_cachedView.Erase.SetActiveEx(!isSelect);
		}

	    private void UpdateEffectModeButtonState()
	    {
			if (EditMode.Instance == null)
			{
				_cachedView.EraseSelected.SetActiveEx(false);
				_cachedView.Erase.SetActiveEx(false);
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

		private void OnMapDataChanged () {
			UpdateModifyItemList ();
		}



		private void Broadcast(ECommandType type)
        {
            //关掉UICtrlItem
            GM2DGUIManager.Instance.CloseUI<UICtrlItem>();
            Messenger<ECommandType>.Broadcast(EMessengerType.OnCommandChanged, type);
        }

        #region event 

	    private void AfterCommandChanged()
	    {
		    //UpdateMoveButtonState();
			if (_editMode == EMode.Edit) {
				UpdateEraseButtonState ();
			} else if (_editMode == EMode.ModifyEdit) {
			}
		}

		#endregion

		#endregion
	}
}
