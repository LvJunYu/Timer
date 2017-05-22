/********************************************************************
** Filename : ChangeStateButtonGroup  
** Author : ake
** Date : 3/14/2017 4:58:35 PM
** Summary : ChangeStateButtonGroup  
***********************************************************************/


using System;
using SoyEngine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameA.Game
{
	public class ChangeStateButtonGroup:MonoBehaviour
	{
		public Button[] ButtonArray;

		private int _defaultStateIndex = 0;

		public void SetDefaultActive(int index)
		{
			Button b = GetButtonByIndex(index);
			if (b == null)
			{
				LogHelper.Error("SetDefaultActive called but GetButtonByIndex return null! index is {0}",index);
				return;
			}
			_defaultStateIndex = index;
			SetButtonVisible(_defaultStateIndex);
		}

		public void SetActiveButton(int index)
		{
			Button b = GetButtonByIndex(index);
			if (b == null)
			{
				LogHelper.Error("SetActiveButton called but GetButtonByIndex return null! index is {0}", index);
				return;
			}
			SetButtonVisible(index);
		}

		public void AddClickEvent(int index,Action e)
		{
			if (e == null)
			{
				LogHelper.Error("AddClickEvent called but action e is null!");
				return;
			}
			Button b = GetButtonByIndex(index);
			if (b == null)
			{
				LogHelper.Error("AddClickEvent called but GetButtonByIndex return null! index is {0}", index);
				return;
			}
			b.onClick.AddListener(new UnityAction(e));
		}
		#region private
		private Button GetButtonByIndex(int index)
		{
			if (ButtonArray == null)
			{
				LogHelper.Error("GetButtonByIndex called but buttonArray is null!");
				return null ;
			}
			if (index >= ButtonArray.Length || index < 0)
			{
				LogHelper.Error("GetButtonByIndex called buttonarray.length is {0} index is {1}", ButtonArray.Length, index);
				return null;
			}
			if (ButtonArray[index] == null)
			{
				LogHelper.Error("GetButtonByIndex called but ButtonArray[index] is null! index is {0}", index);
				return null; 
			}
			return ButtonArray[index];
		}
		/// <summary>
		/// 内部调用,需要检查
		/// </summary>
		/// <param name="index"></param>
		private void SetButtonVisible(int index)
		{
			for (int i = 0; i < ButtonArray.Length; i++)
			{
				var item = ButtonArray[i];
				if (item == null)
				{
					continue;
				}
				if (i == index)
				{
					item.gameObject.SetActive(true);
				}
				else
				{
					item.gameObject.SetActive(false);
				}
			}
		}

		#endregion

	}
}