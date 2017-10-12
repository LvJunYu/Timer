using System;
using System.Collections.Generic;
using SoyEngine.Proto;

namespace GameA
{
    public partial class RelationUserList
    {
        private List<UserInfoDetail> _followRelationList;
        private List<UserInfoDetail> _blockRelationList;
        private List<UserInfoDetail> _dataDetailList;

        public List<UserInfoDetail> DataDetailList
        {
            get { return _dataDetailList; }
        }

        public void RequestFollowList(
            long userId,
            int startInx,
            int maxCount,
            ERelationUserOrderBy orderBy,
            EOrderType orderType,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            Request(
                userId,
                ERelationUserType.RUT_FollowedByMe,
                startInx,
                maxCount,
                orderBy,
                orderType,
                () =>
                {
                    _followRelationList = _dataDetailList;
                    if (successCallback != null)
                    {
                        successCallback();
                    }
                }, failedCallback);
        }

        public void RequestBlockList(
            long userId,
            int startInx,
            int maxCount,
            ERelationUserOrderBy orderBy,
            EOrderType orderType,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            Request(
                userId,
                ERelationUserType.RUT_BlockByMe,
                startInx,
                maxCount,
                orderBy,
                orderType,
                () =>
                {
                    _blockRelationList = _dataDetailList;
                    if (successCallback != null)
                    {
                        successCallback();
                    }
                }, failedCallback);
        }

        public List<UserInfoDetail> FollowRelationList
        {
            get { return _followRelationList; }
            set
            {
                if (_followRelationList != value)
                {
                    _followRelationList = value;
                    SetDirty();
                }
            }
        }

        public List<UserInfoDetail> BlockRelationList
        {
            get { return _blockRelationList; }
            set
            {
                if (_blockRelationList != value)
                {
                    _blockRelationList = value;
                    SetDirty();
                }
            }
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
    }
}