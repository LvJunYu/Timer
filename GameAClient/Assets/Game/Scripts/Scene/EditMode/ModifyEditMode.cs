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

		public void OnModifyDelete (UnitEditData orig) {
			RemovedUnits.Add (new ModifyData(orig, orig));
			Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
		}

		public void OnModifyModify (UnitEditData orig, UnitEditData modified) {
            // 检查是否是对已修改地块再次进行修改
            for (int i = 0, n = ModifiedUnits.Count; i < n; i++) {
                if (ModifiedUnits [i].ModifiedUnit.UnitDesc.Guid == orig.UnitDesc.Guid) {
                    ModifyData data = ModifiedUnits [i];
                    data.ModifiedUnit = modified;
                    ModifiedUnits [i] = data;
                    return;
                }
            }
			ModifiedUnits.Add (new ModifyData(orig, modified));
			Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
		}

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
			if (!AddUnit(data.OrigUnit.UnitDesc)) {
				RemovedUnits.Insert (idx, data);
				LogHelper.Error ("Can't undo the {0}'s erase action when add unit, unitdesc: {1}", idx, data.OrigUnit);
			} else {
				DataScene2D.Instance.ProcessUnitExtra(data.OrigUnit.UnitDesc.Guid, data.OrigUnit.UnitExtra);
			}
            Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
		}
		/// <summary>
		/// 撤销改造修改
		/// </summary>
		public void UndoModifModify (int idx) {
			if (idx >= ModifiedUnits.Count) {
				LogHelper.Error ("Try to undo the {0}'s modify action out of range");
				return;
			}
			ModifyData data = ModifiedUnits [idx];
			ModifiedUnits.RemoveAt (idx);
//			bool success = true;

			if (!DeleteUnit(data.ModifiedUnit.UnitDesc)) {
				ModifiedUnits.Insert (idx, data);
				LogHelper.Error ("Can't undo the {0}'s modify action when delete unit, unitdesc: {1}", idx, data.ModifiedUnit);
				return;
			}
			if (!AddUnit(data.OrigUnit.UnitDesc)) {
				ModifiedUnits.Insert (idx, data);
				LogHelper.Error ("Can't undo the {0}'s modify action when add unit, unitdesc: {1}", idx, data.OrigUnit);
				return;
			} else {
				DataScene2D.Instance.ProcessUnitExtra(data.OrigUnit.UnitDesc.Guid, data.OrigUnit.UnitExtra);
			}
            Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
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
				_currentCommand = new ModifyDeleteCommand ();
				break;
			case ECommandType.Modify:
				UnitDesc outValue;
				if (TryGetSelectedObject(Input.mousePosition, out outValue))
				{
                    if (ChackCanModifyModify (outValue, outValue)) {
                        _currentCommand = new ModifyClickItemCommand (outValue, gesture.position);
                    }
				}
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

//		public override bool DeleteUnit (UnitDesc unitDesc)
//		{
//            if (CheckCanModifyErase (unitDesc)) {
//				return base.DeleteUnit (unitDesc);
//			} else {
//				return false;
//			}
//		}

//		protected override void AfterDeleteUnit (UnitDesc unitDesc, Table_Unit tableUnit)
//		{
//			base.AfterDeleteUnit (unitDesc, tableUnit);
//		}

		/// <summary>
		/// 检查是否可以改造擦除
		/// </summary>
		/// <returns><c>true</c>, if can modify erase was checked, <c>false</c> otherwise.</returns>
        public bool CheckCanModifyErase (UnitDesc unitDesc) {
            // 检查是否是删除的物体
            for (int i = 0, n = RemovedUnits.Count; i < n; i++) {
                if (RemovedUnits [i].OrigUnit.UnitDesc.Guid == unitDesc.Guid) {
                    // 出错的才会到这里
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "不能重复删除物体");
                    return false;
                }
            }
            // 检查是否是改动的物体
            for (int i = 0, n = ModifiedUnits.Count; i < n; i++) {
                if (ModifiedUnits [i].OrigUnit.UnitDesc.Guid == unitDesc.Guid ||
                    ModifiedUnits [i].ModifiedUnit.UnitDesc.Guid == unitDesc.Guid) {
                    return false;
                }
            }
            // 检查是否是添加的物体
            for (int i = 0, n = AddedUnits.Count; i < n; i++) {
                if (AddedUnits [i].ModifiedUnit.UnitDesc.Guid == unitDesc.Guid) {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "不能删除改造添加的物体");
                    return false;
                }
            }
            // 检查是否达到了删除数量上限
			// todo 限制数量从玩家属性中取
			if (RemovedUnits.Count >= 5) {
                Messenger<string>.Broadcast(EMessengerType.GameLog, "擦除次数已用完");
				return false;
			}
            return true;
		}
        /// <summary>
        /// 检查是否可以改造修改
        /// </summary>
        /// <returns><c>true</c>, if can modify modify was chacked, <c>false</c> otherwise.</returns>
        /// <param name="orig">Original.</param>
        /// <param name="modified">Modified.</param>
        public bool ChackCanModifyModify (UnitDesc orig, UnitDesc modified) {
            // 检查目标位置是否存在删除物体
            for (int i = 0, n = RemovedUnits.Count; i < n; i++) {
                if (RemovedUnits [i].OrigUnit.UnitDesc.Guid == modified.Guid) {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "不能覆盖改造删除的物体");
                    return false;
                }
            }
            // 检查原物体是否是添加物体
            for (int i = 0, n = AddedUnits.Count; i < n; i++) {
                if (AddedUnits [i].ModifiedUnit.UnitDesc.Guid == orig.Guid) {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "不能覆盖改造添加的物体");
                    return false;
                }
            }
            // 检查目标位置是否是已修改物体的原位置
            for (int i = 0, n = ModifiedUnits.Count; i < n; i++) {
                if (ModifiedUnits [i].OrigUnit.UnitDesc.Guid == modified.Guid) {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "不能移动到已移动物体的原位置");
                    return false;
                }
            }
            // todo 限制数量从玩家属性中取
            if (ModifiedUnits.Count >= 5) {
                Messenger<string>.Broadcast(EMessengerType.GameLog, "改变次数已用完");
                return false;
            }
            return true;
        }


		// disabled features
		protected override void OnScreenOperator (EScreenOperator eScreenOperator) {}
		protected override void ForceUpdateCameraMaskSize () {}
		#endregion
    }
}
