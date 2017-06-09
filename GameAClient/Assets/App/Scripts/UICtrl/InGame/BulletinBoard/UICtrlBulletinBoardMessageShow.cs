/********************************************************************
** Filename : UICtrlBulletinBoardMessageShow  
** Author : ake
** Date : 9/27/2016 4:06:23 PM
** Summary : UICtrlBulletinBoardMessageShow  
***********************************************************************/



using SoyEngine;
using UnityEngine;
using GameA.Game;

namespace GameA
{

    [UIAutoSetup(EUIAutoSetupType.Add)]
	public class UICtrlBulletinBoardMessageShow: UICtrlInGameBase<UIViewBulletinBoardMessageShow>
	{
		private int _curShowCount =  0 ;

		protected override void InitGroupId()
		{
			_groupId = (int) EUIGroupType.InGameMainUI;
		}


		protected override void OnViewCreated()
		{
			base.OnViewCreated();
			MessageOperator(true); 
		}

		protected override void OnDestroy()
		{
			MessageOperator(false);
			base.OnDestroy();
		}

		public override void Open(object parameter)
		{
			base.Open(parameter);
			_curShowCount = 0;
		}

		protected override void InitEventListener()
		{
			base.InitEventListener();
			RegisterEvent(EMessengerType.OnEdit, OnEditMode);
			RegisterEvent(EMessengerType.OnPlay, OnPlayMode);
		}

		#region event

		private void OnEditMode()
		{
			ClearAndClose();
		}

		private void OnPlayMode()
		{
			ClearAndClose();
		}

		private void MessageOperator(bool value)
		{
			if (value)
			{
				Messenger<IntVec3>.AddListener(EMessengerType.OnTriggerBulletinBoardEnter, OnTriggerBulletinBoardEnter);
				Messenger.AddListener(EMessengerType.OnTriggerBulletinBoardExit, OnTriggerBulletinBoardExit);
			}
			else
			{
				Messenger<IntVec3>.RemoveListener(EMessengerType.OnTriggerBulletinBoardEnter, OnTriggerBulletinBoardEnter);
				Messenger.RemoveListener(EMessengerType.OnTriggerBulletinBoardExit, OnTriggerBulletinBoardExit);
			}
		}

		private void OnTriggerBulletinBoardEnter(IntVec3 index)
		{
			if (!IsOpen)
			{
                SocialGUIManager.Instance.OpenUI<UICtrlBulletinBoardMessageShow>();
			}
			_curShowCount ++;
		    string value = DataScene2D.Instance.GetUnitExtra(index).Msg;
			_cachedView.ShowText.text = string.IsNullOrEmpty(value) ? GM2DUIConstDefine.GameBulletinBoardMessageDefaultMessage : value;
			UpdateUIShow();
			
		}

		private void OnTriggerBulletinBoardExit()
		{
			_curShowCount --;
			if (_curShowCount <= 0)
			{
				_curShowCount = 0;
				UpdateUIShow();
			}
		}
		#endregion

		#region private

		private void ClearAndClose()
		{
			_curShowCount = 0;
            SocialGUIManager.Instance.CloseUI<UICtrlBulletinBoardMessageShow>();
		}

		private void UpdateUIShow()
		{
			if (_curShowCount == 0)
			{
				_cachedView.BgImage.CrossFadeAlpha(0, 0.3f, true);
				_cachedView.ShowText.CrossFadeAlpha(0, 0.3f, true);
			}
			else
			{
				_cachedView.BgImage.CrossFadeAlpha(1, 0.5f, true);
				_cachedView.ShowText.CrossFadeAlpha(1, 0.5f, true);
			}
		}

		#endregion
	}
}