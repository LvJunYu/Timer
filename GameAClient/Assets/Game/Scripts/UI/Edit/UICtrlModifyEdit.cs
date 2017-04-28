/********************************************************************
** Filename : UICtrlEdit
** Author : Dong
** Date : 2015/7/2 16:30:13
** Summary : UICtrlEdit
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA.Game
{
    [UIAutoSetup(EUIAutoSetupType.Create)]
    public class UICtrlModifyEdit : UICtrlGenericBase<UIViewModifyEdit>
    {
        #region 常量与字段

        /// <summary>
        /// 当前选择的item序号
        /// </summary>
        private int _selectedItemIdx;

        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

			_cachedView.ModifyEraseBtn.onClick.AddListener (OnModifyEraseBtn);
			_cachedView.ModifyModifyBtn.onClick.AddListener (OnModifyModifyBtn);
			_cachedView.ModifyAddBtn.onClick.AddListener (OnModifyAddBtn);

            _cachedView.SelectAddItemPanel.SetActive (false);

            for (int i = 0; i < _cachedView.ModifyItems.Length; i++) {
                _cachedView.ModifyItems [i].id = i;
                _cachedView.ModifyItems[i].DelBtnCb = OnModifyItemDelBtn;
                _cachedView.ModifyItems [i].IconBtnCb = OnModifyItemIconBtn;
            }

            _selectedItemIdx = -1;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.AfterCommandChanged, AfterCommandChanged);
            RegisterEvent(EMessengerType.OnModifyUnitChanged, OnModifyUnitChanged);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);

            UpdateModifyItemList ();
            UpdateSelectItemList ();
        }

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			Messenger.RemoveListener(EMessengerType.AfterCommandChanged, AfterCommandChanged);
			Messenger.RemoveListener(EMessengerType.OnModifyUnitChanged, OnModifyUnitChanged);
		}

        public override void OnUpdate ()
        {
            base.OnUpdate ();
        }


		private void OnModifyAddBtn () {
			if (EditMode.Instance.CurCommandType != ECommandType.Create) {
				SwitchModifyMode (ECommandType.Create);
			}
		}
		private void OnModifyEraseBtn () {
			if (EditMode.Instance.CurCommandType != ECommandType.Erase) {
				SwitchModifyMode (ECommandType.Erase);
			}
		}
		private void OnModifyModifyBtn () {
			if (EditMode.Instance.CurCommandType != ECommandType.Modify) {
				SwitchModifyMode (ECommandType.Modify);
			}
		}

		private void SwitchModifyMode (ECommandType type) {
			switch (type) {
            case ECommandType.Create:
                _cachedView.ModifyAddBtn.transform.localScale = Vector3.one * 1.2f;
                _cachedView.ModifyEraseBtn.transform.localScale = Vector3.one;
                _cachedView.ModifyModifyBtn.transform.localScale = Vector3.one;

                _cachedView.SelectAddItemPanel.SetActive (true);
                UpdateSelectItemList ();
				break;
			case ECommandType.Erase:
				_cachedView.ModifyAddBtn.transform.localScale = Vector3.one;
				_cachedView.ModifyEraseBtn.transform.localScale = Vector3.one * 1.2f;
				_cachedView.ModifyModifyBtn.transform.localScale = Vector3.one;

                _cachedView.SelectAddItemPanel.SetActive (false);
				break;
			case ECommandType.Modify:
				_cachedView.ModifyAddBtn.transform.localScale = Vector3.one;
				_cachedView.ModifyEraseBtn.transform.localScale = Vector3.one;
				_cachedView.ModifyModifyBtn.transform.localScale = Vector3.one * 1.2f;

                _cachedView.SelectAddItemPanel.SetActive (false);
				break;
			}
			Messenger<ECommandType>.Broadcast(EMessengerType.OnCommandChanged, type);

			// update modifyItemList
			UpdateModifyItemList();
		}

		/// <summary>
		/// 改造列表删除按钮响应函数
		/// </summary>
		/// <param name="idx">Index.</param>
		private void OnModifyItemDelBtn (int idx) {
			if (EditMode.Instance.CurCommandType == ECommandType.Create) {
                ((ModifyEditMode)EditMode.Instance).UndoModifyAdd (idx);
			} else if (EditMode.Instance.CurCommandType == ECommandType.Erase) {
				((ModifyEditMode)EditMode.Instance).UndoModifyErase (idx);
			} else if (EditMode.Instance.CurCommandType == ECommandType.Modify) {
				((ModifyEditMode)EditMode.Instance).UndoModifModify (idx);
			}
		}

        /// <summary>
        /// 改造列表图标按钮响应函数
        /// </summary>
        /// <param name="idx">Index.</param>
        private void OnModifyItemIconBtn (int idx) {
//            if (EditMode.Instance.CurCommandType == ECommandType.Create) {
//                ((ModifyEditMode)EditMode.Instance).UndoModifyAdd (idx);
//            } else if (EditMode.Instance.CurCommandType == ECommandType.Erase) {
//                ((ModifyEditMode)EditMode.Instance).UndoModifyErase (idx);
//            } else if (EditMode.Instance.CurCommandType == ECommandType.Modify) {
//                ((ModifyEditMode)EditMode.Instance).UndoModifModify (idx);
//            }
            Debug.Log ("____________Press icon btn " + idx);
        }

        private void OnModifySelectItemBtn (int idx) {
            if (_selectedItemIdx == idx) {
                _cachedView.SelectItems [idx].SetSelected (false);
                _selectedItemIdx = -1;
                Messenger<ushort>.Broadcast(EMessengerType.OnSelectedItemChanged, 0);
                return;
            }
            ModifyEditMode editMode = EditMode.Instance as ModifyEditMode;
            for (int i = 0; i < _cachedView.SelectItems.Length; i++) {
                if (idx == i) {
                    if (editMode.TempAvailableUnits[idx].y == 0) {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "这种东西用完了");
                        return;
                    }
                    _cachedView.SelectItems [i].SetSelected (true);
                } else {
                    _cachedView.SelectItems [i].SetSelected (false);
                }
            }

            Messenger<ushort>.Broadcast(EMessengerType.OnSelectedItemChanged, (ushort)editMode.TempAvailableUnits[idx].x);
        }
		/// <summary>
		/// 更新改造地块列表界面
		/// </summary>
		private void UpdateModifyItemList () {
			
			ModifyEditMode modifyEditMode = EditMode.Instance as ModifyEditMode;
			if (null == modifyEditMode)
				return;
            List<ModifyData> descs = null;
			switch (EditMode.Instance.CurCommandType) {
			case ECommandType.Create:
				descs = modifyEditMode.AddedUnits;
				break;
			case ECommandType.Erase:
				descs = modifyEditMode.RemovedUnits;
				break;
			case ECommandType.Modify:
				descs = modifyEditMode.ModifiedUnits;
				break;
			}
			if (null == descs)
				return;
			int i = 0;
			for (; i < _cachedView.ModifyItems.Length && i < descs.Count; i++) {
                var tableUnit = TableManager.Instance.GetUnit (descs [i].OrigUnit.UnitDesc.Id);
				if (null == tableUnit) {
                    LogHelper.Error ("can't find tabledata of modifyItem id{0}", descs[i].OrigUnit.UnitDesc.Id);
				} else {
					Sprite texture;
					if (GameResourceManager.Instance.TryGetSpriteByName(tableUnit.Icon, out texture))
					{
						_cachedView.ModifyItems [i].SetItem (texture);
					}
					else
					{
						LogHelper.Error("tableUnit {0} icon {1} invalid! tableUnit.EGeneratedType is {2}", tableUnit.Id,
							tableUnit.Icon, tableUnit.EGeneratedType);
					}
				}
			}
			for (; i < _cachedView.ModifyItems.Length; i++) {
				_cachedView.ModifyItems [i].SetEmpty ();
			}
		}

        /// <summary>
        /// 更新添加地块列表
        /// </summary>
        private void UpdateSelectItemList () {
            ModifyEditMode editMode = EditMode.Instance as ModifyEditMode;
            for (int i = 0; i < _cachedView.SelectItems.Length; i++) {
                _cachedView.SelectItems [i].id = i;
                _cachedView.SelectItems[i].BtnCb = OnModifySelectItemBtn;
                Table_Unit tableUnit = TableManager.Instance.GetUnit (editMode.TempAvailableUnits[i].x);
                Sprite texture;
                if (GameResourceManager.Instance.TryGetSpriteByName (tableUnit.Icon, out texture)) {
                    _cachedView.SelectItems [i].SetItem (texture, editMode.TempAvailableUnits[i].y);
                } else {
                    LogHelper.Error ("Can't find icon of unit id: {0}", tableUnit.Id);
                }
            }
        }

        #region event 
		private void OnModifyUnitChanged () {
			UpdateModifyItemList ();
            ModifyEditMode editMode = EditMode.Instance as ModifyEditMode;
//            if (editMode.SelectedItemId == 0) {
//                _selectedItemIdx = -1;
//                for (int i = 0; i < _cachedView.SelectItems.Length; i++) {
//                    _cachedView.SelectItems [i].SetSelected (false);
//                }
//            }
            if (EditMode.Instance.CurCommandType == ECommandType.Create) {
                UpdateSelectItemList ();
            }
		}


	    private void AfterCommandChanged()
	    {
		}

		#endregion

		#endregion
	}
}
