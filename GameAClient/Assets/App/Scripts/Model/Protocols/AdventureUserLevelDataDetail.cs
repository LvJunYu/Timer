// 获取冒险关卡用户数据 | 获取冒险关卡用户数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AdventureUserLevelDataDetail : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 
        /// </summary>
        private AdventureUserLevelData _simpleData;
        /// <summary>
        /// 
        /// </summary>
        private Record _highScoreRecord;
        /// <summary>
        /// 
        /// </summary>
        private Record _star1FlagRecord;
        /// <summary>
        /// 
        /// </summary>
        private Record _star2FlagRecord;
        /// <summary>
        /// 
        /// </summary>
        private Record _star3FlagRecord;
        /// <summary>
        /// 
        /// </summary>
        private UserInfoSimple _highScoreFriendInfo;
        /// <summary>
        /// 
        /// </summary>
        private List<Record> _recentRecordList;

        // cs fields----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        private long _cs_userId;
        /// <summary>
        /// 章节
        /// </summary>
        private int _cs_section;
        /// <summary>
        /// 
        /// </summary>
        private EAdventureProjectType _cs_projectType;
        /// <summary>
        /// 
        /// </summary>
        private int _cs_level;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 
        /// </summary>
        public AdventureUserLevelData SimpleData { 
            get { return _simpleData; }
            set { if (_simpleData != value) {
                _simpleData = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public Record HighScoreRecord { 
            get { return _highScoreRecord; }
            set { if (_highScoreRecord != value) {
                _highScoreRecord = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public Record Star1FlagRecord { 
            get { return _star1FlagRecord; }
            set { if (_star1FlagRecord != value) {
                _star1FlagRecord = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public Record Star2FlagRecord { 
            get { return _star2FlagRecord; }
            set { if (_star2FlagRecord != value) {
                _star2FlagRecord = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public Record Star3FlagRecord { 
            get { return _star3FlagRecord; }
            set { if (_star3FlagRecord != value) {
                _star3FlagRecord = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public UserInfoSimple HighScoreFriendInfo { 
            get { return _highScoreFriendInfo; }
            set { if (_highScoreFriendInfo != value) {
                _highScoreFriendInfo = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public List<Record> RecentRecordList { 
            get { return _recentRecordList; }
            set { if (_recentRecordList != value) {
                _recentRecordList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }
        /// <summary>
        /// 章节
        /// </summary>
        public int CS_Section { 
            get { return _cs_section; }
            set { _cs_section = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public EAdventureProjectType CS_ProjectType { 
            get { return _cs_projectType; }
            set { _cs_projectType = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int CS_Level { 
            get { return _cs_level; }
            set { _cs_level = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _simpleData && _simpleData.IsDirty) {
                    return true;
                }
                if (null != _highScoreRecord && _highScoreRecord.IsDirty) {
                    return true;
                }
                if (null != _star1FlagRecord && _star1FlagRecord.IsDirty) {
                    return true;
                }
                if (null != _star2FlagRecord && _star2FlagRecord.IsDirty) {
                    return true;
                }
                if (null != _star3FlagRecord && _star3FlagRecord.IsDirty) {
                    return true;
                }
                if (null != _highScoreFriendInfo && _highScoreFriendInfo.IsDirty) {
                    return true;
                }
                if (null != _recentRecordList) {
                    for (int i = 0; i < _recentRecordList.Count; i++) {
                        if (null != _recentRecordList[i] && _recentRecordList[i].IsDirty) {
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
		/// 获取冒险关卡用户数据
		/// </summary>
		/// <param name="userId">用户.</param>
		/// <param name="section">章节.</param>
		/// <param name="projectType">.</param>
		/// <param name="level">.</param>
        public void Request (
            long userId,
            int section,
            EAdventureProjectType projectType,
            int level,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_userId != userId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_section != section) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_projectType != projectType) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_level != level) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_userId = userId;
                _cs_section = section;
                _cs_projectType = projectType;
                _cs_level = level;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_AdventureUserLevelDataDetail msg = new Msg_CS_DAT_AdventureUserLevelDataDetail();
                msg.UserId = userId;
                msg.Section = section;
                msg.ProjectType = projectType;
                msg.Level = level;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_AdventureUserLevelDataDetail>(
                    SoyHttpApiPath.AdventureUserLevelDataDetail, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_AdventureUserLevelDataDetail msg)
        {
            if (null == msg) return false;
            if (null == _simpleData) {
                _simpleData = new AdventureUserLevelData(msg.SimpleData);
            } else {
                _simpleData.OnSyncFromParent(msg.SimpleData);
            }
            if (null == _highScoreRecord) {
                _highScoreRecord = new Record(msg.HighScoreRecord);
            } else {
                _highScoreRecord.OnSyncFromParent(msg.HighScoreRecord);
            }
            if (null == _star1FlagRecord) {
                _star1FlagRecord = new Record(msg.Star1FlagRecord);
            } else {
                _star1FlagRecord.OnSyncFromParent(msg.Star1FlagRecord);
            }
            if (null == _star2FlagRecord) {
                _star2FlagRecord = new Record(msg.Star2FlagRecord);
            } else {
                _star2FlagRecord.OnSyncFromParent(msg.Star2FlagRecord);
            }
            if (null == _star3FlagRecord) {
                _star3FlagRecord = new Record(msg.Star3FlagRecord);
            } else {
                _star3FlagRecord.OnSyncFromParent(msg.Star3FlagRecord);
            }
            if (null == _highScoreFriendInfo) {
                _highScoreFriendInfo = new UserInfoSimple(msg.HighScoreFriendInfo);
            } else {
                _highScoreFriendInfo.OnSyncFromParent(msg.HighScoreFriendInfo);
            }
            _recentRecordList = new List<Record>();
            for (int i = 0; i < msg.RecentRecordList.Count; i++) {
                _recentRecordList.Add(new Record(msg.RecentRecordList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_AdventureUserLevelDataDetail msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdventureUserLevelDataDetail (Msg_SC_DAT_AdventureUserLevelDataDetail msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdventureUserLevelDataDetail () { 
            _simpleData = new AdventureUserLevelData();
            _highScoreRecord = new Record();
            _star1FlagRecord = new Record();
            _star2FlagRecord = new Record();
            _star3FlagRecord = new Record();
            _highScoreFriendInfo = new UserInfoSimple();
            _recentRecordList = new List<Record>();
        }
        #endregion
    }
}