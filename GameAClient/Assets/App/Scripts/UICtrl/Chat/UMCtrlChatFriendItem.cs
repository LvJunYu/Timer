using SoyEngine;
using UnityEngine;

namespace GameA
{/*
    public class UMCtrlChatFriendItem : UMCtrlBase<UMViewChatFriendItem>, IDataItemRenderer
    {
        private static string _onlineStr = "在线";
        private static string _outlineStr = "离线";
        private UserInfoDetail _userInfoDetail;
        //private UPCtrlChatFriend _upCtrlChatFriend;

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public int Index { get; set; }

        public object Data
        {
            get { return _userInfoDetail; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.SelectBtn.onClick.AddListener(OnSelectBtn);
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
        }

        private void OnHeadBtn()
        {
            if (_userInfoDetail == null) return;
            SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_userInfoDetail);
        }

        public void Set(object data)
        {
            _userInfoDetail = data as UserInfoDetail;
            RefreshView();
        }

        private void RefreshView()
        {
            if (_userInfoDetail == null) return;
            _cachedView.NickNameTxt.text = _userInfoDetail.UserInfoSimple.NickName;
            _cachedView.OnlineInfoTxt.text = _userInfoDetail.IsOnline ? _onlineStr : _outlineStr;
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadImage,
                _userInfoDetail.UserInfoSimple.HeadImgUrl, _cachedView.HeadDefaltTexture);
            _userInfoDetail.UserInfoSimple.BlueVipData.RefreshBlueVipView(_cachedView.BlueVipDock,
                _cachedView.BlueImg, _cachedView.SuperBlueImg, _cachedView.BlueYearVipImg);
            RefreshSelectStatus();
        }
        public void RefreshSelectStatus()
        {
            _cachedView.SelectedObj.SetActive(_userInfoDetail != null &&
                                              _upCtrlChatFriend.CurFriendId == _userInfoDetail.UserInfoSimple.UserId);
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.HeadImage, _cachedView.HeadDefaltTexture);
        }

        private void OnSelectBtn()
        {
            if (_userInfoDetail == null) return;
            _upCtrlChatFriend.RefreshFriendChatData(_userInfoDetail);
        }

        public void SetMenu(UPCtrlChatFriend upCtrlChatFriend)
        {
            _upCtrlChatFriend = upCtrlChatFriend;
        }
    }
    */
}