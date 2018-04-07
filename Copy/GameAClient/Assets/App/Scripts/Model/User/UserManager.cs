using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public class UserManager
    {
        public static readonly UserManager Instance = new UserManager();

        private readonly LRUCache<long, UserInfoDetail> _caches;

        public UserManager()
        {
            _caches = new LRUCache<long, UserInfoDetail>(ConstDefine.MaxLRUUserCount);
            _caches.Insert(LocalUser.Instance.UserGuid, LocalUser.Instance.User);
        }

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

        public List<UserInfoDetail> UpdateData(List<Msg_SC_DAT_UserInfoSimple> msgDatas)
        {
            if (msgDatas == null) return null;
            List<UserInfoDetail> users = new List<UserInfoDetail>(msgDatas.Count);
            for (int i = 0; i < msgDatas.Count; i++)
            {
                users.Add(UpdateData(msgDatas[i]));
            }

            return users;
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

        public void GetDataOnAsync(long userId, Action<UserInfoDetail> successCallback, Action failedCallback = null,
            bool isrequest = false)
        {
            UserInfoDetail userInfoDetail;

            if (TryGetData(userId, out userInfoDetail))
            {
                if (successCallback != null)
                {
                    successCallback(userInfoDetail);
                }

                if (isrequest)
                {
                    userInfoDetail.Request(userId, () =>
                    {
                        _caches.Insert(userId, userInfoDetail);
                        if (successCallback != null)
                        {
                            successCallback(userInfoDetail);
                        }
                    }, code =>
                    {
                        LogHelper.Error("UserInfoDetail Request fail, code = {0}", code);
                        if (failedCallback != null)
                        {
                            failedCallback();
                        }
                    });
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
                LogHelper.Error("UserInfoDetail Request fail, code = {0}", code);
                if (failedCallback != null)
                {
                    failedCallback();
                }
            });
        }

        public void GetDataOnAsync(List<long> userIdList, Action<List<UserInfoDetail>> successCallback,
            Action failedCallback = null)
        {
            RemoteCommands.UserInfoSimpleBatch(userIdList, msg =>
            {
                var users = UpdateData(msg.DataList);
                if (successCallback != null)
                {
                    successCallback.Invoke(users);
                }
            }, code =>
            {
                LogHelper.Error("Get UserInfoSimpleBatch fail, code = {0}", code);
                if (failedCallback != null)
                {
                    failedCallback.Invoke();
                }
            });
        }

        private bool TryGetData(long guid, out UserInfoDetail user)
        {
            if (_caches.TryGetItem(guid, out user))
            {
                return true;
            }

            return false;
        }

        public UserInfoDetail UpdateData(Msg_SC_DAT_UserInfoDetail msgData)
        {
            if (msgData == null)
            {
                return null;
            }

            UserInfoDetail userInfoDetail;
            if (_caches.TryGetItem(msgData.UserInfoSimple.UserId, out userInfoDetail))
            {
                userInfoDetail.CopyMsgData(msgData);
                return userInfoDetail;
            }

            userInfoDetail = new UserInfoDetail(msgData);
            _caches.Insert(msgData.UserInfoSimple.UserId, userInfoDetail);
            return userInfoDetail;
        }
    }
}