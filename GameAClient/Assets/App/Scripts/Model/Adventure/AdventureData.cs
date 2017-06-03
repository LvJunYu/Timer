//   /********************************************************************
//   ** Filename : AdventureData.cs
//   ** Author : quan
//   ** Date : 3/10/2017 3:24 PM
//   ** Summary : AdventureData.cs
//   ***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
	public class AdventureData
    {
        #region 字段
		private AdventureProjectList _projectList = new AdventureProjectList ();
		private AdventureUserData _userData = new AdventureUserData ();

		private long _lastRequiredLevelToken;

		private EAdventureProjectType _lastPlayedLevelType;
		private int _lastPlayedChapterIdx;
		private int _lastPlayedLevelIdx;

        /// <summary>
        /// 上次冒险关卡奖励
        /// </summary>
        private Reward _lastAdvReward;
        #endregion 字段

        #region 属性
		public AdventureProjectList ProjectList {
			get { return _projectList; }
		}
		public AdventureUserData UserData {
			get { return _userData; }
		}
        public Reward LastAdvReward {
            get { return _lastAdvReward; }
        }

        public EAdventureProjectType LastPlayedLevelType {
            get {
                return _lastPlayedLevelType;
            }
        }
        public int LastPlayedChapterIdx {
            get {
                return _lastPlayedChapterIdx;
            }
        }
        public int LastPlayedLevelIdx {
            get {
                return _lastPlayedLevelIdx;
            }
        }

        public long LastRequiredLevelToken
        {
            get
            {
                return _lastRequiredLevelToken;
            }
        }
        #endregion 属性

        #region 方法
        public void Init()
        {
			_projectList = new AdventureProjectList ();
			_userData = new AdventureUserData ();
        }

        public void PrepareAllData(Action successCallback, Action<ENetResultCode> failedCallback)
        {
            ParallelTaskHelper<ENetResultCode> taskHelper = new ParallelTaskHelper<ENetResultCode>(successCallback, failedCallback);
            taskHelper.AddTask(PrepareAdventureProjectList);
            taskHelper.AddTask(PrepareAdventureUserData);
        }

        public void PrepareAdventureProjectList(Action successCallback, Action<ENetResultCode> failedCallback)
        {
			_projectList.Request(0,5, successCallback, failedCallback);
        }

        public void PrepareAdventureUserData(Action successCallback, Action<ENetResultCode> failedCallback)
        {
			_userData.Request(LocalUser.Instance.UserGuid, successCallback, failedCallback);
        }

		/// <summary>
		/// 请求进入冒险模式关卡
		/// </summary>
		/// <param name="sectionId">章节id</param>
		/// <param name="levelId">关卡id</param>
		/// <param name="type">关卡类型</param>
		/// <param name="successCallback">Success callback.</param>
		/// <param name="failedCallback">Failed callback.</param>
		public void PlayAdventureLevel (
			int sectionId, 
			int levelIdx, 
			EAdventureProjectType type,
			Action successCallback, Action<ENetResultCode> failedCallback) {
            if (_projectList.SectionList.Count < sectionId) {
                LogHelper.Error ("no adv project data of section {0}", sectionId);
                if (null != failedCallback) {
                    failedCallback.Invoke (ENetResultCode.NR_None);
                }
                return;
            }
            Project project = null;
            if (EAdventureProjectType.APT_Normal == type) {
                if (_projectList.SectionList [sectionId - 1].NormalProjectList.Count < levelIdx) {
                    LogHelper.Error ("no adv project data of section {0}, level {1}", sectionId, levelIdx);
                    if (null != failedCallback) {
                        failedCallback.Invoke (ENetResultCode.NR_None);
                    }
                    return;    
                }
                project = _projectList.SectionList [sectionId - 1].NormalProjectList [levelIdx - 1];
            }
            if (EAdventureProjectType.APT_Bonus == type) {
                if (_projectList.SectionList [sectionId - 1].BonusProjectList.Count < levelIdx) {
                    LogHelper.Error ("no adv project data of section {0}, level {1}", sectionId, levelIdx);
                    if (null != failedCallback) {
                        failedCallback.Invoke (ENetResultCode.NR_None);
                    }
                    return;
                }
                project = _projectList.SectionList [sectionId - 1].BonusProjectList [levelIdx - 1];
            }
            if (null == project) {
                if (null != failedCallback) {
                    failedCallback.Invoke (ENetResultCode.NR_None);
                }
                return;
            };
            project.SectionId = sectionId;
            project.LevelId = levelIdx;
			RemoteCommands.PlayAdventureLevel (sectionId, type, levelIdx, ret => {
				if (ret.ResultCode == (int)EPlayAdventureLevelCode.PALC_Success) {
					_lastRequiredLevelToken = ret.Token;
					_lastPlayedLevelType = type;
					_lastPlayedChapterIdx = sectionId - 1;
					_lastPlayedLevelIdx = levelIdx - 1;
					if (null != successCallback)
					{
						successCallback.Invoke();
                        if (AppData.Instance.AdventureData.ProjectList.SectionList.Count < sectionId) {
                            LogHelper.Error ("No project data of chapter {0}", sectionId);
                            return;
                        } else {
                            List<Project> projectList = EAdventureProjectType.APT_Bonus == type ?
                                AppData.Instance.AdventureData.ProjectList.SectionList [levelIdx - 1].BonusProjectList :
                                AppData.Instance.AdventureData.ProjectList.SectionList [levelIdx - 1].NormalProjectList;
                            if (projectList.Count < levelIdx) {
                                LogHelper.Error ("No project data of level in idx {0} in chapter {1}", levelIdx, sectionId);
                                return;

                            } else {
                                projectList [levelIdx - 1].PrepareRes (
                                    () => {
                                        if (EAdventureProjectType.APT_Bonus == type) {
                                            GameManager.Instance.RequestPlayAdvBonus (projectList [levelIdx - 1]);
                                        } else {
                                            GameManager.Instance.RequestPlayAdvNormal (projectList [levelIdx - 1]);
                                        }
                                        SocialGUIManager.Instance.ChangeToGameMode ();
                                    }
                                );
                            }
                        }
					}
				} else {
					if (null != failedCallback)
					{
						failedCallback.Invoke(ENetResultCode.NR_None);
					}
				}
			}, errorCode => {
				if (null != failedCallback)
				{
					failedCallback.Invoke(errorCode);
				}
			});
		}

        /// <summary>
        /// 重新尝试冒险关卡
        /// </summary>
        public void RetryAdvLevel (Action successCallback, Action<ENetResultCode> failedCallback) {
            PlayAdventureLevel (_lastPlayedChapterIdx + 1, _lastPlayedLevelIdx + 1, _lastPlayedLevelType, successCallback, failedCallback);
        }

        public Game.Table_StandaloneLevel GetAdvLevelTable (int chapteIdx, int levelIdx, EAdventureProjectType type) {
            if (chapteIdx == 0 || levelIdx == 0) return null;

            var tableChapter = Game.TableManager.Instance.GetStandaloneChapter (chapteIdx);
            if (null == tableChapter) {
                return null;
            }
            int levelId = 0;
            if (EAdventureProjectType.APT_Bonus == type) {
                if (levelIdx <= tableChapter.BonusLevels.Length) {
                    levelId = tableChapter.BonusLevels [levelIdx - 1];
                } else {
                    return null;
                }
            } else {
                if (levelIdx <= tableChapter.NormalLevels.Length) {
                    levelId = tableChapter.NormalLevels [levelIdx - 1];
                } else {
                    return null;
                }
            }
            return Game.TableManager.Instance.GetStandaloneLevel (levelId);
        }

		/// <summary>
		/// 提交冒险模式关卡成绩
		/// </summary>
		/// <param name="success">是否过关</param>
		/// <param name="usedTime">使用的时间</param>
		/// <param name="star1Flag">星1标志</param>
		/// <param name="star2Flag">星2标志</param>
		/// <param name="star3Flag">星3标志</param>
		/// <param name="score">最终得分</param>
		/// <param name="scoreItemCount">奖分道具数</param>
		/// <param name="killMonsterCount">击杀怪物数</param>
		/// <param name="leftTime">剩余时间数</param>
		/// <param name="leftLife">剩余生命</param>
		/// <param name="successCallback">Success callback.</param>
		/// <param name="failedCallback">Failed callback.</param>
		public void CommitLevelResult (
			bool success,
			float usedTime,
			int score,
			int scoreItemCount,
			int killMonsterCount,
			int leftTime,
			int leftLife,
            byte [] recordBytes,
			Action successCallback, Action<ENetResultCode> failedCallback) {
			if (_lastRequiredLevelToken == 0) {
				if (null != failedCallback)
				{
					failedCallback.Invoke(ENetResultCode.NR_None);
				}
                return;
			}
            var table = GetAdvLevelTable (_lastPlayedChapterIdx + 1, _lastPlayedLevelIdx + 1, _lastPlayedLevelType);
            if (null == table)
            {
                if (null != failedCallback) {
                    failedCallback.Invoke (ENetResultCode.NR_None);
                }
                return;
            }

            bool star1 = false;
            if (table.StarConditions.Length > 0)
            {
                star1 = CheckStarRequire (table.StarConditions [0], table.Star1Value, Game.PlayMode.Instance.Statistic);
            }
            bool star2 = false;
            if (table.StarConditions.Length > 1) {
                star2 = CheckStarRequire (table.StarConditions [1], table.Star2Value, Game.PlayMode.Instance.Statistic);
            }
            bool star3 = false;
            if (table.StarConditions.Length > 2) {
                star3 = CheckStarRequire (table.StarConditions [2], table.Star3Value, Game.PlayMode.Instance.Statistic);
            }

            UnityEngine.WWWForm form = new UnityEngine.WWWForm ();
            form.AddBinaryData ("recordFile", recordBytes);

			RemoteCommands.CommitAdventureLevelResult (
				_lastRequiredLevelToken,
				success,
				usedTime,
                star1,
                star2,
                star3,
				score,
				scoreItemCount,
				killMonsterCount,
				leftTime,
				leftLife,
                null,
				ret => {
                    if ((int)ECommitAdventureLevelResultCode.CALRC_Success == ret.ResultCode) {
                        if (null != successCallback) {
                            _lastRequiredLevelToken = 0;
                            LocalRefreshCommitedLevelData (success, usedTime, star1, star2, star3, score);
                            _lastAdvReward = new Reward (ret.Reward);
                            successCallback.Invoke ();
                        }
                    } else {
                        if (null != failedCallback) {
                            failedCallback.Invoke (ENetResultCode.NR_Error);
                        }
                    }
				},
				code => {
					if (null != failedCallback)
					{
						failedCallback.Invoke(code);
					}
				},
                form
			);
		}
        // 成功提交冒险模式通关数据后，客户端自主刷洗数据
		private void LocalRefreshCommitedLevelData (
			bool success,
			float usedTime,
			bool star1Flag,
			bool star2Flag,
			bool star3Flag,
			int score) {

			AdventureUserSection sectionData;
			if (_lastPlayedChapterIdx >= _userData.SectionList.Count) {
				LogHelper.Error("Can't get chapterData of idx {0}", _lastPlayedChapterIdx);
				return;
			} else {
				sectionData = _userData.SectionList[_lastPlayedChapterIdx];
			}
			List<AdventureUserLevelDataDetail> levelDataList = _lastPlayedLevelType == EAdventureProjectType.APT_Bonus ? 
				sectionData.BonusLevelUserDataList : 
				sectionData.NormalLevelUserDataList;
			AdventureUserLevelDataDetail levelData;
			if (_lastPlayedLevelIdx > levelDataList.Count) {
				LogHelper.Error("Try to modify {0}'s level data in chapter {1} in an unexpected way", _lastPlayedLevelIdx, _lastPlayedChapterIdx);
				return;
			} else if (_lastPlayedLevelIdx == levelDataList.Count) {
				levelData = sectionData.LocalAddLevelData(_lastPlayedLevelType);
			} else {
				levelData = levelDataList[_lastPlayedLevelIdx];
			}

			levelData.SimpleData.Star1Flag = star1Flag;
			levelData.SimpleData.Star2Flag = star2Flag;
			levelData.SimpleData.Star3Flag = star3Flag;
			if (success) levelData.SimpleData.SuccessCount++;
//			levelData.Request(
//				LocalUser.Instance.UserGuid,
//				_lastPlayedChapterIdx,
//				_lastPlayedLevelType,
//				,
//				()=>{
//					RefreshChapterInfo();
//				},null
//			);

			if (_userData.AdventureUserProgress.CompleteLevel < (_lastPlayedLevelIdx + 1)) {
				_userData.AdventureUserProgress.CompleteLevel = _lastPlayedLevelIdx + 1;
			}
		}

        private bool CheckStarRequire(int starType, int starValue, Game.GameStatistic statistic)
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
            else if (starType == 2)
            {
                return statistic.UsedTime < starValue;
            }
            // 死亡次数少于N次
            else if (starType == 3) {
                return statistic.DeathCnt < starValue;
            }
            // 得分超过N
            else if (starType == 4) {
                return statistic.Score >= starValue;
            }
            // 拾取所有钻石
            else if (starType == 5) {
                return statistic.CollectGem == Game.PlayMode.Instance.SceneState.TotalGem;
            }
            // 消灭所有怪物
            else if (starType == 6) {
                return statistic.MonsterDeathCnt == Game.PlayMode.Instance.SceneState.MonsterCount;
            }
            // 行走距离小于米
            else if (starType == 7) {
                return false;
            }
            // 跳跃次数小于N次
            else if (starType == 8) {
                return statistic.JumpCnt < starValue;
            }
            // 消灭的怪物少于N个
            else if (starType == 9) {
                return statistic.MonsterDeathCnt < starValue;
            }
            // 触发开关少于N次
            else if (starType == 10) {
                return statistic.SwitchTriggerCnt < starValue;
            }
            // 不使用传送门
            else if (starType == 11) {
                return statistic.PortalUsedCnt == 0;
            }
            // 用激光消灭N个怪物
            else if (starType == 12) {
                return statistic.MonsterKilledByLazerCnt >= starValue;
            }
            // 使怪物坠亡N次
            else if (starType == 13) {
                return statistic.MonsterKilledByFallCnt >= starValue;
            }
            return false;
        }
        #endregion 方法
    }
}
