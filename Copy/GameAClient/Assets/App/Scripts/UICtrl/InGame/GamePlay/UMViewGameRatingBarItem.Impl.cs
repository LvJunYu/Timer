/********************************************************************
** Filename : UMViewGameRatingBarItem  
** Author : ake
** Date : 5/18/2016 4:42:21 PM
** Summary : UMViewGameRatingBarItem  
***********************************************************************/


using System;
using SoyEngine;

namespace GameA
{
	public partial class UMViewGameRatingBarItem
	{
		/// <summary>
		/// 从1开始
		/// </summary>
		private int _itemId;

		private Action<int> _clickAction; 



		public void Init(int id,Action<int> clickAction )
		{
			_itemId = id;
			_clickAction = clickAction;
			ClickButton.onClick.AddListener(OnButtonClick);
		}

		public void UpdateShow(int value)
		{
			bool isEnable  = value >= _itemId;
			EnableSprite.SetActiveEx(isEnable);
			DisableSprite.SetActiveEx(!isEnable);
		}

		private void OnButtonClick()
		{
			if (_clickAction != null)
			{
				_clickAction(_itemId);
			}
		}
	}
}