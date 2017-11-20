// 关卡扩展信息 | 关卡扩展信息
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ProjectExtend : SyncronisticData<Msg_SC_DAT_ProjectExtend> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 关卡Id
        /// </summary>
        private long _projectId;
        /// <summary>
        /// 是否有效
        /// </summary>
        private bool _isValid;
        /// <summary>
        /// 评论条数
        /// </summary>
        private int _commentCount;
        /// <summary>
        /// 被玩次数
        /// </summary>
        private long _playCount;
        /// <summary>
        /// 被通关次数
        /// </summary>
        private int _completeCount;
        /// <summary>
        /// 失败次数
        /// </summary>
        private int _failCount;
        /// <summary>
        /// 踩次数
        /// </summary>
        private int _unlikeCount;
        /// <summary>
        /// 顶次数
        /// </summary>
        private int _likeCount;
        /// <summary>
        /// 收藏次数
        /// </summary>
        private int _favoriteCount;
        /// <summary>
        /// 下载次数
        /// </summary>
        private int _downloadCount;
        /// <summary>
        /// 分享次数
        /// </summary>
        private int _shareCount;

        // cs fields----------------------------------
        /// <summary>
        /// 关卡Id
        /// </summary>
        private long _cs_projectId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 关卡Id
        /// </summary>
        public long ProjectId { 
            get { return _projectId; }
            set { if (_projectId != value) {
                _projectId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { 
            get { return _isValid; }
            set { if (_isValid != value) {
                _isValid = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 评论条数
        /// </summary>
        public int CommentCount { 
            get { return _commentCount; }
            set { if (_commentCount != value) {
                _commentCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 被玩次数
        /// </summary>
        public long PlayCount { 
            get { return _playCount; }
            set { if (_playCount != value) {
                _playCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 被通关次数
        /// </summary>
        public int CompleteCount { 
            get { return _completeCount; }
            set { if (_completeCount != value) {
                _completeCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 失败次数
        /// </summary>
        public int FailCount { 
            get { return _failCount; }
            set { if (_failCount != value) {
                _failCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 踩次数
        /// </summary>
        public int UnlikeCount { 
            get { return _unlikeCount; }
            set { if (_unlikeCount != value) {
                _unlikeCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 顶次数
        /// </summary>
        public int LikeCount { 
            get { return _likeCount; }
            set { if (_likeCount != value) {
                _likeCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 收藏次数
        /// </summary>
        public int FavoriteCount { 
            get { return _favoriteCount; }
            set { if (_favoriteCount != value) {
                _favoriteCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 下载次数
        /// </summary>
        public int DownloadCount { 
            get { return _downloadCount; }
            set { if (_downloadCount != value) {
                _downloadCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 分享次数
        /// </summary>
        public int ShareCount { 
            get { return _shareCount; }
            set { if (_shareCount != value) {
                _shareCount = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 关卡Id
        /// </summary>
        public long CS_ProjectId { 
            get { return _cs_projectId; }
            set { _cs_projectId = value; }
        }

        public override bool IsDirty {
            get {
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 关卡扩展信息
		/// </summary>
		/// <param name="projectId">关卡Id.</param>
        public void Request (
            long projectId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_projectId != projectId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_projectId = projectId;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_ProjectExtend msg = new Msg_CS_DAT_ProjectExtend();
                msg.ProjectId = projectId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_ProjectExtend>(
                    SoyHttpApiPath.ProjectExtend, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_ProjectExtend msg)
        {
            if (null == msg) return false;
            _projectId = msg.ProjectId;           
            _isValid = msg.IsValid;           
            _commentCount = msg.CommentCount;           
            _playCount = msg.PlayCount;           
            _completeCount = msg.CompleteCount;           
            _failCount = msg.FailCount;           
            _unlikeCount = msg.UnlikeCount;           
            _likeCount = msg.LikeCount;           
            _favoriteCount = msg.FavoriteCount;           
            _downloadCount = msg.DownloadCount;           
            _shareCount = msg.ShareCount;           
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_ProjectExtend msg)
        {
            if (null == msg) return false;
            _projectId = msg.ProjectId;           
            _isValid = msg.IsValid;           
            _commentCount = msg.CommentCount;           
            _playCount = msg.PlayCount;           
            _completeCount = msg.CompleteCount;           
            _failCount = msg.FailCount;           
            _unlikeCount = msg.UnlikeCount;           
            _likeCount = msg.LikeCount;           
            _favoriteCount = msg.FavoriteCount;           
            _downloadCount = msg.DownloadCount;           
            _shareCount = msg.ShareCount;           
            return true;
        } 

        public bool DeepCopy (ProjectExtend obj)
        {
            if (null == obj) return false;
            _projectId = obj.ProjectId;           
            _isValid = obj.IsValid;           
            _commentCount = obj.CommentCount;           
            _playCount = obj.PlayCount;           
            _completeCount = obj.CompleteCount;           
            _failCount = obj.FailCount;           
            _unlikeCount = obj.UnlikeCount;           
            _likeCount = obj.LikeCount;           
            _favoriteCount = obj.FavoriteCount;           
            _downloadCount = obj.DownloadCount;           
            _shareCount = obj.ShareCount;           
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_ProjectExtend msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectExtend (Msg_SC_DAT_ProjectExtend msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectExtend () { 
            OnCreate();
        }
        #endregion
    }
}