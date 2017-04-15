/********************************************************************

** Filename : MouseDragListener
** Author : ake
** Date : 2016/4/13 21:13:33
** Summary : MouseDragListener
***********************************************************************/

using System;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
	public class MouseDragListener:MonoBehaviour
	{
		private bool _is2MouseButtonPress = false;
		private Vector2 _lastMousePos;
		private Vector2 _lastDelta;

		void Start()
		{
			_is2MouseButtonPress = false;
		}

		void Update()
		{
			if (EditMode.Instance.CurCommandType != ECommandType.Move)
			{
				return;
			}
			bool mouse2 = Input.GetMouseButton(1);
			if (_is2MouseButtonPress)
			{
				if (mouse2)
				{
					Vector2 delta = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - _lastMousePos;
					Messenger<Vector2>.Broadcast(EMessengerType.OnDrag_2MouseButton, delta);
					_lastMousePos = Input.mousePosition;
					_lastDelta = delta;
				}
				else
				{
					_is2MouseButtonPress = false;
                    var deltaWorldPos = GM2DTools.ScreenToWorldSize(_lastDelta);
					Messenger<Vector2>.Broadcast(EMessengerType.OnDrag_2MouseEnd, deltaWorldPos);
				}
			}
			else
			{
				if (mouse2)
				{
					_is2MouseButtonPress = true;
					_lastMousePos = Input.mousePosition;
					_lastDelta = Vector2.zero;
				}
			}
		}
	}
}