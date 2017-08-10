﻿using SoyEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameA.Game;

namespace GameA
{
    /// <summary>
    /// 装备拼图
    /// </summary>
	[UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlPuzzleSlots : UICtrlGenericBase<UIViewPuzzleSlots>
    {
        private Dictionary<int, Table_PuzzleSlot> _slots;//拼图装备栏
        private List<PictureFull> _usingPicFull;//装备的拼图
        private List<UMCtrlPuzzleEquipLoc> _allEquipLocs;
        private PictureFull _curPicture;
        public PictureFull CurPicture { get { return _curPicture; } }
        public List<PictureFull> UsingPicFull { get { return _usingPicFull; } set { _usingPicFull = value; } }

        private void RequestData()
        {
            LocalUser.Instance.UserUsingPictureFullData.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get ValidAvatarData, {0}", code); });
        }

        private void InitUI()
        {
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _slots = TableManager.Instance.Table_PuzzleSlotDic;
            _usingPicFull = LocalUser.Instance.UserUsingPictureFullData.ItemDataList;
            //测试用，实际应用服务器数据
            _usingPicFull = new List<PictureFull>(_slots.Count);
            for (int i = 0; i < _slots.Count; i++)
            {
                _usingPicFull.Add(null);
            }

            //创建装备栏
            _allEquipLocs = new List<UMCtrlPuzzleEquipLoc>(_slots.Count);
            int index = 0;
            foreach (int key in _slots.Keys)
            {
                var unlockLv = _slots[key].UnlockLevel;
                var equipLoc = new UMCtrlPuzzleEquipLoc(key, unlockLv);
                _allEquipLocs.Add(equipLoc);
                equipLoc.Init(_cachedView.PuzzleLocsGrid);
                //显示装备的拼图
                if (_usingPicFull.Count > index && _usingPicFull[index] != null)
                    equipLoc.SetUI(_usingPicFull[index]);
                else
                    equipLoc.SetUI(null);
                index++;
            }
        }

        private void OnPuzzleEquip()
        {
            //_usingPicFull = LocalUser.Instance.UserUsingPictureFullData.ItemDataList;
            for (int i = 0; i < _allEquipLocs.Count; i++)
            {
                _allEquipLocs[i].SetUI(_usingPicFull[i]);
            }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitUI();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnPuzzleEquip, OnPuzzleEquip);
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPuzzleSlots>();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _curPicture = parameter as PictureFull;
            //槽位可点击
            for (int i = 0; i < _allEquipLocs.Count; i++)
            {
                _allEquipLocs[i].SetEquipable(true);
            }
        }

        protected override void OnClose()
        {
            //槽位点击关闭
            for (int i = 0; i < _allEquipLocs.Count; i++)
            {
                _allEquipLocs[i].SetEquipable(false);
            }
            base.OnClose();
        }

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.FrontUI;
        }
    }
}