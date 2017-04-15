/********************************************************************
** Filename : User
** Author : Dong
** Date : 2015/10/20 星期二 上午 10:54:41
** Summary : User
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using SoyEngine;

namespace GameA
{
    //[Poolable(MinPoolSize = ConstDefine.MaxLRUUserCount / 10, PreferedPoolSize = ConstDefine.MaxLRUUserCount / 5, MaxPoolSize = ConstDefine.MaxLRUUserCount)]
    public class User : SyncronisticData
    {
        #region field
        private long _UserId;
        private string _userName;
        private string _phoneNum;
        private string _nickName;
        private string _headImgUrl;
        private ESex _sex;
        private long _birthday;
        private string _country;
        private string _province;
        private string _city;
        private EAccountRoleType _accountRoleType;
        /// <summary>
        /// 简介
        /// </summary>
        private string _profile;
        private int _playerLevel;
        private long _playerExp;
        private int _creatorLevel;
        private long _creatorExp;
        private long _goldCoin;
        private long _diamond;


        private int _followCount;
        private int _followerCount;
        private int _friendCount;

        private bool _followMe;
        private bool _followedByMe;
        private bool _isFriend;

        private Dictionary<long, List<Project>> _publishedProjectDict;
        private Dictionary<long, GameTimer> _publishedProjectRequestTimerDict;
        private Dictionary<long, GameTimer> _personalProjectRequestTimerDict;

        private List<User> _followedList;
        private GameTimer _followedListRequestTimer;
        private List<User> _followerList;
        private GameTimer _followerListRequestTimer;
        private GameTimer _userInfoRequestTimer;
        private SnsBinding _snsBinding;

		private HomeAvatarPartData _avatarData;
        #endregion field

        public long UserId
        {
            get { return _UserId; }
        }

        public string UserName
        {
            get { return _userName; }
        }

        public string NickName
        {
            get { return _nickName; }
        }

        public string City
        {
            get { return _city; }
        }

        public string PhoneNum
        {
            get { return _phoneNum; }
            set { _phoneNum = value; }
        }

        public string HeadImgUrl
        {
            get { return _headImgUrl; }
        }

        public ESex Sex
        {
            get { return _sex; }
        }

        public long Birthday
        {
            get { return _birthday; }
        }

        public string Country
        {
            get { return _country; }
        }

        public string Province
        {
            get { return _province; }
        }

        public EAccountRoleType AccountRoleType
        {
            get { return _accountRoleType; }
        }

        public string Profile
        {
            get { return _profile; }
        }

        public int PlayerLevel
        {
            get { return _playerLevel; }
        }

        public long PlayerExp
        {
            get { return _playerExp; }
        }

        public int CreatorLevel
        {
            get { return _creatorLevel; }
        }

        public long CreatorExp
        {
            get { return _creatorExp; }
        }

        public long GoldCoin
        {
            get { return _goldCoin; }
        }

        public long Diamond
        {
            get { return _diamond; }
        }

        public int FollowCount
        {
            get { return _followCount; }
        }

        public int FollowerCount
        {
            get { return _followerCount; }
        }

        public int FriendCount
        {
            get { return _friendCount; }
        }

        public bool FollowMe
        {
            get { return _followMe; }
        }

        public bool FollowedByMe
        {
            get { return _followedByMe; }
        }

        public bool IsFriend
        {
            get { return _isFriend; }
        }

        public List<User> FollowedList
        {
            get { return _followedList; }
        }

        public GameTimer FollowedListRequestTimer
        {
            get
            {
                if (_followedListRequestTimer == null)
                {
                    _followedListRequestTimer = new GameTimer();
                    _followedListRequestTimer.Zero();
                }
                return _followedListRequestTimer;
            }
        }

        public List<User> FollowerList
        {
            get { return _followerList; }
        }


        public GameTimer FollowerListRequestTimer
        {
            get
            {
                if (_followerListRequestTimer == null)
                {
                    _followerListRequestTimer = new GameTimer();
                    _followerListRequestTimer.Zero();
                }
                return _followerListRequestTimer;
            }
        }

        public GameTimer UserInfoRequestGameTimer
        {
            get
            {
                if (_userInfoRequestTimer == null)
                {
                    _userInfoRequestTimer = new GameTimer();
                    _userInfoRequestTimer.Zero();
                }
                return _userInfoRequestTimer;
            }
        }

        public SnsBinding SnsBinding
        {
            get
            {
                return _snsBinding;
            }
        }

		public HomeAvatarPartData AvatarData {
			get {
				if (_avatarData == null) {
					_avatarData = new HomeAvatarPartData (_UserId);
				}
				return _avatarData;
			}
		}

        private ST_CacheList GetSavedProjectCache(long matrixId = 0)
        {
            var stl = LocalCacheManager.Instance.LoadObject<ST_CacheList>(ECacheDataType.CDT_SavedProject,
                          string.Format("{0}_{1}", _UserId, matrixId));
            if (stl == null)
            {
                stl = new ST_CacheList();
            }
            return stl;
        }

        private void SaveSavedProjectCache(ST_CacheList stl)
        {
            LocalCacheManager.Instance.SaveObject(ECacheDataType.CDT_SavedProject, stl,
                string.Format("{0}_{1}", _UserId, 0));
        }

        public long GetSavedProjectLastUpdateTime(long matrixId = 0)
        {
            ST_CacheList cacheList = GetSavedProjectCache(matrixId);
            return cacheList.LastUpdateTime;
        }

        public int GetSavedProjectCount(long matrixId = 0)
        {
            ST_CacheList cacheList = GetSavedProjectCache(matrixId);
            return cacheList.ContentList.Count;
        }

        public List<Project> GetSavedProjectList(long matrixId = 0)
        {
            var savedProjectCache = GetSavedProjectCache(matrixId);
            var list = ProjectManager.Instance.GetDatas(savedProjectCache.ContentList);
            list = list.FindAll(p =>
            {
                if (p == null)
                {
                    return false;
                }

                return true;
            });
            list.Sort((p1, p2) =>
            {
                return -p1.UpdateTime.CompareTo(p2.UpdateTime);
            });
            return list;
        }


        public GameTimer GetSavedPrjectRequestTimer(long matrixId = 0)
        {
            if (_personalProjectRequestTimerDict == null)
            {
                _personalProjectRequestTimerDict = new Dictionary<long, GameTimer>();
            }
            GameTimer timer = null;
            if (!_personalProjectRequestTimerDict.TryGetValue(matrixId, out timer))
            {
                timer = new GameTimer();
                timer.Zero();
                _personalProjectRequestTimerDict.Add(matrixId, timer);
            }
            return timer;
        }

        public GameTimer GetPublishedPrjectRequestTimer(long matrixId = 0)
        {
            if (_publishedProjectRequestTimerDict == null)
            {
                _publishedProjectRequestTimerDict = new Dictionary<long, GameTimer>();
            }
            GameTimer timer = null;
            if (!_publishedProjectRequestTimerDict.TryGetValue(matrixId, out timer))
            {
                timer = new GameTimer();
                timer.Zero();
                _publishedProjectRequestTimerDict.Add(matrixId, timer);
            }
            return timer;
        }

//        public void OnSyncUserData(Msg_SC_DAT_UserInfoSimple msg)
//        {
//            _UserId = msg.UserId;
//            _nickName = msg.NickName;
//            _headImgUrl = msg.HeadImgUrl;
//            _sex = msg.Sex;
//            if (msg.RelationWithMe != null)
//            {
//                OnSyncUserRelationWithMe(msg.RelationWithMe);
//            }
//            if (msg.LevelData != null)
//            {
//                OnSyncLevelData(msg.LevelData);
//            }
//        }
		public void OnSyncUserData(UserInfoSimple userSimple)
		{
			_UserId = userSimple.UserId;
			_nickName = userSimple.NickName;
			_headImgUrl = userSimple.HeadImgUrl;
			_sex = userSimple.Sex;
			if (userSimple.RelationWithMe != null)
			{
				OnSyncUserRelationWithMe(userSimple.RelationWithMe);
			}
			if (userSimple.LevelData != null)
			{
				OnSyncLevelData(userSimple.LevelData);
			}
		}

        public Msg_SC_DAT_UserInfoSimple ToMsgUserInfoSimple()
        {
            Msg_SC_DAT_UserInfoSimple msg = new Msg_SC_DAT_UserInfoSimple();
            msg.UserId = _UserId;
            msg.HeadImgUrl = _headImgUrl;
            msg.NickName = _nickName;
            return msg;
        }

        public void OnSyncUserData(Msg_SC_DAT_UserInfoDetail msg)
        {
			_UserId = msg.UserInfoSimple.UserId;
            _userName = msg.UserName;
            _phoneNum = msg.PhoneNum;
			_headImgUrl = msg.UserInfoSimple.HeadImgUrl;
			_sex = msg.UserInfoSimple.Sex;
            _birthday = msg.BirthDay;
			_nickName = msg.UserInfoSimple.NickName;
            _profile = msg.Profile;
            _country = msg.Country;
            _province = msg.Province;
            _city = msg.City;
            _accountRoleType = (EAccountRoleType)msg.RoleType;
			if (msg.UserInfoSimple.RelationWithMe != null)
            {
				OnSyncUserRelationWithMe(msg.UserInfoSimple.RelationWithMe);
            }
            if (msg.RelationStatistic != null)
            {
                OnSyncUserRelationStatistic(msg.RelationStatistic);
            }
			if (msg.UserInfoSimple.LevelData != null)
            {
				OnSyncLevelData(msg.UserInfoSimple.LevelData);
            }
			// todo broadcast user data update message
        }

        public void OnSyncUserRelationStatistic(Msg_SC_DAT_UserRelationStatistic msg)
        {
            _followCount = msg.FollowCount;
            _followerCount = msg.FollowerCount;
            _friendCount = msg.FriendCount;
        }

        public void OnSyncUserRelationWithMe(Msg_SC_DAT_UserRelationWithMe msg)
        {
            _followMe = msg.FollowMe;
            _followedByMe = msg.FollowedByMe;
            _isFriend = msg.IsFriend;
        }

		public void OnSyncUserRelationWithMe(UserRelationWithMe msg)
		{
			_followMe = msg.FollowMe;
			_followedByMe = msg.FollowedByMe;
			_isFriend = msg.IsFriend;
		}

//        public void OnSyncUserSnsBinding(List<Msg_SnsBinding> snsBindingList)
//        {
//            if (_snsBinding == null)
//            {
//                _snsBinding = new SnsBinding();
//            }
//            _snsBinding.SetData(snsBindingList);
//        }

//        public void OnSyncLevelData(Msg_SC_DAT_UserLevel msg)
//        {
//            _playerLevel = msg.PlayerLevel;
//            _playerExp = msg.PlayerExp;
//            _creatorLevel = msg.CreatorLevel;
//            _creatorExp = msg.CreatorExp;
//            _goldCoin = msg.GoldCoin;
//            _diamond = msg.Diamond;
//        }

		public void OnSyncLevelData(UserLevel msg)
		{
			_playerLevel = msg.PlayerLevel;
			_playerExp = msg.PlayerExp;
			_creatorLevel = msg.CreatorLevel;
			_creatorExp = msg.CreatorExp;
			_goldCoin = msg.GoldCoin;
			_diamond = msg.Diamond;
		}

		public void OnSyncLevelData(Msg_SC_DAT_UserLevel msg)
		{
			_playerLevel = msg.PlayerLevel;
			_playerExp = msg.PlayerExp;
			_creatorLevel = msg.CreatorLevel;
			_creatorExp = msg.CreatorExp;
			_goldCoin = msg.GoldCoin;
			_diamond = msg.Diamond;
		}

        public void ClearRalationWithMe()
        {
            _followMe = false;
            _followedByMe = false;
            _isFriend = false;
        }

        public List<Project> GetPublishedProjectList(long matrixId = 0)
        {
            if (_publishedProjectDict == null)
            {
                return null;
            }
            List<Project> list = null;
            if (_publishedProjectDict.TryGetValue(matrixId, out list))
            {
                return list;
            }
            return null;
        }


//        public void OnSyncUserPublishedProjectList(Msg_AC_ProjectList projectList, long matrixGuid = 0)
//        {
//            if (_publishedProjectDict == null)
//            {
//                _publishedProjectDict = new Dictionary<long, List<Project>>();
//            }
//            List<Project> ppList = null;
//            if (!_publishedProjectDict.TryGetValue(matrixGuid, out ppList))
//            {
//                ppList = new List<Project>();
//                _publishedProjectDict.Add(matrixGuid, ppList);
//                if (projectList.Result == ECachedDataState.CDS_None
//                   || projectList.Result == ECachedDataState.CDS_Uptodate)
//                {
//                    MessengerAsync.Broadcast(EMessengerType.OnUserPublishedProjectChanged);
//                }
//            }
//            if (projectList.Result == ECachedDataState.CDS_Recreate)
//            {
//                ppList.Clear();
//            }
//            projectList.ProjectList.ForEach(msgItem => ppList.Add(ProjectManager.Instance.OnSyncProject(msgItem)));
//            ppList.Sort((p1, p2) => -p1.Guid.CompareTo(p2.Guid));
//            MessengerAsync.Broadcast(EMessengerType.OnUserPublishedProjectChanged);
//        }

        public void OnUnpublishProject(List<Project> list)
        {
            HashSet<long> totalRemoveSet = new HashSet<long>();
            Dictionary<long, HashSet<long>> needRemoveSetDict = new Dictionary<long, HashSet<long>>();
            needRemoveSetDict.Add(0, totalRemoveSet);
            for (int i = 0; i < list.Count; i++)
            {
                Project p = list[i];
                totalRemoveSet.Add(p.ProjectId);
                HashSet<long> set = null;
                set.Add(p.ProjectId);
                p.IsValid = false;
                p.ProjectIntoRequestTimer.Zero();
                p.Delete();
            }
            List<Project> tempList = new List<Project>();
            foreach (var entity in needRemoveSetDict)
            {
                long mGuid = entity.Key;
                var pGuidSet = entity.Value;
                var pp = GetPublishedProjectList(mGuid);
                if (pp == null)
                {
                    continue;
                }
                tempList.Capacity = Math.Max(pp.Count, tempList.Capacity);
                tempList.AddRange(pp);
                pp.Clear();
                tempList.ForEach(p =>
                {
                    if (!pGuidSet.Contains(p.ProjectId))
                    {
                        pp.Add(p);
                    }
                });
                tempList.Clear();
            }

            Messenger.Broadcast(EMessengerType.OnUserPublishedProjectChanged);
        }

		public void OnSyncUserSavedProjectList(Msg_SC_DAT_PersonalProjectList msg, long matrixId = 0)
        {
			if (msg.ResultCode == (int)ECachedDataState.CDS_None || msg.ResultCode == (int)ECachedDataState.CDS_Uptodate)
            {
                GetSavedPrjectRequestTimer(matrixId).Reset();
                return;
            }
            var localSavedProjectList = GetSavedProjectList(matrixId);

            if (msg.ResultCode == (int)ECachedDataState.CDS_Recreate)
            {
                localSavedProjectList.ForEach(p =>
                {
                    p.Delete();
                });
                localSavedProjectList.Clear();
            }
            Dictionary<long, Project> localProjectDict = new Dictionary<long, Project>();
            localSavedProjectList.ForEach(p=>localProjectDict.Add(p.ProjectId, p));
            for (var i = 0; i < msg.ProjectList.Count; i++)
            {
                var msgProject = msg.ProjectList[i];
                Project project = null;
				if (localProjectDict.TryGetValue(msgProject.ProjectId, out project))
                {
                    project.DeleteResCache();
					msgProject.LocalDataState = ELocalDataState.LDS_Uptodate;
                    project = ProjectManager.Instance.OnSyncProject(msgProject, true);
                }
                else
                {

					msgProject.LocalDataState = ELocalDataState.LDS_Uptodate;
                    project = ProjectManager.Instance.OnSyncProject(msgProject, true);
                    localSavedProjectList.Add(project);
                }

            }
            ST_CacheList newLocalSavedProjectList = new ST_CacheList();
            foreach (var entry in localSavedProjectList)
            {
                newLocalSavedProjectList.ContentList.Add(new ST_ValueItem() { Num0 = entry.ProjectId });
            }
            newLocalSavedProjectList.LastUpdateTime = msg.UpdateTime;
            SaveSavedProjectCache(newLocalSavedProjectList);
            GetSavedPrjectRequestTimer(matrixId).Reset();
        }


        public void DeleteUserSavedProject(List<Project> list, Action successCallback, Action failedCallback)
        {
            if (list == null || list.Count == 0)
            {
                if (successCallback != null)
                {
                    successCallback.Invoke();
                }
                return;
            }

            HashSet<long> needDeleteSet = new HashSet<long>();
            Msg_CS_CMD_DeleteProject deleteMsg = new Msg_CS_CMD_DeleteProject();
            list.ForEach((project) =>
            {
                needDeleteSet.Add(project.ProjectId);
					deleteMsg.ProjectId.Add(project.ProjectId);
            });

			NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_DeleteProject>(SoyHttpApiPath.DeleteProject, deleteMsg, ret =>
            {
					if (ret.ResultCode != (int)EProjectOperateResult.POR_Success)
                {
                    if (failedCallback != null)
                    {
                        failedCallback.Invoke();
                    }
                    return;
                }
                ST_CacheList oldList = GetSavedProjectCache();
                ST_CacheList newList = new ST_CacheList();
                newList.LastUpdateTime = oldList.LastUpdateTime;
                oldList.ContentList.ForEach((item) => {
                    if (!needDeleteSet.Contains(item.Num0))
                    {
                        newList.ContentList.Add(item);
                    }
                });
                SaveSavedProjectCache(newList);
                list.ForEach(p=>p.Delete());
                if (successCallback != null)
                {
                    successCallback.Invoke();
                }
            }, (errCode, errMsg) =>
            {
                SoyHttpClient.ShowErrorTip(errCode);
                if (failedCallback != null)
                {
                    failedCallback.Invoke();
                }
            });
        }

        public void OnGoldCoinChange(int change)
        {
            _goldCoin += change;
            _goldCoin = Math.Max(0L, _goldCoin);
        }

		public void OnProjectCreated(Msg_SC_DAT_Project msg, Project p)
        {
            ProjectManager.Instance.OnCreateProject(msg, p);
            ST_CacheList cachedList = GetSavedProjectCache();
            cachedList.ContentList.Add(new ST_ValueItem() {Num0 = p.ProjectId});
            SaveSavedProjectCache(cachedList);
        }

//        public void OnSyncFollowList(Msg_AC_UserInfoList userInfoList)
//        {
//            if (_followedList == null)
//            {
//                _followedList = new List<User>();
//                if (userInfoList.Result == ECachedDataState.CDS_None || userInfoList.Result == ECachedDataState.CDS_Uptodate)
//                {
//                    MessengerAsync.Broadcast(EMessengerType.OnUserFollowChanged);
//                }
//            }
//            _followCount = userInfoList.TotalCount;
//            if (userInfoList.Result == ECachedDataState.CDS_None || userInfoList.Result == ECachedDataState.CDS_Uptodate)
//            {
//                return;
//            }
//            if (userInfoList.Result == ECachedDataState.CDS_Recreate)
//            {
//                _followedList.Clear();
//            }
//            for (var i = 0; i < userInfoList.DataList.Count; i++)
//            {
//                var msgItem = userInfoList.DataList[i];
//                User item = null;
//                item = UserManager.Instance.OnSyncUserData(msgItem);
//                _followedList.Add(item);
//            }
//            MessengerAsync.Broadcast(EMessengerType.OnUserFollowChanged);
//        }

//        public void OnSyncFollowerList(Msg_AC_UserInfoList userInfoList)
//        {
//            if (_followerList == null)
//            {
//                _followerList = new List<User>();
//                if (userInfoList.Result == ECachedDataState.CDS_None || userInfoList.Result == ECachedDataState.CDS_Uptodate)
//                {
//                    MessengerAsync.Broadcast(EMessengerType.OnUserFollowerChanged);
//                }
//            }
//            _followerCount = userInfoList.TotalCount;
//            if (userInfoList.Result == ECachedDataState.CDS_None || userInfoList.Result == ECachedDataState.CDS_Uptodate)
//            {
//                return;
//            }
//            if (userInfoList.Result == ECachedDataState.CDS_Recreate)
//            {
//                _followerList.Clear();
//            }
//            for (var i = 0; i < userInfoList.DataList.Count; i++)
//            {
//                var msgItem = userInfoList.DataList[i];
//                User item = null;
//                item = UserManager.Instance.OnSyncUserData(msgItem);
//                _followerList.Add(item);
//            }
//            MessengerAsync.Broadcast(EMessengerType.OnUserFollowerChanged);
//        }

        public void UpdateFollowState(bool follow, Action<bool> callback = null)
        {
//            if (_UserId == LocalUser.Instance.UserGuid)
//            {
//                if (callback != null)
//                {
//                    callback.Invoke(false);
//                }
//                return;
//            }
//            if (_followedByMe == follow)
//            {
//                if (callback != null)
//                {
//                    callback.Invoke(false);
//                }
//                return;
//            }
//            object msg = null;
//            string apiPath = null;
//            if (follow)
//            {
//                msg = new Msg_CA_AddFollow()
//                {
//                    UserId = _UserId
//                };
//                apiPath = SoyHttpApiPath.AddFollow;
//            }
//            else {
//                msg = new Msg_CA_RemoveFollow()
//                {
//                    UserId = _UserId
//                };
//                apiPath = SoyHttpApiPath.RemoveFollow;
//            }
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_OperateUserRet>(apiPath, msg, ret =>
//            {
//                if (ret.ResultCode != ECommonResultCode.CRC_Success)
//                {
//                    if (callback != null)
//                    {
//                        callback.Invoke(false);
//                    }
//                    return;
//                }
//                var u = UserManager.Instance.OnSyncUserData(ret.UserInfo);
//                if (u != this)
//                {
//                    OnSyncUserData(ret.UserInfo);
//                }
//                if (callback != null)
//                {
//                    callback.Invoke(true);
//                }
//                if (LocalUser.Instance.User != null)
//                {
//                    LocalUser.Instance.User.FollowedListRequestTimer.Zero();
//                    LocalUser.Instance.User.UserInfoRequestGameTimer.Zero();
//                    FollowerListRequestTimer.Zero();
//                }
//                MessengerAsync.Broadcast(EMessengerType.OnUserInfoChanged);
//            }, (code, msgStr) =>
//            {
//                if (callback != null)
//                {
//                    callback.Invoke(false);
//                }
//            });
        }


        public void OnLogout()
        {
            _publishedProjectDict = null;
            _publishedProjectRequestTimerDict = null;
            _personalProjectRequestTimerDict = null;
            _followedList = null;
            _followedListRequestTimer = null;
            _followerList = null;
            _followerListRequestTimer = null;
            _userInfoRequestTimer = null;
            _snsBinding = null;
        }
    }
}
