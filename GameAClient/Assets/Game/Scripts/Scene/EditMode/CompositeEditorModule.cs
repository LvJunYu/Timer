/********************************************************************
** Filename : CompositeEditorModule  
** Author : ake
** Date : 3/14/2017 11:37:31 AM
** Summary : CompositeEditorModule  
***********************************************************************/


using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using SoyEngine;

namespace GameA.Game
{

	public enum EMoveOperatorRes
	{
		Success,
		Failed,
	}
	public class CompositeEditorModule
	{
		private ECompositeEditorState _lastCompositeEditorState= ECompositeEditorState.None;
		private ECompositeEditorState _curCompositeEditorState = ECompositeEditorState.None;
		private bool _isSelecting = false;
		private List<UnitDesc> _selectDescList;
		private Dictionary<IntVec3,UnitDesc> _cachedSelectDescDic;

		private Grid2D _curSelectItemGrid;

		private const int GridCellRatio = 640;

		public ECompositeEditorState LastState
		{
			get { return _lastCompositeEditorState; }
		}

		public ECompositeEditorState CurState
		{
			get
			{
				return _curCompositeEditorState;
			}
		}

		public bool IsSelecting
		{
			get { return _isSelecting; }
		}

		public bool IsInCompositeEditorMode
		{
			get { return _curCompositeEditorState != ECompositeEditorState.None; }
		}

		public bool HasSelectSomething
		{
			get { return _selectDescList.Count != 0; }
		}

		public CompositeEditorModule()
		{
			_selectDescList = new List<UnitDesc>();
			_cachedSelectDescDic = new Dictionary<IntVec3, UnitDesc>();
		}

		public void Init()
		{
			//Messenger.AddListener(EMessengerType.OnCancelSelectState, OnCancelSelectMode);
			
		}

		public void Clear()
		{
			//Messenger.RemoveListener(EMessengerType.OnCancelSelectState, OnCancelSelectMode);
		}

		public void EnterSelectMode()
		{
			_isSelecting = true;
			_curCompositeEditorState = ECompositeEditorState.MultipleSelect;
		}

		public void ExitSelectMode()
		{
			_isSelecting = false;
			_curCompositeEditorState = ECompositeEditorState.None;
			for (int i = 0; i<_selectDescList.Count; i++)
			{
				var item = _selectDescList[i];
				UnitBase itemUnitBase = null;
				if (!ColliderScene2D.Instance.TryGetUnit(item.Guid, out itemUnitBase))
				{
					LogHelper.Error("ColliderScene2D.Instance.TryGetUnit(item.Guid, out itemUnitBase) return false,guid is {0} ¡£", item.Guid);
					return;
				}
				CancelSelectItem(item, itemUnitBase,false);
			}
			_selectDescList.Clear();
			_cachedSelectDescDic.Clear();
		}

		public void EnterMoveMode()
		{
			SetCompositeState(ECompositeEditorState.Move);
		}


		public void OnClickItem(UnitDesc item)
		{
			if (item == UnitDesc.zero)
			{
				LogHelper.Error("OnClickItem called but item is equal to UnitDesc.Zero");
				return;
			}
			UnitBase itemUnitBase = null;
			if (!ColliderScene2D.Instance.TryGetUnit(item.Guid, out itemUnitBase))
			{
				LogHelper.Error("ColliderScene2D.Instance.TryGetUnit(item.Guid, out itemUnitBase) return false,guid is {0} ¡£",item.Guid);
				return;
			}
			UnitDesc findRes = UnitDesc.zero;
			bool itemInList = false;
			if (TryFindItemFromList(item.Guid, out findRes))
			{
				if (findRes != item)
				{
					LogHelper.Error("item {0} findres {1} not equal¡£",item,findRes);
					return;
				}
				itemInList = true;
			}
			if (itemInList)
			{
				CancelSelectItem(item, itemUnitBase,true);
			}
			else
			{
				SelectItem(item, itemUnitBase);
			}
		}

		public void OnMoveSelectItems(EDirectionType dirType)
		{
			SortList(dirType);
			for (int i = 0; i < _selectDescList.Count; i++)
			{
				var item = _selectDescList[i];
			}
		}


		#region private 

		private void SetCompositeState(ECompositeEditorState state)
		{
			if (_curCompositeEditorState == state)
			{
				return;
			}

			_lastCompositeEditorState = _curCompositeEditorState;
			switch (_lastCompositeEditorState)
			{
				case ECompositeEditorState.MultipleSelect:
				{
					_isSelecting = false;
					break;
				}
			}
			_curCompositeEditorState = state;
			Messenger.Broadcast(EMessengerType.OnCurCompositeEditorStateChanged);
		}

		private bool TryFindItemFromList(IntVec3 id, out UnitDesc findRes)
		{
			return _cachedSelectDescDic.TryGetValue(id, out findRes);
		}

		private void SelectItem(UnitDesc item, UnitBase itemBase)
		{
			_selectDescList.Add(item);
			_cachedSelectDescDic.Add(item.Guid, item);
			itemBase.OnSelectStateChanged(true);
			UpdateCurSelectGrid();
		}

		private void CancelSelectItem(UnitDesc item, UnitBase itemBase,bool removeData)
		{
			if (removeData)
			{
				DeleteItemFromData(item);
			}
			itemBase.OnSelectStateChanged(false);
			UpdateCurSelectGrid();
		}

		private void DeleteItemFromData(UnitDesc unit)
		{
			for (int i = 0; i < _selectDescList.Count; i++)
			{
				var item = _selectDescList[i];
				if (item.Guid == unit.Guid)
				{
					_selectDescList.RemoveAt(i);
					break;
				}
			}
			if (_cachedSelectDescDic.ContainsKey(unit.Guid))
			{
				_cachedSelectDescDic.Remove(unit.Guid);
			}
		}

		private void UpdateCurSelectGrid()
		{
			bool hasInit = false;
			Grid2D res = Grid2D.zero;
			for (int i = 0; i < _selectDescList.Count; i++)
			{
				var item = _selectDescList[i];
				Table_Unit unit = UnitManager.Instance.GetTableUnit(item.Id);
				if (unit == null)
				{
					return;
				}
				var grid = unit.GetColliderGrid(ref item);
				if (!hasInit)
				{
					res = grid;
					hasInit = true;
				}
				res.Merge(grid);
			}
			_curSelectItemGrid = res;
			LogHelper.Debug(_curSelectItemGrid.ToString() +"_" +_curSelectItemGrid.XMin);
		}


		#endregion

		#region event

		private void OnCompositeEditorStateChanged()
		{
			
		}

		private void OnCancelSelectMode()
		{
			
		}


		#endregion

		#region sorting

		private void SortList(EDirectionType dirType)
		{
			if (dirType < EDirectionType.Up || dirType > EDirectionType.Left)
			{
				return;
			}
			if (_selectDescList == null)
			{
				return;
			}
			switch (dirType)
			{
				case EDirectionType.Left:
				{
					_selectDescList.Sort(SortFromLeft);
					break;
				}
				case EDirectionType.Down:
				{
					_selectDescList.Sort(SortFromDown);
					break;
				}
				case EDirectionType.Right:
				{
					_selectDescList.Sort(SortFromRight);
					break;
				}
				case EDirectionType.Up:
				{
					_selectDescList.Sort(SortFromUp);
					break;
				}
			}
		}

		private int SortFromUp(UnitDesc u1, UnitDesc u2)
		{
			return u1.Guid.y.CompareTo(u2.Guid.y);
		}

		private int SortFromDown(UnitDesc u1, UnitDesc u2)
		{
			return -u1.Guid.y.CompareTo(u2.Guid.y);
		}

		private int SortFromRight(UnitDesc u1, UnitDesc u2)
		{
			return u1.Guid.x.CompareTo(u2.Guid.x);
		}

		private int SortFromLeft(UnitDesc u1, UnitDesc u2)
		{
			return -u1.Guid.x.CompareTo(u2.Guid.x);
		}
		#endregion


	}
}
