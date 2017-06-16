  /********************************************************************
  ** Filename : UICtrlFollowerUserList.cs
  ** Author : quan
  ** Date : 2016/6/10 10:04
  ** Summary : UICtrlFollowerUserList.cs
  ***********************************************************************/
using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlFollowerUserList : UISocialContentCtrlBase<UIViewFollowerUserList>, IUIWithTitle
    {
        #region 常量与字段
        private const long RequestInterval = 10 * GameTimer.Minute2Ticks;
        private const int PageCount = 20;
        private GameTimer _requestTimer;
        private bool _isRequest = false;
        private User _user;
        private List<User> _content;
        private bool _isEnd;
        private SocialContentRefreshHelper _refreshHelper;
        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridScroller.SetCallback(OnItemRefresh, GetItemRenderer);

            _cachedView.RefreshController.SetRefreshCallback(()=>{ RequestData(); });
            _refreshHelper = new SocialContentRefreshHelper(_cachedView.RefreshController, _cachedView.Content, _cachedView.EmptyTip, _cachedView.ErrorTip);
            _isEnd = false;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void OnOpen(object parameter)
        {
            User user = parameter as User;
            _requestTimer = user.FollowerListRequestTimer;
            if(user == null)
            {
                LogHelper.Error("UICtrlFollowerUserList onOpen error, param is null");
                return;
            }
            if(_user != user)
            {
                _user = user;
                ScrollToTop();
                ResetView();
            }
            Refresh();
            base.OnOpen(parameter);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void ResetView()
        {
            _cachedView.GridScroller.SetItemCount(0);
            _cachedView.RefreshController.ExitRefresh();
        }

        private void RequestData(bool append = false)
        {
            if(_isRequest)
            {
                if(!append)
                {
                    _cachedView.RefreshController.ExitRefresh();
                }
                return;
            }
            if(!append && _requestTimer.GetInterval() < RequestInterval)
            {
                _cachedView.RefreshController.ExitRefresh();
                return;
            }
            _refreshHelper.State = SocialContentRefreshHelper.EState.None;
            _isRequest = true;
//            Msg_CA_RequestFollower msg = new Msg_CA_RequestFollower();
//            msg.UserGuid = _user.UserGuid;
//            if(append)
//            {
//                if(_content != null)
//                {
//                    msg.StartInx = _content.Count;
//                }
//            }
//            msg.MaxCount = PageCount;
//            User user = _user;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_UserInfoList>(SoyHttpApiPath.GetFollowerList, msg, ret=>{
//                _isRequest = false;
//                user.OnSyncFollowerList(ret);
//                if(user == _user)
//                {
//                    _content = _user.FollowerList;
//                    if(!append)
//                    {
//                        _requestTimer.Reset();
//                        _cachedView.RefreshController.ExitRefresh();
//                        if(_content.Count == 0)
//                        {
//                            _refreshHelper.State = SocialContentRefreshHelper.EState.SuccessEmpty;
//                        }
//                        else
//                        {
//                            _refreshHelper.State = SocialContentRefreshHelper.EState.SuccessExsit;
//                        }
//                    }
//                    if(ret.DataList.Count < PageCount)
//                    {
//                        _isEnd = true;
//                    }
//                    RefreshView();
//                }
//            }, (code, msgStr)=>{
//                _isRequest = false;
//                if(user == _user && !append)
//                {
//                    _cachedView.RefreshController.ExitRefresh();
//                    if(_user.FollowerList == null)
//                    {
//                        _refreshHelper.State = SocialContentRefreshHelper.EState.FailedNone;
//                    }
//                    else if(_user.FollowerList.Count == 0)
//                    {
//                        _refreshHelper.State = SocialContentRefreshHelper.EState.FailedEmpty;
//                    }
//                    else
//                    {
//                        _refreshHelper.State = SocialContentRefreshHelper.EState.FailedExsit;
//                    }
//                }
//            });
        }


        public void Refresh()
        {
            _isEnd = false;
            _content = _user.FollowerList;
            if(_content == null)
            {
                _refreshHelper.State = SocialContentRefreshHelper.EState.None;
                _cachedView.RefreshController.StartRefresh();
            }
            else
            {
                if(_content.Count == 0)
                {
                    _refreshHelper.State = SocialContentRefreshHelper.EState.SuccessEmpty;
                }
                else
                {
                    _refreshHelper.State = SocialContentRefreshHelper.EState.SuccessExsit;
                }
                if(_requestTimer.GetInterval() >= RequestInterval)
                {
                    _cachedView.RefreshController.StartRefresh();
                }
            }
            RefreshView();
        }


        private void RefreshView()
        {
            _content = _user.FollowerList;
            if(_content == null)
            {
                _cachedView.GridScroller.SetItemCount(0);
            }
            else
            {
                _cachedView.GridScroller.SetItemCount(_content.Count);
            }
        }


        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlRelationUser();
            item.Init(parent, Vector3.zero);
            return item;
        }

        private void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if(_content == null)
            {
                return;
            }
            if(inx >= _content.Count)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            item.Set(_content[inx]);
            if(!_isEnd)
            {
                if(inx > _content.Count - 4)
                {
                    RequestData(true);
                }
            }
        }

        #endregion

        #region 事件处理
        #endregion 事件处理

        #region 接口
        public object GetTitle()
        {
            return "粉丝";
        }

        #endregion
    }
}

