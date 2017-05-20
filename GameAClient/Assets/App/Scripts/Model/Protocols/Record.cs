// 录像 | 录像
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class Record : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 
        /// </summary>
        private long _recordId;
        /// <summary>
        /// 
        /// </summary>
        private long _projectId;
        /// <summary>
        /// 作者
        /// </summary>
        private UserInfoSimple _userInfo;
        /// <summary>
        /// 
        /// </summary>
        private int _score;
        /// <summary>
        /// 
        /// </summary>
        private float _usedTime;
        /// <summary>
        /// 
        /// </summary>
        private long _createTime;
        /// <summary>
        /// 
        /// </summary>
        private string _recordPath;
        /// <summary>
        /// 
        /// </summary>
        private int _result;
        /// <summary>
        /// 
        /// </summary>
        private long _playCount;
        /// <summary>
        /// 
        /// </summary>
        private long _lastPlayTime;
        /// <summary>
        /// 
        /// </summary>
        private long _playUserCount;
        /// <summary>
        /// 
        /// </summary>
        private int _favoriteCount;
        /// <summary>
        /// 
        /// </summary>
        private int _likeCount;
        /// <summary>
        /// 
        /// </summary>
        private int _commentCount;
        /// <summary>
        /// 
        /// </summary>
        private int _shareCount;
        /// <summary>
        /// 
        /// </summary>
        private Project _projectData;
        /// <summary>
        /// 
        /// </summary>
        private bool _userBuy;
        /// <summary>
        /// 
        /// </summary>
        private bool _userLike;
        /// <summary>
        /// 
        /// </summary>
        private bool _userFavorite;

        // cs fields----------------------------------
        /// <summary>
        /// Id
        /// </summary>
        private long _cs_recordId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 
        /// </summary>
        public long RecordId { 
            get { return _recordId; }
            set { if (_recordId != value) {
                _recordId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long ProjectId { 
            get { return _projectId; }
            set { if (_projectId != value) {
                _projectId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 作者
        /// </summary>
        public UserInfoSimple UserInfo { 
            get { return _userInfo; }
            set { if (_userInfo != value) {
                _userInfo = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int Score { 
            get { return _score; }
            set { if (_score != value) {
                _score = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public float UsedTime { 
            get { return _usedTime; }
            set { if (_usedTime != value) {
                _usedTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long CreateTime { 
            get { return _createTime; }
            set { if (_createTime != value) {
                _createTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public string RecordPath { 
            get { return _recordPath; }
            set { if (_recordPath != value) {
                _recordPath = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int Result { 
            get { return _result; }
            set { if (_result != value) {
                _result = value;
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
        public long LastPlayTime { 
            get { return _lastPlayTime; }
            set { if (_lastPlayTime != value) {
                _lastPlayTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long PlayUserCount { 
            get { return _playUserCount; }
            set { if (_playUserCount != value) {
                _playUserCount = value;
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
        public int ShareCount { 
            get { return _shareCount; }
            set { if (_shareCount != value) {
                _shareCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public Project ProjectData { 
            get { return _projectData; }
            set { if (_projectData != value) {
                _projectData = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public bool UserBuy { 
            get { return _userBuy; }
            set { if (_userBuy != value) {
                _userBuy = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public bool UserLike { 
            get { return _userLike; }
            set { if (_userLike != value) {
                _userLike = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public bool UserFavorite { 
            get { return _userFavorite; }
            set { if (_userFavorite != value) {
                _userFavorite = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// Id
        /// </summary>
        public long CS_RecordId { 
            get { return _cs_recordId; }
            set { _cs_recordId = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _userInfo && _userInfo.IsDirty) {
                    return true;
                }
                if (null != _projectData && _projectData.IsDirty) {
                    return true;
                }
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 录像
		/// </summary>
		/// <param name="recordId">Id.</param>
        public void Request (
            long recordId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_recordId != recordId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_recordId = recordId;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_Record msg = new Msg_CS_DAT_Record();
                msg.RecordId = recordId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_Record>(
                    SoyHttpApiPath.Record, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_Record msg)
        {
            if (null == msg) return false;
            _recordId = msg.RecordId;           
            _projectId = msg.ProjectId;           
            if (null == _userInfo) {
                _userInfo = new UserInfoSimple(msg.UserInfo);
            } else {
                _userInfo.OnSyncFromParent(msg.UserInfo);
            }
            _score = msg.Score;           
            _usedTime = msg.UsedTime;           
            _createTime = msg.CreateTime;           
            _recordPath = msg.RecordPath;           
            _result = msg.Result;           
            _playCount = msg.PlayCount;           
            _lastPlayTime = msg.LastPlayTime;           
            _playUserCount = msg.PlayUserCount;           
            _favoriteCount = msg.FavoriteCount;           
            _likeCount = msg.LikeCount;           
            _commentCount = msg.CommentCount;           
            _shareCount = msg.ShareCount;           
            if (null == _projectData) {
                _projectData = new Project(msg.ProjectData);
            } else {
                _projectData.OnSyncFromParent(msg.ProjectData);
            }
            _userBuy = msg.UserBuy;           
            _userLike = msg.UserLike;           
            _userFavorite = msg.UserFavorite;           
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_Record msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Record (Msg_SC_DAT_Record msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Record () { 
            _userInfo = new UserInfoSimple();
            _projectData = new Project();
        }
        #endregion
    }
}