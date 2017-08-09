using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    /// <summary>
    /// 拼图装格栏位
    /// </summary>
    public partial class UMCtrlPuzzleEquipLoc : UMCtrlBase<UMViewPuzzleEquipLoc>
    {
        private int _slotID;
        public int _unlockLv;
        private PictureFull _curEquipedPic;

        public int CurPicID
        {
            get
            {
                if (_curEquipedPic == null)
                    return 0;
                return (int)_curEquipedPic.PictureId;
            }
        }

        public UMCtrlPuzzleEquipLoc(int slotID, int unlockLv)
        {
            this._slotID = slotID;
            this._unlockLv = unlockLv;
        }

        public void SetUI(PictureFull picture)
        {
            _curEquipedPic = picture;
            SetUI();
        }

        public void SetUI()
        {
            _cachedView.PuzzleItem.SetActive(_curEquipedPic != null);
            //测试用
            _cachedView.LockObj.SetActive(_unlockLv > 5);
            //_cachedView.LockObj.SetActive(_unlockLv > LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel);
        }

        public void SetEquipable(bool value)
        {
            _cachedView.EquipBtn.gameObject.SetActive(value);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.UnlockLvTxt.text = _unlockLv.ToString();
            _cachedView.EquipBtn.onClick.AddListener(OnEquipBtn);
        }

        private void OnEquipBtn()
        {
            var picture = SocialGUIManager.Instance.GetUI<UICtrlPuzzleSlots>().CurPicture;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在装备拼图");
            RemoteCommands.ChangePictureFull(_slotID, picture.PictureId, CurPicID, res =>
              {
                  if (res.ResultCode == (int)EChangePictureFullCode.CPFC_Success)
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
            if (_curEquipedPic != null)
                _curEquipedPic.Unload();
            //替换当前
            _curEquipedPic = SocialGUIManager.Instance.GetUI<UICtrlPuzzleSlots>().CurPicture;
            _curEquipedPic.EquipPuzzle();
            SetUI();

            //不应用slotID
            //LocalUser.Instance.UserUsingPictureFullData.ItemDataList[_slotID - 1] = _curEquipedPic;
            SocialGUIManager.Instance.GetUI<UICtrlPuzzle>().UsingPicFull[_slotID - 1] = _curEquipedPic;
            Messenger.Broadcast(EMessengerType.OnPuzzleEquip);
        }
    }
}
