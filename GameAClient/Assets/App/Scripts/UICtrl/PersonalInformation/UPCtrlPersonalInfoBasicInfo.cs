using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class UPCtrlPersonalInfoBasicInfo : UPCtrlPersonalInfoBase
    {
        private static string _expFormat = "{0}/{1}";
        private static string _lvFormat = "Lv{0}";
        private UserInfoDetail _userInfoDetail;
        private bool _isEditing;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.EditBtn.onClick.AddListener(OnEditBtn);
            _cachedView.SaveEditBtn.onClick.AddListener(OnSaveEditBtn);
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
        }

        public override void Open()
        {
            base.Open();
            _userInfoDetail = _mainCtrl.UserInfoDetail;
            _isEditing = false;
            RefreshView();
        }

        public override void RefreshView()
        {
            if (!_isOpen) return;
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadImg,
                _userInfoDetail.UserInfoSimple.HeadImgUrl, _cachedView.HeadDefaltTexture);
            _cachedView.Name.text = _userInfoDetail.UserInfoSimple.NickName;
            _cachedView.Desc.text = _userInfoDetail.Profile;
            _cachedView.FollowNum.text = _userInfoDetail.RelationStatistic.FollowCount.ToString();
            _cachedView.FansNum.text = _userInfoDetail.RelationStatistic.FollowerCount.ToString();
            _cachedView.MaleObj.SetActive(_userInfoDetail.UserInfoSimple.Sex == ESex.S_Male);
            _cachedView.FamaleObj.SetActive(_userInfoDetail.UserInfoSimple.Sex == ESex.S_Female);
            int advLv = _userInfoDetail.UserInfoSimple.LevelData.PlayerLevel;
            int createLv = _userInfoDetail.UserInfoSimple.LevelData.CreatorLevel;
            long curAdvExp = _userInfoDetail.UserInfoSimple.LevelData.PlayerExp;
            long curCreateExp = _userInfoDetail.UserInfoSimple.LevelData.CreatorExp;
            long needAdvExp = TableManager.Instance.GetPlayerLvToExp(advLv + 1) != null
                ? TableManager.Instance.GetPlayerLvToExp(advLv + 1).AdvExp
                : curAdvExp;
            long needCreateExp = TableManager.Instance.GetPlayerLvToExp(createLv + 1) != null
                ? TableManager.Instance.GetPlayerLvToExp(createLv + 1).MakerExp
                : curCreateExp;
            _cachedView.AdvLv.text = string.Format(_lvFormat, advLv);
            _cachedView.CreateLv.text = string.Format(_lvFormat, createLv);
            _cachedView.AdvExp.text = string.Format(_expFormat, curAdvExp, needAdvExp);
            _cachedView.CreateExp.text = string.Format(_expFormat, curCreateExp, needCreateExp);
            _cachedView.AdvExpBar.fillAmount = curAdvExp / (float) needAdvExp;
            _cachedView.CreateExpBar.fillAmount = curCreateExp / (float) needCreateExp;
            _cachedView.EditBtn.SetActiveEx(_mainCtrl.IsMyself);
            ChangeEditStatus(_isEditing);
            _cachedView.EditObj.SetActiveEx(_isEditing);
            LocalUser.Instance.User.UserInfoSimple.BlueVipData.RefreshBlueVipView(_cachedView.BlueVipDock,
                _cachedView.BlueImg, _cachedView.SuperBlueImg, _cachedView.BlueYearVipImg);
        }

        private void OnHeadBtn()
        {
            if (_mainCtrl.IsMyself)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlHeadPhotoChoose>(_userInfoDetail.UserInfoSimple.HeadImgUrl);
            }
        }

        private void OnEditBtn()
        {
            ChangeEditStatus(true);
            _cachedView.NormalObj.SetActiveEx(!_isEditing);
            _cachedView.EditObj.SetActiveEx(_isEditing);
            _cachedView.NameInputField.text = _userInfoDetail.UserInfoSimple.NickName;
            _cachedView.DescInputField.text = _userInfoDetail.Profile;
            _cachedView.MaleToggle.isOn = _userInfoDetail.UserInfoSimple.Sex == ESex.S_Male;
            _cachedView.FamaleToggle.isOn = _userInfoDetail.UserInfoSimple.Sex == ESex.S_Female;
        }

        private void OnSaveEditBtn()
        {
            bool needUpdateInfo = false;
            Msg_SC_DAT_UserInfoDetail userDataChanged = new Msg_SC_DAT_UserInfoDetail();
            userDataChanged.UserInfoSimple = new Msg_SC_DAT_UserInfoSimple();
            ESex sex = ESex.S_None;
            if (_cachedView.MaleToggle.isOn)
            {
                sex = ESex.S_Male;
            }
            if (_cachedView.FamaleToggle.isOn)
            {
                sex = ESex.S_Female;
            }
            if (sex != _userInfoDetail.UserInfoSimple.Sex)
            {
                userDataChanged.UserInfoSimple.Sex = sex;
                needUpdateInfo = true;
            }
            if (_cachedView.NameInputField.text != _userInfoDetail.UserInfoSimple.NickName)
            {
                if (CheckTools.CheckNickName(_cachedView.NameInputField.text) ==
                    CheckTools.ECheckNickNameResult.Success)
                {
                    userDataChanged.UserInfoSimple.NickName = _cachedView.NameInputField.text;
                    needUpdateInfo = true;
                }
                else
                {
                    ChangeEditStatus(false);
                    SocialGUIManager.ShowPopupDialog("昵称格式错误");
                    return;
                }
            }
            if (_cachedView.DescInputField.text != _userInfoDetail.Profile)
            {
                if (CheckTools.CheckProfile(_cachedView.DescInputField.text) == CheckTools.ECheckProfileResult.Success)
                {
                    if (string.IsNullOrEmpty(_cachedView.DescInputField.text))
                    {
                        userDataChanged.Profile = string.Empty;
                    }
                    else
                    {
                        userDataChanged.Profile = _cachedView.DescInputField.text;
                    }
                    needUpdateInfo = true;
                }
                else
                {
                    ChangeEditStatus(false);
                    SocialGUIManager.ShowPopupDialog("签名格式错误");
                    return;
                }
            }
            _isEditing = false;
            if (needUpdateInfo)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在保存信息");
                RemoteCommands.UpdateUserInfo(userDataChanged, ret =>
                {
                    if (ret.ResultCode == (int) EUpdateUserInfoCode.UUC_Success)
                    {
                        LocalUser.Instance.User.OnSync(ret.UserInfo);
                        _isEditing = false;
                        Messenger.Broadcast(EMessengerType.OnUserInfoChanged);
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    }
                    else
                    {
                        ChangeEditStatus(false);
                        SocialGUIManager.ShowPopupDialog("更新信息失败");
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    }
                }, code =>
                {
                    ChangeEditStatus(false);
                    SocialGUIManager.ShowPopupDialog("更新信息失败");
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                });
            }
            else
            {
                ChangeEditStatus(false);
            }
        }

        private void ChangeEditStatus(bool isEditing)
        {
            _isEditing = isEditing;
            _cachedView.NormalObj.SetActiveEx(!isEditing);
            _cachedView.EditObj.SetActiveEx(isEditing);
        }
    }
}