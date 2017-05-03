// 关卡扩展信息 | 关卡扩展信息
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ProjectExtend : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 关卡Id
        /// </summary>
        private long _projectId;
        /// <summary>
        /// 
        /// </summary>
        private bool _isValid;
        /// <summary>
        /// 
        /// </summary>
        private float _rate;
        /// <summary>
        /// 
        /// </summary>
        private int _rateCount;
        /// <summary>
        /// 
        /// </summary>
        private int _commentCount;
        /// <summary>
        /// 
        /// </summary>
        private long _playCount;
        /// <summary>
        /// 
        /// </summary>
        private int _completeCount;
        /// <summary>
        /// 
        /// </summary>
        private int _failCount;
        /// <summary>
        /// 
        /// </summary>
        private int _likeCount;
        /// <summary>
        /// 
        /// </summary>
        private int _favoriteCount;
        /// <summary>
        /// 
        /// </summary>
        private int _downloadCount;
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public bool IsValid { 
            get { return _isValid; }
            set { if (_isValid != value) {
                _isValid = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public float Rate { 
            get { return _rate; }
            set { if (_rate != value) {
                _rate = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int RateCount { 
            get { return _rateCount; }
            set { if (_rateCount != value) {
                _rateCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int CommentCount { 
            get { return _commentCount; }
            set { if (_commentCount != value) {
                _commentCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long PlayCount { 
            get { return _playCount; }
            set { if (_playCount != value) {
                _playCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int CompleteCount { 
            get { return _completeCount; }
            set { if (_completeCount != value) {
                _completeCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int FailCount { 
            get { return _failCount; }
            set { if (_failCount != value) {
                _failCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int LikeCount { 
            get { return _likeCount; }
            set { if (_likeCount != value) {
                _likeCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int FavoriteCount { 
            get { return _favoriteCount; }
            set { if (_favoriteCount != value) {
                _favoriteCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int DownloadCount { 
            get { return _downloadCount; }
            set { if (_downloadCount != value) {
                _downloadCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
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

        public bool OnSync (Msg_SC_DAT_ProjectExtend msg)
        {
            if (null == msg) return false;
            _projectId = msg.ProjectId;           
            _isValid = msg.IsValid;           
            _rate = msg.Rate;           
            _rateCount = msg.RateCount;           
            _commentCount = msg.CommentCount;           
            _playCount = msg.PlayCount;           
            _completeCount = msg.CompleteCount;           
            _failCount = msg.FailCount;           
            _likeCount = msg.LikeCount;           
            _favoriteCount = msg.FavoriteCount;           
            _downloadCount = msg.DownloadCount;           
            _shareCount = msg.ShareCount;           
            OnSyncPartial();
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
        }
        #endregion
    }
}