using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserInfoDetail : SyncronisticData
    {
        #region 常量与字段

        private Dictionary<long, GameTimer> _publishedProjectRequestTimerDict;
        private Dictionary<long, GameTimer> _personalProjectRequestTimerDict;
        private GameTimer _followedListRequestTimer;
        private GameTimer _userInfoRequestTimer;

        #endregion

        #region 属性

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

        #endregion

        #region 方法


        public UserInfoDetail(UserInfoSimple userInfoSimple)
        {
            _userInfoSimple = userInfoSimple;
        }

        public void UpdateFollowState(bool follow, Action successCallback = null,
            Action<ENetResultCode> failedCallback = null)
        {
            RemoteCommands.UpdateFollowState(_userInfoSimple.UserId, follow, ret =>
            {
                if (ret.ResultCode != (int) EUpdateFollowStateCode.UFSS_Success)
                {
                    if (failedCallback != null)
                    {
                        failedCallback.Invoke(ENetResultCode.NR_None);
                    }
                    return;
                }
                //if (_projectUserData != null)
                //{
                //    _projectUserData.Favorite = true;
                //}
                if (successCallback != null)
                {
                    successCallback.Invoke();
                }
            }, code =>
            {
                if (failedCallback != null)
                {
                    failedCallback.Invoke(code);
                }
            });
        }

        public void UpdateBlockState(bool block, Action successCallback = null,
            Action<ENetResultCode> failedCallback = null)
        {
            RemoteCommands.UpdateBlockState(_userInfoSimple.UserId, block, ret =>
            {
                if (ret.ResultCode != (int) EUpdateFollowStateCode.UFSS_Success)
                {
                    if (failedCallback != null)
                    {
                        failedCallback.Invoke(ENetResultCode.NR_None);
                    }
                    return;
                }
                //if (_projectUserData != null)
                //{
                //    _projectUserData.Favorite = true;
                //}
                if (successCallback != null)
                {
                    successCallback.Invoke();
                }
            }, code =>
            {
                if (failedCallback != null)
                {
                    failedCallback.Invoke(code);
                }
            });
        }


        public void OnProjectCreated(Msg_SC_DAT_Project msg, Project p)
        {
            ProjectManager.Instance.OnCreateProject(msg, p);
            ST_CacheList cachedList = GetSavedProjectCache();
            cachedList.ContentList.Add(new ST_ValueItem() {Num0 = p.ProjectId});
            SaveSavedProjectCache(cachedList);
        }

        private ST_CacheList GetSavedProjectCache(long matrixId = 0)
        {
            var stl = LocalCacheManager.Instance.LoadObject<ST_CacheList>(ECacheDataType.CDT_SavedProject,
                string.Format("{0}_{1}", _userInfoSimple.UserId, matrixId));
            if (stl == null)
            {
                stl = new ST_CacheList();
            }
            return stl;
        }

        private void SaveSavedProjectCache(ST_CacheList stl)
        {
            LocalCacheManager.Instance.SaveObject(ECacheDataType.CDT_SavedProject, stl,
                string.Format("{0}_{1}", _userInfoSimple.UserId, 0));
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

        public void OnSyncUserSavedProjectList(Msg_SC_DAT_PersonalProjectList msg, long matrixId = 0)
        {
            if (msg.ResultCode == (int) ECachedDataState.CDS_None ||
                msg.ResultCode == (int) ECachedDataState.CDS_Uptodate)
            {
                GetSavedPrjectRequestTimer(matrixId).Reset();
                return;
            }
            var localSavedProjectList = GetSavedProjectList(matrixId);

            if (msg.ResultCode == (int) ECachedDataState.CDS_Recreate)
            {
                localSavedProjectList.ForEach(p =>
                {
                    p.Delete();
                });
                localSavedProjectList.Clear();
            }
            Dictionary<long, Project> localProjectDict = new Dictionary<long, Project>();
            localSavedProjectList.ForEach(p => localProjectDict.Add(p.ProjectId, p));
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
                newLocalSavedProjectList.ContentList.Add(new ST_ValueItem() {Num0 = entry.ProjectId});
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

            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_DeleteProject>(SoyHttpApiPath.DeleteProject, deleteMsg,
                ret =>
                {
                    if (ret.ResultCode != (int) EProjectOperateResult.POR_Success)
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
                    oldList.ContentList.ForEach((item) =>
                    {
                        if (!needDeleteSet.Contains(item.Num0))
                        {
                            newList.ContentList.Add(item);
                        }
                    });
                    SaveSavedProjectCache(newList);
                    list.ForEach(p => p.Delete());
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

        public void OnLogout()
        {
//			_publishedProjectDict = null;
            _publishedProjectRequestTimerDict = null;
            _personalProjectRequestTimerDict = null;
//			_followedList = null;
            _followedListRequestTimer = null;
//			_followerList = null;
//			_followerListRequestTimer = null;
            _userInfoRequestTimer = null;
//			_snsBinding = null;
        }

        protected void OnSyncPart(Msg_SC_DAT_UserInfoDetail msg)
        {
            UserManager.Instance.OnSyncUserData(msg);
            {

            }

            #endregion
        }
    }
}