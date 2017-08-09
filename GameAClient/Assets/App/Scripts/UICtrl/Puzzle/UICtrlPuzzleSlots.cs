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
        private int _userLv;
        private Dictionary<int, Table_PuzzleSlot> _slots;//拼图装备栏
        private List<PictureFull> _equipedPuzzles;//装备的拼图
        private PictureFull _curPicture;

        private void InitData()
        {
            _userLv = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel;
            _equipedPuzzles = LocalUser.Instance.UserUsingPictureFullData.ItemDataList;
        }

        private void RequestData()
        {
            LocalUser.Instance.UserUsingPictureFullData.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get ValidAvatarData, {0}", code); });
        }

        private void InitUI()
        {
            //创建装备栏
            int index = 0;
            foreach (int key in _slots.Keys)
            {
                var unlockLv = _slots[key].UnlockLevel;
                var equipLoc = new UMCtrlPuzzleEquipLoc(unlockLv, unlockLv > _userLv);
                equipLoc.Init(_cachedView.PuzzleLocsGrid);
                //显示装备的拼图
                if (_equipedPuzzles.Count > index && _equipedPuzzles[index] != null)
                    equipLoc.SetData(_equipedPuzzles[index]);
                else
                    equipLoc.SetData(null);
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
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _slots = TableManager.Instance.Table_PuzzleSlotDic;
            InitData();
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
        }
    }
}