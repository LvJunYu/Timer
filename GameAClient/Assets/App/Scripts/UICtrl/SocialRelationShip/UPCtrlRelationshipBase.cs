using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlRelationshipBase : UPCtrlBase<UICtrlSocialRelationship, UIViewSocialRelationship>,
        IOnChangeHandler<long>
    {
        protected List<UserInfoDetail> _userInfoDetailList;
        protected List<UMCtrlRelationLongItem> _umCtrlRelationItems;
        protected RelationUserList _data;
        protected bool _isRequesting;
        protected bool _hasInited;
        protected EResScenary _resScenary;
        protected const int _maxFollows = 100;
        protected UICtrlSocialRelationship.EMenu _menu;

        public void SetResScenary(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public void SetMenu(UICtrlSocialRelationship.EMenu menu)
        {
            _menu = menu;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScrollers[(int)_menu].Set(OnItemRefresh, GetItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.Pannels[(int)_menu].SetActiveEx(true);
            if (!_hasInited)
            {
                RequestData();
            }
            RefreshView();
        }

        public override void Close()
        {
            _cachedView.Pannels[(int)_menu].SetActiveEx(false);
            base.Close();
        }

        public override void OnDestroy()
        {
            _userInfoDetailList = null;
            _umCtrlRelationItems = null;
            _data = null;
            _hasInited = false;
            base.OnDestroy();
        }

        protected void TempData()
        {
            _userInfoDetailList = new List<UserInfoDetail>(10);
            for (int i = 0; i < 10; i++)
            {
                var user = new UserInfoDetail();
                user.UserInfoSimple.NickName = "测试数据" + i;
                _userInfoDetailList.Add(user);
            }
        }

        protected virtual void RequestData()
        {
        }

        protected void RefreshView()
        {
            if (_userInfoDetailList == null)
            {
                _cachedView.GridDataScrollers[(int)_menu].SetEmpty();
                return;
            }
            _cachedView.GridDataScrollers[(int)_menu].SetItemCount(_userInfoDetailList.Count);
        }

        protected void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (inx >= _userInfoDetailList.Count)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            item.Set(_userInfoDetailList[inx]);
        }

        protected IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlRelationLongItem();
            item.SetMenu(_menu);
            item.Init(parent, _resScenary);
            if (null == _umCtrlRelationItems)
            {
                _umCtrlRelationItems = new List<UMCtrlRelationLongItem>(6);
            }
            _umCtrlRelationItems.Add(item);
            return item;
        }

        public void OnChangeHandler(long val)
        {
            if (_userInfoDetailList != null && _umCtrlRelationItems != null)
            {
                int inx = _userInfoDetailList.FindIndex(user => user.UserInfoSimple.UserId == val);
                if (inx >= 0)
                {
                    var um = _umCtrlRelationItems.Find(p => p.Index == inx);
                    if (um != null)
                    {
                        um.RefreshView();
                    }
                }
            }
        }
    }
}