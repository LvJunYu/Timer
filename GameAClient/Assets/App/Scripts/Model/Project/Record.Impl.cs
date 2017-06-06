/********************************************************************
** Filename : ProjectPlayRecord.cs
** Author : quan
** Date : 1/4/2017 7:30 PM
** Summary : ProjectPlayRecord.cs
***********************************************************************/
using System;
using SoyEngine.Proto;
using SoyEngine;
using System.Collections.Generic;
using UnityEngine;
using SoyEngine;

namespace GameA
{

	public partial class Record : SyncronisticData
    {
        public const long RecordFullInfoRequestInterval = 1 * GameTimer.Minute2Ticks;
        public const long RecordTimelineCommentRequestInterval = 2*GameTimer.Minute2Ticks;

        private long _id;
        private User _user;
        private Project _project;
//        private float _usedTime;
//        private long _createTime;
//        private string _recordPath;
//        private long _projectId;
//        private EGameResult _result;
//        private long _playCount;
//        private long _lastPlayTime;
//        private long _playUserCount;
//        private int _favoriteCount;
//        private int _likeCount;
//        private int _commentCount;
//        private int _shareCount;
//        private bool _userBuy;
//        private bool _userLike;
//        private bool _userFavorite;

        private GameTimer _recordFullInfoRequestTimer;

//        private List<RecordComment> _recordCommentList;
        private GameTimer _recordCommentListRequestTimer;

//        private List<RecordComment> _timelineRecordCommentList;
        private GameTimer _timelineRecordCommentListRequestTimer;

        private int _timelineLastSyncTimePos;
        private long _timelineLastSyncCreateTime;
        private bool _timelineLastSyncIsEnd;

        public long Id
        {
            get { return _id; }
        }

        public User User
        {
            get { return _user; }
        }

        public Project Project
        {
            get { return _project; }
        }

//        public float UsedTime
//        {
//            get { return _usedTime; }
//        }

//        public long CreateTime
//        {
//            get { return _createTime; }
//        }

//        public string RecordPath
//        {
//            get { return _recordPath; }
//        }

        public byte[] RecordData
        {
            get
            {
                if (string.IsNullOrEmpty(_recordPath))
                {
                    return null;
                }
                else
                {
                    return LocalCacheManager.Instance.Load(LocalCacheManager.EType.File, _recordPath);
                }
            }
        }

//        public long ProjectId
//        {
//            get
//            {
//                return _projectId;
//            }
//        }

//        public EGameResult Result
//        {
//            get
//            {
//                return _result;
//            }
//        }

//        public long PlayCount
//        {
//            get
//            {
//                return _playCount;
//            }
//        }

//        public long LastPlayTime
//        {
//            get
//            {
//                return _lastPlayTime;
//            }
//        }
//
//        public long PlayUserCount
//        {
//            get
//            {
//                return _playUserCount;
//            }
//        }
//
//        public int FavoriteCount
//        {
//            get
//            {
//                return _favoriteCount;
//            }
//        }
//
//        public int LikeCount
//        {
//            get
//            {
//                return _likeCount;
//            }
//        }

//        public int CommentCount
//        {
//            get
//            {
//                return _commentCount;
//            }
//        }
//
//        public int ShareCount
//        {
//            get
//            {
//                return _shareCount;
//            }
//        }
//
//        public bool UserBuy
//        {
//            get
//            {
//                return _userBuy;
//            }
//        }
//
//        public bool UserLike
//        {
//            get
//            {
//                return _userLike;
//            }
//        }
//
//        public bool UserFavorite
//        {
//            get
//            {
//                return _userFavorite;
//            }
//        }

        public GameTimer RecordFullInfoRequestTimer
        {
            get
            {
                if (_recordFullInfoRequestTimer == null)
                {
                    _recordFullInfoRequestTimer = new GameTimer();
                    _recordFullInfoRequestTimer.Zero();
                }
                return _recordFullInfoRequestTimer;
            }
        }

//        public List<RecordComment> RecordCommentList
//        {
//            get
//            {
//                return _recordCommentList;
//            }
//        }

        public GameTimer RecordCommentListRequestTimer
        {
            get
            {
                if (_recordCommentListRequestTimer == null)
                {
                    _recordCommentListRequestTimer = new GameTimer();
                    _recordCommentListRequestTimer.Zero();
                }
                return _recordCommentListRequestTimer;
            }
        }

//        public List<RecordComment> TimelineRecordCommentList
//        {
//            get
//            {
//                return _timelineRecordCommentList;
//            }
//        }

        public GameTimer TimelineRecordCommentListRequestTimer
        {
            get
            {
                if (_timelineRecordCommentListRequestTimer == null)
                {
                    _timelineRecordCommentListRequestTimer = new GameTimer();
                    _timelineRecordCommentListRequestTimer.Zero();
                }
                return _timelineRecordCommentListRequestTimer;
            }
        }

        public int TimelineLastSyncTimePos
        {
            get
            {
                return _timelineLastSyncTimePos;
            }
        }

        public long TimelineLastSyncCreateTime
        {
            get
            {
                return _timelineLastSyncCreateTime;
            }
        }

        public bool TimelineLastSyncIsEnd
        {
            get
            {
                return _timelineLastSyncIsEnd;
            }
        }

//        public Record()
//        {
//        }

        public Record(Msg_SC_DAT_Record msg, Project project, bool full = true)
        {
            Set(msg, project, full);
        }

        public void Set(Msg_SC_DAT_Record msg, Project project, bool full = true)
        {
            Project oldProject = _project;
            _id = msg.RecordId;
//			_user = UserManager.Instance.OnSyncUserData(_userInfo);
            _usedTime = msg.UsedTime;
            _createTime = msg.CreateTime;
            _recordPath = msg.RecordPath;
            _project = project;
            if (msg.ProjectData != null)
            {
                _project = ProjectManager.Instance.OnSyncProject(msg.ProjectData);
            }
            _projectId = msg.ProjectId;
            _result = msg.Result;
            _playCount = msg.PlayCount;
            _lastPlayTime = msg.LastPlayTime;
            _playUserCount = msg.PlayUserCount;
            _favoriteCount = msg.FavoriteCount;
            _likeCount = msg.LikeCount;
            _commentCount = msg.CommentCount;
            _shareCount = msg.ShareCount;
            _userBuy = msg.UserBuy;
            _userLike = msg.UserLike;
            _userFavorite = msg.UserFavorite;
            if (_project == null)
            {
                ProjectManager.Instance.TryGetData(_projectId, out _project);
                if (_project == null && oldProject != null && oldProject.ProjectId == _projectId)
                {
                    _project = oldProject;
                }
            }
            if (full)
            {
                RecordFullInfoRequestTimer.Reset();
            }
        }

//        public void RequestPlay(EPlayRecordTicket ticket, Action successCallback, Action<EPlayRecordRetCode> failedCallback = null)
//        {
//            Msg_CA_RequestPlayRecord msg = new Msg_CA_RequestPlayRecord();
//            msg.RecordId = _id;
//            msg.Ticket = ticket;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_PlayRecordRet>(SoyHttpApiPath.PlayRecord, msg, ret =>
//            {
//                if (ret.ResultCode == (int)EPlayRecordRetCode.PRRC_Success)
//                {
//                    if (ticket == EPlayRecordTicket.PRT_UseFreeOpportunity)
//                    {
//                        //UserMatrixData.Item item = AppData.Instance.UserMatrixData.GetData(_project.MatrixGuid);
//                        //item.TodayRecordFreeOpportunityCount++;
//                        _userBuy = true;
//                    }
//                    else if(ticket == EPlayRecordTicket.PRT_Buy)
//                    {
//                        if (LocalUser.Instance.User != null)
//                        {
//                            LocalUser.Instance.User.UserInfoRequestGameTimer.Zero();
//                        }
//                        _userBuy = true;
//                        RecordFullInfoRequestTimer.Zero();
//                    }
//                    if (successCallback != null)
//                    {
//                        successCallback.Invoke();
//                    }
//                }
//                else
//                {
//                    if (failedCallback != null)
//                    {
//                        failedCallback.Invoke((EPlayRecordRetCode)ret.ResultCode);
//                    }
//                }
//
//            }, (errCode, errMsg) =>
//            {
//                SoyHttpClient.ShowErrorTip(errCode);
//                if (failedCallback != null)
//                {
//                    failedCallback.Invoke(EPlayRecordRetCode.PRRC_None);
//                }
//            });
//        }

        public void PrepareRecord(Action successCallback, Action failedCallback = null)
        {
            if (string.IsNullOrEmpty(_recordPath))
            {
                if (failedCallback != null)
                {
                    failedCallback.Invoke();
                }
                return;
            }
            byte[] recordData = LocalCacheManager.Instance.Load(LocalCacheManager.EType.File, _recordPath);
            if (recordData != null)
            {
                if (successCallback != null)
                {
                    successCallback.Invoke();
                }
                return;
            }
            SFile file = SFile.GetFileWithUrl(SoyPath.Instance.GetFileUrl(_recordPath));
            file.DownloadAsync((f) =>
                {
                    LocalCacheManager.Instance.Save(f.FileBytes, LocalCacheManager.EType.File, _recordPath);
                    if (successCallback != null)
                    {
                        successCallback.Invoke();
                    }
                }, sFile =>
                {
                    if (failedCallback != null)
                    {
                        failedCallback.Invoke();
                    }
                });
        }


        public void UpdateFavorite(bool favorite, Action<bool> callback = null)
        {
//            if (_userFavorite == favorite)
//            {
//                if (callback != null)
//                {
//                    callback.Invoke(false);
//                }
//                return;
//            }
//            Msg_CA_UpdateRecordFavorite msg = new Msg_CA_UpdateRecordFavorite();
//            msg.RecordId = _id;
//            msg.Favorite = favorite;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_RecordOperateResult>(SoyHttpApiPath.UpdateRecordFavorite, msg, ret =>
//            {
//                Record p;
//                if (ret.ResultCode != (int)EProjectRecordOperateResultCode.PRORC_Success)
//                {
//                    if (ret.RecordData != null)
//                    {
//                        p = RecordManager.Instance.OnSync(ret.RecordData, _project);
//                        if (p != this)
//                        {
//                            Set(ret.RecordData, _project);
//                        }
//                        if (callback != null)
//                        {
//                            callback.Invoke(false);
//                        }
//                        return;
//                    }
//                }
//                p = RecordManager.Instance.OnSync(ret.RecordData, _project);
//                if (p != this)
//                {
//                    Set(ret.RecordData, _project);
//                }
//                if (callback != null)
//                {
//                    callback.Invoke(true);
//                }
//            }, (code, msgStr) =>
//            {
//                if (callback != null)
//                {
//                    callback.Invoke(false);
//                }
//            });
        }

        public void UpdateLike(bool likeFlag, Action<bool> callback = null)
        {
//            if (_userLike == likeFlag)
//            {
//                if (callback != null)
//                {
//                    callback.Invoke(false);
//                }
//                return;
//            }
//            Msg_CA_UpdateRecordLike msg = new Msg_CA_UpdateRecordLike();
//            msg.RecordId = _id;
//            msg.LikeFlag = likeFlag;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_RecordOperateResult>(SoyHttpApiPath.UpdateRecordLike, msg, ret =>
//            {
//                Record p;
//                if (ret.ResultCode != (int)EProjectRecordOperateResultCode.PRORC_Success)
//                {
//                    if (ret.RecordData != null)
//                    {
//                        p = RecordManager.Instance.OnSync(ret.RecordData, _project);
//                        if (p != this)
//                        {
//                            Set(ret.RecordData, _project);
//                        }
//                    }
//                    if (callback != null)
//                    {
//                        callback.Invoke(false);
//                    }
//                    return;
//                }
//                if (ret.RecordData != null)
//                {
//                    p = RecordManager.Instance.OnSync(ret.RecordData, _project);
//                    if (p != this)
//                    {
//                        Set(ret.RecordData, _project);
//                    }
//                }
//                if (callback != null)
//                {
//                    callback.Invoke(true);
//                }
//            }, (code, msgStr) =>
//            {
//                if (callback != null)
//                {
//                    callback.Invoke(false);
//                }
//            });
        }

        public void AddShareCount(Action<bool> callback = null)
        {
//            Msg_CA_OnShareSuccess msg = new Msg_CA_OnShareSuccess();
//            msg.RefType = EAppContentItemType.ACIT_Record;
//            msg.RefId = _id;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_OnShareSuccessRet>(SoyHttpApiPath.OnShareSuccess, msg, ret =>
//            {
//                if (ret.OnShareSuccessRetCode != (int)EOnShareSuccessRetCode.OSSRC_Success)
//                {
//                    if (callback != null)
//                    {
//                        callback.Invoke(false);
//                    }
//                    return;
//                }
//                else
//                {
//                    _shareCount = _shareCount + 1;
//                    if (callback != null)
//                    {
//                        callback.Invoke(true);
//                    }
//                }
//            }, (code, msgStr) =>
//            {
//                if (callback != null)
//                {
//                    callback.Invoke(false);
//                }
//            });
        }


//        public void SendComment(int timePos, ERecordCommentStyle style, Vector2 pos, string comment, Action<bool> callback)
//        {
//            if (LocalUser.Instance.User == null || string.IsNullOrEmpty(comment) || comment.Length > SoyConstDefine.MaxRecordCommentLength)
//            {
//                if (callback != null)
//                {
//                    callback.Invoke(false);
//                }
//                return;
//            }
//            long oldId = LocalCacheManager.Instance.GetLocalGuid();
//            RecordComment localData = new RecordComment();
//            localData.UserInfo = LocalUser.Instance.User;
//            localData.RecordId = _id;
//            localData.Comment = comment;
//            localData.CreateTime = DateTimeUtil.GetServerTimeNowTimestampMillis();
//            localData.Guid = oldId;
//            localData.Pos = pos;
//            localData.TimePos = timePos;
//            if (_timelineRecordCommentList == null)
//            {
//                _timelineRecordCommentList = new List<RecordComment>();
//            }
//            if (_recordCommentList == null)
//            {
//                _recordCommentList = new List<RecordComment>();
//            }
//            _timelineRecordCommentList.Add(localData);
//            _timelineRecordCommentList.Sort(TimelineRecordCommentListSortFunction);
//            _recordCommentList.Insert(0, localData);
//            _commentCount++;
//            MessengerAsync.Broadcast(EMessengerType.OnRecordCommentChanged);
//            Msg_CA_PostRecordComment msg = new Msg_CA_PostRecordComment();
//            msg.RecordId = _id;
//            msg.TimePos = timePos;
//            msg.Style = style;
//            msg.PosX = pos.x;
//            msg.PosY = pos.y;
//            msg.Comment = comment;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_PostRecordComment>(SoyHttpApiPath.CommentRecord, msg, ret =>
//            {
//                if (ret.ResultCode != (int)EProjectRecordOperateResultCode.PRORC_Success)
//                {
//                    if (callback != null)
//                    {
//                        callback.Invoke(false);
//                    }
//                    return;
//                }
//                _recordCommentListRequestTimer.Zero();
//                localData.Set(ret.RecordComment);
//                _recordCommentList.Sort(RecordCommentListSortFunction);
//                _timelineRecordCommentList.Sort(TimelineRecordCommentListSortFunction);
//                Messenger<Msg_AC_Reward>.Broadcast(EMessengerType.OnReceiveReward, ret.Reward);
//                Messenger<long, long>.Broadcast(EMessengerType.OnSendRecordCommentSuccess, oldId, ret.RecordComment.RecordCommentId);
//                if (callback != null)
//                {
//                    callback.Invoke(true);
//                }
//            }, (code, msgStr) =>
//            {
//                _commentCount--;
//                if (callback != null)
//                {
//                    callback.Invoke(false);
//                }
//                MessengerAsync.Broadcast(EMessengerType.OnRecordCommentChanged);
//            });
//        }

//        public void OnSyncCommentList(Msg_AC_RecordCommentList msg)
//        {
//            if (_recordCommentList == null)
//            {
//                _recordCommentList = new List<RecordComment>();
//                if (msg.Result == ECachedDataState.CDS_None
//                   || msg.Result == ECachedDataState.CDS_Uptodate)
//                {
//                    MessengerAsync.Broadcast(EMessengerType.OnRecordCommentChanged);
//                }
//            }
//            _commentCount = msg.TotalCount;
//            if (msg.Result == ECachedDataState.CDS_None
//                || msg.Result == ECachedDataState.CDS_Uptodate)
//            {
//                return;
//            }
//            if (msg.Result == ECachedDataState.CDS_Recreate)
//            {
//                _recordCommentList.Clear();
//            }
//            msg.DataList.ForEach(msgItem => _recordCommentList.Add(new RecordComment(msgItem)));
//            _recordCommentList.Sort(RecordCommentListSortFunction);
//            MessengerAsync.Broadcast(EMessengerType.OnRecordCommentChanged);
//        }

//        public void OnSyncTimelineCommentList(Msg_AC_RecordCommentList msg)
//        {
//            if (_timelineRecordCommentList == null)
//            {
//                _timelineRecordCommentList = new List<RecordComment>();
//                if (msg.Result == ECachedDataState.CDS_None
//                   || msg.Result == ECachedDataState.CDS_Uptodate)
//                {
//                    MessengerAsync.Broadcast(EMessengerType.OnRecordCommentChanged);
//                }
//            }
//            _commentCount = msg.TotalCount;
//            if (msg.Result == ECachedDataState.CDS_None
//                || msg.Result == ECachedDataState.CDS_Uptodate)
//            {
//                _timelineLastSyncIsEnd = true;
//                return;
//            }
//            if (msg.Result == ECachedDataState.CDS_Recreate)
//            {
//                _timelineRecordCommentList.Clear();
//            }
//            msg.DataList.ForEach(msgItem => _timelineRecordCommentList.Add(new RecordComment(msgItem)));
//            _timelineRecordCommentList.Sort(TimelineRecordCommentListSortFunction);
//            if (_timelineRecordCommentList.Count > 0)
//            {
//                RecordComment lastItem = _timelineRecordCommentList[_timelineRecordCommentList.Count - 1];
//                _timelineLastSyncTimePos = lastItem.TimePos;
//                _timelineLastSyncCreateTime = lastItem.CreateTime;
//            }
//            if (msg.DataList.Count < SoyConstDefine.MaxRecordCommentFetchSize)
//            {
//                _timelineLastSyncIsEnd = true;
//            }
//            else
//            {
//                _timelineLastSyncIsEnd = false;
//            }
//            MessengerAsync.Broadcast(EMessengerType.OnRecordCommentChanged);
//        }

//        private int RecordCommentListSortFunction(RecordComment c1, RecordComment c2)
//        {
//            return -c1.CreateTime.CompareTo(c2.CreateTime);
//        }
//
//        private int TimelineRecordCommentListSortFunction(RecordComment c1, RecordComment c2)
//        {
//            if (c1.TimePos != c2.TimePos)
//            {
//                return c1.TimePos.CompareTo(c2.TimePos);
//            }
//            else
//            {
//                return -c1.CreateTime.CompareTo(c2.CreateTime);
//            }
//        }
    }

    public class RecordRankHolder
    {
        private Record _record;
        private int _rank;

        public Record Record
        {
            get
            {
                return _record;
            }
        }

        public int Rank
        {
            get
            {
                return _rank;
            }
        }

        public RecordRankHolder(Msg_SC_DAT_Record msg, Project project, int rank)
        {
            _record = RecordManager.Instance.OnSync(msg, project, false);
            _rank = rank;
        }

        public RecordRankHolder(Record record, int rank)
        {
            _record = record;
            _rank = rank;
        }
    }
}
