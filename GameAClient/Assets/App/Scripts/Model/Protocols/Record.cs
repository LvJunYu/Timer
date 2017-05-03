// 工坊关卡 | 工坊关卡
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class Record : SyncronisticData 
    {
        #region 字段
        // sc fields----------------------------------
        // 作者
        private UserInfoSimple _userInfo;
        // 
        private long _usedTime;
        // 
        private long _createTime;
        // 
        private string _recordPath;
        // 
        private long _recordId;
        // 
        private long _projectId;
        // 
        private int _result;
        // 
        private long _playCount;
        // 
        private long _lastPlayTime;
        // 
        private long _playUserCount;
        // 
        private int _favoriteCount;
        // 
        private int _likeCount;
        // 
        private int _commentCount;
        // 
        private int _shareCount;
        // 
        private Project _projectData;
        // 
        private bool _userBuy;
        // 
        private bool _userLike;
        // 
        private bool _userFavorite;

        // cs fields----------------------------------
        // Id
        private long _cs_recordId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        // 作者
        public UserInfoSimple UserInfo { 
            get { return _userInfo; }
            set { if (_userInfo != value) {
                _userInfo = value;
                SetDirty();
            }}
        }
        // 
        public long UsedTime { 
            get { return _usedTime; }
            set { if (_usedTime != value) {
                _usedTime = value;
                SetDirty();
            }}
        }
        // 
        public long CreateTime { 
            get { return _createTime; }
            set { if (_createTime != value) {
                _createTime = value;
                SetDirty();
            }}
        }
        // 
        public string RecordPath { 
            get { return _recordPath; }
            set { if (_recordPath != value) {
                _recordPath = value;
                SetDirty();
            }}
        }
        // 
        public long RecordId { 
            get { return _recordId; }
            set { if (_recordId != value) {
                _recordId = value;
                SetDirty();
            }}
        }
        // 
        public long ProjectId { 
            get { return _projectId; }
            set { if (_projectId != value) {
                _projectId = value;
                SetDirty();
            }}
        }
        // 
        public int Result { 
            get { return _result; }
            set { if (_result != value) {
                _result = value;
                SetDirty();
            }}
        }
        // 
        public long PlayCount { 
            get { return _playCount; }
            set { if (_playCount != value) {
                _playCount = value;
                SetDirty();
            }}
        }
        // 
        public long LastPlayTime { 
            get { return _lastPlayTime; }
            set { if (_lastPlayTime != value) {
                _lastPlayTime = value;
                SetDirty();
            }}
        }
        // 
        public long PlayUserCount { 
            get { return _playUserCount; }
            set { if (_playUserCount != value) {
                _playUserCount = value;
                SetDirty();
            }}
        }
        // 
        public int FavoriteCount { 
            get { return _favoriteCount; }
            set { if (_favoriteCount != value) {
                _favoriteCount = value;
                SetDirty();
            }}
        }
        // 
        public int LikeCount { 
            get { return _likeCount; }
            set { if (_likeCount != value) {
                _likeCount = value;
                SetDirty();
            }}
        }
        // 
        public int CommentCount { 
            get { return _commentCount; }
            set { if (_commentCount != value) {
                _commentCount = value;
                SetDirty();
            }}
        }
        // 
        public int ShareCount { 
            get { return _shareCount; }
            set { if (_shareCount != value) {
                _shareCount = value;
                SetDirty();
            }}
        }
        // 
        public Project ProjectData { 
            get { return _projectData; }
            set { if (_projectData != value) {
                _projectData = value;
                SetDirty();
            }}
        }
        // 
        public bool UserBuy { 
            get { return _userBuy; }
            set { if (_userBuy != value) {
                _userBuy = value;
                SetDirty();
            }}
        }
        // 
        public bool UserLike { 
            get { return _userLike; }
            set { if (_userLike != value) {
                _userLike = value;
                SetDirty();
            }}
        }
        // 
        public bool UserFavorite { 
            get { return _userFavorite; }
            set { if (_userFavorite != value) {
                _userFavorite = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        // Id
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
		/// 工坊关卡
		/// </summary>
		/// <param name="recordId">Id.</param>
        public void Request (
            long recordId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
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

        public bool OnSync (Msg_SC_DAT_Record msg)
        {
            if (null == msg) return false;
            if (null == _userInfo) {
                _userInfo = new UserInfoSimple(msg.UserInfo);
            } else {
                _userInfo.OnSyncFromParent(msg.UserInfo);
            }
            _usedTime = msg.UsedTime;           
            _createTime = msg.CreateTime;           
            _recordPath = msg.RecordPath;           
            _recordId = msg.RecordId;           
            _projectId = msg.ProjectId;           
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