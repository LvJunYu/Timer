using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    /// <summary>
    /// 拼图装格栏位
    /// </summary>
    public class UMCtrlPuzzleEquipLoc : UMCtrlBase<UMViewPuzzleEquipLoc>
    {
        private int _slotID;
        public int _unlockLv;
        private PictureFull _curPicFull;

        public int CurPicID
        {
            get
            {
                if (_curPicFull == null)
                    return 0;
                return (int) _curPicFull.PictureId;
            }
        }

        public UMCtrlPuzzleEquipLoc(int slotID, int unlockLv)
        {
            _slotID = slotID;
            _unlockLv = unlockLv;
        }

        public void SetPic(PictureFull picture)
        {
            _curPicFull = picture;
            RefreshView();
        }

        public void RefreshView()
        {
            if (_curPicFull != null)
            {
                _cachedView.PuzzleImg.sprite = _curPicFull.PicSprite;
                _cachedView.PuzzleImg.gameObject.SetActive(true);
            }
            else
                _cachedView.PuzzleImg.gameObject.SetActive(false);
            //测试用，应取实际等级
            bool unlock = _unlockLv > 2;
            //bool unlock = _unlockLv > LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel;
            _cachedView.ActiveImage.enabled = !unlock;
            _cachedView.LockObj.SetActive(unlock);
        }

        //用于UICtrlPuzzleSlots中
        public void SetEquipable(bool value)
        {
            _cachedView.EquipBtn.gameObject.SetActive(value);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.EquipBtn.onClick.AddListener(OnEquipBtn);
            _cachedView.UnlockLvTxt.text = _unlockLv.ToString();
        }

        private void OnEquipBtn()
        {
            var picture = SocialGUIManager.Instance.GetUI<UICtrlPuzzleSlots>().CurPicture;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在装备拼图");
            RemoteCommands.ChangePictureFull(_slotID, picture.PictureId, CurPicID, res =>
            {
                if (res.ResultCode == (int) EChangePictureFullCode.CPFC_Success)
                {
                    Equip();
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    LogHelper.Debug("装备成功");
                }
                else
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    LogHelper.Debug("装备失败");
                }
            }, code =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                //测试，服务器完成后删除
                LogHelper.Debug("服务器请求失败，进行装备测试");
                Equip();
                //LogHelper.Debug("装备失败");
            });
        }

        private void Equip()
        {
            if (_curPicFull != null)
                _curPicFull.Unload();
            //替换当前
            _curPicFull = SocialGUIManager.Instance.GetUI<UICtrlPuzzleSlots>().CurPicture;
            //若已经装备，则删除之前槽位数据
            if (_curPicFull.IsUsing)
            {
                SetSlotData(_curPicFull.Slot, false);
                _curPicFull.Unload();
            }
            _curPicFull.EquipPuzzle(_slotID);
            //更新当前槽位数据
            SetSlotData(_slotID, true);
            Messenger.Broadcast(EMessengerType.OnPuzzleEquip);
            SocialGUIManager.Instance.CloseUI<UICtrlPuzzleSlots>();
        }

        private void SetSlotData(int slotID, bool value)
        {
            //不应用slotID
            //LocalUser.Instance.UserUsingPictureFullData.ItemDataList[slotID - 1] = _curEquipedPic;
            //测试用
            SocialGUIManager.Instance.GetUI<UICtrlPuzzle>().UsingPicFull[slotID - 1] = value ? _curPicFull : null;
            SocialGUIManager.Instance.GetUI<UICtrlPuzzleSlots>().UsingPicFull[slotID - 1] = value ? _curPicFull : null;
        }
    }
}