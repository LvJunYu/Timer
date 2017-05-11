/********************************************************************
** Filename : UICtrlMyCreatedProject
** Author : Dong
** Date : 2015/4/30 16:35:16
** Summary : UICtrlMyCreatedProject
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
    public class UICtrlPersonalProjects : UISocialContentCtrlBase<UIViewPersonalProjects>
    {
//        #region 常量与字段
//        private const long RequestInterval = 1 * GameTimer.Hour2Ticks;
//        private const int PageCount = 10;
//        private List<CardDataRendererWrapper<Project>> _content = new List<CardDataRendererWrapper<Project>>();
//        private EMode _mode = EMode.None;
//        private int _currentSelectedCount = 0;
//        private GameTimer _requestTimer;
//        private bool _isRequest = false;
//        private bool _isEnd;
//        private SocialContentRefreshHelper _refreshHelper;
//        #endregion
//
//        #region 属性
//
//        #endregion
//
//        #region 方法
//        protected override void InitGroupId()
//        {
//            _groupId = (int)EUIGroupType.MainUI;
//        }
//
//        protected override void OnViewCreated()
//        {
//            base.OnViewCreated();
//            _cachedView.GridScroller.SetCallback(OnItemRefresh, GetItemRenderer);
//            _cachedView.CancelButton.onClick.AddListener(OnCancelButtonClick);
//            _cachedView.DeleteButton.onClick.AddListener(OnDeleteButtonClick);
//
//            _cachedView.RefreshController.SetRefreshCallback(()=>{ RequestData(); });
//            _refreshHelper = new SocialContentRefreshHelper(_cachedView.RefreshController, _cachedView.Content, _cachedView.EmptyTip, _cachedView.ErrorTip);
//            _isEnd = false;
//        }
//
//        protected override void InitEventListener()
//        {
//            base.InitEventListener();
//        }
//
//        protected override void OnOpen(object parameter)
//        {
//            _requestTimer = LocalUser.Instance.User.GetSavedPrjectRequestTimer();
//            SetMode(EMode.Normal);
//            Refresh();
//            base.OnOpen(parameter);
//        }
//
//        protected override void OnDestroy()
//        {
//            base.OnDestroy();
//        }
//
//        public void Refresh()
//        {
//            _isEnd = false;
//            var pList = LocalUser.Instance.User.GetSavedProjectList();
//            if(pList == null)
//            {
//                _refreshHelper.State = SocialContentRefreshHelper.EState.None;
//                _cachedView.RefreshController.StartRefresh();
//            }
//            else
//            {
//                if(_content.Count == 0)
//                {
//                    _refreshHelper.State = SocialContentRefreshHelper.EState.SuccessEmpty;
//                }
//                else
//                {
//                    _refreshHelper.State = SocialContentRefreshHelper.EState.SuccessExsit;
//                }
//            }
//            RefreshView();
//        }
//
//        private void RequestData(bool append = false)
//        {
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
//            Msg_CS_DAT_PersonalProjectList msg = new Msg_CS_DAT_PersonalProjectList();
//            if(append)
//            {
//                long minUpdateTime = long.MaxValue;
//                if(_content.Count > 0 && _content[_content.Count - 1].Content.LocalDataState == ELocalDataState.LDS_Uptodate)
//                {
//                    minUpdateTime = _content[_content.Count - 1].Content.UpdateTime;
//                }
//                else
//                {
//                    _content.ForEach(item=>{
//                        minUpdateTime = Math.Min(minUpdateTime, item.Content.UpdateTime);
//                    });
//                }
//                msg.MaxUpdateTime = minUpdateTime;
//            }
//            msg.MaxCount = PageCount;
////            User user = LocalUser.Instance.UserLegacy;
//			var user = LocalUser.Instance.User;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_PersonalProjectList>(SoyHttpApiPath.PersonalProjectList, msg, ret=>{
//                _isRequest = false;
//                if(LocalUser.Instance.User  == user)
//                {
//                    user.OnSyncUserSavedProjectList(ret);
//                    if(!append)
//                    {
//                        _requestTimer.Reset();
//                        _cachedView.RefreshController.ExitRefresh();
//                        if(user.GetSavedProjectList().Count == 0)
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
//                if(!append)
//                {
//                    if(user.GetSavedProjectList().Count == 0)
//                    {
//                        _refreshHelper.State = SocialContentRefreshHelper.EState.FailedNone;
//                    }
//                    else
//                    {
//                        _refreshHelper.State = SocialContentRefreshHelper.EState.FailedExsit;
//                    }
//                }
//            });
//        }
//
//        private void RefreshView()
//        {
//            List<Project> list = LocalUser.Instance.User.GetSavedProjectList();
//            _content.Clear();
//            _content.Capacity = list.Capacity;
//            for(int i=0; i<list.Count; i++)
//            {
//                var wrapper = new CardDataRendererWrapper<Project>(list[i], OnItemClick);
//                if(_mode == EMode.Edit)
//                {
//                    wrapper.CardMode = ECardMode.Selectable;
//                    wrapper.IsSelected = false;
//                }
//                else
//                {
//                    wrapper.CardMode = ECardMode.Normal;
//                    wrapper.IsSelected = false;
//                }
//                _content.Add(wrapper);
//            }
//            _cachedView.GridScroller.SetItemCount(_content.Count);
//            _currentSelectedCount = 0;
//
//        }
//
//        private IDataItemRenderer GetItemRenderer(RectTransform parent)
//        {
//            var item = new UMCtrlProjectCardSimple();
//            item.Init(parent, Vector3.zero);
//            return item;
//        }
//
//        private void OnItemClick(CardDataRendererWrapper<Project> item)
//        {
//            if(_mode == EMode.Normal)
//            {
//                AppLogicUtil.EditPersonalProject(item.Content);
//            }
//            else
//            {
//                item.IsSelected = !item.IsSelected;
//                item.BroadcastDataChanged();
//                if(item.IsSelected)
//                {
//                    _currentSelectedCount++;
//                    RefreshBottomMenu();
//                }
//                else
//                {
//                    _currentSelectedCount--;
//                    RefreshBottomMenu();
//                }
//            }
//        }
//
//        private void OnItemRefresh(IDataItemRenderer item, int inx)
//        {
//            if(inx >= _content.Count)
//            {
//                LogHelper.Error("OnItemRefresh Error Inx > count");
//                return;
//            }
//            item.Set(_content[inx]);
//            if(!_isEnd && _mode != EMode.Edit)
//            {
//                if(inx > _content.Count - 2)
//                {
//                    RequestData(true);
//                }
//            }
//        }
//
//        private void RefreshBottomMenu()
//        {
//            _cachedView.DeleteButtonText.text = string.Format("删除（已选 {0}）", _currentSelectedCount);
//        }
//
//        private void SetMode(EMode mode)
//        {
//            _mode = mode;
//            for(int i=0; i<_content.Count; i++)
//            {
//                if(mode == EMode.Normal || mode == EMode.None)
//                {
//                    _content[i].CardMode = ECardMode.Normal;
//                    _content[i].IsSelected = false;
//                    _content[i].BroadcastDataChanged();
//                }
//                else if(mode == EMode.Edit)
//                {
//                    _content[i].CardMode = ECardMode.Selectable;
//                    _content[i].IsSelected = false;
//                    _content[i].BroadcastDataChanged();
//                }
//            }
//            if(mode == EMode.Normal || mode == EMode.None)
//            {
//                _cachedView.BottomMenuDock.gameObject.SetActive(false);
//            }
//            else
//            {
//                _cachedView.BottomMenuDock.gameObject.SetActive(true);
//                _currentSelectedCount = 0;
//                RefreshBottomMenu();
//            }
//
////            _uiStack.Titlebar.RefreshRightButton();
//        }
//        #endregion
//
//        #region 事件处理
//
//        private void OnDeleteButtonClick()
//        {
//            if(_currentSelectedCount == 0)
//            {
//                return;
//            }
//            List<Project> projectList = new List<Project>(_currentSelectedCount);
//            for(int i=0; i<_content.Count; i++)
//            {
//                var w = _content[i];
//                if(w.IsSelected)
//                {
//                    projectList.Add(w.Content);
//                }
//            }
//            CommonTools.ShowPopupDialog(string.Format("确定要删除这 {0} 个作品吗？", _currentSelectedCount), null,
//                new KeyValuePair<string, Action>("确定",()=>{
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在删除");
//                    LocalUser.Instance.User.DeleteUserSavedProject(projectList, ()=>{
//                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                        SetMode(EMode.Normal);
//                        RefreshView();
//                    }, ()=>{
//                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                        CommonTools.ShowPopupDialog("删除失败");
//                    });
//                }), new KeyValuePair<string, Action>("取消", ()=>{
//                    LogHelper.Info("Cancel Delete");
//                }));
//        }
//
//        private void OnCancelButtonClick()
//        {
//            SetMode(EMode.Normal);
//        }
//
//        #endregion 事件处理
//
//        #region 接口
//
////        public void OnRightButtonClick(UICtrlTitlebar titleBar)
////        {
////            if(_mode == EMode.Edit)
////            {
////                SetMode(EMode.Normal);
////            }
////            else
////            {
////                SetMode(EMode.Edit);
////            }
////        }
//
//        #endregion
//
//        private enum EMode
//        {
//            None,
//            Normal,
//            Edit,
//        }
    }
}
