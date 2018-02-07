using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlInviteFriend : UICtrlAnimationBase<UIViewInviteFriend>
    {
//        private const int _maxSharedNum = 10;
        private bool _allSelected;
        private List<long> _sharedList = new List<long>();
        private List<UserInfoDetail> _dataList;

        private List<CardDataRendererWrapper<UserInfoDetail>> _contentList =
            new List<CardDataRendererWrapper<UserInfoDetail>>();

        public bool AllSelected
        {
            get { return _allSelected; }
            set
            {
                _allSelected = value;
                if (_cachedView.AllSelectTog.isOn != _allSelected)
                {
                    _cachedView.AllSelectTog.isOn = _allSelected;
                }
            }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.AddFriendsBtn.onClick.AddListener(OnAddFriendsBtn);
            _cachedView.OKBtn.onClick.AddListener(OnOKBtn);
            _cachedView.CancelBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.AllSelectTog.onValueChanged.AddListener(OnAllSelectValueChanged);
            _cachedView.GridDataScroller.Set(OnItemRefresh, GetItemRenderer);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            RequestData();
            RefreshView();
        }

        protected override void OnClose()
        {
            _dataList = null;
            _contentList.Clear();
            _sharedList.Clear();
            AllSelected = false;
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
                _dataList = LocalUser.Instance.RelationUserList.FriendList;
                if (_isOpen)
                {
                    RefreshView();
                }
            }, code => { SocialGUIManager.ShowPopupDialog("请求数据失败。"); });
        }

        private void RefreshView()
        {
            if (_dataList == null)
            {
                _cachedView.GridDataScroller.SetEmpty();
                _cachedView.FriendPannel.SetActive(false);
                _cachedView.EmptyPannel.SetActive(true);
                return;
            }
            _contentList.Clear();
            _contentList.Capacity = _dataList.Count;
            for (int i = 0; i < _dataList.Count; i++)
            {
                CardDataRendererWrapper<UserInfoDetail> w =
                    new CardDataRendererWrapper<UserInfoDetail>(_dataList[i], OnItemClick);
                _contentList.Add(w);
            }
            _cachedView.GridDataScroller.SetItemCount(_contentList.Count);
            _cachedView.FriendPannel.SetActive(_contentList.Count != 0);
            _cachedView.EmptyPannel.SetActive(_contentList.Count == 0);
            _cachedView.AllSelectTog.isOn = AllSelected;
        }

        private void OnItemClick(CardDataRendererWrapper<UserInfoDetail> item)
        {
            if (item.IsSelected && !_sharedList.Contains(item.Content.UserInfoSimple.UserId))
            {
                _sharedList.Add(item.Content.UserInfoSimple.UserId);
            }
            else if (!item.IsSelected && _sharedList.Contains(item.Content.UserInfoSimple.UserId))
            {
                _sharedList.Remove(item.Content.UserInfoSimple.UserId);
                AllSelected = false;
            }
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlWorldShareProject();
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

        private void OnAllSelectValueChanged(bool value)
        {
            if (value)
            {
                if (!AllSelected)
                {
                    for (int i = 0; i < _contentList.Count; i++)
                    {
                        _contentList[i].IsSelected = true;
                        _contentList[i].FireOnClick();
                    }
                    _cachedView.GridDataScroller.RefreshCurrent();
                    AllSelected = true;
                }
            }
            else
            {
                if (AllSelected)
                {
                    for (int i = 0; i < _contentList.Count; i++)
                    {
                        _contentList[i].IsSelected = false;
                        _contentList[i].FireOnClick();
                    }
                    _cachedView.GridDataScroller.RefreshCurrent();
                    AllSelected = false;
                }
            }
        }

        private void OnOKBtn()
        {
        }

        private void OnAddFriendsBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlSocialRelationship>(UICtrlSocialRelationship.EMenu.AddNew);
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlInviteFriend>();
        }
    }
}