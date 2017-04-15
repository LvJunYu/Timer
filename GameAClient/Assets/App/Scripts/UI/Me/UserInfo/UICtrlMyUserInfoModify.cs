  /********************************************************************
  ** Filename : UICtrlMyUserInfoModify.cs
  ** Author : quan
  ** Date : 2016/4/13 14:32
  ** Summary : UICtrlMyUserInfoModify.cs
  ***********************************************************************/



using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlMyUserInfoModify : UISocialContentCtrlBase<UIViewMyUserInfoModify>, IUIWithTitle
    {
        #region 常量与字段
        private const int MinHeadImageSize = 256;
        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.HeadImageBtn.onClick.AddListener(OnHeadImageClick);
            _cachedView.NickNameBtn.onClick.AddListener(OnNickNameClick);
            _cachedView.ProfileBtn.onClick.AddListener(OnProfileClick);
            _cachedView.SexBtn.onClick.AddListener(OnSexClick);
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            RefreshView();
        }

        protected override void OnDestroy()
        {
        }


        private void RefreshView()
        {
            User user = LocalUser.Instance.User;
            if(user == null)
            {
                LogHelper.Error("UICtrlMyUserInfoModify LocalUser.user is null");
                return;
            }
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadImage, user.HeadImgUrl);
            DictionaryTools.SetContentText(_cachedView.NickNameText, user.NickName);
            DictionaryTools.SetContentText(_cachedView.SexText, EnumStringDefine.GetSexString(user.Sex));
            DictionaryTools.SetContentText(_cachedView.ProfileText, user.Profile);
        }

        #endregion

        #region 事件处理

        private void OnHeadImageClick()
        {
            if(Application.isMobilePlatform)
            {
                JoyNativeTool.OnImagePicked += OnImagePicked;
                JoyNativeTool.Instance.PickImage();
            }
            else if(Application.isEditor)
            {
                Texture2D headImgTexture = new Texture2D(1024, 768, TextureFormat.RGB24, false);
                OnImagePicked(headImgTexture, "test");
            }
        }

        private void OnImagePicked(Texture2D headImgTexture, string name)
        {
            if(Application.isMobilePlatform)
            {
                JoyNativeTool.OnImagePicked -= OnImagePicked;
            }
            if(headImgTexture == null || string.IsNullOrEmpty(name))
            {
                return;
            }
            if(headImgTexture.width < MinHeadImageSize || headImgTexture.height < MinHeadImageSize)
            {
                CommonTools.ShowPopupDialog("选取的图片分辨率过小，图片宽高不小于" + MinHeadImageSize);
                return;
            }
            Tuple<Texture2D, Action<byte[]>> tuple = new Tuple<Texture2D, Action<byte[]>>(headImgTexture, ModifyHeadImgCallback);

            SocialGUIManager.Instance.OpenUI<UICtrlPictureCrop>(tuple);
        }

        private void ModifyHeadImgCallback(byte[] bytes)
        {
            User user = LocalUser.Instance.User;
            if(bytes == null || bytes.Length == 0)
            {
                return;
            }
            const string localHeadImageName = "localHeadImg.jpg";
            ImageResourceManager.Instance.SaveOrUpdateImageData(localHeadImageName, bytes);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadImage, localHeadImageName);

//            Msg_CA_UpdateUserInfo msg = new Msg_CA_UpdateUserInfo();
//            msg.UserInfo = new Msg_UserInfoDetail();
//            SendUpdateUserInfo(msg, (ret)=>{
//                ImageResourceManager.Instance.DeleteImageCache(user.HeadImgUrl);
//                ImageResourceManager.Instance.DeleteImageCache(localHeadImageName);
//                ImageResourceManager.Instance.SaveOrUpdateImageData(ret.HeadImgUrl, bytes);
//                ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadImage, ret.HeadImgUrl);
//            }, null, bytes);
        }

        private void OnNickNameClick()
        {
            Tuple<string, Action<string>> tuple = new Tuple<string, Action<string>>(LocalUser.Instance.User.NickName, OnNickNameCallback);
            SocialGUIManager.Instance.OpenUI<UICtrlNickNameEditor>(tuple);
        }

        private void OnNickNameCallback(string newNickName)
        {
            User user = LocalUser.Instance.User;
            if(newNickName == user.NickName)
            {
                return;
            }

            DictionaryTools.SetContentText(_cachedView.NickNameText, newNickName);

//            Msg_CA_UpdateUserInfo msg = new Msg_CA_UpdateUserInfo();
//            msg.UserInfo = new Msg_UserInfoDetail();
//            msg.UserInfo.UserNickName = newNickName;
//            SendUpdateUserInfo(msg, null, ()=>{
//                DictionaryTools.SetContentText(_cachedView.NickNameText, user.NickName);
//            });
        }

        private void OnProfileClick()
        {
            Tuple<string, Action<string>> tuple = new Tuple<string, Action<string>>(LocalUser.Instance.User.Profile, OnProfileCallback);
            SocialGUIManager.Instance.OpenUI<UICtrlProfileEditor>(tuple);
        }

        private void OnProfileCallback(string newProfile)
        {
            User user = LocalUser.Instance.User;
            if(newProfile == user.Profile)
            {
                return;
            }

            DictionaryTools.SetContentText(_cachedView.ProfileText, newProfile);

//            Msg_CA_UpdateUserInfo msg = new Msg_CA_UpdateUserInfo();
//            msg.UserInfo = new Msg_UserInfoDetail();
//            msg.UserInfo.Profile = newProfile;
//            SendUpdateUserInfo(msg, null, ()=>{
//                DictionaryTools.SetContentText(_cachedView.ProfileText, user.Profile);
//            });
        }

        private void OnSexClick()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlSexEditor>( new Action<ESex>(OnSexCallback));
        }

        private void OnSexCallback(ESex sex)
        {
            User user = LocalUser.Instance.User;
            if(sex == user.Sex)
            {
                return;
            }

            DictionaryTools.SetContentText(_cachedView.SexText, EnumStringDefine.GetSexString(sex));

//            Msg_CA_UpdateUserInfo msg = new Msg_CA_UpdateUserInfo();
//            msg.UserInfo = new Msg_UserInfoDetail();
//            msg.UserInfo.Sex = sex;
//            SendUpdateUserInfo(msg);
        }

//        private void SendUpdateUserInfo(Msg_CA_UpdateUserInfo msg, Action<Msg_UserInfoDetail> successCallback = null, Action failedCallback = null, byte[] headImgBytes = null)
//        {
//            WWWForm form = null;
//            if(headImgBytes != null)
//            {
//                form = new WWWForm();
//                form.AddBinaryData("headFile", headImgBytes);
//            }
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_UpdateUserInfoRet>(SoyHttpApiPath.UpdateUserInfo, msg, ret=>{
//                if(ret.ResultCode != EUpdateUserInfoResult.UUIR_Success)
//                {
//                    if(ret.ResultCode == EUpdateUserInfoResult.UUIR_NickNameExsit)
//                    {
//                        CommonTools.ShowPopupDialog("昵称已经存在");
//                    }
//                    if(failedCallback != null)
//                    {
//                        failedCallback.Invoke();
//                    }
//                    return;
//                }
//                if(successCallback != null)
//                {
//                    successCallback.Invoke(ret.UserInfo);
//                }
//                UserManager.Instance.OnSyncUserData(ret.UserInfo, true);
//                LocalUser.Instance.User.OnSyncUserData(ret.UserInfo);
//            }, (code, msgStr)=>{
//                if(failedCallback != null)
//                {
//                    failedCallback.Invoke();
//                }
//            }, form);
//        }
        #endregion 事件处理

        #region 接口
        public object GetTitle()
        {
            return "个人信息";
        }
        #endregion
    }
}
