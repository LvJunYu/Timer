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
        private List<UserInfoDetail> _friendList;
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

        private void RequestFriendsData()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Empty);
            LocalUser.Instance.RelationUserList.RequestMyFriends(() =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                _friendList = LocalUser.Instance.RelationUserList.FriendList;
                if (_isOpen)
                {
                    SortFriendList();
                    RefreshFriendsView();
                }
            }, code => { SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this); });
        }

        public void RefreshFriendChatData(UserInfoDetail userInfoDetail)
        {
            CurFriendId = userInfoDetail.UserInfoSimple.UserId;
            _dataList = userInfoDetail.ChatHistory;
            _cachedView.FriendGridDataScroller.RefreshCurrent();
            RefreshView();
        }

        public void ChatToFriend(int index, List<UserInfoDetail> _friends)
        {
            _friendList = _friends;
            var friend = _friends[index];
            SortFriendList(true);
            RefreshFriendsView();
            int newIndex = _friendList.IndexOf(friend);
            if (newIndex >= 0)
            {
                //设置Content位置到当前选中好友位置
                _cachedView.FriendGridDataScroller.SetContentPosY(newIndex, 1);
                RefreshFriendChatData(friend);
            }
            else
            {
                LogHelper.Error("_friendData.IndexOf(userInfoDetail) < 0 when ChatToFriend");
            }
        }

        private void RefreshFriendsView()
        {
            if (_friendList == null)
            {
                _cachedView.FriendGridDataScroller.SetEmpty();
                return;
            }
            _cachedView.FriendGridDataScroller.SetItemCount(_friendList.Count);
            if (CurFriendId == 0 && _friendList.Count > 0)
            {
                RefreshFriendChatData(_friendList[0]);
            }
        }

        public void OnUpdate()
        {
            if (!_isOpen) return;
            if (_curTimer > _checkOnlineInterval)
            {
                CheckOnLine();
                CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitFrames(3, () =>
                {
                    SortFriendList();
                    RefreshFriendsView();
                }));
            }
            else
            {
                _curTimer += Time.deltaTime;
            }
        }

        private void CheckOnLine()
        {
            if (_friendList == null) return;
            for (int i = 0; i < _friendList.Count; i++)
            {
                var i1 = i;
                YIMManager.Instance.CheckUserOnLine(_friendList[i].UserInfoSimple.UserId.ToString(),
                    userStatus =>
                    {
                        bool isOnline = userStatus == UserStatus.STATUS_ONLINE;
                        if (isOnline != _friendList[i1].IsOnline)
                        {
                            _needUpdateSort = true;
                            _friendList[i1].IsOnline = isOnline;
                        }
                    });
            }
        }

        private void SortFriendList(bool forceUpdate = false)
        {
            if (!forceUpdate && !_needUpdateSort || _friendList == null)
            {
                return;
            }
            _friendList.Sort((p, q) =>
            {
                return (q.IsOnline ? 10000 : 0) + q.UserInfoSimple.RelationWithMe.Friendliness -
                       ((p.IsOnline ? 10000 : 0) + p.UserInfoSimple.RelationWithMe.Friendliness);
            });
            _needUpdateSort = false;
        }

        private void OnFriendItemRefresh(IDataItemRenderer item, int inx)
        {
            if (inx >= _friendList.Count)
            {
                LogHelper.Error("OnFriendItemRefresh Error Inx > count");
                return;
            }
            item.Set(_friendList[inx]);
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