/********************************************************************
** Filename : UICtrlGameCompositeEditorTools  
** Author : ake
** Date : 3/14/2017 5:23:02 PM
** Summary : UICtrlGameCompositeEditorTools  
***********************************************************************/


using JetBrains.Annotations;
using SoyEngine;
using UnityEngine.Events;

namespace GameA.Game
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
	public class UICtrlGameCompositeEditorTools: UICtrlGenericBase<UIViewGameCompositeEditorTools>
	{
		protected override void InitGroupId()
		{
			_groupId = (int)EUIGroupType.MainUI;
		}

		protected override void OnViewCreated()
		{
			base.OnViewCreated();
			InitUI();

		}

		protected override void InitEventListener()
		{
			base.InitEventListener();
			RegisterEvent(EMessengerType.OnCurCompositeEditorStateChanged, OnCompositeStateChanged);
		}

		#region private 

		private void InitUI()
		{
			_cachedView.ShowGroupButton.SetDefaultActive(0);
			_cachedView.SelectMode.SetDefaultActive(0);
			_cachedView.MoveMode.SetDefaultActive(0);
			_cachedView.ToolbarRootTrans.SetActiveEx(false);

			_cachedView.ShowGroupButton.AddClickEvent(0, OnSpreadToolBar);
			_cachedView.ShowGroupButton.AddClickEvent(1, OnHideToolBar);
			_cachedView.SelectMode.AddClickEvent(0, OnEnterSelectMode);
			_cachedView.SelectMode.AddClickEvent(1, OnExitSelectMode);
			_cachedView.MoveMode.AddClickEvent(0, OnEnterMoveMode);

			_cachedView.MoveButtonRoot.SetActiveEx(false);
			_cachedView.MoveUpBt.onClick.AddListener(new UnityAction(MoveUp));
			_cachedView.MoveDownBt.onClick.AddListener(new UnityAction(MoveDown));
			_cachedView.MoveLeftBt.onClick.AddListener(new UnityAction(MoveLeft));
			_cachedView.MoveRightBt.onClick.AddListener(new UnityAction(MoveRight));

		}


		#endregion


		#region uivent

		private void OnSpreadToolBar()
		{
			_cachedView.ShowGroupButton.SetActiveButton(1);
			_cachedView.ToolbarRootTrans.SetActiveEx(true);
		}

		private void OnHideToolBar()
		{
			_cachedView.ShowGroupButton.SetActiveButton(0);
			_cachedView.ToolbarRootTrans.SetActiveEx(false);
		}

		private void OnEnterSelectMode()
		{
			_cachedView.SelectMode.SetActiveButton(1);
			//Messenger<ECompositeEditorState>.Broadcast(EMessengerType.OnCurCompositeEditorStateChanged,ECompositeEditorState.MultipleSelect);
			EditMode.Instance.CompositeModule.EnterSelectMode();
			//_cachedView.SelectMode.set
		}

		private void OnExitSelectMode()
		{
			_cachedView.SelectMode.SetActiveButton(0);
			EditMode.Instance.CompositeModule.ExitSelectMode();
		}

		private void OnEnterMoveMode()
		{
			if (!EditMode.Instance.CompositeModule.HasSelectSomething)
			{
//				Messenger<string>.Broadcast(EMessengerType.GameErrorLog,LocaleManager.GameLocale("ui_error_enter_move_mode_select_none"));
				return;
			}
			EditMode.Instance.CompositeModule.EnterMoveMode();
		}

		private void OnExitMoveMode()
		{
			
		}

		private void MoveUp()
		{
			LogHelper.Debug("MoveUp");
		}

		private void MoveDown()
		{
			LogHelper.Debug("MoveDown");
		}

		private void MoveLeft()
		{
			LogHelper.Debug("MoveLeft");
		}

		private void MoveRight()
		{
			LogHelper.Debug("MoveRight");
		}


		#endregion

		#region data event

		private void OnCompositeStateChanged()
		{
			var curState = EditMode.Instance.CompositeModule.CurState;
			var lastState = EditMode.Instance.CompositeModule.LastState;
			if (curState == lastState)
			{
				return;
			}
			switch (curState)
			{
				case ECompositeEditorState.Move:
				{
					_cachedView.SelectMode.SetActiveButton(0);
					_cachedView.MoveMode.SetActiveButton(1);
					_cachedView.MoveButtonRoot.SetActiveEx(true);
					break;
				}
			}
			switch (lastState)
			{
				case ECompositeEditorState.Move:
				{
					_cachedView.MoveButtonRoot.SetActiveEx(false);
					break;
				}
			}
		}

		#endregion
	}
}