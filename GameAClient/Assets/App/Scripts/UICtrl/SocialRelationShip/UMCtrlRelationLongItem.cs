using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UMCtrlRelationLongItem : UMCtrlBase<UMViewRelationLongItem>, IDataItemRenderer, IRelationShipItem
    {
        private UserInfoDetail _userInfoDetail;
        private UICtrlSocialRelationship.EMenu _belongMenu;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.TalkBtn.onClick.AddListener(OnTalkBtn);
            _cachedView.GiftBtn.onClick.AddListener(OnGiftBtn);
            _cachedView.FollowBtn.onClick.AddListener(OnFollowBtn);
            _cachedView.InfoBtn.onClick.AddListener(OnInfoBtn);
        }

        public void RefreshView()
        {
            if (null == _userInfoDetail)
            {
                return;
            }
            _cachedView.FollowBtn.SetActiveEx(_belongMenu == UICtrlSocialRelationship.EMenu.Fans);
            _cachedView.UserNickNameTxt.text = _userInfoDetail.UserInfoSimple.NickName;
            _cachedView.Male.SetActiveEx(_userInfoDetail.UserInfoSimple.Sex == ESex.S_Male);
            _cachedView.Famale.SetActiveEx(_userInfoDetail.UserInfoSimple.Sex == ESex.S_Female);
            _cachedView.AdventureLvTxt.text =
                GameATools.GetLevelString(_userInfoDetail.UserInfoSimple.LevelData.PlayerLevel);
            _cachedView.CreateLvTxt.text =
                GameATools.GetLevelString(_userInfoDetail.UserInfoSimple.LevelData.CreatorLevel);
            _cachedView.FriendlinessTxt.text = _userInfoDetail.UserInfoSimple.RelationWithMe.Friendliness.ToString();
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadImg, _userInfoDetail.UserInfoSimple.HeadImgUrl, _cachedView.DefaultTexture);
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

        private void OnGiftBtn()
        {
        }

        private void OnTalkBtn()
        {
        }
        
        private void OnInfoBtn()
        {
            if(null == _userInfoDetail) return;
            SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_userInfoDetail);
        }

        public void SetMenu(UICtrlSocialRelationship.EMenu eMenu)
        {
            _belongMenu = eMenu;
        }
    }
}