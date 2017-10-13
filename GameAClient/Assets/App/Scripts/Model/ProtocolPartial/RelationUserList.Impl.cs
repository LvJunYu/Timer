using System.Collections.Generic;
using GameA.Game;
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
            RemoteCommands.UpdateBlockState(userInfoDetail.UserInfoSimple.UserId, false, res =>
            {
                if (res.ResultCode == (int) EUpdateBlockStateCode.UBSS_Success)
                {
                    RemoveBlockUser(userInfoDetail);
                }
                else
                {
                    RemoveBlockUser(userInfoDetail);
                }
            }, code =>
            {
                LogHelper.Debug("服务器请求失败，客服端模拟测试");
                RemoveBlockUser(userInfoDetail);
            });
            
        }

        public void RequestFollowUser(UserInfoDetail userInfoDetail)
        {
            RemoteCommands.UpdateFollowState(userInfoDetail.UserInfoSimple.UserId, true, res =>
            {
                if (res.ResultCode == (int) EUpdateFollowStateCode.UFSS_Success)
                {
                    FollowUser(userInfoDetail);
                }
                else
                {
                    FollowUser(userInfoDetail);
                }
            }, code =>
            {
                LogHelper.Debug("服务器请求失败，客服端模拟测试");
                FollowUser(userInfoDetail);
            });
            
        }

        private void RemoveBlockUser(UserInfoDetail userInfoDetail)
        {
            userInfoDetail.UserInfoSimple.RelationWithMe.BlockedByMe = false;
            if (BlockList == null) return;
            if (BlockList.Contains(userInfoDetail))
            {
                BlockList.Remove(userInfoDetail);
            }
            Messenger.Broadcast(EMessengerType.OnRemoveBlockUser);
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
        }
    }
}