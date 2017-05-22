/********************************************************************
** Filename : UMViewAudioItem  
** Author : ake
** Date : 6/8/2016 8:27:32 PM
** Summary : UMViewAudioItem  
***********************************************************************/


using System;
using SoyEngine;

namespace GameA
{
	public partial class UMViewAudioItem: UMViewBase
	{
		private Action _clickCallback;

		public void InitItem( Action callback )
		{
			_clickCallback = callback;
			ClickButton.onClick.AddListener(OnClick);
		}

		public void UpdateShow(bool value)
		{
			Unselected.SetActiveEx(!value);
			Selected.SetActiveEx(value);
		}

		public void OnClick()
		{
			if (_clickCallback != null)
			{
				_clickCallback();
			}
		}
	}

}