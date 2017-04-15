//  /********************************************************************
//  ** Filename : UICommentList.cs
//  ** Author : quan
//  ** Date : 2016/6/12 2:07
//  ** Summary : UICommentList.cs
//  ***********************************************************************/
//using System;
//using System.Collections.Generic;
//using SoyEngine;
//using SoyEngine.Proto;
//using SoyEngine;
//using UnityEngine;
//using UnityEngine.UI;
//
//namespace GameA
//{
//    public class UICommentList : MonoBehaviour
//    {
//        private const long RequestInterval = 30 * GameTimer.Second2Ticks;
//        private const int PageCount = 10;
//        private GameTimer _requestTimer;
//        private bool _isRequest = false;
//        private bool _isRequestAppend = false;
//        private bool _isEnd = false;
//        private Project _project;
//        private List<ProjectComment> _content;
//        public TableDataScroller TableScroller;
//        public ScrollRect ScrollRect;
//        public Action<UMCtrlProjectComment> ItemClickCallback;
//        private bool _hasInit = false;
//        private bool _load = true;
//
//        void Awake()
//        {
//            Init();
//        }
//
//        private void Init()
//        {
//            if(_hasInit)
//            {
//                return;
//            }
//            _hasInit = true;
//            TableScroller.SetCallback(OnItemRefresh, GetItemRenderer);
//            _isEnd = false;
//        }
//
//        public void SetProject(Project project)
//        {
//            _project = project;
//            _requestTimer = _project.ProjectCommentListRequestTimer;
//            _isEnd = false;
//            TableScroller.SetEmpty();
//            TableScroller.ScrollRect.verticalNormalizedPosition = 1;
//            if(gameObject.activeInHierarchy)
//            {
//                _load = true;
//                Refresh();
//            }
//        }
//
//        private void RequestData(bool append = false)
//        {
//            if(_isRequest && !append
//                || _isRequestAppend && append)
//            {
//                return;
//            }
//            Msg_CA_RequestProjectComment msg = new Msg_CA_RequestProjectComment();
//            if(append)
//            {
//                if(_content.Count > 0)
//                {
//                    msg.MaxUpdateTime = _content[_content.Count - 1].CreateTime;
//                }
//                _isRequestAppend = true;
//            }
//            else
//            {
//                if(_content != null && _content.Count >PageCount)
//                {
//                    msg.MinUpdateTime = _content[0].CreateTime;
//                }
//                _isRequest = true;
//            }
//            msg.MaxCount = PageCount;
//            msg.ProjectGuid = _project.Guid;
//            Project project = _project;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_ProjectCommentList>(SoyHttpApiPath.GetProjectCommentList, msg, ret=>{
//                if(append)
//                {
//                    _isRequestAppend = false;
//                }
//                else
//                {
//                    _isRequest = false;
//                }
//                project.OnSyncProjectCommentList(ret);
//                if(_project == project)
//                {
//                    _content = _project.ProjectCommentList;
//                    if(!append)
//                    {
//                        _requestTimer.Reset();
//                        TableScroller.SetEmpty();
//                    }
//                    else
//                    {
//                        TableScroller.RefreshCurrent();
//                    }
//                    if(ret.DataList.Count < PageCount && msg.MinUpdateTime == 0)
//                    {
//                        _isEnd = true;
//                    }
//                    RefreshView();
//                }
//            }, (code, msgStr)=>{
//                if(append)
//                {
//                    _isRequestAppend = false;
//                }
//                else
//                {
//                    _isRequest = false;
//                }
//            });
//        }
//
//        public void Refresh()
//        {
//            Init();
//            _load = true;
//            _content = _project.ProjectCommentList;
//            if(_content == null)
//            {
//                RequestData();
//            }
//            else
//            {
//                if(_requestTimer.GetInterval() > RequestInterval)
//                {
//                    RequestData();
//                }
//            }
//            RefreshView();
//        }
//
//        public void OnClose()
//        {
//            _load = false;
//            TableScroller.RefreshCurrent();
//        }
//
//
//        private void RefreshView()
//        {
//            Vector2 pos =  Vector2.zero;
//            if(TableScroller.ScrollRect != null && TableScroller.ScrollRect.content != null)
//            {
//                pos = TableScroller.ScrollRect.content.anchoredPosition;
//            }
//            if(_content == null)
//            {
//                TableScroller.SetEmpty();
//            }
//            else
//            {
//                TableScroller.SetItemCount(_content.Count);
//            }
//            TableScroller.ScrollRect.content.anchoredPosition = pos;
//            TableScroller.RefreshCurrent();
//        }
//
//
//        private IDataItemRenderer GetItemRenderer(RectTransform parent)
//        {
//            var item = new UMCtrlProjectComment();
//            item.SetCallback(ItemClickCallback);
//            item.Init(parent, Vector3.zero);
//            return item;
//        }
//
//        private void OnItemRefresh(IDataItemRenderer item, int inx)
//        {
//            if(_content == null)
//            {
//                return;
//            }
//            if(inx >= _content.Count)
//            {
//                LogHelper.Error("OnItemRefresh Error Inx > count");
//                return;
//            }
//            if(!_load)
//            {
//                item.Unload();
//                return;
//            }
//            item.Set(_content[inx]);
//            if(!_isEnd)
//            {
//                if(inx > _content.Count - 2)
//                {
//                    RequestData(true);
//                }
//            }
//        }
//    }
//}