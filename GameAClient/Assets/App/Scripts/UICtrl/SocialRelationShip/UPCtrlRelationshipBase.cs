using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlRelationshipBase<T> : UPCtrlBase<UICtrlSocialRelationship, UIViewSocialRelationship>,
        IOnChangeHandler<long> where T : IDataItemRenderer, IRelationShipItem, new()
    {
        protected List<UserInfoDetail> _userInfoDetailList;
        protected List<T> _umCtrlRelationItems;
        protected bool _isRequesting;
        protected bool _hasInited;
        protected EResScenary _resScenary;
        protected int _maxFollows = 100;
        protected UICtrlSocialRelationship.EMenu _menu;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScrollers[(int) _menu].Set(OnItemRefresh, GetItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.Pannels[(int) _menu].SetActiveEx(true);
            if (!_hasInited)
            {
                RequestData();
            }
            RefreshView();
        }

        public override void Close()
        {
            _cachedView.Pannels[(int) _menu].SetActiveEx(false);
            base.Close();
        }

        public override void OnDestroy()
        {
            _userInfoDetailList = null;
            _umCtrlRelationItems = null;
            _hasInited = false;
            base.OnDestroy();
        }

        protected virtual void TempData()
        {
            _userInfoDetailList = new List<UserInfoDetail>(10);
            for (int i = 0; i < 10; i++)
            {
                var user = new UserInfoDetail();
                user.UserInfoSimple.NickName = "测试数据" + i;
                user.UserInfoSimple.Sex = i % 2 == 0 ? ESex.S_Male : ESex.S_Female;
                _userInfoDetailList.Add(user);
            }
        }

        protected virtual void RequestData()
        {
        }

        public void RefreshView()
        {
            if (!_isOpen) return;
            if (_userInfoDetailList == null)
            {
                _cachedView.GridDataScrollers[(int) _menu].SetEmpty();
                return;
            }
            _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_userInfoDetailList.Count);
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
            var item = new T();
            item.SetMenu(_menu);
            item.Init(parent, _resScenary);
            if (null == _umCtrlRelationItems)
            {
                _umCtrlRelationItems = new List<T>(6);
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

        public void SetResScenary(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public void SetMenu(UICtrlSocialRelationship.EMenu menu)
        {
            _menu = menu;
        }
    }

    public interface IRelationShipItem
    {
        void SetMenu(UICtrlSocialRelationship.EMenu eMenu);
        void RefreshView();
        bool Init(RectTransform parent, EResScenary resScenary, Vector3 localpos = new Vector3());
    }
}