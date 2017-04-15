/********************************************************************
** Filename : MousePinchListener  
** Author : ake
** Date : 5/4/2016 2:15:45 PM
** Summary : MousePinchListener  
***********************************************************************/



using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
	public class MousePinchListener:MonoBehaviour
	{
		public const float SensibilityValue = 0.01f;
		public const float MaxIntervalTime = 0.5f;

		private bool _hasStarted = false;
		private float _lastValidInputTime = 0;


		void Update()
		{
			if (EditMode.Instance.CurCommandType != ECommandType.Move)
			{
				return;
			}
			bool isValidInput;
			float value = Input.GetAxis("Mouse ScrollWheel");
			isValidInput = !(Mathf.Abs(value) - SensibilityValue < 0);
			if (isValidInput)
			{
				if (!_hasStarted)
				{
					_hasStarted = true;
					Messenger.Broadcast(EMessengerType.OnPinchMouseButtonStart);
				}

				Messenger<float>.Broadcast(EMessengerType.OnPinchMouseButton, value);
				_lastValidInputTime = Time.realtimeSinceStartup;
			}
			else
			{
				if(_hasStarted)
				{
					if (Time.realtimeSinceStartup - _lastValidInputTime > MaxIntervalTime)
					{
						_hasStarted = false;
						Messenger.Broadcast(EMessengerType.OnPinchMouseButtonEnd);
					}
				}
			}

		}
	}
}