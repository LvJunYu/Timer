//   /********************************************************************
//   ** Filename : AdventureData.cs
//   ** Author : quan
//   ** Date : 3/10/2017 3:24 PM
//   ** Summary : AdventureData.cs
//   ***********************************************************************/

using System;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using PlayMode = GameA.Game.PlayMode;

namespace GameA
{
    public class AdventureData
    {
        #region 字段

        private AdventureProjectList _projectList = new AdventureProjectList();
        private AdventureUserData _userData = new AdventureUserData();

        private long _lastRequiredLevelToken;

        private EAdventureProjectType _lastPlayedLevelType;
        private int _lastPlayedChapterIdx;
        private int _lastPlayedLevelIdx;

        /// <summary>
        /// 上次冒险关卡奖励
        /// </summary>
        private Reward _lastAdvReward;

        /// <summary>
        /// 上次冒险关卡得星数
        /// </summary>
        private int _lastAdvStarCnt;

        /// <summary>
        /// 上次冒险是否过关
        /// </summary>
        private bool _lastAdvSuccess;

        #endregion 字段

        #region 属性

        public AdventureProjectList ProjectList
        {
            get { return _projectList; }
        }

        public AdventureUserData UserData
        {
            get { return _userData; }
        }

        public Reward LastAdvReward
        {
            get { return _lastAdvReward; }
        }

        public int LastAdvStarCnt
        {
            get { return _lastAdvStarCnt; }
        }

        public long LastRequiredLevelToken
        {
            get { return _lastRequiredLevelToken; }
        }

        public bool LastAdvSuccess
        {
            get { return _lastAdvSuccess; }
        }

        public EAdventureProjectType LastPlayedLevelType
        {
            get { return _lastPlayedLevelType; }
        }

        public int LastPlayedChapterIdx
        {
            get { return _lastPlayedChapterIdx; }
        }

        public int LastPlayedLevelIdx
        {
            get { return _lastPlayedLevelIdx; }
        }

        #endregion 属性

        #region 方法

        public void Init()
        {
            _projectList = new AdventureProjectList();
            _userData = new AdventureUserData();
        }

        public void PrepareAllData(Action successCallback, Action<ENetResultCode> failedCallback)
        {
            ParallelTaskHelper<ENetResultCode> taskHelper =
                new ParallelTaskHelper<ENetResultCode>(successCallback, failedCallback);
            taskHelper.AddTask(PrepareAdventureProjectList);
            taskHelper.AddTask(PrepareAdventureUserData);
        }

        public void PrepareAdventureProjectList(Action successCallback, Action<ENetResultCode> failedCallback)
        {
            _projectList.Request(0, 5, successCallback, failedCallback);
        }

        public void PrepareAdventureUserData(Action successCallback, Action<ENetResultCode> failedCallback)
        {
            _userData.Request(LocalUser.Instance.UserGuid, successCallback, failedCallback);
        }

        public bool TryGetNextNormalLevel(int section, int level, out int nextLevel)
        {
            nextLevel = level;
            if (level >= 9)
            {
                return false;
            }
            nextLevel = level + 1;
            return true;
        }

        public AdventureUserLevelDataDetail GetAdventureUserLevelDataDetail(int section,
            EAdventureProjectType projectType,
            int level)
        {
            if (null == _userData)
            {
                return null;
            }
            if (section <= 0
                || projectType == EAdventureProjectType.APT_None
                || level <= 0)
            {
                return null;
            }
            for (int i = _userData.SectionList.Count; i < section; i++)
            {
                _userData.SectionList.Add(new AdventureUserSection() {Section = i + 1});
            }
            var sectionData = _userData.SectionList[section - 1];

            List<AdventureUserLevelDataDetail> projectList = projectType == EAdventureProjectType.APT_Normal
                ? sectionData.NormalLevelUserDataList
                : sectionData.BonusLevelUserDataList;
            for (int i = projectList.Count; i < level; i++)
            {
                projectList.Add(new AdventureUserLevelDataDetail());
            }
            return projectList[level - 1];
        }

        /// <summary>
        /// 请求进入冒险模式关卡
        /// </summary>
        /// <param name="sectionId">章节id</param>
        /// <param name="levelIdx">关卡id</param>
        /// <param name="type">关卡类型</param>
        /// <param name="successCallback">Success callback.</param>
        /// <param name="failedCallback">Failed callback.</param>
        public void PlayAdventureLevel(
            int sectionId,
            int levelIdx,
            EAdventureProjectType type,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            Project project = GetAdvLevelProject(sectionId, type, levelIdx);
            if (null == project)
            {
                if (null != failedCallback)
                {
                    failedCallback.Invoke(ENetResultCode.NR_None);
                }
                return;
            }
            RemoteCommands.PlayAdventureLevel(sectionId, type, levelIdx, ret =>
            {
                if (ret.ResultCode == (int) EPlayAdventureLevelCode.PALC_Success)
                {
                    _lastRequiredLevelToken = ret.Token;
                    _lastPlayedLevelType = type;
                    _lastPlayedChapterIdx = sectionId;
                    _lastPlayedLevelIdx = levelIdx;
                    _lastAdvSuccess = false;
                    if (null != successCallback)
                    {
                        successCallback.Invoke();
                        project.PrepareRes(() =>
                        {
                            var param = new SituationAdventureParam();
                            param.ProjectType = type;
                            param.Section = sectionId;
                            param.Level = levelIdx;
                            if (EAdventureProjectType.APT_Bonus == type)
                            {
                                GameManager.Instance.RequestPlayAdvBonus(project, param);
                            }
                            else
                            {
                                GameManager.Instance.RequestPlayAdvNormal(project, param);
                            }
                            SocialApp.Instance.ChangeToGame();
                        });
                    }
                }
                else
                {
                    if (null != failedCallback)
                    {
                        failedCallback.Invoke(ENetResultCode.NR_None);
                    }
                }
            }, errorCode =>
            {
                if (null != failedCallback)
                {
                    failedCallback.Invoke(errorCode);
                }
            });
        }

        public void TestPlayAdventureLevel(
            int sectionId,
            int levelIdx,
            EAdventureProjectType type,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            _lastPlayedLevelType = type;
            _lastPlayedChapterIdx = sectionId;
            _lastPlayedLevelIdx = levelIdx;
            _lastAdvSuccess = true;
            var levelDetail = GetAdventureUserLevelDataDetail(sectionId, type, levelIdx);
            levelDetail.SimpleData.Star1Flag = true;
            levelDetail.SimpleData.Star2Flag = false;
            levelDetail.SimpleData.Star3Flag = true;
            levelDetail.SimpleData.SuccessCount++;
            levelDetail.SimpleData.LastPlayTime = DateTimeUtil.GetNowUnixTimestampMillis();
            _userData.AdventureUserProgress.CompleteLevel++;
            if (successCallback != null)
            {
                successCallback.Invoke();
            }
        }
        
        /// <summary>
        /// 重新尝试冒险关卡
        /// </summary>
        public void RetryAdvLevel(Action successCallback, Action<ENetResultCode> failedCallback)
        {
            PlayAdventureLevel(_lastPlayedChapterIdx, _lastPlayedLevelIdx, _lastPlayedLevelType,
                successCallback, failedCallback);
        }

        public Table_StandaloneLevel GetAdvLevelTable(int chapteIdx, EAdventureProjectType type, int levelIdx)
        {
            if (chapteIdx == 0 || levelIdx == 0) return null;

            var tableChapter = TableManager.Instance.GetStandaloneChapter(chapteIdx);
            if (null == tableChapter)
            {
                return null;
            }
            int levelId;
            if (EAdventureProjectType.APT_Bonus == type)
            {
                if (levelIdx <= tableChapter.BonusLevels.Length)
                {
                    levelId = tableChapter.BonusLevels[levelIdx - 1];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (levelIdx <= tableChapter.NormalLevels.Length)
                {
                    levelId = tableChapter.NormalLevels[levelIdx - 1];
                }
                else
                {
                    return null;
                }
            }
            return TableManager.Instance.GetStandaloneLevel(levelId);
        }

        public Project GetAdvLevelProject(int chapteIdx, EAdventureProjectType type, int levelIdx)
        {
            if (chapteIdx <= 0 || levelIdx <= 0) return null;
            int chapteInx = chapteIdx - 1;
            int levelInx = levelIdx - 1;
            if (_projectList.SectionList == null || _projectList.SectionList.Count <= chapteInx)
            {
                return null;
            }
            AdventureSection section = _projectList.SectionList[chapteInx];
            List<Project> projectList;
            if (EAdventureProjectType.APT_Normal == type)
            {
                projectList = section.NormalProjectList;
            }
            else
            {
                projectList = section.BonusProjectList;
            }
            if (projectList == null || projectList.Count <= levelInx)
            {
                return null;
            }
            return projectList[levelInx];
        }


        /// <summary>
        /// 提交冒险模式关卡成绩
        /// </summary>
        /// <param name="success">是否过关</param>
        /// <param name="usedTime">使用的时间</param>
        /// <param name="score">最终得分</param>
        /// <param name="scoreItemCount">奖分道具数</param>
        /// <param name="killMonsterCount">击杀怪物数</param>
        /// <param name="leftTime">剩余时间数</param>
        /// <param name="leftLife">剩余生命</param>
        /// <param name="recordBytes">关卡数据</param>
        /// <param name="successCallback">Success callback.</param>
        /// <param name="failedCallback">Failed callback.</param>
        public void CommitLevelResult(
            bool success,
            int usedTime,
            int score,
            int scoreItemCount,
            int killMonsterCount,
            int leftTime,
            int leftLife,
            byte[] recordBytes,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_lastRequiredLevelToken == 0)
            {
                if (null != failedCallback)
                {
                    failedCallback.Invoke(ENetResultCode.NR_None);
                }
                return;
            }
            var table = GetAdvLevelTable(_lastPlayedChapterIdx, _lastPlayedLevelType, _lastPlayedLevelIdx);
            if (null == table)
            {
                if (null != failedCallback)
                {
                    failedCallback.Invoke(ENetResultCode.NR_None);
                }
                return;
            }

            bool star1 = false;
            bool star2 = false;
            bool star3 = false;
            if (null != table.StarConditions)
            {
                if (table.StarConditions.Length > 0)
                {
                    star1 = CheckStarRequire(table.StarConditions[0], table.Star1Value, PlayMode.Instance.Statistic);
                }
                if (table.StarConditions.Length > 1)
                {
                    star2 = CheckStarRequire(table.StarConditions[1], table.Star2Value, PlayMode.Instance.Statistic);
                }
                if (table.StarConditions.Length > 2)
                {
                    star3 = CheckStarRequire(table.StarConditions[2], table.Star3Value, PlayMode.Instance.Statistic);
                }
            }

            // 记录得星数以便在结算界面显示
            _lastAdvStarCnt = (star1 ? 1 : 0) + (star2 ? 1 : 0) + (star3 ? 1 : 0);

            WWWForm form = new WWWForm();
            form.AddBinaryData("recordFile", recordBytes);

            Msg_RecordUploadParam recordUploadParam = new Msg_RecordUploadParam
            {
                Success = success,
                UsedTime = usedTime,
                Star1Flag = star1,
                Star2Flag = star2,
                Star3Flag = star3,
                Score = score,
                ScoreItemCount = scoreItemCount,
                KillMonsterCount = killMonsterCount,
                LeftTime = leftTime,
                LeftLife = leftLife,
                DeadPos = null
            };
            RemoteCommands.CommitAdventureLevelResult(
                _lastRequiredLevelToken, recordUploadParam, ret =>
                {
                    if ((int) ECommitAdventureLevelResultCode.CALRC_Success == ret.ResultCode)
                    {
                        _lastAdvSuccess = success;
                        if (null != successCallback)
                        {
                            _lastRequiredLevelToken = 0;
                            LocalRefreshCommitedLevelData(success, star1, star2, star3);
                            if (ret.Reward != null)
                            {
                                _lastAdvReward = new Reward(ret.Reward);
                                for (int i = 0; i < _lastAdvReward.ItemList.Count; i++)
                                {
                                    _lastAdvReward.ItemList[i].AddToLocal();
                                }
                            }
                            successCallback.Invoke();
                        }
                    }
                    else
                    {
                        if (null != failedCallback)
                        {
                            failedCallback.Invoke(ENetResultCode.NR_Error);
                        }
                    }
                },
                code =>
                {
                    if (null != failedCallback)
                    {
                        failedCallback.Invoke(code);
                    }
                },
                form
            );
        }

        // 成功提交冒险模式通关数据后，客户端自主刷新数据
        private void LocalRefreshCommitedLevelData(
            bool success,
            bool star1Flag,
            bool star2Flag,
            bool star3Flag)
        {
            AdventureUserLevelDataDetail levelData =
                GetAdventureUserLevelDataDetail(_lastPlayedChapterIdx, _lastPlayedLevelType, _lastPlayedLevelIdx);

            levelData.SimpleData.Star1Flag = star1Flag;
            levelData.SimpleData.Star2Flag = star2Flag;
            levelData.SimpleData.Star3Flag = star3Flag;
            if (success)
            {
                levelData.SimpleData.SuccessCount++;
                if (_userData.AdventureUserProgress.CompleteSection < _lastPlayedChapterIdx)
                {
                    _userData.AdventureUserProgress.CompleteSection = _lastPlayedChapterIdx;
                    _userData.AdventureUserProgress.CompleteLevel = 1;
                }
                else if (_userData.AdventureUserProgress.CompleteLevel < _lastPlayedLevelIdx)
                {
                    _userData.AdventureUserProgress.CompleteLevel = _lastPlayedLevelIdx;
                }
            }
        }

        // 检查三星条件
        public bool CheckStarRequire(int starType, int starValue, GameStatistic statistic)
        {
            if (null == statistic)
            {
                return false;
            }
            // 通过本关
            if (starType == 1)
            {
                return statistic.Passed;
            }
            // 通关用时少于N秒
            if (starType == 2)
            {
                return statistic.UsedTime < starValue;
            }
            // 死亡次数少于N次
            if (starType == 3)
            {
                return statistic.DeathCnt < starValue;
            }
            // 得分超过N
            if (starType == 4)
            {
                return statistic.Score >= starValue;
            }
            // 拾取所有钻石
            if (starType == 5)
            {
                return statistic.CollectGem == PlayMode.Instance.SceneState.TotalGem;
            }
            // 消灭所有怪物
            if (starType == 6)
            {
                return statistic.MonsterDeathCnt == PlayMode.Instance.SceneState.MonsterCount;
            }
            // 行走距离小于米
            if (starType == 7)
            {
                return false;
            }
            // 跳跃次数小于N次
            if (starType == 8)
            {
                return statistic.JumpCnt < starValue;
            }
            // 消灭的怪物少于N个
            if (starType == 9)
            {
                return statistic.MonsterDeathCnt < starValue;
            }
            // 触发开关少于N次
            if (starType == 10)
            {
                return statistic.SwitchTriggerCnt < starValue;
            }
            // 不使用传送门
            if (starType == 11)
            {
                return statistic.PortalUsedCnt == 0;
            }
            // 用激光消灭N个怪物
            if (starType == 12)
            {
                return statistic.MonsterKilledByLazerCnt >= starValue;
            }
            // 使怪物坠亡N次
            if (starType == 13)
            {
                return statistic.MonsterKilledByFallCnt >= starValue;
            }
            // 剩余血量大于{0}%
            if (starType == 14)
            {
                float currentHPPercentage =
                    (float) PlayerManager.Instance.MainPlayer.Hp /
                    PlayerManager.Instance.MainPlayer.MaxHp;
                return currentHPPercentage > starValue * 0.01f;
            }
            return false;
        }

        
        public static int GetNormalProgress(int section, int level) {
            return Mathf.Max(0, (section-1) * 9 + level);
        }

        public static int GetNormalSectionByProgress(int val) {
            return (val - 1) / 9 + 1;
        }

        public static int GetNormalLevelByProgress(int val) {
            return (val - 1) % 9 + 1;
        }

        #endregion 方法
    }
}