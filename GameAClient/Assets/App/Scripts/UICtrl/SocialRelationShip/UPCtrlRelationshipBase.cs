using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public abstract class UPCtrlRelationshipBase : UPCtrlBase<UICtrlSocialRelationship, UIViewSocialRelationship>,
        IOnChangeHandler<UserInfoDetail>
    {
        public bool HasInited;
        protected List<UserInfoDetail> _userInfoDetailList;
        protected bool _isRequesting;
        protected EResScenary _resScenary;
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
            if (!HasInited)
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
            base.OnDestroy();
        }

        protected virtual void TempData()
        {
            if (_userInfoDetailList != null && _userInfoDetailList.Count != 0) return;
            _userInfoDetailList = new List<UserInfoDetail>(10);
            for (int i = 0; i < 10; i++)
            {
                var user = new UserInfoDetail();
                user.UserInfoSimple.UserId = 10000 + i;
                user.UserInfoSimple.NickName = "测试数据" + i;
                user.UserInfoSimple.Sex = i % 2 == 0 ? ESex.S_Male : ESex.S_Female;
                _userInfoDetailList.Add(user);
            }
        }

        protected abstract void RequestData();

        public void RefreshView()
        {
            if (!_isOpen) return;
            if (_userInfoDetailList == null)
            {
                _cachedView.GridDataScrollers[(int) _menu].SetEmpty();
                return;
            }
            _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_userInfoDetailList.Count);
            _cachedView.GridDataScrollers[(int) _menu].RefreshCurrent();
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

        protected abstract IDataItemRenderer GetItemRenderer(RectTransform parent);

        public void SetResScenary(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public void SetMenu(UICtrlSocialRelationship.EMenu menu)
        {
            _menu = menu;
        }

        public void OnChangeHandler(UserInfoDetail val)
        {
            RefreshView();
        }
    }
}