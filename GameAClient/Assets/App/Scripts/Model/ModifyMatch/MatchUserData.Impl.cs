// 用户匹配改造数据 | 用户匹配改造数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class MatchUserData : SyncronisticData {
        public enum EChallengeState {
            None,
            WaitForChance,
            ChanceReady,
            Selecting,
            Challenging,
            Max,
        }
        #region 字段
//        // 默认最大体力点
//        private const int DefaultEnergyCapacity = 30;
        // 默认体力增长时间／每点
        private const int DefaultChallengeGenerateTime = 300;
        public const long PublishedProjectValidTimeLength = 90 * GameTimer.Second2Ms;
        /// <summary>
        /// 玩挑战关卡的token
        /// </summary>
        private long _playChallengeToken;

        private bool _isRequestPlayChallenge;
        /// <summary>
        /// 上一次挑战成功奖励
        /// </summary>
        private Reward _lastChallengeReward = new Reward ();

        ///// <summary>
        ///// 当前正在进行的挑战关卡是否已经提交过成绩了
        ///// </summary>
        //private bool _challengeResultCommitted = false;
        #endregion

        #region 属性
        /// <summary>
        /// 下一次自动增长挑战机会的时间
        /// </summary>
        /// <returns>The generate time.</returns>
        public long NextChallengeChanceGenerateTime {
            get {
                if (_leftChallengeCount >= _challengeCapacity)
                    return long.MaxValue;
                return _leftChallengeCountRefreshTime + 1000 * DefaultChallengeGenerateTime;
            }
        }

        /// <summary>
        /// 下一次自动增长改造机会的时间
        /// </summary>
        /// <returns>The generate time.</returns>
        public long NextModifyChanceGenerateTime {
            get {
                if (_curReformState == (int)EReformState.RS_WaitForChance) {
                    return _curPublishTime + 1000 * _reformIntervalSeconds;
                } else {
                    return long.MaxValue;
                }
            }
        }

        public Reward LastChallengeReward
        {
            get
            {
                return _lastChallengeReward;
            }
        }

        /// <summary>
        /// 当前正在进行的挑战关卡是否已经提交过成绩了
        /// </summary>
        public bool ChallengeResultCommitted
        {
            get
            {
                return _playChallengeToken <= 0;
            }
        }

        public long PlayChallengeToken
        {
            get
            {
                return _playChallengeToken;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获得特定id的可添加地块数量
        /// </summary>
        /// <returns>The can add unit number of identifier.</returns>
        /// <param name="unitId">Unit identifier.</param>
        public int GetCanAddUnitNumOfId (int unitId) {
            if (null == _unitData)
                return 0;
            for (int i = 0; i < _unitData.ItemList.Count; i++) {
                if (_unitData.ItemList [i].UnitId == unitId) {
                    return _unitData.ItemList [i].UnitCount;
                }
            }
            return 0;
        }

        public string GetProjectIconPath (int chapterIdx, int levelIdx) {
            Project advProject = null;
            if (chapterIdx >= AppData.Instance.AdventureData.ProjectList.SectionList.Count) {
                // todo out of range exception
                return string.Empty;
            }
            var section = AppData.Instance.AdventureData.ProjectList.SectionList [chapterIdx];
            if (levelIdx >= section.NormalProjectList.Count) {
                // todo out of range exception
                return string.Empty;
            }
            advProject = section.NormalProjectList [levelIdx];
            return advProject.IconPath;
        }

        /// <summary>
        /// 客户端刷新当前挑战和改造倒计时
        /// </summary>
        public void LocalRefresh () {
            long now = DateTimeUtil.GetServerTimeNowTimestampMillis ();
            // challenge
            if (LeftChallengeCount >= ChallengeCapacity)
            {
                LeftChallengeCountRefreshTime = now;
                LeftChallengeCount = ChallengeCapacity;
            }
            long passedTime = now - LeftChallengeCountRefreshTime;
            int generatedPoint = (int)(passedTime / 1000 / UnityEngine.Mathf.Max(1, ReformIntervalSeconds));
            if (generatedPoint > 0) {
                LeftChallengeCountRefreshTime += generatedPoint * 1000 * ReformIntervalSeconds;
                LeftChallengeCount += generatedPoint;
                if (_leftChallengeCount >= ChallengeCapacity) {
                    LeftChallengeCountRefreshTime = now;
                    LeftChallengeCount = ChallengeCapacity;
                }
            }


            // modify
            if (EReformState.RS_WaitForChance == (EReformState)CurReformState)
            {
                passedTime = now - CurPublishTime;
                if (passedTime > ReformIntervalSeconds * 1000) {
                    CurReformState = (int)EReformState.RS_ChanceReady;
                }
            }

        }
        /// <summary>
        /// 当天挑战状态
        /// </summary>
        /// <returns>The challenge state.</returns>
        public EChallengeState CurrentChallengeState () {
            if (!_inited)
                return EChallengeState.None;
            if (!_easyChallengeProjectData.IsInited &&
                !_mediumChallengeProjectData.IsInited &&
                !_difficultChallengeProjectData.IsInited &&
                !_randomChallengeProjectData.IsInited) {

                if (_leftChallengeCount <= 0)
                    return EChallengeState.WaitForChance;
                else
                    return EChallengeState.ChanceReady;
            } else {
                if (_curSelectedChallengeType == (int)EChallengeProjectType.CPT_None) {
                    return EChallengeState.Selecting;
                } else {
                    return EChallengeState.Challenging;
                }
            }

        }

        /// <summary>
        /// 获取当前选择的挑战project
        /// </summary>
        /// <returns>The challenge project.</returns>
        public Project CurrentChallengeProject () {
            if (!_inited)
                return null;
            if (_curSelectedChallengeType == (int)EChallengeProjectType.CPT_None)
                return null;
            if (_curSelectedChallengeType == (int)EChallengeProjectType.CPT_Easy)
                return _easyChallengeProjectData;
            if (_curSelectedChallengeType == (int)EChallengeProjectType.CPT_Medium)
                return _mediumChallengeProjectData;
            if (_curSelectedChallengeType == (int)EChallengeProjectType.CPT_Difficult)
                return _difficultChallengeProjectData;
            if (_curSelectedChallengeType == (int)EChallengeProjectType.CPT_Random)
                return _randomChallengeProjectData;
            return null;
        }

        /// <summary>
        /// 请求放弃挑战成功后的本地数据刷新
        /// </summary>
        public void OnAbandomChallengeSuccess () {
            CurSelectedChallengeType = (int)EChallengeProjectType.CPT_None;
            EasyChallengeProjectData = new Project();
            MediumChallengeProjectData = new Project();
            DifficultChallengeProjectData = new Project();
            RandomChallengeProjectData = new Project();
        }

        /// <summary>
        /// 请求玩挑战关卡
        /// </summary>
        public void PlayChallenge (Action succcessCB, Action failureCB) {
            if (_isRequestPlayChallenge)
                return;
            Project targetProject = LocalUser.Instance.MatchUserData.CurrentChallengeProject();
            if (null == targetProject || !targetProject.IsInited)
                // todo error handle
                return;
            _isRequestPlayChallenge = true;
            targetProject.PrepareRes (null);
            RemoteCommands.PlayMatchChallengeLevel (
                targetProject.ProjectId,
                msg => {
                    if (msg.ResultCode == (int)EPlayMatchChallengeLevelCode.PMCLC_Success) {
                        _playChallengeToken = msg.Token;
                        targetProject.PrepareRes(
                            () => {
                                if (null != succcessCB) {
                                    succcessCB.Invoke();
                                }
                                _isRequestPlayChallenge = false;
                                GameManager.Instance.RequestPlay(targetProject);
                                SocialGUIManager.Instance.ChangeToGameMode();
                            },
                            () => {
                                // todo err handle
                                _isRequestPlayChallenge = false;
                                if (null != failureCB) {
                                    failureCB.Invoke();
                                }
                            }
                        );
                    } else {
                        // todo err handle
                        _isRequestPlayChallenge = false;
                        if (null != failureCB) {
                            failureCB.Invoke();
                        }
                    }
                },
                code => {
                    // todo err handle
                    _isRequestPlayChallenge = false;
                    if (null != failureCB) {
                        failureCB.Invoke();
                    }
                }
            );
        }

        public void CommitChallengeResult (
            bool success,
            int usedTime,
            int score,
            int scoreItemCount,
            int killMonsterCount,
            int leftTime,
            int leftLife,
            byte [] recordBytes,
            Action successCb,
            Action<ENetResultCode> failureCb) {
            if (0 == _playChallengeToken) {
                if (null != failureCb) {
                    failureCb.Invoke (ENetResultCode.NR_Error);
                }
                return;
            }
            Msg_RecordUploadParam recordUploadParam = new Msg_RecordUploadParam()
            {
                Success = success,
                UsedTime = usedTime,
                Score = score,
                ScoreItemCount = scoreItemCount,
                KillMonsterCount = killMonsterCount,
                LeftTime = leftTime,
                LeftLife = leftLife,
                DeadPos = null
            };
            RemoteCommands.CommitMatchChallengeLevelResult (
                _playChallengeToken,
                recordUploadParam,
                msg => {
                    if ((int)ECommitMatchChallengeLevelResultCode.CMCLRC_Success == msg.ResultCode) {
                        _lastChallengeReward.OnSyncFromParent (msg.Reward);
                        _playChallengeToken = 0;
                        if (success) {
                            _curSelectedChallengeType = (int)EChallengeProjectType.CPT_None;
                            _easyChallengeProjectData = new Project ();
                            _mediumChallengeProjectData = new Project ();
                            _difficultChallengeProjectData = new Project ();
                            _randomChallengeProjectData = new Project ();
                        }
                        if (null != successCb) {
                            successCb.Invoke ();
                        }
                    } else {
                        if (null != failureCb) {
                            failureCb.Invoke(ENetResultCode.NR_Error);
                        }
                    }
                },
                code => {
                    if (null != failureCb) {
                        failureCb.Invoke(code);
                    }
                }
            );
        }
        #endregion
    }
}