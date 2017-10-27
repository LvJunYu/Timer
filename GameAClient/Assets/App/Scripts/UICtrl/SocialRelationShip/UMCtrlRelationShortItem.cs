using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UMCtrlRelationShortItem : UMCtrlBase<UMViewRelationShortItem>, IDataItemRenderer
    {
        private UserInfoDetail _userInfoDetail;
        private UICtrlSocialRelationship.EMenu _belongMenu;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.InfoBtn.onClick.AddListener(OnInfoBtn);
            if (_belongMenu == UICtrlSocialRelationship.EMenu.AddNew)
            {
                _cachedView.BtnTxt.text = "关注";
                _cachedView.Btn.onClick.AddListener(OnFollowBtn);
            }
            else if (_belongMenu == UICtrlSocialRelationship.EMenu.Block)
            {
                _cachedView.BtnTxt.text = "移除";
                _cachedView.Btn.onClick.AddListener(OnRemoveBlockBtn);
            }
        }

        public void RefreshView()
        {
            if (null == _userInfoDetail)
            {
                return;
            }
            _cachedView.UserNickNameTxt.text = _userInfoDetail.UserInfoSimple.NickName;
            _cachedView.Male.SetActiveEx(_userInfoDetail.UserInfoSimple.Sex == ESex.S_Male);
            _cachedView.Famale.SetActiveEx(_userInfoDetail.UserInfoSimple.Sex == ESex.S_Female);
            _cachedView.AdventureLvTxt.text =
                GameATools.GetLevelString(_userInfoDetail.UserInfoSimple.LevelData.PlayerLevel);
            _cachedView.CreateLvTxt.text =
                GameATools.GetLevelString(_userInfoDetail.UserInfoSimple.LevelData.CreatorLevel);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadImg,
                _userInfoDetail.UserInfoSimple.HeadImgUrl, _cachedView.DefaultTexture);
            RefreshBtn();
        }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public int Index { get; set; }

        public object Data
        {
            get { return _userInfoDetail; }
        }

        public void Set(object data)
        {
            if (data != null)
            {
                _userInfoDetail = data as UserInfoDetail;
            }
            RefreshView();
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.HeadImg, _cachedView.DefaultTexture);
        }

        private void OnFollowBtn()
        {
            if (_userInfoDetail == null)
            {
                LogHelper.Error("follow user, but _userInfoDetail == null");
                return;
            }
            LocalUser.Instance.RelationUserList.RequestFollowUser(_userInfoDetail);
        }

        private void RefreshBtn()
        {
            if (_belongMenu == UICtrlSocialRelationship.EMenu.AddNew && _userInfoDetail != null)
            {
                _cachedView.Btn.SetActiveEx(!_userInfoDetail.UserInfoSimple.RelationWithMe.FollowedByMe);
                _cachedView.Btn_Disable.SetActiveEx(_userInfoDetail.UserInfoSimple.RelationWithMe.FollowedByMe);
            }
        }

        private void OnRemoveBlockBtn()
        {
            if (_userInfoDetail == null)
            {
                LogHelper.Error("remove follow user, but _userInfoDetail == null");
                return;
            }
            LocalUser.Instance.RelationUserList.RequestRemoveBlockUser(_userInfoDetail);
        }
        
        private void OnInfoBtn()
        {
            if(null==_userInfoDetail) return;
            SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_userInfoDetail);
        }

        public void SetMenu(UICtrlSocialRelationship.EMenu eMenu)
        {
            _belongMenu = eMenu;
        }
    }
}