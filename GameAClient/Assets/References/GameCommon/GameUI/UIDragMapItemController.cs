/********************************************************************
** Filename : UIDragMapItemController  
** Author : ake
** Date : 7/19/2016 11:22:16 AM
** Summary : UIDragMapItemController  
***********************************************************************/


using UnityEngine;
using UnityEngine.EventSystems;
using GameA.Game;

namespace SoyEngine
{
	public class UIDragMapItemController:MonoBehaviour,IDragHandler,IPointerUpHandler,IPointerDownHandler
	{
		private UnitBase _curDragingItem;
		private Table_Unit _curSelectTable;

		void Awake()
		{
			AddOrRemoveEvent(true);
		}

		public static UIDragMapItemController AddEventListenerTo(GameObject go)
		{
			if (go == null)
			{
				LogHelper.Error("AddEventListenerTo called but go is null!");
				return null;
			}

			UIDragMapItemController res = go.AddComponent<UIDragMapItemController>();
			return res;
		}

		public void SetCurSelectId(int id)
		{
			_curSelectTable = UnitManager.Instance.GetTableUnit(id);
        }


		#region  input event

		public void OnPointerUp(PointerEventData eventData)
		{
			if (_curDragingItem != null)
			{
				AddOrCoverCommand command = new AddOrCoverCommand(_curDragingItem);
				EditMode.Instance.CommandManager.Execute(command, eventData.position);
				_curDragingItem = null;
				EditMode.Instance.SetDraggingState(false);
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{ 
			if (_curSelectTable != null)
			{
                Messenger<ushort>.Broadcast(GameA.EMessengerType.OnSelectedItemChanged, (ushort)_curSelectTable.Id);
			}
		}

		public void OnDrag(PointerEventData data)
		{
			if (_curDragingItem == null)
			{
				if (data.hovered.Count == 0)
				{
					_curDragingItem = UnitManager.Instance.GetUnit(_curSelectTable, EDirectionType.Up);
					EditMode.Instance.SetDraggingState(true);
				}
			}
			if (_curDragingItem == null || _curDragingItem.Trans == null)
			{
				return;
			}

            _curDragingItem.Trans.position = GM2DTools.ScreenToWorldPoint(data.position) - GM2DTools.GetUnitDragingOffset(_curSelectTable.Id);

		} 

		private void OnTouchStart(Gesture gesture)
		{
			
		}

		private void OnTouchDown(Gesture gesture)
		{
			
		}

		private void OnTouchUp(Gesture gesture)
		{
			
		}


		#endregion

		private void AddOrRemoveEvent(bool add)
		{

		}
	}
}