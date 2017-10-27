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
using UnityEngine;

namespace GameA
{
    public class UserManager
    {
        public static readonly UserManager Instance = new UserManager();

        private readonly LRUCache<long, UserInfoDetail> _caches =
            new LRUCache<long, UserInfoDetail>(ConstDefine.MaxLRUUserCount);

        public UserInfoDetail UpdateData(UserInfoDetail newData)
        {
            UserInfoDetail userInfoDetail;
            if (_caches.TryGetItem(newData.UserInfoSimple.UserId, out userInfoDetail))
            {
                userInfoDetail.DeepCopy(newData);
                return userInfoDetail;
            }
            _caches.Insert(newData.UserInfoSimple.UserId, newData);
            return newData;
        }

        public UserInfoDetail UpdateData(Msg_SC_DAT_UserInfoSimple msgData)
        {
            if (msgData == null)
            {
                return null;
            }
            UserInfoDetail userInfoDetail;
            if (_caches.TryGetItem(msgData.UserId, out userInfoDetail))
            {
                userInfoDetail.UserInfoSimple.CopyMsgData(msgData);
                return userInfoDetail;
            }
            userInfoDetail = new UserInfoDetail();
            userInfoDetail.UserInfoSimple = new UserInfoSimple(msgData);
            _caches.Insert(msgData.UserId, userInfoDetail);
            return userInfoDetail;
        }

        public UserInfoDetail UpdateData(UserInfoSimple newSimpleData)
        {
            UserInfoDetail userInfoDetail;
            if (_caches.TryGetItem(newSimpleData.UserId, out userInfoDetail))
            {
                userInfoDetail.UserInfoSimple.DeepCopy(newSimpleData);
                return userInfoDetail;
            }
            userInfoDetail = new UserInfoDetail();
            userInfoDetail.UserInfoSimple = newSimpleData;
            _caches.Insert(newSimpleData.UserId, userInfoDetail);
            return userInfoDetail;
        }

        public void GetDataOnAsync(long userId, Action<UserInfoDetail> successCallback, Action failedCallback = null)
        {
            UserInfoDetail userInfoDetail;
            if (TryGetData(userId, out userInfoDetail))
            {
                if (successCallback != null)
                {
                    successCallback(userInfoDetail);
                }
                return;
            }
            userInfoDetail = new UserInfoDetail();
            userInfoDetail.Request(userId, () =>
            {
                _caches.Insert(userId, userInfoDetail);
                if (successCallback != null)
                {
                    successCallback(userInfoDetail);
                }
            }, code =>
            {
                if (failedCallback != null)
                {
                    failedCallback();
                }
            });
        }

        public bool TryGetData(long guid, out UserInfoDetail user)
        {
            if (_caches.TryGetItem(guid, out user))
            {
                return true;
            }
            return false;
        }

        public UserInfoDetail OnSyncUserData(Msg_SC_DAT_UserInfoDetail msg, bool save = false)
        {
            ReactiveLocalUser();
            UserInfoDetail user;
            if (!_caches.TryGetItem(msg.UserInfoSimple.UserId, out user))
            {
                //user = PoolFactory<User>.Get();
                user = new UserInfoDetail();
                _caches.Insert(msg.UserInfoSimple.UserId, user);
            }
            //user.OnSyncFromParent(msg);
            if (save)
            {
                LocalCacheManager.Instance.SaveObject(ECacheDataType.CDT_UserData, msg, msg.UserInfoSimple.UserId);
            }
            return user;
        }

        public UserInfoDetail OnSyncUserData(Msg_SC_DAT_UserInfoSimple msg, bool save = false)
        {
            ReactiveLocalUser();
            UserInfoSimple userSimple = new UserInfoSimple();
            UserInfoDetail user;
            if (!_caches.TryGetItem(msg.UserId, out user))
            {
                user = new UserInfoDetail();
                _caches.Insert(msg.UserId, user);
            }
            userSimple.OnSyncFromParent(msg);
            user = new UserInfoDetail(userSimple);
            if (save)
            {
                LocalCacheManager.Instance.SaveObject(ECacheDataType.CDT_UserData, msg, msg.UserId);
            }
            return user;
        }

        //public List<UserInfoDetail> GetFollowList()
        //{
        //    List<UserInfoDetail> FollowList = new List<UserInfoDetail>();

        //    for (int i = 0; i < _caches.Size(); i++)
        //    {
        //        var item = _caches.ElementAt(i).Value;
        //    }
        //    return FollowList;
        //}

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

        private void ReactiveLocalUser()
        {
//            if (LocalUser.Instance.UserLegacy != null)
//            {
//				_caches.Insert (LocalUser.Instance.UserLegacy.UserId, LocalUser.Instance.UserLegacy);
//            }
        }
    }
}