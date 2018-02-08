using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlInviteFriend : UICtrlAnimationBase<UIViewInviteFriend>
    {
        private const int MaxInviteNum = 20;
        private List<long> _inviteList = new List<long>();
        private List<UserInfoDetail> _dataList;
        private EInviteType _inviteType;

        private List<CardDataRendererWrapper<UserInfoDetail>> _contentList =
            new List<CardDataRendererWrapper<UserInfoDetail>>();

        public EInviteType InviteType
        {
            get { return _inviteType; }
            set { _inviteType = value; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.AddFriendsBtn.onClick.AddListener(OnAddFriendsBtn);
            _cachedView.OKBtn.onClick.AddListener(OnOKBtn);
            _cachedView.CancelBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.GridDataScroller.Set(OnItemRefresh, GetItemRenderer);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _dataList = null;
            _cachedView.FriendPannel.SetActive(true);
            _cachedView.EmptyPannel.SetActive(false);
            RequestData();
            RefreshView();
        }

        protected override void OnClose()
        {
            _dataList = null;
            _contentList.Clear();
            _inviteList.Clear();
            base.OnClose();
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.FrontUI2;
        }

        private void RequestData()
        {
            LocalUser.Instance.RelationUserList.RequestMyFriends(() =>
            {
                var friends = LocalUser.Instance.RelationUserList.FriendList;
                if (friends.Count == 0)
                {
//                    _cachedView.FriendPannel.SetActive(false);
//                    _cachedView.EmptyPannel.SetActive(true);
                    return;
                }
                var list = new List<long>();
                for (int i = 0; i < friends.Count; i++)
                {
                    list.Add(friends[i].UserInfoSimple.UserId);
                }
                RoomManager.Instance.SendQueryUserList(list); 
            }, code =>
            {
                LogHelper.Error("RelationUserList RequestData fail, code = {0}", code);
                SocialGUIManager.ShowPopupDialog("请求数据失败。");
            });
        }

        private void RefreshView()
        {
            if (_dataList == null)
            {
                _cachedView.GridDataScroller.SetEmpty();
                return;
            }

            _contentList.Clear();
            _contentList.Capacity = Mathf.Max(_contentList.Capacity, _dataList.Count);
            for (int i = 0; i < _dataList.Count; i++)
            {
                CardDataRendererWrapper<UserInfoDetail> w =
                    new CardDataRendererWrapper<UserInfoDetail>(_dataList[i], OnItemClick);
                _contentList.Add(w);
            }

            _cachedView.GridDataScroller.SetItemCount(_contentList.Count);
        }

        private void OnItemClick(CardDataRendererWrapper<UserInfoDetail> item)
        {
            if (item.IsSelected && !_inviteList.Contains(item.Content.UserInfoSimple.UserId))
            {
                _inviteList.Add(item.Content.UserInfoSimple.UserId);
            }
            else if (!item.IsSelected && _inviteList.Contains(item.Content.UserInfoSimple.UserId))
            {
                _inviteList.Remove(item.Content.UserInfoSimple.UserId);
            }
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlInviteFriend();
            item.Init(parent, ResScenary);
            return item;
        }

        private void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (!_isOpen)
            {
                item.Set(null);
            }
            else
            {
                if (inx >= _contentList.Count)
                {
                    LogHelper.Error("OnItemRefresh Error Inx > count");
                    return;
                }

                item.Set(_contentList[inx]);
            }
        }

        private void OnOKBtn()
        {
            if (_inviteList.Count == 0)
            {
                SocialGUIManager.ShowPopupDialog("请选择邀请的好友。");
            }
            else if (_inviteList.Count > MaxInviteNum)
            {
                SocialGUIManager.ShowPopupDialog("最多邀请20个好友。");
            }
            else
            {
                RoomManager.Instance.SendInviteFriends(_inviteList);
                SocialGUIManager.Instance.CloseUI<UICtrlInviteFriend>();
            }
        }

        private void OnAddFriendsBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlSocialRelationship>(UICtrlSocialRelationship.EMenu.AddNew);
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlInviteFriend>();
        }

        public void OnQueryUserList(List<Msg_MC_RoomUserInfo> msgDataList)
        {
            var list = new List<long>();
            for (int i = 0; i < msgDataList.Count; i++)
            {
                if (msgDataList[i].InTeam || msgDataList[i].Status == EMCUserStatus.MCUS_Offline || msgDataList[i].Status == EMCUserStatus.MCUS_None)
                {
                    continue;
                }
                list.Add(msgDataList[i].UserGuid);
            }
            UserManager.Instance.GetDataOnAsync(list, users =>
            {
                for (int i = 0; i < users.Count; i++)
                {
                    var user = users[i];
                    var msgUser = msgDataList.Find(u => u.UserGuid == user.UserInfoSimple.UserId);
                    if (msgUser != null)
                    {
                        user.InGame = msgDataList[i].Status == EMCUserStatus.MCUS_InGame;
                    }
                    else
                    {
                        LogHelper.Error("msgUser == null");
                    }
                }
                _dataList = users;
                RefreshView();
            });
        }
        
        public enum EInviteType
        {
            Room,
            Team
        }
    }
}