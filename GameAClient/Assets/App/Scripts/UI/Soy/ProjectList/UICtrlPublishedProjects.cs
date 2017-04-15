  /********************************************************************
  ** Filename : UICtrlPublishedProjects.cs
  ** Author : quan
  ** Date : 2016/6/10 10:16
  ** Summary : UICtrlPublishedProjects.cs
  ***********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlPublishedProjects : UISocialContentCtrlBase<UIViewPublishedPorjects>, IUIWithTitle, IUIWithRightCustomButton
    {
        #region 常量与字段
        private const long RequestInterval = 5 * GameTimer.Minute2Ticks;
        private const int PageCount = 10;
        private User _user;
        private GameTimer _requestTimer;
        private List<CardDataRendererWrapper<Project>> _content = new List<CardDataRendererWrapper<Project>>();
        private EMode _mode = EMode.None;
        private int _currentSelectedCount = 0;
        private bool _isRequest = false;
        private bool _isEnd;
        private SocialContentRefreshHelper _refreshHelper;
        private bool _load;
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
            _cachedView.RefreshController.SetRefreshCallback(()=>{ RequestData(); });
            _refreshHelper = new SocialContentRefreshHelper(_cachedView.RefreshController, _cachedView.Content, _cachedView.EmptyTip, _cachedView.ErrorTip);
            _isEnd = false;
            _cachedView.GridScroller.SetCallback(OnItemRefresh, GetItemRenderer);
            _cachedView.CancelButton.onClick.AddListener(OnCancelButtonClick);
            _cachedView.DeleteButton.onClick.AddListener(OnDeleteButtonClick);
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void OnOpen(object parameter)
        {
            User param = parameter as User;
            if(param == null)
            {
                LogHelper.Error("UICtrlPublishedProjects OnOpen Param is null");
                return;
            }
            _load = true;
            if(_user != param)
            {
                _user = param;
                _requestTimer = _user.GetPublishedPrjectRequestTimer();
                ScrollToTop();
                ResetView();
            }
            SetMode(EMode.Normal);
            Refresh();
            base.OnOpen(parameter);
        }

        protected override void OnClose()
        {
            base.OnClose();
            _load = false;
            _cachedView.GridScroller.RefreshCurrent();
        }

        private void ResetView()
        {
            _cachedView.GridScroller.SetItemCount(0);
            _cachedView.RefreshController.ExitRefresh();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void Refresh()
        {
            _isEnd = false;
            var pList = _user.GetPublishedProjectList();
            if(pList == null)
            {
                _refreshHelper.State = SocialContentRefreshHelper.EState.None;
                _cachedView.RefreshController.StartRefresh();
            }
            else
            {
                if(pList.Count == 0)
                {
                    _refreshHelper.State = SocialContentRefreshHelper.EState.SuccessEmpty;
                }
                else
                {
                    _refreshHelper.State = SocialContentRefreshHelper.EState.SuccessExsit;
                }
            }
            RefreshView();
        }

        private void RequestData(bool append = false)
        {
//            if(_isRequest)
//            {
//                if(!append)
//                {
//                    _cachedView.RefreshController.ExitRefresh();
//                }
//                return;
//            }
//            if(!append && _requestTimer.GetInterval() < RequestInterval)
//            {
//                _cachedView.RefreshController.ExitRefresh();
//                return;
//            }
//            _refreshHelper.State = SocialContentRefreshHelper.EState.None;
//            _isRequest = true;
//            Msg_CA_RequestPublishedProject msg = new Msg_CA_RequestPublishedProject();
//            if(append)
//            {
//                if(_content != null && _content.Count > 0)
//                {
//                    msg.MaxUpdateTime = _content[_content.Count - 1].Content.CreateTime;
//                }
//            }
//            msg.MaxCount = PageCount;
//            msg.UserGuid = _user.UserGuid;
//            User user = _user;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_ProjectList>(SoyHttpApiPath.GetPublishedProjectList, msg, ret=>{
//                _isRequest = false;
////                user.OnSyncUserPublishedProjectList(ret);
//                if(_user == user)
//                {
//                    if(!append)
//                    {
//                        _requestTimer.Reset();
//                        _cachedView.RefreshController.ExitRefresh();
//                        if(user.GetPublishedProjectList().Count == 0)
//                        {
//                            _refreshHelper.State = SocialContentRefreshHelper.EState.SuccessEmpty;
//                        }
//                        else
//                        {
//                            _refreshHelper.State = SocialContentRefreshHelper.EState.SuccessExsit;
//                        }
//                    }
//                    if(ret.ProjectList.Count < PageCount && msg.MinUpdateTime == 0)
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
//                    if(_user.GetPublishedProjectList() == null)
//                    {
//                        _refreshHelper.State = SocialContentRefreshHelper.EState.FailedNone;
//                    }
//                    else if(_user.GetPublishedProjectList().Count == 0)
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

        private void RefreshView()
        {
            List<Project> list = _user.GetPublishedProjectList();
            _content.Clear();
            if(list != null)
            {
                _content.Capacity = Mathf.Max(list.Count, _content.Capacity);
                for(int i=0; i<list.Count; i++)
                {
                    var wrapper = new CardDataRendererWrapper<Project>(list[i], OnItemClick);
                    if(_mode == EMode.Edit)
                    {
                        wrapper.CardMode = ECardMode.Selectable;
                        wrapper.IsSelected = false;
                    }
                    else
                    {
                        wrapper.CardMode = ECardMode.Normal;
                        wrapper.IsSelected = false;
                    }
                    _content.Add(wrapper);
                }
            }
            _cachedView.GridScroller.SetItemCount(_content.Count);
            _currentSelectedCount = 0;

        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlProjectCardSimple();
            item.Init(parent, Vector3.zero);
            return item;
        }

        private void OnItemClick(CardDataRendererWrapper<Project> item)
        {
            if(_mode == EMode.Normal)
            {
                ProjectParams param = new ProjectParams(){
                    Type = EProjectParamType.Project,
                    Project = item.Content
                };
                SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(param);
            }
            else
            {
                item.IsSelected = !item.IsSelected;
                item.BroadcastDataChanged();
                if(item.IsSelected)
                {
                    _currentSelectedCount++;
                    RefreshBottomMenu();
                }
                else
                {
                    _currentSelectedCount--;
                    RefreshBottomMenu();
                }
            }
        }

        private void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if(inx >= _content.Count)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            if(!_load)
            {
                item.Unload();
                return;
            }
            item.Set(_content[inx]);
            if(!_isEnd && _mode != EMode.Edit)
            {
                if(inx > _content.Count - 2)
                {
                    RequestData(true);
                }
            }
        }

        private void RefreshBottomMenu()
        {
            _cachedView.DeleteButtonText.text = string.Format("删除（已选 {0}）", _currentSelectedCount);
        }

        private void SetMode(EMode mode)
        {
            _mode = mode;
            for(int i=0; i<_content.Count; i++)
            {
                if(mode == EMode.Normal || mode == EMode.None)
                {
                    _content[i].CardMode = ECardMode.Normal;
                    _content[i].IsSelected = false;
                    _content[i].BroadcastDataChanged();
                }
                else if(mode == EMode.Edit)
                {
                    _content[i].CardMode = ECardMode.Selectable;
                    _content[i].IsSelected = false;
                    _content[i].BroadcastDataChanged();
                }
            }
            if(mode == EMode.Normal || mode == EMode.None)
            {
                _cachedView.BottomMenuDock.gameObject.SetActive(false);
            }
            else
            {
                _cachedView.BottomMenuDock.gameObject.SetActive(true);
                _currentSelectedCount = 0;
                RefreshBottomMenu();
            }

//            _uiStack.Titlebar.RefreshRightButton();
        }


        private void RequestUnpublish(List<Project> projectList)
        {
//            User user = _user;
//            Msg_CA_UnpublishProject msg = new Msg_CA_UnpublishProject();
//            for(int i=0; i<projectList.Count; i++)
//            {
//                msg.ProjectGuid.Add(projectList[i].Guid);
//            }
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_OperateProjectRet>(SoyHttpApiPath.UnpublishProject, msg, ret=>{
//                LogHelper.Info("RequestUnpublish, ResultCode: {0}", ret.Result);
//                if(ret.Result == (int)EProjectOperateResult.POR_Success)
//                {
//                    if(_isOpen && user == _user)
//                    {
//                        LocalUser.Instance.User.OnUnpublishProject(projectList);
//                        SetMode(EMode.Normal);
//                        RefreshView();
//                    }
//                }
//                else
//                {
//                    if(_isOpen && user == _user)
//                    {
//                        CommonTools.ShowPopupDialog("删除失败");
//                    }
//                }
//            }, (intCode, errMsg)=>{
//                if(_isOpen  && user == _user)
//                {
//                    ENetResultCode code = (ENetResultCode)intCode;
//                    if(code == ENetResultCode.NR_NetworkNotReachable)
//                    {
//                        CommonTools.ShowPopupDialog("删除失败，当前网络环境不佳，在网络良好的条件下重新操作");
//                    }
//                    else if(code == ENetResultCode.NR_Timeout)
//                    {
//                        CommonTools.ShowPopupDialog("删除操作超时，在网络良好的条件下重新操作");
//                    }
//                    else if(code == ENetResultCode.NR_Unauthorized)
//                    {
//                        CommonTools.ShowPopupDialog("删除失败，您没有权限操作");
//                    }
//                    else
//                    {
//                        CommonTools.ShowPopupDialog("删除失败，未知错误");
//                    }
//                }
//            });
        }
        #endregion

        #region 事件处理

        private void OnDeleteButtonClick()
        {
            if(_currentSelectedCount == 0)
            {
                return;
            }
            List<Project> projectList = new List<Project>(_currentSelectedCount);
            for(int i=0; i<_content.Count; i++)
            {
                var w = _content[i];
                if(w.IsSelected)
                {
                    projectList.Add(w.Content);
                }
            }
            CommonTools.ShowPopupDialog(string.Format("确定要删除这 {0} 个作品吗？", _currentSelectedCount), null,
                new KeyValuePair<string, Action>("确定",()=>{
                    RequestUnpublish(projectList);
                }), new KeyValuePair<string, Action>("取消", ()=>{
                    LogHelper.Info("Cancel Delete");
                }));
        }

        private void OnCancelButtonClick()
        {
            SetMode(EMode.Normal);
        }

        #endregion 事件处理

        #region 接口
        public object GetTitle()
        {
            if(_user == LocalUser.Instance.User)
            {
                return "我的发布";
            }
            else
            {
                return _user.NickName+"的发布";
            }
        }

        public Button GetRightButton()
        {
            if(_user != LocalUser.Instance.User)
            {
                return null;
            }
            if(_mode == EMode.None)
            {
                return null;
            }
            else if(_mode == EMode.Normal)
            {
                return _cachedView.EditButtonRightResource;
            }
            else if(_mode == EMode.Edit)
            {
                return _cachedView.CancelButtonRightResource;
            }
            return null;
        }

//        public void OnRightButtonClick(UICtrlTitlebar titleBar)
//        {
//            if(_mode == EMode.Edit)
//            {
//                SetMode(EMode.Normal);
//            }
//            else
//            {
//                SetMode(EMode.Edit);
//            }
//        }

        #endregion

        private enum EMode
        {
            None,
            Normal,
            Edit,
        }
    }
}

