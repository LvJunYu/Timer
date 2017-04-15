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
//        private List<Section> _sectionList = new List<Section>();
//        private UserAdventureData _userData;
		private AdventureProjectList _projectList = new AdventureProjectList ();
		private AdventureUserData _userData = new AdventureUserData ();

		private long _lastRequiredLevelToken;

		private EAdventureProjectType _lastPlayedLevelType;
		private int _lastPlayedChapterIdx;
		private int _lastPlayedLevelIdx;
        #endregion 字段

        #region 属性
//        public List<Section> SectionList
//        {
//            get { return _sectionList; }
//        }

//        public UserAdventureData UserData
//        {
//            get { return _userData; }
//        }
		public AdventureProjectList ProjectList {
			get { return _projectList; }
		}
		public AdventureUserData UserData {
			get { return _userData; }
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
//			Msg_CS_DAT_AdventureProjectList msg = new Msg_CS_DAT_AdventureProjectList();
//			NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_AdventureProjectList>(SoyHttpApiPath.AdventureProjectList, msg, ret =>
//            {
//                SetAdventureProjectList(ret);
//                if (null != successCallback)
//                {
//                    successCallback.Invoke();
//                }
//            }, (errorCode, errorMsg) => {
//                if (null != failedCallback)
//                {
//                    failedCallback.Invoke(errorCode);
//                }
//            });
			_projectList.Request(0,5, successCallback, failedCallback);
        }

        public void PrepareAdventureUserData(Action successCallback, Action<ENetResultCode> failedCallback)
        {
//			Msg_CS_DAT_AdventureUserData msg = new Msg_CS_DAT_AdventureUserData();
//			NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_AdventureUserData>(SoyHttpApiPath.AdventureUserData, msg, ret =>
//            {
//                SetAdventureUserData(ret);
//                if (null != successCallback)
//                {
//                    successCallback.Invoke();
//                }
//            }, (errorCode, errorMsg) => {
//                if (null != failedCallback)
//                {
//                    failedCallback.Invoke(errorCode);
//                }
//            });
			_userData.Request(LocalUser.Instance.UserGuid, successCallback, failedCallback);
        }

//        public void SetAdventureProjectList(Msg_SC_DAT_AdventureProjectList msg)
//        {
//            _sectionList.Clear();
//			for (int i = 0; i < msg.SectionList.Count; i++)
//            {
//                _sectionList.Add(new Section(msg.SectionList[i]));
//            }
//        }

//        public void SetAdventureUserData(Msg_SC_DAT_AdventureUserData msg)
//        {
//            if (null == _userData)
//            {
//                _userData = new UserAdventureData();
//            }
//            _userData.Set(msg);
//        }

//		public void RequestPlayNormalLevel (int sectionId, int levelId,
//			Action successCallback, Action<ENetResultCode> failedCallback) {
//			RequestPlayLevel (sectionId, levelId, EAdventureProjectType.APT_Normal,
//				successCallback, failedCallback);
//		}
//		public void RequestPlayBonusLevel (int sectionId, int levelId,
//			Action successCallback, Action<ENetResultCode> failedCallback) {
//			RequestPlayLevel (sectionId, levelId, EAdventureProjectType.APT_Bonus,
//				successCallback, failedCallback);
//		}
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
			RemoteCommands.PlayAdventureLevel (sectionId, type, levelIdx, ret => {
				if (ret.ResultCode == (int)EPlayAdventureLevelCode.PALC_Success) {
					_lastRequiredLevelToken = ret.Token;
					_lastPlayedLevelType = type;
					_lastPlayedChapterIdx = sectionId - 1;
					_lastPlayedLevelIdx = levelIdx - 1;
					if (null != successCallback)
					{
						successCallback.Invoke();
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
			bool star1Flag,
			bool star2Flag,
			bool star3Flag,
			int score,
			int scoreItemCount,
			int killMonsterCount,
			int leftTime,
			int leftLife,
			Action successCallback, Action<ENetResultCode> failedCallback) {
			if (_lastRequiredLevelToken == 0) {
				if (null != failedCallback)
				{
					failedCallback.Invoke(ENetResultCode.NR_None);
				}
			}
			RemoteCommands.CommitAdventureLevelResult (
				_lastRequiredLevelToken,
				success,
				usedTime,
				star1Flag,
				star2Flag,
				star3Flag,
				score,
				scoreItemCount,
				killMonsterCount,
				leftTime,
				leftLife,
				ret => {
					if (null != successCallback)
					{
						_lastRequiredLevelToken = 0;
						LocalRefreshCommitedLevelData(success, usedTime, star1Flag, star2Flag, star3Flag, score);
						successCallback.Invoke();
					}
				},
				code => {
					if (null != failedCallback)
					{
						failedCallback.Invoke(code);
					}
				}
			);
		}

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
        #endregion 方法

        #region 内部类
//        public class Section
//        {
//            #region 字段
//            private List<Project> _normalProjectList = new List<Project>();
//            private List<Project> _bonusProjectList = new List<Project>();
//            #endregion 字段
//
//            #region 属性
//            public List<Project> NormalProjectList
//            {
//                get { return _normalProjectList; }
//            }
//
//            public List<Project> BonusProjectList
//            {
//                get { return _bonusProjectList; }
//            }
//            #endregion 属性
//
//            #region 方法
//            public Section() { }
//
//            public Section(Msg_AdventureSection msg)
//            {
//                Set(msg);
//            }
//
//            public void Set(Msg_AdventureSection msg)
//            {
//                _normalProjectList.Clear();
//                for (int i = 0; i < msg.NormalProjectList.Count; i++)
//                {
//                    _normalProjectList.Add(ProjectManager.Instance.OnSyncProject(msg.NormalProjectList[i]));
//                }
//                _bonusProjectList.Clear();
//                for (int i = 0; i < msg.BonusProjectList.Count; i++)
//                {
//                    _bonusProjectList.Add(ProjectManager.Instance.OnSyncProject(msg.BonusProjectList[i]));
//                }
//            }
//            #endregion 方法
//        }

//        public class UserAdventureData
//        {
//            #region 字段
//            private int _completeSection;
//            private int _completeLevel;
//            private int _encouragePoint;
//            private int _sectionKeyCount;
//			private int _sectionUnlockProgress;
//
//            private UserEnergyData _userEnergyData;
//            private List<SectionUserData> _sectionUserDataList;
//            #endregion 字段
//
//            #region 属性
//            public int CompleteSection
//            {
//                get
//                {
//                    return _completeSection;
//                }
//            }
//
//            public int CompleteLevel
//            {
//                get
//                {
//                    return _completeLevel;
//                }
//            }
//
//            public int EncouragePoint
//            {
//                get
//                {
//                    return _encouragePoint;
//                }
//            }
//
//            public int SectionKeyCount
//            {
//                get
//                {
//                    return _sectionKeyCount;
//                }
//            }
//
//			public int SectionUnlockProgress {
//				get { return _sectionUnlockProgress; }
//			}
//
//            public UserEnergyData EnergyData
//            {
//                get
//                {
//                    return _userEnergyData;
//                }
//            }
//
//            public List<SectionUserData> SectionUserDataList
//            {
//                get
//                {
//                    return _sectionUserDataList;
//                }
//            }
//            #endregion 属性
//
//            #region 方法
//            public UserAdventureData() { }
//
//            public UserAdventureData(Msg_SC_DAT_AdventureUserData msg)
//            {
//                Set(msg);
//            }
//
//            public void Set(Msg_SC_DAT_AdventureUserData msg)
//            {
//                SetUserProgress(msg.AdventureUserProgress);
//                SetUserEnergyData(msg.UserEnergyData);
//                SetUserSectionDataList(msg.SectionList);
//            }
//
//            public void SetUserProgress(Msg_SC_DAT_AdventureUserProgress msg)
//            {
//                _completeSection = msg.CompleteSection;
//                _completeLevel = msg.CompleteLevel;
//                _encouragePoint = msg.EncouragePoint;
//                _sectionKeyCount = msg.SectionKeyCount;
//				_sectionUnlockProgress = msg.SectionUnlockProgress;
//            }
//
//            public void SetUserEnergyData(Msg_SC_DAT_UserEnergy msg)
//            {
//                if (null == _userEnergyData)
//                {
//                    _userEnergyData = new UserEnergyData();
//                }
//                _userEnergyData.Set(msg);
//            }
//
//            public void SetUserSectionDataList(List<Msg_SC_DAT_AdventureUserSection> msg)
//            {
//                if (null == _sectionUserDataList)
//                {
//                    _sectionUserDataList = new List<SectionUserData>(msg.Count);
//                }
//                else
//                {
//                    _sectionUserDataList.Clear();
//                }
//                for (int i = 0; i < msg.Count; i++)
//                {
//                    _sectionUserDataList.Add(new SectionUserData(msg[i]));
//                }
//            }
//
//
//			public void LocalSetCompleteLevel (int value) {
//				_completeLevel = value;
//			}
//            #endregion 方法
//        }

//        public class UserEnergyData
//        {
//            #region 字段
//			// 默认最大体力点
//			private const int DefaultEnergyCapacity = 30;
//			// 默认体力增长时间／每点
//			private const int DefaultEnergyGenerateTime = 300;
//
//
//            private int _energy;
//            private long _energyBoostingEndTime;
//            private long _energyLastRefreshTime;
//            private int _energyCapacity;
//            #endregion 字段
//
//            #region 属性
//            public int Energy
//            {
//                get
//                {
//					LocalRefresh ();
//                    return _energy;
//                }
//            }
//
//            public long EnergyBoostingEndTime
//            {
//                get
//                {
//                    return _energyBoostingEndTime;
//                }
//            }
//
//            public long EnergyLastRefreshTime
//            {
//                get
//                {
//                    return _energyLastRefreshTime;
//                }
//            }
//
//            public int EnergyCapacity
//            {
//                get
//                {
//                    return _energyCapacity;
//                }
//            }
//			/// <summary>
//			/// 下一次自动增长体力点的时间
//			/// </summary>
//			/// <returns>The generate time.</returns>
//			public long NextGenerateTime {
//				get {
//					if (_energy >= _energyCapacity)
//						return long.MaxValue;
//					return _energyLastRefreshTime + 1000 * DefaultEnergyGenerateTime;
//				}
//			}
//            #endregion 属性
//
//            #region 方法
//            public UserEnergyData() {}
//
//            public UserEnergyData(Msg_SC_DAT_UserEnergy msg)
//            {
//                Set(msg);
//            }
//
//            public void Set(Msg_SC_DAT_UserEnergy msg)
//            {
//                _energy = msg.Energy;
//                _energyBoostingEndTime = msg.EnergyBoostingEndTime;
//                _energyLastRefreshTime = msg.EnergyLastRefreshTime;
//                _energyCapacity = msg.EnergyCapacity;
//            }
//
//            public void Refresh(Action successCallback, Action<ENetResultCode> failedCallback)
//            {
//                Msg_CS_DAT_UserEnergy msg = new Msg_CS_DAT_UserEnergy();
//				NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserEnergy>(SoyHttpApiPath.UserEnergy, msg, ret =>
//                {
//                    Set(ret);
//                    if (successCallback != null)
//                    {
//                        successCallback.Invoke();
//                    }
//                }, (errorCode, errorMsg) => {
//                    if (failedCallback != null)
//                    {
//                        failedCallback.Invoke(errorCode);
//                    }
//                });
//            }
//			/// <summary>
//			/// 客户端刷新当前体力信息
//			/// </summary>
//			public void LocalRefresh () {
//				long now = DateTimeUtil.GetServerTimeNowTimestampMillis ();
//				if (_energy >= _energyCapacity) {
//					_energyLastRefreshTime = now;
//					return;
//				}
//				long passedTime = now - _energyLastRefreshTime;
//				int generatedEnergy = (int)(passedTime / 1000 / DefaultEnergyGenerateTime);
//				if (generatedEnergy > 0) {
//					_energyLastRefreshTime += generatedEnergy * 1000 * DefaultEnergyCapacity;
//					_energy += generatedEnergy;
//					if (_energy >= _energyCapacity) {
//						_energyLastRefreshTime = now;
//						_energy = _energyCapacity;
//					}
//				}
//			}
//
//			public void LocalSetEnergy (int value) {
//				if (value < _energyCapacity) {
//					_energyLastRefreshTime = DateTimeUtil.GetServerTimeNowTimestampMillis ();
//				}
//				_energy = value;
//			}
//            #endregion 方法
//
//        }

//        public class SectionUserData
//        {
//            #region 字段
//            private int _treasureMapBuyCount;
//            private List<LevelUserData> _normalLevelUserDataList;
//            private List<LevelUserData> _bonusLevelUserDataList;
//            #endregion 字段
//
//            #region 属性
//            public int TreasureMapBuyCount
//            {
//                get { return _treasureMapBuyCount; }
//            }
//
//            public List<LevelUserData> NormalLevelUserDataList
//            {
//                get { return _normalLevelUserDataList; }
//            }
//
//            public List<LevelUserData> BonusLevelUserDataList
//            {
//                get { return _bonusLevelUserDataList; }
//            }
//
//			public int GotStarCnt {
//				get {
//					int cnt = 0;
//					for (int i = 0, n = _normalLevelUserDataList.Count; i < n; i++) {
//						cnt += _normalLevelUserDataList [i].GotStarCnt;
//					}
//					return cnt;
//				}
//			}
//            #endregion 属性
//
//            #region 方法
//            public SectionUserData() { }
//
//            public SectionUserData(Msg_SC_DAT_AdventureUserSection msg)
//            {
//                Set(msg);
//            }
//
//            public void Set(Msg_SC_DAT_AdventureUserSection msg)
//            {
//                _treasureMapBuyCount = msg.TreasureMapBuyCount;
//                if (_normalLevelUserDataList == null)
//                {
//                    _normalLevelUserDataList = new List<LevelUserData>(msg.NormalLevelUserDataList.Count);
//                }
//                if (_bonusLevelUserDataList == null)
//                {
//                    _bonusLevelUserDataList = new List<LevelUserData>(msg.BonusLevelUserDataList.Count);
//                }
//                _normalLevelUserDataList.Clear();
//                _bonusLevelUserDataList.Clear();
//                for (int i = 0; i < msg.NormalLevelUserDataList.Count; i++)
//                {
//                    _normalLevelUserDataList.Add(new LevelUserData(msg.NormalLevelUserDataList[i]));
//                }
//                for (int i = 0; i < msg.BonusLevelUserDataList.Count; i++)
//                {
//                    _bonusLevelUserDataList.Add(new LevelUserData(msg.BonusLevelUserDataList[i]));
//                }
//            }
//
//            public void Refresh(Action successCallback, Action<ENetResultCode> failedCallback)
//            {
//                Msg_CS_DAT_AdventureUserSection msg = new Msg_CS_DAT_AdventureUserSection();
//				NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_AdventureUserSection>(SoyHttpApiPath.AdventureUserSection, msg, ret =>
//                {//                    Set(ret);
//                    if (successCallback != null)
//                    {
//                        successCallback.Invoke();
//                    }
//                }, (errorCode, errorMsg) => {
//                    if (failedCallback != null)
//                    {
//                        failedCallback.Invoke(errorCode);
//                    }
//                });
//            }
//
//			public void LocalAddLevelData (bool isBonus) {
//				LevelUserData newLevelData = new LevelUserData ();
//				if (isBonus) {
//					_bonusLevelUserDataList.Add (newLevelData);
//				} else {
//					_normalLevelUserDataList.Add (newLevelData);
//				}
//			}
//
//            #endregion 方法
//        }


//		#region Test
//		public class NewLevelUserData : SyncronisticData {
//			
//			#region 字段
//			private LevelUserDataSimple _levelUserDataSimple;
////			private LevelUserDataDetail _levelUserDataDetail;
//
//			private long _highScore;
//			private long _friendHighScore;
//			//				optional Msg_UserInfoSimple HighScoreFriendInfo = 10;
//			private int _challengeCount;
//			//			private int _successCount;
//			private int _failureCount;
//			private long _lastPlayTime;
//
//			private Msg_SC_Data_Record _highScoreRecord;
//			private Msg_SC_Data_Record _star1FlagRecord;
//			private Msg_SC_Data_Record _star2FlagRecord;
//			private Msg_SC_Data_Record _star3FlagRecord;
//			#endregion
//
//			#region 属性
//			public LevelUserDataSimple LevelUserDataSimple {
//				get {
//					return this._levelUserDataSimple;
//				}
//			}
//
//			public long HighScore {
//				get {
//					return this._highScore;
//				}
//			}
//
//			public long FriendHighScore {
//				get {
//					return this._friendHighScore;
//				}
//			}
//
//			public int ChallengeCount {
//				get {
//					return this._challengeCount;
//				}
//			}
//
//			public int FailureCount {
//				get {
//					return this._failureCount;
//				}
//			}
//
//			public long LastPlayTime {
//				get {
//					return this._lastPlayTime;
//				}
//			}
//
//			public Msg_SC_Data_Record HighScoreRecord {
//				get {
//					return this._highScoreRecord;
//				}
//			}
//
//			public Msg_SC_Data_Record Star1FlagRecord {
//				get {
//					return this._star1FlagRecord;
//				}
//			}
//
//			public Msg_SC_Data_Record Star2FlagRecord {
//				get {
//					return this._star2FlagRecord;
//				}
//			}
//
//			public Msg_SC_Data_Record Star3FlagRecord {
//				get {
//					return this._star3FlagRecord;
//				}
//			}
//			// Properties from LevelUserDataSimple
//			public bool Star1Flag
//			{
//				get { return _levelUserDataSimple.Star1Flag; }
//			}
//			public bool Star2Flag
//			{
//				get { return _levelUserDataSimple.Star2Flag; }
//			}
//			public bool Star3Flag
//			{
//				get { return _levelUserDataSimple.Star3Flag; }
//			}
//			public int SucessCount {
//				get { return _levelUserDataSimple.SuccessCount; }
//			}
//
//			public override bool IsDirty {
//				get {
//					return base.IsDirty;
//				}
//			}
//			#endregion
//
//			#region 方法
//			// local set interface
//			public void LocalSetHighScore (long value) {
//				_highScore = value;
//				_dirty = true;
//			}
//
//
//			protected override void Sync (Action successCallback, Action<ENetResultCode> failedCallback)
//			{
//				base.Sync (successCallback, failedCallback);
//
//			}
//			#endregion
//		}
//
//		public partial class LevelUserDataSimple : PartialSyncronisticData<NewLevelUserData> {
//			#region 字段
//			private bool _star1Flag;
//			private bool _star2Flag;
//			private bool _star3Flag;
//			private int _successCount;
//			#endregion
//
//			#region 属性
//			public bool Star1Flag
//			{
//				get { return _star1Flag; }
//			}
//			public bool Star2Flag
//			{
//				get { return _star2Flag; }
//			}
//			public bool Star3Flag
//			{
//				get { return _star3Flag; }
//			}
//			public int SuccessCount {
//				get { return _successCount; }
//			}
//
//
//			#endregion
//
//			#region 方法
//			public void LocalSetStar1Flag (bool value) {
//				_star1Flag = value;
//				SetDirty ();
//			}
//			public void LocalSetStar2Flag (bool value) {
//				_star2Flag = value;
//				SetDirty ();
//			}
//			public void LocalSetStar3Flag (bool value) {
//				_star3Flag = value;
//				SetDirty ();
//			}
//			public void LocalSetSucessCount (int value) {
//				_successCount = value;
//				SetDirty ();
//			}
//
//			protected override void Sync (Action successCallback, Action<ENetResultCode> failedCallback)
//			{
//				base.Sync (successCallback, failedCallback);
//
//			}
//
//			public void Deserialize(Msg_AdventureUserLevelData msg)
//			{
//				_star1Flag = msg.Star1Flag;
//				_star2Flag = msg.Star2Flag;
//				_star3Flag = msg.Star3Flag;
//				//	                _highScore = msg.HighScore;
//				//					_challengeCount = msg.ChallengeCount;
//				_successCount = msg.SuccessCount;
//				//					_failureCount = msg.FailureCount;
//				//					_lastPlayTime = msg.LastPlayTime;
//
//			}
//			#endregion
//		}
//
//		public partial class LevelUserDataDetail : PartialSyncronisticData<NewLevelUserData> {
//			#region 字段
//			//				private bool _star1Flag;
//			//				private bool _star2Flag;
//			//				private bool _star3Flag;
//			private long _highScore;
//			private long _friendHighScore;
//			//				optional Msg_UserInfoSimple HighScoreFriendInfo = 10;
//			private int _challengeCount;
////			private int _successCount;
//			private int _failureCount;
//			private long _lastPlayTime;
//
//			private Msg_SC_Data_Record _highScoreRecord;
//			private Msg_SC_Data_Record _star1FlagRecord;
//			private Msg_SC_Data_Record _star2FlagRecord;
//			private Msg_SC_Data_Record _star3FlagRecord;
//			#endregion
//
//			#region 属性
//			public long HighScore
//			{
//				get { return _highScore; }
//			}
//
//
//			public int ChallengeCount {
//				get { return _challengeCount; }
//			}
////			public int SucessCount {
////				get { return _successCount; }
////			}
//			public int FailureCount {
//				get { return _failureCount; }
//			}
//			public long LastPlayTime {
//				get { return _lastPlayTime; }
//			}
//			public Msg_SC_Data_Record HighScoreRecord {
//				get {
//					return this._highScoreRecord;
//				}
//			}
//
//			public Msg_SC_Data_Record Star1FlagRecord {
//				get {
//					return this._star1FlagRecord;
//				}
//			}
//
//			public Msg_SC_Data_Record Star2FlagRecord {
//				get {
//					return this._star2FlagRecord;
//				}
//			}
//
//			public Msg_SC_Data_Record Star3FlagRecord {
//				get {
//					return this._star3FlagRecord;
//				}
//			}
//			#endregion
//
//			#region 方法
//
//			public void LocalSetChallengeCount (int value) {
//				_challengeCount = value;
//				SetDirty ();
//			}
//
////			protected override void Request (
////				long userId,
////				int section,
////				EAdventureProjectType projectType,
////				int level,
////				Action successCallback, Action<ENetResultCode> failedCallback)
////			{
////				if (!IsDirty) {
////					successCallback.Invoke ();
////				}
////					return;
////				
////				base.Sync (successCallback, failedCallback);
////
////				Msg_CS_DAT_AdventureUserLevelDataDetail msg = new Msg_CS_DAT_AdventureUserLevelDataDetail();
////				msg.UserId = userId;
////				msg.Section = section;
////				msg.ProjectType = projectType;
////				msg.Level = level;
////				NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_AdventureUserLevelDataDetail>(SoyHttpApiPath.GetAdventureUserLevelDataDetail, msg, ret =>
////					{
////						OnSync(ret);
////						OnSyncSucceed();
////					}, (failedCode, failedMsg) => {
////						OnSyncFailed(failedCode, failedMsg);
////					});
////			}
//
//			private void OnSync (Msg_SC_DAT_AdventureUserLevelDataDetail msg)
//			{
//				_highScore = msg.HighScore;
//				_friendHighScore = msg.FriendHighScore;
//				_challengeCount = msg.ChallengeCount;
//				_failureCount = msg.FailureCount;
//				_lastPlayTime = msg.LastPlayTime;
//				_star1FlagRecord = msg.Star1FlagRecord;
//				_star2FlagRecord = msg.Star2FlagRecord;
//				_star3FlagRecord = msg.Star3FlagRecord;
//				SetPartial ();
//			}
//			#endregion
//		}
//		// write by human
//		public partial class LevelUserDataDetail : PartialSyncronisticData<NewLevelUserData> {
//			#region 字段
//			private Record _hiScoreRecord;
//			private Record _star1Record;
//			private Record _star2Record;
//			private Record _star3Record;
//
//			#endregion
//
//			#region 属性
//			public Record HiScoreRecord
//			{
//				get { return _hiScoreRecord; }
//			}
//
//			public Record Star1Record
//			{
//				get { return _star1Record; }
//			}
//
//			public Record Star2Record
//			{
//				get { return _star2Record; }
//			}
//
//			public Record Star3Record
//			{
//				get { return _star3Record; }
//			}
//
//			#endregion
//
//			#region 方法
//			protected override void SetPartial () {
//				if (_highScoreRecord != null)
//				{
//					_hiScoreRecord = RecordManager.Instance.OnSync(_highScoreRecord, null);
//				}
//				if (_star1FlagRecord != null)
//				{
//					_star1Record = RecordManager.Instance.OnSync(_star1FlagRecord, null);
//				}
//				if (_star2FlagRecord != null)
//				{
//					_star2Record = RecordManager.Instance.OnSync(_star2FlagRecord, null);
//				}
//				if (_star3FlagRecord != null)
//				{
//					_star3Record = RecordManager.Instance.OnSync(_star3FlagRecord, null);
//				}
//			}
//			#endregion
//		}
//
//		#endregion


//        public class LevelUserData
//        {
//            #region 字段
//            private long _highScore;
//            private Record _highScoreRecord;
//            private bool _star1Flag;
//            private Record _star1Record;
//            private bool _star2Flag;
//            private Record _star2Record;
//            private bool _star3Flag;
//            private Record _star3Record;
//			private int _challengeCount;
//			private int _successCount;
//			private int _failureCount;
//			private long _lastPlayTime;
//            #endregion 字段
//
//            #region 属性
//            public long HighScore
//            {
//                get { return _highScore; }
//            }
//
//            public Record HighScoreRecord
//            {
//                get { return _highScoreRecord; }
//            }
//
//            public bool Star1Flag
//            {
//                get { return _star1Flag; }
//            }
//
//            public Record Star1Record
//            {
//                get { return _star1Record; }
//            }
//
//            public bool Star2Flag
//            {
//                get { return _star2Flag; }
//            }
//
//            public Record Star2Record
//            {
//                get { return _star2Record; }
//            }
//
//            public bool Star3Flag
//            {
//                get { return _star3Flag; }
//            }
//
//            public Record Star3Record
//            {
//                get { return _star3Record; }
//            }
//
//			public int ChallengeCount {
//				get { return _challengeCount; }
//			}
//			public int SucessCount {
//				get { return _successCount; }
//			}
//			public int FailureCount {
//				get { return _failureCount; }
//			}
//			public long LastPlayTime {
//				get { return _lastPlayTime; }
//			}
//
//			public int GotStarCnt {
//				get { return (_star1Flag ? 1 : 0) + (_star2Flag ? 1 : 0) + (Star3Flag ? 1 : 0); }
//			}
//            #endregion 属性
//
//            #region 方法
//            public LevelUserData() { }
//
//            public LevelUserData(Msg_AdventureUserLevelData msg)
//            {
//                Set(msg);
//            }
//
//            public void Set(Msg_AdventureUserLevelData msg)
//            {
//                _star1Flag = msg.Star1Flag;
//                _star2Flag = msg.Star2Flag;
//                _star3Flag = msg.Star3Flag;
//                _highScore = msg.HighScore;
//				_challengeCount = msg.ChallengeCount;
//				_successCount = msg.SuccessCount;
//				_failureCount = msg.FailureCount;
//				_lastPlayTime = msg.LastPlayTime;
//
//            }
//
//			public void RefreshDetail(
//				long userId,
//				int section,
//				EAdventureProjectType projectType,
//				int level,
//				Action successCallback, Action<ENetResultCode> failedCallback)
//            {
//                Msg_CS_DAT_AdventureUserLevelDataDetail msg = new Msg_CS_DAT_AdventureUserLevelDataDetail();
//				msg.UserId = userId;
//				msg.Section = section;
//				msg.ProjectType = projectType;
//				msg.Level = level;
//				NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_AdventureUserLevelDataDetail>(SoyHttpApiPath.AdventureUserLevelDataDetail, msg, ret =>
//                {
//                    Set(ret);
//                    if (successCallback != null)
//                    {
//                        successCallback.Invoke();
//                    }
//                }, (failedCode, failedMsg) => {
//                    if (failedCallback != null)
//                    {
//                        failedCallback.Invoke(failedCode);
//                    }
//                });
//            }
//
//            private void Set(Msg_SC_DAT_AdventureUserLevelDataDetail msg)
//            {
////                _star1Flag = msg.Star1Flag;
//                if (msg.Star1FlagRecord != null)
//                {
//                    _star1Record = RecordManager.Instance.OnSync(msg.Star1FlagRecord, null);
//                }
////                _star2Flag = msg.Star2Flag;
//                if (msg.Star2FlagRecord != null)
//                {
//                    _star2Record = RecordManager.Instance.OnSync(msg.Star2FlagRecord, null);
//                }
////                _star3Flag = msg.Star3Flag;
//                if (msg.Star3FlagRecord != null)
//                {
//                    _star3Record = RecordManager.Instance.OnSync(msg.Star3FlagRecord, null);
//                }
////                _highScore = msg.HighScore;
//                if (msg.HighScoreRecord != null)
//                {
//                    _highScoreRecord = RecordManager.Instance.OnSync(msg.HighScoreRecord, null);
//                }
//            }
//
//			public void LocalSetStar1Flg (bool value) {
//				_star1Flag = value;
//			}
//			public void LocalSetStar2Flg (bool value) {
//				_star2Flag = value;
//			}
//			public void LocalSetStar3Flg (bool value) {
//				_star3Flag = value;
//			}
//			public void LocalSetSuccessCount (int value) {
//				_successCount = value;
//			}
//            #endregion 方法
//        }

        #endregion 内部类
    }
}
