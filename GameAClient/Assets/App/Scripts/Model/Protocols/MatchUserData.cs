// 用户匹配改造数据 | 用户匹配改造数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class MatchUserData : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 用户Id
        /// </summary>
        private long _userId;
        /// <summary>
        /// 可用地块数据
        /// </summary>
        private UserMatchUnitData _unitData;
        /// <summary>
        /// 最后发布时间
        /// </summary>
        private long _curPublishTime;
        /// <summary>
        /// 最后发布的关卡数据
        /// </summary>
        private Project _curPublishProject;
        /// <summary>
        /// 当前改造关卡所属章节
        /// </summary>
        private int _curReformSection;
        /// <summary>
        /// 当前改造关卡所属关卡
        /// </summary>
        private int _curReformLevel;
        /// <summary>
        /// 当前改造状态
        /// </summary>
        private int _curReformState;
        /// <summary>
        /// 改造间隔秒数
        /// </summary>
        private int _reformIntervalSeconds;
        /// <summary>
        /// 当前正在改造的关卡数据
        /// </summary>
        private Project _curReformProject;
        /// <summary>
        /// 改造可改变地块数
        /// </summary>
        private int _reformModifyUnitCapacity;
        /// <summary>
        /// 改造可添加地块数
        /// </summary>
        private int _reformAddUnitCapacity;
        /// <summary>
        /// 改造可删除地块数
        /// </summary>
        private int _reformDeleteUnitCapacity;
        /// <summary>
        /// 剩余挑战次数
        /// </summary>
        private int _leftChallengeCount;
        /// <summary>
        /// 剩余挑战次数刷新时间ms
        /// </summary>
        private long _leftChallengeCountRefreshTime;
        /// <summary>
        /// 挑战次数累计间隔秒数s
        /// </summary>
        private int _challengeIntervalSecond;
        /// <summary>
        /// 最大可累计挑战次数
        /// </summary>
        private int _challengeCapacity;
        /// <summary>
        /// 当前匹配已挑战次数
        /// </summary>
        private int _curMatchChallengeCount;
        /// <summary>
        /// 当前已选挑战难度EChallengeProjectType
        /// </summary>
        private int _curSelectedChallengeType;
        /// <summary>
        /// 简单挑战关卡数据
        /// </summary>
        private Project _easyChallengeProjectData;
        /// <summary>
        /// 中等挑战关卡数据
        /// </summary>
        private Project _mediumChallengeProjectData;
        /// <summary>
        /// 困难挑战关卡数据
        /// </summary>
        private Project _difficultChallengeProjectData;
        /// <summary>
        /// 随机挑战关卡数据
        /// </summary>
        private Project _randomChallengeProjectData;
        /// <summary>
        /// 已发布关卡奖励积攒数量
        /// </summary>
        private int _playCountForReward;
        /// <summary>
        /// 已发布关卡奖励积攒数量最大值
        /// </summary>
        private int _playCountForRewardCapacity;

        // cs fields----------------------------------
        /// <summary>
        /// 用户Id
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { 
            get { return _userId; }
            set { if (_userId != value) {
                _userId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 可用地块数据
        /// </summary>
        public UserMatchUnitData UnitData { 
            get { return _unitData; }
            set { if (_unitData != value) {
                _unitData = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 最后发布时间
        /// </summary>
        public long CurPublishTime { 
            get { return _curPublishTime; }
            set { if (_curPublishTime != value) {
                _curPublishTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 最后发布的关卡数据
        /// </summary>
        public Project CurPublishProject { 
            get { return _curPublishProject; }
            set { if (_curPublishProject != value) {
                _curPublishProject = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 当前改造关卡所属章节
        /// </summary>
        public int CurReformSection { 
            get { return _curReformSection; }
            set { if (_curReformSection != value) {
                _curReformSection = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 当前改造关卡所属关卡
        /// </summary>
        public int CurReformLevel { 
            get { return _curReformLevel; }
            set { if (_curReformLevel != value) {
                _curReformLevel = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 当前改造状态
        /// </summary>
        public int CurReformState { 
            get { return _curReformState; }
            set { if (_curReformState != value) {
                _curReformState = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 改造间隔秒数
        /// </summary>
        public int ReformIntervalSeconds { 
            get { return _reformIntervalSeconds; }
            set { if (_reformIntervalSeconds != value) {
                _reformIntervalSeconds = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 当前正在改造的关卡数据
        /// </summary>
        public Project CurReformProject { 
            get { return _curReformProject; }
            set { if (_curReformProject != value) {
                _curReformProject = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 改造可改变地块数
        /// </summary>
        public int ReformModifyUnitCapacity { 
            get { return _reformModifyUnitCapacity; }
            set { if (_reformModifyUnitCapacity != value) {
                _reformModifyUnitCapacity = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 改造可添加地块数
        /// </summary>
        public int ReformAddUnitCapacity { 
            get { return _reformAddUnitCapacity; }
            set { if (_reformAddUnitCapacity != value) {
                _reformAddUnitCapacity = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 改造可删除地块数
        /// </summary>
        public int ReformDeleteUnitCapacity { 
            get { return _reformDeleteUnitCapacity; }
            set { if (_reformDeleteUnitCapacity != value) {
                _reformDeleteUnitCapacity = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 剩余挑战次数
        /// </summary>
        public int LeftChallengeCount { 
            get { return _leftChallengeCount; }
            set { if (_leftChallengeCount != value) {
                _leftChallengeCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 剩余挑战次数刷新时间ms
        /// </summary>
        public long LeftChallengeCountRefreshTime { 
            get { return _leftChallengeCountRefreshTime; }
            set { if (_leftChallengeCountRefreshTime != value) {
                _leftChallengeCountRefreshTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 挑战次数累计间隔秒数s
        /// </summary>
        public int ChallengeIntervalSecond { 
            get { return _challengeIntervalSecond; }
            set { if (_challengeIntervalSecond != value) {
                _challengeIntervalSecond = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 最大可累计挑战次数
        /// </summary>
        public int ChallengeCapacity { 
            get { return _challengeCapacity; }
            set { if (_challengeCapacity != value) {
                _challengeCapacity = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 当前匹配已挑战次数
        /// </summary>
        public int CurMatchChallengeCount { 
            get { return _curMatchChallengeCount; }
            set { if (_curMatchChallengeCount != value) {
                _curMatchChallengeCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 当前已选挑战难度EChallengeProjectType
        /// </summary>
        public int CurSelectedChallengeType { 
            get { return _curSelectedChallengeType; }
            set { if (_curSelectedChallengeType != value) {
                _curSelectedChallengeType = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 简单挑战关卡数据
        /// </summary>
        public Project EasyChallengeProjectData { 
            get { return _easyChallengeProjectData; }
            set { if (_easyChallengeProjectData != value) {
                _easyChallengeProjectData = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 中等挑战关卡数据
        /// </summary>
        public Project MediumChallengeProjectData { 
            get { return _mediumChallengeProjectData; }
            set { if (_mediumChallengeProjectData != value) {
                _mediumChallengeProjectData = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 困难挑战关卡数据
        /// </summary>
        public Project DifficultChallengeProjectData { 
            get { return _difficultChallengeProjectData; }
            set { if (_difficultChallengeProjectData != value) {
                _difficultChallengeProjectData = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 随机挑战关卡数据
        /// </summary>
        public Project RandomChallengeProjectData { 
            get { return _randomChallengeProjectData; }
            set { if (_randomChallengeProjectData != value) {
                _randomChallengeProjectData = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 已发布关卡奖励积攒数量
        /// </summary>
        public int PlayCountForReward { 
            get { return _playCountForReward; }
            set { if (_playCountForReward != value) {
                _playCountForReward = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 已发布关卡奖励积攒数量最大值
        /// </summary>
        public int PlayCountForRewardCapacity { 
            get { return _playCountForRewardCapacity; }
            set { if (_playCountForRewardCapacity != value) {
                _playCountForRewardCapacity = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 用户Id
        /// </summary>
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _unitData && _unitData.IsDirty) {
                    return true;
                }
                if (null != _curPublishProject && _curPublishProject.IsDirty) {
                    return true;
                }
                if (null != _curReformProject && _curReformProject.IsDirty) {
                    return true;
                }
                if (null != _easyChallengeProjectData && _easyChallengeProjectData.IsDirty) {
                    return true;
                }
                if (null != _mediumChallengeProjectData && _mediumChallengeProjectData.IsDirty) {
                    return true;
                }
                if (null != _difficultChallengeProjectData && _difficultChallengeProjectData.IsDirty) {
                    return true;
                }
                if (null != _randomChallengeProjectData && _randomChallengeProjectData.IsDirty) {
                    return true;
                }
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 用户匹配改造数据
		/// </summary>
		/// <param name="userId">用户Id.</param>
        public void Request (
            long userId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_userId != userId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_userId = userId;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_MatchUserData msg = new Msg_CS_DAT_MatchUserData();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_MatchUserData>(
                    SoyHttpApiPath.MatchUserData, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_MatchUserData msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            if (null == _unitData) {
                _unitData = new UserMatchUnitData(msg.UnitData);
            } else {
                _unitData.OnSyncFromParent(msg.UnitData);
            }
            _curPublishTime = msg.CurPublishTime;           
            if (null == _curPublishProject) {
                _curPublishProject = new Project(msg.CurPublishProject);
            } else {
                _curPublishProject.OnSyncFromParent(msg.CurPublishProject);
            }
            _curReformSection = msg.CurReformSection;           
            _curReformLevel = msg.CurReformLevel;           
            _curReformState = msg.CurReformState;           
            _reformIntervalSeconds = msg.ReformIntervalSeconds;           
            if (null == _curReformProject) {
                _curReformProject = new Project(msg.CurReformProject);
            } else {
                _curReformProject.OnSyncFromParent(msg.CurReformProject);
            }
            _reformModifyUnitCapacity = msg.ReformModifyUnitCapacity;           
            _reformAddUnitCapacity = msg.ReformAddUnitCapacity;           
            _reformDeleteUnitCapacity = msg.ReformDeleteUnitCapacity;           
            _leftChallengeCount = msg.LeftChallengeCount;           
            _leftChallengeCountRefreshTime = msg.LeftChallengeCountRefreshTime;           
            _challengeIntervalSecond = msg.ChallengeIntervalSecond;           
            _challengeCapacity = msg.ChallengeCapacity;           
            _curMatchChallengeCount = msg.CurMatchChallengeCount;           
            _curSelectedChallengeType = msg.CurSelectedChallengeType;           
            if (null == _easyChallengeProjectData) {
                _easyChallengeProjectData = new Project(msg.EasyChallengeProjectData);
            } else {
                _easyChallengeProjectData.OnSyncFromParent(msg.EasyChallengeProjectData);
            }
            if (null == _mediumChallengeProjectData) {
                _mediumChallengeProjectData = new Project(msg.MediumChallengeProjectData);
            } else {
                _mediumChallengeProjectData.OnSyncFromParent(msg.MediumChallengeProjectData);
            }
            if (null == _difficultChallengeProjectData) {
                _difficultChallengeProjectData = new Project(msg.DifficultChallengeProjectData);
            } else {
                _difficultChallengeProjectData.OnSyncFromParent(msg.DifficultChallengeProjectData);
            }
            if (null == _randomChallengeProjectData) {
                _randomChallengeProjectData = new Project(msg.RandomChallengeProjectData);
            } else {
                _randomChallengeProjectData.OnSyncFromParent(msg.RandomChallengeProjectData);
            }
            _playCountForReward = msg.PlayCountForReward;           
            _playCountForRewardCapacity = msg.PlayCountForRewardCapacity;           
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_MatchUserData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public MatchUserData (Msg_SC_DAT_MatchUserData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public MatchUserData () { 
            _unitData = new UserMatchUnitData();
            _curPublishProject = new Project();
            _curReformProject = new Project();
            _easyChallengeProjectData = new Project();
            _mediumChallengeProjectData = new Project();
            _difficultChallengeProjectData = new Project();
            _randomChallengeProjectData = new Project();
            OnCreate();
        }
        #endregion
    }
}