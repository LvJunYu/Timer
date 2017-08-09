using SoyEngine;
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
            _allEquipLocs = new List<UMCtrlPuzzleEquipLoc>(_slots.Count);
            //创建装备栏
            int index = 0;
            foreach (int key in _slots.Keys)
            {
                var unlockLv = _slots[key].UnlockLevel;
                var equipLoc = new UMCtrlPuzzleEquipLoc(key,unlockLv);
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

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.FrontUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitUI();
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
    }
}