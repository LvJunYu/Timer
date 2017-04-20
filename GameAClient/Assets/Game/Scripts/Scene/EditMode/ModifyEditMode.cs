/********************************************************************
** Filename : EditMode
** Author : Dong
** Date : 2016/10/3 星期一 下午 8:46:05
** Summary : EditMode
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Policy;
using SoyEngine;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
	public class ModifyEditMode : EditMode
    {
		#region fileds
//		/// <summary>
//		/// 删除修改的物体堆栈
//		/// </summary>
//		private List<UnitDesc> _removedUnits;
//		/// <summary>
//		/// 改动修改的物体堆栈
//		/// </summary>
//		private List<UnitDesc> _modifiedUnits;
//		/// <summary>
//		/// 添加修改的物体堆栈
//		/// </summary>
//		private List<UnitDesc> _addedUnits;

        #endregion
		#region Properties
		public List<ModifyData> RemovedUnits {
			get {
				return DataScene2D.Instance.RemovedUnits;
			}
		}

		public List<ModifyData> ModifiedUnits {
			get {
				return DataScene2D.Instance.ModifiedUnits;
			}
		}

		public List<ModifyData> AddedUnits {
			get {
				return DataScene2D.Instance.AddedUnits;
			}
		}
        #endregion
		#region Methods

		/// <summary>
		/// 撤销改造擦除
		/// </summary>
		public void UndoModifyErase (int idx) {
			if (idx >= RemovedUnits.Count) {
				LogHelper.Error ("Try to undo the {0}'s erase action out of range");
				return;
			}
			ModifyData data = RemovedUnits [idx];
			RemovedUnits.RemoveAt (idx);
			if (!AddUnit(data.OrigUnit)) {
				RemovedUnits.Insert (idx, data);
				LogHelper.Error ("Can't undo the {0}'s erase action when add unit, unitdesc: {1}", idx, data.OrigUnit);
			}
		}
		/// <summary>
		/// 撤销改造修改
		/// </summary>
		public void UndoModifModify (int idx) {
			
		}
		/// <summary>
		/// 撤销改造添加
		/// </summary>
		public void UndoModifyAdd (int idx) {
			
		}

		public override void Init ()
		{
			base.Init ();
			_commandType = ECommandType.Modify;
//			_removedUnits = new List<UnitDesc> ();
//			_modifiedUnits = new List<UnitDesc> ();
//			_addedUnits = new List<UnitDesc> ();

		}

		protected override void On_TouchStart (Gesture gesture)
		{
//			base.On_TouchStart (gesture);
			_currentCommand = null;
			if (_commandType == ECommandType.Move)
			{
				return;
			}

			switch (_commandType)
			{
			case ECommandType.Erase:
				_currentCommand = new DeleteCommand ();
				break;
			case ECommandType.Modify:
				break;
			case ECommandType.Create:
//				{
//					UnitDesc outValue;
//					if (TryGetSelectedObject(Input.mousePosition, out outValue))
//					{
//						if (!_compositeEditor.IsInCompositeEditorMode)
//						{
//							_currentCommand = new ClickItemCommand(outValue, gesture.position);
//						}
//						else if(_compositeEditor.IsSelecting)
//						{
//							_compositeEditor.OnClickItem(outValue);
//						}
//					}
//					else
//					{
//						if (!_compositeEditor.IsInCompositeEditorMode)
//						{
//							if (_selectedItemId > 0)
//							{
//								Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(_selectedItemId);
//								if (tableUnit.EUnitType == EUnitType.MainPlayer)
//								{
//									_currentCommand = new AddCommandOnce(_mainPlayer);
//								}
//								else if (tableUnit.Count == 1)
//								{
//									UnitDesc unit;
//									_replaceUnits.TryGetValue(_selectedItemId, out unit);
//									_currentCommand = new AddCommandOnce(unit);
//								}
//								else
//								{
//									_currentCommand = new AddCommand();
//								}
//							}
//						}
//
//					}
//				}
				break;
			}


			GM2DGUIManager.Instance.CloseUI<UICtrlItem>();
			if (GM2DGame.Instance.CurrentMode == EMode.Edit)
			{
				//GM2DGUIManager.Instance.OpenUI<UICtrlCreate>();
				GM2DGUIManager.Instance.OpenUI<UICtrlScreenOperator>();
			}
			_lastTouchTime = Time.realtimeSinceStartup;

		}

		public override bool DeleteUnit (UnitDesc unitDesc)
		{
			if (CheckCanModifyErase ()) {
				return base.DeleteUnit (unitDesc);
			} else {
				Messenger<string>.Broadcast(EMessengerType.GameLog, "擦除次数已用完");
				return false;
			}
		}

		protected override void AfterDeleteUnit (UnitDesc unitDesc, Table_Unit tableUnit)
		{
			base.AfterDeleteUnit (unitDesc, tableUnit);
			ModifyData data = new ModifyData ();
			data.OrigUnit = unitDesc;
			RemovedUnits.Add (data);
		}

		/// <summary>
		/// 检查是否可以改造擦除
		/// </summary>
		/// <returns><c>true</c>, if can modify erase was checked, <c>false</c> otherwise.</returns>
		private bool CheckCanModifyErase () {
			// todo 限制数量从玩家属性中取
			if (RemovedUnits.Count >= 5) {
				return false;
			} return true;
		}


		// disabled features
		protected override void OnScreenOperator (EScreenOperator eScreenOperator) {}
		protected override void ForceUpdateCameraMaskSize () {}
		#endregion
    }
}
