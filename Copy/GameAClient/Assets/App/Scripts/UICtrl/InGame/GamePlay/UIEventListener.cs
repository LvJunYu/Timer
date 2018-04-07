/********************************************************************
** Filename : UIEventListener  
** Author : ake
** Date : 11/28/2016 2:32:34 PM
** Summary : UIEventListener  
***********************************************************************/

using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameA.Game
{
	public class UIEventListener: EventTrigger
	{
		public class OnDownHandler : UnityEvent { }

		public class OnUpHandler : UnityEvent{}

		public OnDownHandler OnDown = new OnDownHandler();

		public OnUpHandler OnUp = new OnUpHandler();

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (OnDown != null)
			{
				OnDown.Invoke();
			}
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			if (OnUp != null)
			{
				OnUp.Invoke();
			}
		}
	}
}