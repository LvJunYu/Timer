using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using YIMEngine;

namespace GameA
{
    public class UPCtrlChatFriend : UPCtrlChatBase
    {
        private const float _checkOnlineInterval = 10; //检查好友是否在线的时间间隔
        private float _curTimer;
        private bool _needUpdateSort = true;
        private List<UserInfoDetail> _friendData;
        private bool _hasInited;
        private bool _isRequesting;
        public long CurFriendId;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.FriendGridDataScroller.Set(OnFriendItemRefresh, GetFriendItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.Pannels[(int) _menu].SetActiveEx(true);
            RequestFriendsData();
            RefreshFriendsView();
        }

        public override void AddChatItem(ChatInfo chatInfo)
        {
            long userId = long.Parse(chatInfo.ReceiverId);
            if (CurFriendId == userId)
            {
                _dataList.Add(chatInfo);
                RefreshView();
            }
            else
            {
                if (chatInfo.ReceiverInfoDetail != null)
                {
                    chatInfo.ReceiverInfoDetail.ChatHistory.Add(chatInfo);
                }
                else
                {
                    UserManager.Instance.GetDataOnAsync(userId,
                        userInfoDetail => { userInfoDetail.ChatHistory.Add(chatInfo); });
                }
            }
        }

        private event Action _successCallBack;
        private event Action _failCallBack;

        private void RequestFriendsData(Action successAction = null, Action failAction = null)
        {
            if (_isRequesting)
            {
                _successCallBack += successAction;
                _failCallBack += failAction;
                return;
            }
            _isRequesting = true;
            LocalUser.Instance.RelationUserList.RequestMyFriends(() =>
            {
                _friendData = LocalUser.Instance.RelationUserList.FriendList;
                _hasInited = true;
                _isRequesting = false;
                _successCallBack += successAction;
                if (_successCallBack != null)
                {
                    _successCallBack.Invoke();
                    _successCallBack = null;
                }
                if (!_isOpen)
                {
                    return;
                }
                UpdateSortAndRefreshFriendsView();
            }, code =>
            {
                _isRequesting = false;
                _failCallBack += failAction;
                if (_failCallBack != null)
                {
                    _failCallBack.Invoke();
                    _failCallBack = null;
                }
            });
        }

        public void RefreshFriendChatData(UserInfoDetail userInfoDetail)
        {
            CurFriendId = userInfoDetail.UserInfoSimple.UserId;
            _dataList = userInfoDetail.ChatHistory;
            _cachedView.FriendGridDataScroller.RefreshCurrent();
            RefreshView();
        }

        public void RequestSetToFriend(UserInfoDetail userInfoDetail)
        {
            if (!_hasInited)
            {
                RequestFriendsData(() => SetToFriend(userInfoDetail),
                    () => { SocialGUIManager.ShowPopupDialog("请求好友数据失败。"); });
            }
            else
            {
                SetToFriend(userInfoDetail);
            }
        }

        private void SetToFriend(UserInfoDetail userInfoDetail)
        {
            int inx = _friendData.IndexOf(userInfoDetail);
            if (inx >= 0)
            {
                _cachedView.FriendGridDataScroller.SetContentPosY(inx, 1);
                RefreshFriendChatData(userInfoDetail);
            }
            else
            {
                SocialGUIManager.ShowPopupDialog("没有该好友。");
            }
        }

        private void RefreshFriendsView()
        {
            if (_friendData == null)
            {
                _cachedView.FriendGridDataScroller.SetEmpty();
                return;
            }
            _cachedView.FriendGridDataScroller.SetItemCount(_friendData.Count);
            if (CurFriendId == 0 && _friendData.Count > 0)
            {
                RefreshFriendChatData(_friendData[0]);
            }
        }

        public void OnUpdate()
        {
            if (!_isOpen) return;
            if (_curTimer > _checkOnlineInterval)
            {
                CheckOnLine();
                CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitFrames(3,
                    UpdateSortAndRefreshFriendsView));
            }
            else
            {
                _curTimer += Time.deltaTime;
            }
        }

        private void CheckOnLine()
        {
            if (_friendData == null) return;
            for (int i = 0; i < _friendData.Count; i++)
            {
                var i1 = i;
                YIMManager.Instance.CheckUserOnLine(_friendData[i].UserInfoSimple.UserId.ToString(),
                    userStatus =>
                    {
                        bool isOnline = userStatus == UserStatus.STATUS_ONLINE;
                        if (isOnline != _friendData[i1].IsOnline)
                        {
                            _needUpdateSort = true;
                            _friendData[i1].IsOnline = isOnline;
                        }
                    });
            }
        }

        private void UpdateSortAndRefreshFriendsView()
        {
            if (!_needUpdateSort || _friendData == null)
            {
                return;
            }
            _friendData.Sort((p, q) =>
            {
                return (q.IsOnline ? 10000 : 0) + q.UserInfoSimple.RelationWithMe.Friendliness -
                       ((p.IsOnline ? 10000 : 0) + p.UserInfoSimple.RelationWithMe.Friendliness);
            });
            _needUpdateSort = false;
            RefreshFriendsView();
        }

        private void OnFriendItemRefresh(IDataItemRenderer item, int inx)
        {
            if (inx >= _friendData.Count)
            {
                LogHelper.Error("OnFriendItemRefresh Error Inx > count");
                return;
            }
            item.Set(_friendData[inx]);
        }

        private IDataItemRenderer GetFriendItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlChatFriendItem();
            item.SetMenu(this);
            item.Init(parent, _resScenary);
            return item;
        }

        protected override IDataItemRenderer GetTalkItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlChatTalkSmallItem();
            item.Init(parent, _resScenary);
            return item;
        }
    }
}