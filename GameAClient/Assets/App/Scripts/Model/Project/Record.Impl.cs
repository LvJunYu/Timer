/********************************************************************
** Filename : ProjectPlayRecord.cs
** Author : quan
** Date : 1/4/2017 7:30 PM
** Summary : ProjectPlayRecord.cs
***********************************************************************/

using System;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{

	public partial class Record
	{
	    private UserInfoDetail _userInfoDetail;

	    public UserInfoDetail UserInfoDetail
	    {
	        get { return _userInfoDetail; }
	    }
	    
        public byte[] RecordData
        {
            get
            {
                if (string.IsNullOrEmpty(_recordPath))
                {
                    return null;
                }
                return LocalCacheManager.Instance.LoadSync(LocalCacheManager.EType.File, _recordPath);
            }
        }

	    protected override void OnSyncPartial (Msg_SC_DAT_Record msg)
	    {
	        base.OnSyncPartial ();
	        _userInfoDetail = UserManager.Instance.UpdateData(msg.UserInfo);
	    }
	    
        public void RequestPlay(Action successCallback, Action<ENetResultCode> failedCallback)
        {
            PrepareRecord (() => {
                if (successCallback != null)
                {
                    successCallback.Invoke();
                }
            }, () => {
                if (failedCallback != null)
                {
                    failedCallback.Invoke(ENetResultCode.NR_None);
                }
            });
        }

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
            byte[] recordData = LocalCacheManager.Instance.LoadSync(LocalCacheManager.EType.File, _recordPath);
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

//        public RecordRankHolder(Msg_SC_DAT_Record msg, Project project, int rank)
//        {
//            _record = RecordManager.Instance.OnSync(msg, project, false);
//            _rank = rank;
//        }

        public RecordRankHolder(Record record, int rank)
        {
            _record = record;
            _rank = rank;
        }
    }
}
