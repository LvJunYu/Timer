﻿using System;
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
            if (FriendList != null)
            {
                if (successCallBack != null)
                {
                    successCallBack.Invoke();
                }
                return;
            }
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
            if (FanList != null)
            {
                if (successCallBack != null)
                {
                    successCallBack.Invoke();
                }
                return;
            }
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
            if (FollowList != null)
            {
                if (successCallBack != null)
                {
                    successCallBack.Invoke();
                }
                return;
            }
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
            if (BlockList != null)
            {
                if (successCallBack != null)
                {
                    successCallBack.Invoke();
                }
                return;
            }
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

        public void RequestFollowUser(UserInfoDetail userInfoDetail)
        {
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

        public void RequestBlockUser(UserInfoDetail userInfoDetail)
        {
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

        private void RemoveBlockUser(UserInfoDetail userInfoDetail)
        {
            userInfoDetail.UserInfoSimple.RelationWithMe.BlockedByMe = false;
            if (BlockList != null && BlockList.Contains(userInfoDetail))
            {
                BlockList.Remove(userInfoDetail);
            }
            Messenger.Broadcast(EMessengerType.OnRemoveBlockUser);
            Messenger<UserInfoDetail>.Broadcast(EMessengerType.OnRelationShipChanged, userInfoDetail);
        }

        private void FollowUser(UserInfoDetail userInfoDetail)
        {
            userInfoDetail.UserInfoSimple.RelationWithMe.FollowedByMe = true;
            //如果它关注我，则从粉丝列表移除，添加到好友列表
            if (userInfoDetail.UserInfoSimple.RelationWithMe.FollowMe)
            {
                if (FriendList != null && !FriendList.Contains(userInfoDetail))
                {
                    FriendList.Add(userInfoDetail);
                }
                if (FanList != null && FanList.Contains(userInfoDetail))
                {
                    FanList.Remove(userInfoDetail);
                }
            }
            else
            {
                if (FollowList != null && !FollowList.Contains(userInfoDetail))
                {
                    FollowList.Add(userInfoDetail);
                }
            }
            Messenger.Broadcast(EMessengerType.OnFollowUser);
            Messenger<UserInfoDetail>.Broadcast(EMessengerType.OnRelationShipChanged, userInfoDetail);
        }

        private void BlockUser(UserInfoDetail userInfoDetail)
        {
            userInfoDetail.UserInfoSimple.RelationWithMe.BlockedByMe = true;
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
            Messenger.Broadcast(EMessengerType.OnBlockUser);
            Messenger<UserInfoDetail>.Broadcast(EMessengerType.OnRelationShipChanged, userInfoDetail);
        }
    }
}