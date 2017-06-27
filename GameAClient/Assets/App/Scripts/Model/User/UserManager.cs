/********************************************************************
** Filename : UserManager
** Author : Dong
** Date : 2015/10/20 星期二 上午 10:54:53
** Summary : UserManager
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
	public class UserManager : ICacheDataManager<UserInfoDetail>
    {
        public static readonly UserManager Instance = new UserManager();
		private readonly LRUCache<long, UserInfoDetail> _caches = new LRUCache<long, UserInfoDetail>(ConstDefine.MaxLRUUserCount);
        
		public override bool TryGetData(long guid,out UserInfoDetail user)
        {
            ReactiveLocalUser ();
            if (_caches.TryGetItem(guid, out user))
            {
                return true;
            }
			var msg = LocalCacheManager.Instance.LoadObject<Msg_SC_DAT_UserInfoDetail>(ECacheDataType.CDT_UserData, guid);
            if (msg == null)
            {
                return false;
            }
            user = OnSyncUserData(msg);
            return true;
        }

		public UserInfoDetail OnSyncUserData(Msg_SC_DAT_UserInfoDetail msg, bool save = false)
        {
            ReactiveLocalUser ();
			UserInfoDetail user;
			if (!_caches.TryGetItem(msg.UserInfoSimple.UserId, out user))
            {
                //user = PoolFactory<User>.Get();
				user = new UserInfoDetail();
				_caches.Insert(msg.UserInfoSimple.UserId, user);
            }
			user.OnSyncFromParent(msg);
            if (save)
            {
				LocalCacheManager.Instance.SaveObject(ECacheDataType.CDT_UserData, msg, msg.UserInfoSimple.UserId);
            }
            return user;
        }

//		public User OnSyncUserData(Msg_SC_DAT_UserInfoSimple msg)
//        {
//            ReactiveLocalUser ();                                                                     
//            User user;
//            if (!_caches.TryGetItem(msg.UserId, out user))
//            {
//                //user = PoolFactory<User>.Get();
//                user = new User();
//                _caches.Insert(msg.UserId, user);
//            }
//            user.OnSyncUserData(msg);
//            return user;
//        }
//		public UserInfoDetail OnSyncUserData(UserInfoDetail userDetail)
//		{
//			ReactiveLocalUser ();
//			UserInfoDetail user;
//			if (!_caches.TryGetItem(userDetail.UserInfoSimple.UserId, out user))
//			{
//				//user = PoolFactory<User>.Get();
//				user = new UserInfoDetail();
//				_caches.Insert(userDetail.UserInfoSimple.UserId, user);
//			}
//			user.OnSync(userSimple);
//			return user;
//		}


        public void BatchRequestUserInfo(List<long> userGuidList, bool force, Action onSuccess, Action onError)
        {
//            ReactiveLocalUser ();
//            List<long> needRequest = null;
//            if (force)
//            {
//                needRequest = userGuidList;
//            }
//            else
//            {
//                IsAllGuidsHasData(userGuidList, out needRequest);
//            }
//            if (needRequest.Count == 0)
//            {
//                if (onSuccess != null)
//                {
//                    onSuccess.Invoke();
//                }
//            }
//            else
//            {
//                Msg_CA_RequestUserInfoList msg = new Msg_CA_RequestUserInfoList();
//                msg.UserGuidList.AddRange(needRequest);
//                NetworkManager.AppHttpClient.SendWithCb<Msg_AC_UserInfoList>(SoyHttpApiPath.BatchGetUserInfo, msg, ret =>
//                {
//                    for (int i = 0; i < ret.DataList.Count; i++)
//                    {
//                        OnSyncUserData(ret.DataList[i]);
//                    }
//                    if (onSuccess != null)
//                    {
//                        onSuccess.Invoke();
//                    }
//                }, (intCode, str) =>
//                {
//                    if (onError != null)
//                    {
//                        onError.Invoke();
//                    }
//                });
//            }
        }

        private void ReactiveLocalUser ()
        {
//            if (LocalUser.Instance.UserLegacy != null)
//            {
//				_caches.Insert (LocalUser.Instance.UserLegacy.UserId, LocalUser.Instance.UserLegacy);
//            }
        }
    }
}