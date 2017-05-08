/********************************************************************
** Filename : DoubleClickEventListener  
** Author : ake
** Date : 12/28/2016 5:56:46 PM
** Summary : DoubleClickEventListener  
***********************************************************************/


using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameA
{
	public class DoubleClickEventListener:EventTrigger
	{
		public Action OnClickEvent;
		public Action OnDoubleClickEvent;

		public const float DoubleClickInterval = 0.3f;

		private float _lastClickTime = 0;

		private bool _checkDoubleClick = false;


		public override void OnPointerClick(PointerEventData eventData)
		{
			if (_checkDoubleClick)
			{
				DoDoubleClickEvent();
			}
			else
			{
				_checkDoubleClick = true;
			}
			_lastClickTime = Time.realtimeSinceStartup;
		}
        

		private void Update()
		{
			if (!_checkDoubleClick)
			{
				return;
			}
			float curTime = Time.realtimeSinceStartup;
			if (curTime - _lastClickTime > DoubleClickInterval)
			{
				DoClickEvent();
			}
		}


		private void DoDoubleClickEvent()
		{
			if (OnDoubleClickEvent != null)
			{
				OnDoubleClickEvent();
			}
			_checkDoubleClick = false;
		}

		private void DoClickEvent()
		{
			if (OnClickEvent != null)
			{
				OnClickEvent();
			}
			_checkDoubleClick = false;
		}

	}
}