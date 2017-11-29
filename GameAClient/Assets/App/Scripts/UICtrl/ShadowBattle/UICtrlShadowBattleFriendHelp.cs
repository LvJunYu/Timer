using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlShadowBattleFriendHelp : UICtrlAnimationBase<UIViewShadowBattleFriendHelp>
    {
        private long _battleId;
        private long _selectFriendId;
        private List<UserInfoDetail> _dataList;

        private List<CardDataRendererWrapper<UserInfoDetail>> _contentList =
            new List<CardDataRendererWrapper<UserInfoDetail>>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.AddFriendsBtn.onClick.AddListener(OnAddFriendsBtn);
            _cachedView.OKBtn.onClick.AddListener(OnOKBtn);
            _cachedView.CancelBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.GridDataScroller.Set(OnItemRefresh, GetItemRenderer);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _selectFriendId = -1;
            RequestData();
            RefreshView();
        }

        protected override void OnClose()
        {
            _dataList = null;
            _contentList.Clear();
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
            _groupId = (int) EUIGroupType.PopUpDialog;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<long>(EMessengerType.OnShadowBattleStart, OnShadowBattleStart);
        }

        private void OnShadowBattleStart(long battleId)
        {
            _battleId = battleId;
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
                _cachedView.SharePannel.SetActive(false);
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
            _cachedView.SharePannel.SetActive(_contentList.Count != 0);
            _cachedView.EmptyPannel.SetActive(_contentList.Count == 0);
        }

        private void OnItemClick(CardDataRendererWrapper<UserInfoDetail> item)
        {
            if (item.IsSelected)
            {
                _selectFriendId = item.Content.UserInfoSimple.UserId;
            }
            else
            {
                if (_selectFriendId == item.Content.UserInfoSimple.UserId)
                {
                    _selectFriendId = -1;
                }
            }
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlShadowBattleFriendHelp();
            item.Init(parent, ResScenary);
            item.SetToggleGroup(_cachedView.ToggleGroup);
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

        private void OnOKBtn()
        {
            if (_selectFriendId == -1)
            {
                SocialGUIManager.ShowPopupDialog("请选择助战好友。");
            }
            else
            {
                RemoteCommands.RequestHelpShadowBattle(_battleId, _selectFriendId, msg =>
                {
                    if (msg.ResultCode == (int) EShareProjectCode.SPC_Success)
                    {
                        SocialGUIManager.ShowPopupDialog("请求助战成功");
                        SocialGUIManager.Instance.CloseUI<UICtrlShadowBattleFriendHelp>();
                        Messenger.Broadcast(EMessengerType.OnShadowBattleFriendHelp);
                    }
                    else
                    {
                        SocialGUIManager.ShowPopupDialog("请求助战失败");
                        SocialGUIManager.Instance.CloseUI<UICtrlShadowBattleFriendHelp>();
                    }
                }, code =>
                {
                    SocialGUIManager.ShowPopupDialog("请求助战失败");
                    SocialGUIManager.Instance.CloseUI<UICtrlShadowBattleFriendHelp>();
                });
            }
        }

        private void OnAddFriendsBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlSocialRelationship>(UICtrlSocialRelationship.EMenu.AddNew);
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlShadowBattleFriendHelp>();
        }
    }
}