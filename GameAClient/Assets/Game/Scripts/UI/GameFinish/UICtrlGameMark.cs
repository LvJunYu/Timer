/********************************************************************
** Filename : UICtrlGameMark  
** Author : ake
** Date : 6/1/2016 3:28:39 PM
** Summary : UICtrlGameMark  
***********************************************************************/


using SoyEngine;

namespace GameA.Game
{
	[UIAutoSetup(EUIAutoSetupType.Add)]
	public class UICtrlGameMark: UICtrlGenericBase<UIViewGameMark>
	{
		protected override void InitGroupId()
		{
			_groupId = (int)EUIGroupType.PopUpUI2;
		}

		protected override void OnViewCreated()
		{
			base.OnViewCreated();
			InitUI();
		}

		protected override void OnOpen(object parameter)
		{
			base.OnOpen(parameter);
		}

		#region private 

		private void InitUI()
		{
			_cachedView.Title.text = GM2DUIConstDefine.GameMarkWindowTitle;

			_cachedView.ButtonEnsure.onClick.AddListener(OnClickEnsureButton);
			_cachedView.ButtonCancle.onClick.AddListener(OnClickCancleButton);
		}

		#endregion

		#region ui event

		private void OnClickEnsureButton()
		{
            if(!AppLogicUtil.CheckLoginAndTip())
            {
                return;
            }

			//如果评论不为空,增加一条评论
			string content = _cachedView.MakrContentValue.text;
			if (!string.IsNullOrEmpty(content))
			{
//				GM2DGame.Instance.Project.SendComment(content, OnSendCommentRes);
			}
			GM2DGUIManager.Instance.CloseUI<UICtrlGameMark>();
		}

		private void OnClickCancleButton()
		{
			GM2DGUIManager.Instance.CloseUI<UICtrlGameMark>();
		}

		#endregion

		#region server event

		//private void OnUpdateRateRes(bool isSuccess)
		//{
		//	if (_isOpen)
		//	{
		//		return;
		//	}
		//	if (isSuccess)
		//	{
		//		if (_cachedView != null && _cachedView.MakrContentValue != null)
		//		{
		//			_cachedView.MakrContentValue.text = "";
		//		}
		//	}
		//	else
		//	{
		//		//打错误日志
		//	}
		//}

		private void OnSendCommentRes(bool isSuccess)
		{
			if (_isOpen)
			{
				return;
			}
			if (isSuccess)
			{
				if (_cachedView != null && _cachedView.MakrContentValue != null)
				{
					_cachedView.MakrContentValue.text = "";
				}
                CommonTools.ShowPopupDialog("评论发送成功");
			}
			else
            {
                CommonTools.ShowPopupDialog("评论失败");
			}
		}

		#endregion
	}
}