/********************************************************************

** Filename : UICtrlScreenOperator
** Author : ake
** Date : 2016/4/11 15:15:57
** Summary : UICtrlScreenOperator
***********************************************************************/

using System;
using System.Diagnostics;

using SoyEngine;
using UnityEngine;
using GameA.Game;

namespace GameA
{
	[UIAutoSetup(EUIAutoSetupType.Create)]
	public class UICtrlScreenOperator: UICtrlInGameBase<UIViewScreenOperator>
	{
		protected override void InitGroupId()
		{
			_groupId = (int)EUIGroupType.InGameMainUI;
		}

		protected override void OnViewCreated()
		{
			base.OnViewCreated();
			Messenger.AddListener(EMessengerType.OnEditorModeCameraMove, OnEditorModeCameraMove);
			Messenger.AddListener(GameA.EMessengerType.OnGameStartComplete, OnGameStartedComplete);

		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			Messenger.RemoveListener(EMessengerType.OnEditorModeCameraMove, OnEditorModeCameraMove);
			Messenger.RemoveListener(GameA.EMessengerType.OnGameStartComplete, OnGameStartedComplete);
		}


		#region  event

		private void OnGameStartedComplete()
		{
			OnEditorModeCameraMove();
		}

		private void OnEditorModeCameraMove()
		{
			Vector3 pos = Game.CameraManager.Instance.FinalPos;
			bool reachLeft = Game.CameraManager.Instance.CheckReachLimitLeft(pos);
			bool reachRight = Game.CameraManager.Instance.CheckReachLimitRight(pos);
			bool reachTop = Game.CameraManager.Instance.CheckReachLimitTop(pos);
            _cachedView.TopGo.SetActiveEx(false);
            if (reachTop && MapConfig.PermitMapSize.y != ConstDefineGM2D.DefaultValidMapRectSize.y)
		    {
                _cachedView.TopGo.SetActiveEx(true);
		    }
			_cachedView.LeftGo.SetActiveEx(false);
			_cachedView.RightGo.SetActiveEx(reachRight);
		}

		#endregion
	}
}