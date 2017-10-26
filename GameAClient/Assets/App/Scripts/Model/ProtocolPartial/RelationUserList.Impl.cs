using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public partial class RelationUserList
    {
        private List<UserInfoDetail> _dataDetailList;
        public List<UserInfoDetail> FollowList;
        public List<UserInfoDetail> FanList;
        public List<UserInfoDetail> FriendList;
        public List<UserInfoDetail> BlockList;

        public List<UserInfoDetail> DataDetailList
        {
            get { return _dataDetailList; }
        }

        public void RequestFriends(long instanceUserGuid, Action successCallBack, Action<ENetResultCode> failCallBack)
        {
            Request(instanceUserGuid, ERelationUserType.RUT_FollowEachOther, 0, 100,
                ERelationUserOrderBy.RUOB_Friendliness,
                EOrderType.OT_Asc, () =>
                {
                    FriendList = _dataDetailList;
                    if (successCallBack != null)
                    {
                        successCallBack.Invoke();
                    }
                }, failCallBack);
        }

        public void RequestFans(long instanceUserGuid, Action successCallBack, Action<ENetResultCode> failCallBack)
        {
            Request(instanceUserGuid, ERelationUserType.RUT_FollowMe, 0, 100,
                ERelationUserOrderBy.RUOB_Friendliness,
                EOrderType.OT_Asc, () =>
                {
                    FanList = _dataDetailList;
                    if (successCallBack != null)
                    {
                        successCallBack.Invoke();
                    }
                }, failCallBack);
        }

        public void RequestFollows(long instanceUserGuid, Action successCallBack, Action<ENetResultCode> failCallBack)
        {
            Request(instanceUserGuid, ERelationUserType.RUT_FollowedByMe, 0, 100,
                ERelationUserOrderBy.RUOB_Friendliness,
                EOrderType.OT_Asc, () =>
                {
                    FollowList = _dataDetailList;
                    if (successCallBack != null)
                    {
                        successCallBack.Invoke();
                    }
                }, failCallBack);
        }

        public void RequestBlocks(long instanceUserGuid, Action successCallBack, Action<ENetResultCode> failCallBack)
        {
            Request(instanceUserGuid, ERelationUserType.RUT_BlockByMe, 0, 100,
                ERelationUserOrderBy.RUOB_Friendliness,
                EOrderType.OT_Asc, () =>
                {
                    BlockList = _dataDetailList;
                    if (successCallBack != null)
                    {
                        successCallBack.Invoke();
                    }
                }, failCallBack);
        }

        protected override void OnSyncPartial()
        {
            if (_dataList == null) return;
            _dataDetailList = new List<UserInfoDetail>(_dataList.Count);
            for (int i = 0; i < _dataList.Count; i++)
            {
                _dataDetailList.Add(UserManager.Instance.UpdateData(_dataList[i]));
            }
        }

        public void RequestFollowUser(UserInfoDetail userInfoDetail)
        {
            if (userInfoDetail.UserInfoSimple.UserId == LocalUser.Instance.UserGuid)
            {
                SocialGUIManager.ShowPopupDialog("不能关注自己！");
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Empty);
            RemoteCommands.UpdateFollowState(userInfoDetail.UserInfoSimple.UserId, true, res =>
            {
                if (res.ResultCode == (int) EUpdateFollowStateCode.UFSS_Success)
                {
                    FollowUser(userInfoDetail);
                }
                else
                {
                    SocialGUIManager.ShowPopupDialog("关注失败。");
                }
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            }, code =>
            {
                SocialGUIManager.ShowPopupDialog("关注失败。");
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            });
        }

        public void RequestRemoveFollowUser(UserInfoDetail userInfoDetail)
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Empty);
            RemoteCommands.UpdateFollowState(userInfoDetail.UserInfoSimple.UserId, false, res =>
            {
                if (res.ResultCode == (int) EUpdateFollowStateCode.UFSS_Success)
                {
                    RemoveFollowUser(userInfoDetail);
                }
                else
                {
                    SocialGUIManager.ShowPopupDialog("取消关注失败。");
                }
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            }, code =>
            {
                SocialGUIManager.ShowPopupDialog("取消关注失败。");
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            });
        }

        public void RequestBlockUser(UserInfoDetail userInfoDetail)
        {
            if (userInfoDetail.UserInfoSimple.UserId == LocalUser.Instance.UserGuid)
            {
                SocialGUIManager.ShowPopupDialog("不能屏蔽自己！");
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Empty);
            RemoteCommands.UpdateBlockState(userInfoDetail.UserInfoSimple.UserId, true, res =>
            {
                if (res.ResultCode == (int) EUpdateBlockStateCode.UBSS_Success)
                {
                    BlockUser(userInfoDetail);
                }
                else
                {
                    SocialGUIManager.ShowPopupDialog("屏蔽失败。");
                }
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            }, code =>
            {
                SocialGUIManager.ShowPopupDialog("屏蔽失败。");
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            });
        }

        public void RequestRemoveBlockUser(UserInfoDetail userInfoDetail)
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Empty);
            RemoteCommands.UpdateBlockState(userInfoDetail.UserInfoSimple.UserId, false, res =>
            {
                if (res.ResultCode == (int) EUpdateBlockStateCode.UBSS_Success)
                {
                    RemoveBlockUser(userInfoDetail);
                }
                else
                {
                    SocialGUIManager.ShowPopupDialog("解除屏蔽失败。");
                }
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            }, code =>
            {
                SocialGUIManager.ShowPopupDialog("解除屏蔽失败。");
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            });
        }

        private void FollowUser(UserInfoDetail userInfoDetail)
        {
            userInfoDetail.UserInfoSimple.RelationWithMe.FollowedByMe = true;
            if (FollowList != null && !FollowList.Contains(userInfoDetail))
            {
                FollowList.Add(userInfoDetail);
            }
            if (userInfoDetail.UserInfoSimple.RelationWithMe.FollowMe)
            {
                if (FriendList != null && !FriendList.Contains(userInfoDetail))
                {
                    FriendList.Add(userInfoDetail);
                }
            }
            Messenger<UserInfoDetail>.Broadcast(EMessengerType.OnRelationShipChanged, userInfoDetail);
        }

        private void RemoveFollowUser(UserInfoDetail userInfoDetail)
        {
            userInfoDetail.UserInfoSimple.RelationWithMe.FollowedByMe = false;
            if (FollowList != null && FollowList.Contains(userInfoDetail))
            {
                FollowList.Remove(userInfoDetail);
            }
            if (userInfoDetail.UserInfoSimple.RelationWithMe.FollowMe)
            {
                if (FriendList != null && FriendList.Contains(userInfoDetail))
                {
                    FriendList.Remove(userInfoDetail);
                }
            }
            Messenger<UserInfoDetail>.Broadcast(EMessengerType.OnRelationShipChanged, userInfoDetail);
        }

        private void BlockUser(UserInfoDetail userInfoDetail)
        {
            userInfoDetail.UserInfoSimple.RelationWithMe.BlockedByMe = true;
            userInfoDetail.UserInfoSimple.RelationWithMe.FollowedByMe = false;
            userInfoDetail.UserInfoSimple.RelationWithMe.FollowMe = false;
            if (BlockList != null && !BlockList.Contains(userInfoDetail))
            {
                BlockList.Add(userInfoDetail);
            }
            if (FollowList != null && FollowList.Contains(userInfoDetail))
            {
                FollowList.Remove(userInfoDetail);
            }
            if (FanList != null && FanList.Contains(userInfoDetail))
            {
                FanList.Remove(userInfoDetail);
            }
            if (FriendList != null && FriendList.Contains(userInfoDetail))
            {
                FriendList.Remove(userInfoDetail);
            }
            Messenger<UserInfoDetail>.Broadcast(EMessengerType.OnRelationShipChanged, userInfoDetail);
        }

        private void RemoveBlockUser(UserInfoDetail userInfoDetail)
        {
            userInfoDetail.UserInfoSimple.RelationWithMe.BlockedByMe = false;
            if (BlockList != null && BlockList.Contains(userInfoDetail))
            {
                BlockList.Remove(userInfoDetail);
            }
            Messenger<UserInfoDetail>.Broadcast(EMessengerType.OnRelationShipChanged, userInfoDetail);
        }
    }
}