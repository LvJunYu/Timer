using System;
using System.Collections.Generic;
using System.Linq;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class RelationUserList : SyncronisticData
    {
        private List<UserInfoDetail> _followRelationList = new List<UserInfoDetail>();
        private List<UserInfoDetail> _blockRelationList = new List<UserInfoDetail>();
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
            //OnRequest(successCallback, failedCallback);
            _followRelationList.Clear();
            Request(
                userId,
                ERelationUserType.RUT_FollowedByMe,
                startInx,
                maxCount,
                orderBy,
                orderType,
                () =>
                {
                    for (int i = 0; i < _dataList.Count(); i++)
                    {
                        UserInfoDetail _relationitem =
                            UserManager.Instance.UpdateData(new UserInfoDetail(_dataList[i]));
                        _followRelationList.Add(_relationitem);
                    }
                    _dataList.Clear();
                    successCallback.Invoke();
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
            _blockRelationList.Clear();
            Request(
                userId,
                ERelationUserType.RUT_BlockByMe,
                startInx,
                maxCount,
                orderBy,
                orderType,
                () =>
                {
                    for (int i = 0; i < _dataList.Count(); i++)
                    {
                        UserInfoDetail _relationitem =
                            UserManager.Instance.UpdateData(new UserInfoDetail(_dataList[i]));
                        _blockRelationList.Add(_relationitem);
                    }
                    _dataList.Clear();
                    successCallback.Invoke();
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
            if (_dataDetailList == null)
            {
                _dataDetailList = new List<UserInfoDetail>(_dataList.Count);
            }
            _dataDetailList.Clear();
            for (int i = 0; i < _dataList.Count; i++)
            {
                _dataDetailList.Add(UserManager.Instance.UpdateData(_dataList[i]));
            }
        }
    }
}