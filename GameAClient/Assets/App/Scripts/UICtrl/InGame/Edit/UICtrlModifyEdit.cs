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
using UnityEngine;
using UnityEngine.UI;
using GameA.Game;
using NewResourceSolution;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlModifyEdit : UICtrlInGameBase<UIViewModifyEdit>
    {
        #region 常量与字段

        // 目前版本可能的最大数量限制
        private const int _maxAltLimit = 5;

        private const int _maxDelLimit = 5;
        private const int _maxAddLimit = 5;

        /// <summary>
        /// 当前选择的item序号
        /// </summary>
        private int _selectedItemIdx;

        private int _altLimit;
        private int _delLimit;
        private int _addLimit;

        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

            _cachedView.ModifyEraseBtn.onClick.AddListener(OnModifyEraseBtn);
            _cachedView.ModifyModifyBtn.onClick.AddListener(OnModifyModifyBtn);
            _cachedView.ModifyAddBtn.onClick.AddListener(OnModifyAddBtn);

            _cachedView.SelectAddItemPanel.SetActive(false);

            for (int i = 0; i < _cachedView.ModifyItems.Length; i++)
            {
                _cachedView.ModifyItems[i].id = i;
                _cachedView.ModifyItems[i].DelBtnCb = OnModifyItemDelBtn;
                _cachedView.ModifyItems[i].IconBtnCb = OnModifyItemIconBtn;
            }

            _selectedItemIdx = -1;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.AfterEditModeStateChange, AfterCommandChanged);
            RegisterEvent(EMessengerType.OnModifyUnitChanged, OnModifyUnitChanged);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);

            _altLimit = Mathf.Min(LocalUser.Instance.MatchUserData.ReformModifyUnitCapacity, _maxAltLimit);
            _delLimit = Mathf.Min(LocalUser.Instance.MatchUserData.ReformDeleteUnitCapacity, _maxDelLimit);
            _addLimit = Mathf.Min(LocalUser.Instance.MatchUserData.ReformAddUnitCapacity, _maxAddLimit);

            AfterCommandChanged();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Messenger.RemoveListener(EMessengerType.AfterEditModeStateChange, AfterCommandChanged);
            Messenger.RemoveListener(EMessengerType.OnModifyUnitChanged, OnModifyUnitChanged);
        }


        private void OnModifyAddBtn()
        {
            EditMode.Instance.StartModifyAdd();
        }

        private void OnModifyEraseBtn()
        {
            EditMode.Instance.StartModifyRemove();
        }

        private void OnModifyModifyBtn()
        {
            EditMode.Instance.StartModifyModify();
        }


        /// <summary>
        /// 改造列表删除按钮响应函数
        /// </summary>
        /// <param name="idx">Index.</param>
        private void OnModifyItemDelBtn(int idx)
        {
            if (EditMode.Instance.IsInState(EditModeState.ModifyAdd.Instance))
            {
                EditModeState.ModifyAdd.Instance.UndoModifyAdd(idx);
            }
            else if (EditMode.Instance.IsInState(EditModeState.ModifyRemove.Instance))
            {
                EditModeState.ModifyRemove.Instance.UndoModifyErase(idx);
            }
            else if (EditMode.Instance.IsInState(EditModeState.ModifyModify.Instance))
            {
                EditModeState.ModifyModify.Instance.UndoModifModify(idx);
            }
        }

        /// <summary>
        /// 改造列表图标按钮响应函数
        /// </summary>
        /// <param name="idx">Index.</param>
        private void OnModifyItemIconBtn(int idx)
        {
            IntVec3 unitPos = IntVec3.zero;
            if (EditMode.Instance.IsInState(EditModeState.ModifyAdd.Instance))
            {
                unitPos = DataScene2D.CurScene.AddedUnits[idx].ModifiedUnit.UnitDesc.Guid;
            }
            else if (EditMode.Instance.IsInState(EditModeState.ModifyRemove.Instance))
            {
                unitPos = DataScene2D.CurScene.RemovedUnits[idx].OrigUnit.UnitDesc.Guid;
            }
            else if (EditMode.Instance.IsInState(EditModeState.ModifyModify.Instance))
            {
                unitPos = DataScene2D.CurScene.ModifiedUnits[idx].ModifiedUnit.UnitDesc.Guid;
            }

            CameraManager.Instance.CameraCtrlEdit.SetPos(GM2DTools.TileToWorld(unitPos));
            EditModeState.ModifyAdd.Instance.ShowUnitPosEffect(unitPos);
        }

        private void OnModifySelectItemBtn(int idx)
        {
            if (_selectedItemIdx == idx)
            {
                _cachedView.SelectItems[idx].SetSelected(false);
                _selectedItemIdx = -1;
                Messenger<ushort>.Broadcast(EMessengerType.OnSelectedItemChanged, 0);
                return;
            }
            int unitId = 0;
            for (int i = 0; i < _cachedView.SelectItems.Length; i++)
            {
                if (idx == i)
                {
                    if (LocalUser.Instance.MatchUserData.UnitData.ItemList.Count <= idx ||
                        LocalUser.Instance.MatchUserData.UnitData.ItemList[i].UnitCount <= 0)
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "这种东西用完了");
                        return;
                    }
                    unitId = (int) LocalUser.Instance.MatchUserData.UnitData.ItemList[i].UnitId;
                    _cachedView.SelectItems[i].SetSelected(true);
                }
                else
                {
                    _cachedView.SelectItems[i].SetSelected(false);
                }
            }

            Messenger<ushort>.Broadcast(EMessengerType.OnSelectedItemChanged, (ushort) unitId);
        }

        /// <summary>
        /// 更新改造地块列表界面
        /// </summary>
        private void UpdateModifyItemList()
        {
            int slotNumLimit = 1;
            List<ModifyData> descs = null;
            if (EditMode.Instance.IsInState(EditModeState.ModifyAdd.Instance))
            {
                descs = DataScene2D.CurScene.AddedUnits;
                slotNumLimit = _addLimit;
            }
            else if (EditMode.Instance.IsInState(EditModeState.ModifyRemove.Instance))
            {
                descs = DataScene2D.CurScene.RemovedUnits;
                slotNumLimit = _delLimit;
            }
            else if (EditMode.Instance.IsInState(EditModeState.ModifyModify.Instance))
            {
                descs = DataScene2D.CurScene.ModifiedUnits;
                slotNumLimit = _altLimit;
            }
            if (null == descs)
                return;
            int i = 0;
            for (; i < slotNumLimit && i < descs.Count; i++)
            {
                var tableUnit = TableManager.Instance.GetUnit(descs[i].OrigUnit.UnitDesc.Id);
                if (null == tableUnit)
                {
                    LogHelper.Error("can't find tabledata of modifyItem id{0}", descs[i].OrigUnit.UnitDesc.Id);
                    return;
                }
                else
                {
                    Sprite texture;
                    if (JoyResManager.Instance.TryGetSprite(tableUnit.Icon, out texture))
                    {
                        _cachedView.ModifyItems[i].gameObject.SetActive(true);
                        _cachedView.ModifyItems[i].SetItem(texture);
                    }
                    else
                    {
                        LogHelper.Error("tableUnit {0} icon {1} invalid! tableUnit.EGeneratedType is {2}", tableUnit.Id,
                            tableUnit.Icon, tableUnit.EGeneratedType);
                        return;
                    }
                }
            }
            for (; i < _cachedView.ModifyItems.Length && i < slotNumLimit; i++)
            {
                _cachedView.ModifyItems[i].gameObject.SetActive(true);
                _cachedView.ModifyItems[i].SetEmpty();
            }
            for (; i < _cachedView.ModifyItems.Length; i++)
            {
                _cachedView.ModifyItems[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 更新添加地块列表
        /// </summary>
        private void UpdateSelectItemList()
        {
            int avilableAddUnitCnt = LocalUser.Instance.MatchUserData.UnitData.ItemList.Count;
            int i = 0;
            for (; i < _cachedView.SelectItems.Length && i < avilableAddUnitCnt; i++)
            {
                _cachedView.SelectItems[i].gameObject.SetActive(true);
                _cachedView.SelectItems[i].id = i;
                _cachedView.SelectItems[i].BtnCb = OnModifySelectItemBtn;
                Table_Unit tableUnit = TableManager.Instance.GetUnit(
                    (int) LocalUser.Instance.MatchUserData.UnitData.ItemList[i].UnitId);
                if (null == tableUnit)
                {
                    LogHelper.Error("Can't find table of unit id: {0}",
                        LocalUser.Instance.MatchUserData.UnitData.ItemList[i].UnitId);
                    _cachedView.SelectItems[i].gameObject.SetActive(false);
                    continue;
                }
                Sprite sprite;
                if (JoyResManager.Instance.TryGetSprite(tableUnit.Icon, out sprite))
                {
                    _cachedView.SelectItems[i].SetItem(sprite,
                        LocalUser.Instance.MatchUserData.UnitData.ItemList[i].UnitCount -
                        EditModeState.ModifyAdd.Instance.UsedModifyAddUnitCnt(tableUnit.Id));
                }
                else
                {
                    LogHelper.Error("Can't find icon of unit id: {0}", tableUnit.Id);
                    _cachedView.SelectItems[i].SetItem(null,
                        LocalUser.Instance.MatchUserData.UnitData.ItemList[i].UnitCount -
                        EditModeState.ModifyAdd.Instance.UsedModifyAddUnitCnt(tableUnit.Id));
                }
            }
            for (; i < _cachedView.SelectItems.Length; i++)
            {
                _cachedView.SelectItems[i].gameObject.SetActive(false);
            }
        }

        #region event 

        private void OnModifyUnitChanged()
        {
            if (!_isViewCreated)
            {
                return;
            }
            UpdateModifyItemList();
            if (EditMode.Instance.IsInState(EditModeState.ModifyAdd.Instance))
            {
                UpdateSelectItemList();
            }
        }


        private void AfterCommandChanged()
        {
            if (!_isViewCreated)
            {
                return;
            }
            bool isInModifyAdd = EditMode.Instance.IsInState(EditModeState.ModifyAdd.Instance);
            bool isInModifyRemove = EditMode.Instance.IsInState(EditModeState.ModifyRemove.Instance);
            bool isInModifyModify = EditMode.Instance.IsInState(EditModeState.ModifyModify.Instance);
            _cachedView.ModifyAddBtn.transform.localScale = isInModifyAdd ? Vector3.one * 1.2f : Vector3.one;
            _cachedView.ModifyEraseBtn.transform.localScale = isInModifyRemove ? Vector3.one * 1.2f : Vector3.one;
            _cachedView.ModifyModifyBtn.transform.localScale = isInModifyModify ? Vector3.one * 1.2f : Vector3.one;

            _cachedView.SelectAddItemPanel.SetActive(isInModifyAdd);
            if (isInModifyAdd)
            {
                UpdateSelectItemList();
            }
            // update modifyItemList
            UpdateModifyItemList();
        }

        #endregion

        #endregion
    }
}