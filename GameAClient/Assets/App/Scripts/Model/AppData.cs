/********************************************************************
** Filename : AppData
** Author : quansiwei
** Date : 16/4/18 下午7:12:54
** Summary : AppData
***********************************************************************/

using System.Collections.Generic;
using System.Collections;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using System;

namespace GameA
{
	public partial class AppData : SyncronisticData {
        #region field

        private bool _hasInit = false;
        private bool _isRequest = false;
        private int _appResVersion = 1;
//        private string _gameResRoot;
//        private string _fileUrlRoot;
//        private string _imageUrlRoot;
//        private string _newestAppVersion;
        private AdventureData _adventureData;
        private WorldData _worldData;

		private static AppData _instance = new AppData();

        #endregion field

        #region property

		public static AppData Instance {
			get { return _instance; }
		}

        public int AppResVersion
        {
            get { return _appResVersion; }
        }

//        public string GameResRoot
//        {
//            get { return _gameResRoot; }
//        }

        public bool HasInit
        {
            get { return _hasInit; }
        }

//        public string FileUrlRoot
//        {
//            get { return _fileUrlRoot; }
//        }
//
//        public string ImageUrlRoot
//        {
//            get {  return _imageUrlRoot; }
//        }
//
//        public string NewestAppVersion
//        {
//            get { return _newestAppVersion; }
//        }

        public AdventureData AdventureData
        {
            get { return _adventureData; }
        }

        public WorldData WorldData
        {
            get { return this._worldData; }
        }

        #endregion property

        #region methond

        public void Init()
        {
            _adventureData = new AdventureData();
            _worldData = new WorldData();
            Messenger.AddListener(SoyEngine.EMessengerType.OnNetworkReachableStateChange, OnNetworkReachableStateChange);
            Messenger.AddListener(SoyEngine.EMessengerType.OnAccountLoginStateChanged, OnAccountLoginStateChanged);
        }

        private void ReadCache()
        {
            //Msg_AC_AppData msg = LocalCacheManager.Instance.LoadObject<Msg_AC_AppData>(ECacheDataType.CDT_AppData);
            //if (msg != null)
            //{
            //    _appResVersion = msg.AppResVersion;
            //    SoyPath.Instance.FileUrlRoot = msg.FileUrlRoot;
            //    SoyPath.Instance.ImageUrlRoot = msg.ImageUrlRoot;
            //    _gameResRoot = msg.GameResRoot;
            //}
        }


        private void OnNetworkReachableStateChange()
        {
            Request();
        }

        protected void Request()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable
                || _hasInit || _isRequest)
            {
                return;
            }
            //Msg_CA_RequestAppData msg = new Msg_CA_RequestAppData();
            //msg.MatrixLastUpdateTime = MatrixManager.Instance.AllMatrixLastUpdateTime;
            //if (LocalUser.Instance.Account.HasLogin)
            //{
            //    msg.UserInfoUpdateTime = LocalUser.Instance.User.UpdateTime;
            //}
            //msg.DailyMissionVersion = _dailyMission.DailyMissionVersion;
            //msg.AppVersion = GlobalVar.Instance.AppVersion;
            //if (Application.platform == RuntimePlatform.IPhonePlayer)
            //{
            //    msg.DevicePlatform = EPhoneType.PhT_iPhone;
            //}
            //else if (Application.platform == RuntimePlatform.Android)
            //{
            //    msg.DevicePlatform = EPhoneType.PhT_Android;
            //}
            //else
            //{
            //    msg.DevicePlatform = EPhoneType.PhT_None;
            //}
            //_isRequest = true;
            //NetworkManager.AppHttpClient.SendWithCb<Msg_AC_AppData>(SoyHttpApiPath.AppData, msg, ret =>
            //{
            //    _hasInit = true;
            //    _isRequest = true;
            //    if (ret.AllMatrix != null)
            //    {
            //        MatrixManager.Instance.OnSyncAllMatrixs(ret.AllMatrix, true);
            //    }
            //    if (ret.UserInfo != null)
            //    {
            //        UserManager.Instance.OnSyncUserData(ret.UserInfo, true);
            //        LocalUser.Instance.User.OnSyncUserData(ret.UserInfo);
            //    }
            //    if (ret.Notification != null)
            //    {
            //        _notificationData.OnSyncNotification(ret.Notification);
            //    }
            //    if (ret.DailyMissionList != null)
            //    {
            //        _dailyMission.OnSyncDailyMissionList(ret.DailyMissionList, true);
            //    }
            //    if (ret.Reward != null)
            //    {
            //        Messenger<Msg_AC_Reward>.Broadcast(EMessengerType.OnReceiveReward, ret.Reward);
            //    }

            //    _appResVersion = ret.AppResVersion;
            //    _gameResRoot = ret.GameResRoot;
            //    _newestAppVersion = ret.NewestAppVersion;
            //    SoyPath.Instance.FileUrlRoot = ret.FileUrlRoot;
            //    SoyPath.Instance.ImageUrlRoot = ret.ImageUrlRoot;
            //    if (ret.ServerTime != 0)
            //    {
            //        DateTimeUtil.SyncServerTime(ret.ServerTime);
            //    }
            //    ret.AllMatrix = null;
            //    ret.UserInfo = null;
            //    ret.Notification = null;
            //    ret.DailyMissionList = null;
            //    ret.Reward = null;
            //    ret.NewestAppVersion = null;
            //    LocalCacheManager.Instance.SaveObject(ECacheDataType.CDT_AppData, ret);
            //    Messenger.Broadcast(EMessengerType.OnAppDataChanged);
            //}, (errCode, errMsg) => {
                
            //});
        }

        public void LoadAppData(Action successCallback, Action<ENetResultCode> failedCallback)
        {
//            Request(0, successCallback, failedCallback);
			Request(0, ()=>{
				SoyPath.Instance.FileUrlRoot = _fileUrlRoot;
				SoyPath.Instance.ImageUrlRoot = _imageUrlRoot;
				if (_serverTime != 0)
				{
					DateTimeUtil.SyncServerTime(_serverTime);
				}
				if(successCallback != null)
				{
					successCallback.Invoke();
				}
			}, failedCallback);
        }

//        public void LoadAppData(Action successCallback, Action<ENetResultCode> failedCallback)
//        {
//            Msg_CA_RequestAppData msg = new Msg_CA_RequestAppData();
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_AppData>(SoyHttpApiPath.AppData, msg, ret =>
//            {
//                _hasInit = true;
//                _gameResRoot = ret.GameResRoot;
//                _fileUrlRoot = ret.FileUrlRoot;
//                _imageUrlRoot = ret.ImageUrlRoot;
//                _newestAppVersion = ret.NewestAppVersion;
//                SoyPath.Instance.FileUrlRoot = _fileUrlRoot;
//                SoyPath.Instance.ImageUrlRoot = _imageUrlRoot;
//                if (successCallback != null)
//                {
//                    successCallback.Invoke();
//                }
//            }, (errorCode, errorMsg) => {
//                LogHelper.Error("LoadInitData error");
//                if (failedCallback != null)
//                {
//                    failedCallback.Invoke(errorCode);
//                }
//            });
//        }

        private void OnAccountLoginStateChanged()
        {
            
        }
        #endregion method
    }
}