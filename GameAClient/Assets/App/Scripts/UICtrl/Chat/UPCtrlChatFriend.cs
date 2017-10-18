using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
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
        private bool _isRequesting;
        private bool _hasInited;
        private List<UMCtrlChatFriendItem> _umCtrlChatFriendItemCache;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.FriendGridDataScroller.Set(OnFriendItemRefresh, GetFriendItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.Pannels[(int) _menu].SetActiveEx(true);
            if (!_hasInited)
            {
                RequestFriendsData();
            }
            RefreshFriendsView();
        }

        private void RequestFriendsData()
        {
            if (_isRequesting)
            {
                return;
            }
            _isRequesting = true;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Empty);
            LocalUser.Instance.RelationUserList.RequestFriends(LocalUser.Instance.UserGuid, () =>
            {
                _friendData = LocalUser.Instance.RelationUserList.FriendList;
                _hasInited = true;
                _isRequesting = false;
                if (!_isOpen)
                {
                    return;
                }
                TempData();
                UpdateSort();
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            }, code =>
            {
                _isRequesting = false;
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            });
        }

        private void TempData()
        {
            _friendData = new List<UserInfoDetail>(10);
            for (int i = 0; i < 10; i++)
            {
                var user = new UserInfoDetail();
                user.UserInfoSimple.UserId = 1000 + i;
                user.UserInfoSimple.NickName = "好友测试数据" + i;
                user.UserInfoSimple.Sex = i % 2 == 0 ? ESex.S_Male : ESex.S_Female;
                user.UserInfoSimple.RelationWithMe.IsFriend = true;
                user.IsOnline = i % 3 == 1;
                _friendData.Add(user);
            }
        }

        public void RefreshData(UserInfoDetail userInfoDetail)
        {
            CurFriendId = userInfoDetail.UserInfoSimple.UserId;
            _dataList = userInfoDetail.ChatHistory;
            for (int i = 0; i < _umCtrlChatFriendItemCache.Count; i++)
            {
                _umCtrlChatFriendItemCache[i].RefreshSelectStatus();
            }
            RefreshView();
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
                RefreshData(_friendData[0]);
            }
        }

        public void OnUpdate()
        {
            if (!_isOpen) return;
            if (_curTimer > _checkOnlineInterval)
            {
                CheckOnLine();
                CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitFrames(3, UpdateSort));
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

        private void UpdateSort()
        {
            if (!_needUpdateSort || _friendData == null) return;
            _friendData.Sort((p, q) =>
            {
                return (q.IsOnline ? 10000 : 0) + q.UserInfoSimple.RelationWithMe.Friendliness -
                       ((p.IsOnline ? 10000 : 0) + p.UserInfoSimple.RelationWithMe.Friendliness);
            });
            RefreshFriendsView();
            _needUpdateSort = false;
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
            if (null == _umCtrlChatFriendItemCache)
            {
                _umCtrlChatFriendItemCache = new List<UMCtrlChatFriendItem>(10);
            }
            _umCtrlChatFriendItemCache.Add(item);
            return item;
        }

        protected override IDataItemRenderer GetTalkItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlChatTalkSmallItem();
            item.SetMenu(_menu);
            item.Init(parent, _resScenary);
            if (null == _umCtrlChatTalkItemCache)
            {
                _umCtrlChatTalkItemCache = new List<UMCtrlChatTalkItem>(10);
            }
            _umCtrlChatTalkItemCache.Add(item);
            return item;
        }
    }
}