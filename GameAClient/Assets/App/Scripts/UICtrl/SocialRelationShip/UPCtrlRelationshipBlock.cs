using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class UPCtrlRelationshipBlock : UPCtrlBase<UICtrlSocialRelationship, UIViewSocialRelationship>,
        IOnChangeHandler<long>
    {
        private List<UserInfoDetail> _followRelationList;
        private List<UMCtrlRelationFriend> _umCtrlRelationFriends;
        private RelationUserList _data;
        private bool _isRequest;
        private bool _hasInited;
        private EResScenary _resScenary;

        public void Set(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public override void Open()
        {
            base.Open();
            _cachedView.FollowPannel.SetActiveEx(true);
            _data = LocalUser.Instance.RelationUserList;
            if (!_hasInited)
            {
                RequestData();
            }
            RefreshView();
        }

        public override void Close()
        {
            _cachedView.FollowPannel.SetActiveEx(false);
            base.Close();
        }

        private void RequestData()
        {
            if (_isRequest)
            {
                return;
            }
            _isRequest = true;
            _data.RequestFollowList(LocalUser.Instance.UserGuid, 0, 100, ERelationUserOrderBy.RUOB_Friendliness,
                EOrderType.OT_Asc, () =>
                {
                    _hasInited = true;
                    _isRequest = false;
                    if (!_isOpen)
                    {
                        return;
                    }
                    RefreshView();
                }, code => { _isRequest = false; });
        }

        private void RefreshView()
        {
            _followRelationList = _data.FollowRelationList;
            if(_followRelationList == null) return;
            for (int i = 0; i < _umCtrlRelationFriends.Count; i++)
            {
                if (i < _followRelationList.Count)
                {
                    var user = _followRelationList[i];
                    _umCtrlRelationFriends[i].Set(user);
                }
                else
                {
                    _umCtrlRelationFriends[i].Set(null);
                }
            }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _umCtrlRelationFriends = new List<UMCtrlRelationFriend>(10);
            for (int i = 0; i < 10; i++)
            {
                var umCtrl = new UMCtrlRelationFriend();
                umCtrl.Init(_cachedView.FollowUMRtf, _resScenary);
                _umCtrlRelationFriends.Add(umCtrl);
            }
        }

        public override void OnDestroy()
        {
            _umCtrlRelationFriends.Clear();
            _umCtrlRelationFriends = null;
            base.OnDestroy();
        }

        public void OnChangeHandler(long val)
        {
            if (_followRelationList != null)
            {
                int inx = _followRelationList.FindIndex(user => user.UserInfoSimple.UserId == val);
                if (inx >= 0)
                {
                    _umCtrlRelationFriends[inx].RefreshView();
                }
            }
        }
    }
}