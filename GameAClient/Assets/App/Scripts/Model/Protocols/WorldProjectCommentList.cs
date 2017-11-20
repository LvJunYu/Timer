// 获取世界关卡评论列表 | 获取世界关卡评论列表
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class WorldProjectCommentList : SyncronisticData<Msg_SC_DAT_WorldProjectCommentList> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// ECachedDataState
        /// </summary>
        private int _resultCode;
        /// <summary>
        /// 
        /// </summary>
        private long _updateTime;
        /// <summary>
        /// 
        /// </summary>
        private List<ProjectComment> _commentList;
        /// <summary>
        /// 
        /// </summary>
        private int _totalCount;

        // cs fields----------------------------------
        /// <summary>
        /// 
        /// </summary>
        private long _cs_projectId;
        /// <summary>
        /// 
        /// </summary>
        private int _cs_startInx;
        /// <summary>
        /// 
        /// </summary>
        private int _cs_maxCount;
        /// <summary>
        /// 排序字段
        /// </summary>
        private EProjectCommentOrderBy _cs_orderBy;
        /// <summary>
        /// 升序降序
        /// </summary>
        private EOrderType _cs_orderType;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// ECachedDataState
        /// </summary>
        public int ResultCode { 
            get { return _resultCode; }
            set { if (_resultCode != value) {
                _resultCode = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long UpdateTime { 
            get { return _updateTime; }
            set { if (_updateTime != value) {
                _updateTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public List<ProjectComment> CommentList { 
            get { return _commentList; }
            set { if (_commentList != value) {
                _commentList = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int TotalCount { 
            get { return _totalCount; }
            set { if (_totalCount != value) {
                _totalCount = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 
        /// </summary>
        public long CS_ProjectId { 
            get { return _cs_projectId; }
            set { _cs_projectId = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int CS_StartInx { 
            get { return _cs_startInx; }
            set { _cs_startInx = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int CS_MaxCount { 
            get { return _cs_maxCount; }
            set { _cs_maxCount = value; }
        }
        /// <summary>
        /// 排序字段
        /// </summary>
        public EProjectCommentOrderBy CS_OrderBy { 
            get { return _cs_orderBy; }
            set { _cs_orderBy = value; }
        }
        /// <summary>
        /// 升序降序
        /// </summary>
        public EOrderType CS_OrderType { 
            get { return _cs_orderType; }
            set { _cs_orderType = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _commentList) {
                    for (int i = 0; i < _commentList.Count; i++) {
                        if (null != _commentList[i] && _commentList[i].IsDirty) {
                            return true;
                        }
                    }
                }
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 获取世界关卡评论列表
		/// </summary>
		/// <param name="projectId">.</param>
		/// <param name="startInx">.</param>
		/// <param name="maxCount">.</param>
		/// <param name="orderBy">排序字段.</param>
		/// <param name="orderType">升序降序.</param>
        public void Request (
            long projectId,
            int startInx,
            int maxCount,
            EProjectCommentOrderBy orderBy,
            EOrderType orderType,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_projectId != projectId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_startInx != startInx) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_maxCount != maxCount) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_orderBy != orderBy) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_orderType != orderType) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_projectId = projectId;
                _cs_startInx = startInx;
                _cs_maxCount = maxCount;
                _cs_orderBy = orderBy;
                _cs_orderType = orderType;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_WorldProjectCommentList msg = new Msg_CS_DAT_WorldProjectCommentList();
                msg.ProjectId = projectId;
                msg.StartInx = startInx;
                msg.MaxCount = maxCount;
                msg.OrderBy = orderBy;
                msg.OrderType = orderType;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_WorldProjectCommentList>(
                    SoyHttpApiPath.WorldProjectCommentList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_WorldProjectCommentList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            _commentList = new List<ProjectComment>();
            for (int i = 0; i < msg.CommentList.Count; i++) {
                _commentList.Add(new ProjectComment(msg.CommentList[i]));
            }
            _totalCount = msg.TotalCount;           
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_WorldProjectCommentList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            if (null ==  _commentList) {
                _commentList = new List<ProjectComment>();
            }
            _commentList.Clear();
            for (int i = 0; i < msg.CommentList.Count; i++) {
                _commentList.Add(new ProjectComment(msg.CommentList[i]));
            }
            _totalCount = msg.TotalCount;           
            return true;
        } 

        public bool DeepCopy (WorldProjectCommentList obj)
        {
            if (null == obj) return false;
            _resultCode = obj.ResultCode;           
            _updateTime = obj.UpdateTime;           
            if (null ==  obj.CommentList) return false;
            if (null ==  _commentList) {
                _commentList = new List<ProjectComment>();
            }
            _commentList.Clear();
            for (int i = 0; i < obj.CommentList.Count; i++){
                _commentList.Add(obj.CommentList[i]);
            }
            _totalCount = obj.TotalCount;           
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_WorldProjectCommentList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WorldProjectCommentList (Msg_SC_DAT_WorldProjectCommentList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WorldProjectCommentList () { 
            _commentList = new List<ProjectComment>();
            OnCreate();
        }
        #endregion
    }
}