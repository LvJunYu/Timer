/********************************************************************

** Filename : UMViewItemScreenButton
** Author : ake
** Date : 2016/4/11 14:47:20
** Summary : UMViewItemScreenButton
***********************************************************************/

using System;
using System.Diagnostics;
using SoyEngine;
using Debug = UnityEngine.Debug;

namespace GameA
{
	public partial class UMViewItemScreenButton : UMViewBase
	{
        protected override void Awake()
		{
			if (ClickButton != null)
			{
				ClickButton.onClick.AddListener(OnClickScreenButton);
			}
		}

        void OnDestory()
		{
			if (ClickButton != null)
			{
				ClickButton.onClick.RemoveAllListeners();
			}
		}

		void OnClickScreenButton()
		{
			Messenger<EScreenOperator>.Broadcast(EMessengerType.OnScreenOperator, ScreenOperatorType);
		}

	}
}