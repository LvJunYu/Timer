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
        private static Vector2 MaskEffectOffset = new Vector2(0.35f, 0.4f);
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

        public List<IntVec2> TempAvailableUnits = new List<IntVec2> ();

        private ECommandType _cmdTypeBeforePlayTest;

        /// <summary>
        /// 蒙版标志缓存
        /// </summary>
        private List<UnityNativeParticleItem> _redMaskEffectCache = new List<UnityNativeParticleItem> ();
        private List<UnityNativeParticleItem> _yellowMaskEffectCache = new List<UnityNativeParticleItem> ();

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

        public void OnModifyAdd (UnitEditData orig) {
            AddedUnits.Add (new ModifyData(orig, orig));
            for (int i = 0; i < TempAvailableUnits.Count; i++) {
                if (TempAvailableUnits [i].x == orig.UnitDesc.Id) {
                    IntVec2 updatedInfo = new IntVec2 (orig.UnitDesc.Id, TempAvailableUnits [i].y - 1);
                    TempAvailableUnits [i] = updatedInfo;
//                    if (TempAvailableUnits [i].y == 0) {
//                        _selectedItemId = 0;
//                    }
                    break;
                }
            }
            Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
            UpdateMaskEffects ();
        }

		public void OnModifyDelete (UnitEditData orig) {
			RemovedUnits.Add (new ModifyData(orig, orig));
			Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
            UpdateMaskEffects ();
		}

		public void OnModifyModify (UnitEditData orig, UnitEditData modified) {
            // 检查是否是对已修改地块再次进行修改
            for (int i = ModifiedUnits.Count - 1; i >= 0; i--) {
                if (ModifiedUnits [i].ModifiedUnit.UnitDesc.Guid == orig.UnitDesc.Guid) {
                    // 如果经过变换又回到了初始状态
                    if (ModifiedUnits [i].OrigUnit == modified) {
                        ModifiedUnits.RemoveAt (i);
                        Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
                    } else {
                        ModifyData data = ModifiedUnits [i];
                        data.ModifiedUnit = modified;
                        ModifiedUnits [i] = data;
                    }
                    UpdateMaskEffects ();
                    return;
                }
            }
			ModifiedUnits.Add (new ModifyData(orig, modified));
            UpdateMaskEffects ();
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
            UpdateMaskEffects ();
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

            if (!DeleteUnit (data.ModifiedUnit.UnitDesc)) {
                ModifiedUnits.Insert (idx, data);
                LogHelper.Error ("Can't undo the {0}'s modify action when delete unit, unitdesc: {1}", idx, data.ModifiedUnit);
                return;
            } else {
                DataScene2D.Instance.ProcessUnitExtra(data.OrigUnit.UnitDesc.Guid, UnitExtra.zero);
            }
			if (!AddUnit(data.OrigUnit.UnitDesc)) {
				ModifiedUnits.Insert (idx, data);
				LogHelper.Error ("Can't undo the {0}'s modify action when add unit, unitdesc: {1}", idx, data.OrigUnit);
				return;
			} else {
				DataScene2D.Instance.ProcessUnitExtra(data.OrigUnit.UnitDesc.Guid, data.OrigUnit.UnitExtra);
			}
            Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
            UpdateMaskEffects ();
		}
		/// <summary>
		/// 撤销改造添加
		/// </summary>
		public void UndoModifyAdd (int idx) {
            if (idx >= AddedUnits.Count) {
                LogHelper.Error ("Try to undo the {0}'s add action out of range");
                return;
            }
            ModifyData data = AddedUnits [idx];
            AddedUnits.RemoveAt (idx);
            if (!DeleteUnit (data.ModifiedUnit.UnitDesc)) {
                ModifiedUnits.Insert (idx, data);
                LogHelper.Error ("Can't undo the {0}'s modify action when delete unit, unitdesc: {1}", idx, data.ModifiedUnit);
                return;
            } else {
                DataScene2D.Instance.ProcessUnitExtra(data.ModifiedUnit.UnitDesc.Guid, UnitExtra.zero);
            }

            for (int i = 0; i < TempAvailableUnits.Count; i++) {
                if (TempAvailableUnits [i].x == data.ModifiedUnit.UnitDesc.Id) {
                    IntVec2 updatedInfo = new IntVec2 (data.ModifiedUnit.UnitDesc.Id, TempAvailableUnits [i].y + 1);
                    TempAvailableUnits [i] = updatedInfo;
                    break;
                }
            }

            Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
            UpdateMaskEffects ();

		}


		public override void Init ()
		{
			base.Init ();
			_commandType = ECommandType.Modify;
//			_removedUnits = new List<UnitDesc> ();
//			_modifiedUnits = new List<UnitDesc> ();
//			_addedUnits = new List<UnitDesc> ();

            TempAvailableUnits.Add (new IntVec2 (4006, 3));
            TempAvailableUnits.Add (new IntVec2 (4007, 6));
            TempAvailableUnits.Add (new IntVec2 (4008, 1));

		}

        protected override void OnCommandChanged(ECommandType eCommandType)
        {
            if (eCommandType == ECommandType.Play) {
                _cmdTypeBeforePlayTest = eCommandType;
            }
            base.OnCommandChanged (eCommandType);
            if (eCommandType == ECommandType.Pause) {
                _commandType = _cmdTypeBeforePlayTest;
            }

            UpdateMaskEffects ();
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
				{
    				if (_selectedItemId > 0)
    				{
    					Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(_selectedItemId);
    					
//    					else if (tableUnit.Count == 1)
//    					{
//    						UnitDesc unit;
//    						_replaceUnits.TryGetValue(_selectedItemId, out unit);
//    						_currentCommand = new AddCommandOnce(unit);
//    					}
//    					else
    					{
    						_currentCommand = new ModifyAddCommand();
    					}
    				}
				}
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
        /// 更新表示当前改造操作已影响物体的蒙版特效
        /// </summary>
        private void UpdateMaskEffects () {
            int redCnt = 0;
            int yellowCnt = 0;
            if (_commandType == ECommandType.Create) {
                for (int i = 0; i < AddedUnits.Count; i++) {
                    SetMaskEffectPos (GetUnusedYellowMask (yellowCnt++), AddedUnits [i].ModifiedUnit.UnitDesc.Guid);
                }
                // 所有删除位置、改动位置为红色
                for (int i = 0; i < RemovedUnits.Count; i++) {
                    SetMaskEffectPos (GetUnusedRedMask (redCnt++), RemovedUnits [i].OrigUnit.UnitDesc.Guid);
                }
                for (int i = 0; i < ModifiedUnits.Count; i++) {
                    if (ModifiedUnits [i].OrigUnit.UnitDesc.Guid != ModifiedUnits [i].ModifiedUnit.UnitDesc.Guid) {
                        SetMaskEffectPos (GetUnusedRedMask (redCnt++), ModifiedUnits [i].OrigUnit.UnitDesc.Guid);
                    }
                }
            } else if (_commandType == ECommandType.Erase) {
                for (int i = 0; i < RemovedUnits.Count; i++) {
                    SetMaskEffectPos (GetUnusedYellowMask (yellowCnt++), RemovedUnits [i].OrigUnit.UnitDesc.Guid);
                }
                // 所有添加位置、改动位置为红色
                for (int i = 0; i < AddedUnits.Count; i++) {
                    SetMaskEffectPos (GetUnusedRedMask (redCnt++), AddedUnits [i].ModifiedUnit.UnitDesc.Guid);
                }
                for (int i = 0; i < ModifiedUnits.Count; i++) {
                    SetMaskEffectPos (GetUnusedRedMask (redCnt++), ModifiedUnits [i].ModifiedUnit.UnitDesc.Guid);
                }
            } else if (_commandType == ECommandType.Modify) {
                for (int i = 0; i < ModifiedUnits.Count; i++) {
                    SetMaskEffectPos (GetUnusedYellowMask (yellowCnt++), ModifiedUnits [i].OrigUnit.UnitDesc.Guid);
                    if (ModifiedUnits [i].OrigUnit.UnitDesc.Guid != ModifiedUnits [i].ModifiedUnit.UnitDesc.Guid) {
                        SetMaskEffectPos (GetUnusedYellowMask (yellowCnt++), ModifiedUnits [i].ModifiedUnit.UnitDesc.Guid);
                    }
                }
                // 所有添加位置为红色
                for (int i = 0; i < AddedUnits.Count; i++) {
                    SetMaskEffectPos (GetUnusedRedMask (redCnt++), AddedUnits [i].ModifiedUnit.UnitDesc.Guid);
                }
            }

            for (; yellowCnt < _yellowMaskEffectCache.Count; yellowCnt++) {
                _yellowMaskEffectCache [yellowCnt].Stop ();
            }
            for (; redCnt < _redMaskEffectCache.Count; redCnt++) {
                _redMaskEffectCache [redCnt].Stop ();
            }
        }

        private UnityNativeParticleItem GetUnusedYellowMask (int idx) { 
            if (_yellowMaskEffectCache.Count <= idx) {
                UnityNativeParticleItem newYellowMask = GameParticleManager.Instance.GetUnityNativeParticleItem (ParticleNameConstDefineGM2D.YellowMask, null);
                if (null == newYellowMask) {
                    LogHelper.Error ("Load mask effect failed, name is {0}", ParticleNameConstDefineGM2D.YellowMask);
                    return null;
                }
                _yellowMaskEffectCache.Add (newYellowMask);
            }
            return _yellowMaskEffectCache[idx];
        }
        private UnityNativeParticleItem GetUnusedRedMask (int idx) { 
            if (_redMaskEffectCache.Count <= idx) {
                UnityNativeParticleItem newRedMask = GameParticleManager.Instance.GetUnityNativeParticleItem (ParticleNameConstDefineGM2D.RedMask, null);
                if (null == newRedMask) {
                    LogHelper.Error ("Load mask effect failed, name is {0}", ParticleNameConstDefineGM2D.RedMask);
                    return null;
                }
                _redMaskEffectCache.Add (newRedMask);
            }
            return _redMaskEffectCache[idx];
        }
        private void SetMaskEffectPos (UnityNativeParticleItem effect, IntVec3 guid) {
            Vector3 pos = GM2DTools.TileToWorld (guid);
            pos.z = -60f;
            pos.x += MaskEffectOffset.x;
            pos.y += MaskEffectOffset.y;
            effect.Trans.position = pos;
            effect.Play ();
        }


        /// <summary>
        /// 检查是否可以改造添加
        /// </summary>
        /// <returns><c>true</c>, if can modify add was checked, <c>false</c> otherwise.</returns>
        public bool CheckCanModifyAdd (UnitDesc unitDesc) {
            // 检查是否是删除的物体
            for (int i = 0, n = RemovedUnits.Count; i < n; i++) {
                if (RemovedUnits [i].OrigUnit.UnitDesc.Guid == unitDesc.Guid) {
                    // 出错的才会到这里
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "不能在删除物体的原位置添加");
                    return false;
                }
            }
            // 检查是否是改动的物体
            for (int i = 0, n = ModifiedUnits.Count; i < n; i++) {
                if (ModifiedUnits [i].OrigUnit.UnitDesc.Guid == unitDesc.Guid ||
                    ModifiedUnits [i].ModifiedUnit.UnitDesc.Guid == unitDesc.Guid) {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "不能在变换物体的原位置添加");
                    return false;
                }
            }
            // 检查是否达到了删除数量上限
            // todo 限制数量从玩家属性中取
            if (AddedUnits.Count >= 5) {
                Messenger<string>.Broadcast(EMessengerType.GameLog, "添加次数已用完");
                return false;
            }

            // 检查地块数量是否够
            for (int i = 0; i < TempAvailableUnits.Count; i++) {
                if (TempAvailableUnits [i].x == unitDesc.Id) {
                    if (TempAvailableUnits [i].y <= 0) {
                        Messenger<string>.Broadcast (EMessengerType.GameLog, "这种地块已用完");
                        return false;
                    }
                }
            }

            return true;
        }

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
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "不能删除变换过的物体");
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
            if (orig != modified) {
                for (int i = 0, n = ModifiedUnits.Count; i < n; i++) {
                    if (ModifiedUnits [i].OrigUnit.UnitDesc.Guid == modified.Guid &&
                        ModifiedUnits [i].ModifiedUnit.UnitDesc.Guid != orig.Guid) {
                        Messenger<string>.Broadcast (EMessengerType.GameLog, "不能移动到已移动物体的原位置");
                        return false;
                    }
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
