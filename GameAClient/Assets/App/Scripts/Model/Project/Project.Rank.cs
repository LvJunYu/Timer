/********************************************************************
** Filename : Project.Rank.cs
** Author : quan
** Date : 6/6/2017 2:41 PM
** Summary : Project.Rank.cs
***********************************************************************/

 
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using UnityEngine;
using SoyEngine;

namespace GameA
{
	public partial class Project : SyncronisticData {
        #region 变量
//        private List<PlayedProjectUser> _recentPlayedUserList;


        private WorldProjectRecentRecordList _recentRecordList = new WorldProjectRecentRecordList();
        private WorldProjectRecordRankList _recordRankList = new WorldProjectRecordRankList();
        #endregion 变量
        #region 属性


//        public List<PlayedProjectUser> RecentPlayedUserList
//        {
//            get
//            {
//                return _recentPlayedUserList;
//            }
//        }

        public List<RecordRankHolder> ProjectPlayRecordList
        {
            get
            {
                return _recordRankList.AllList;
            }
        }

        public List<Record> ProjectRecentRecordList
        {
            get
            {
                return _recentRecordList.AllList;
            }
        }


        #endregion 属性
        #region 方法

//        public void OnSyncRecentPlayedUserDataList(Msg_AC_ProjectRecentPlayedUserList msg)
//        {
//            if (_recentPlayedUserList == null)
//            {
//                _recentPlayedUserList = new List<PlayedProjectUser>();
//                if (msg.Result == ECachedDataState.CDS_None
//                   || msg.Result == ECachedDataState.CDS_Uptodate)
//                {
//                    MessengerAsync.Broadcast(EMessengerType.OnProjectRecentPlayedChanged);
//                }
//            }
//            if (msg.Result == ECachedDataState.CDS_None
//                || msg.Result == ECachedDataState.CDS_Uptodate)
//            {
//                return;
//            }
//            if (msg.Result == ECachedDataState.CDS_Recreate)
//            {
//                _recentPlayedUserList.Clear();
//            }
//            msg.DataList.ForEach(msgItem =>
//                                 _recentPlayedUserList.Add(new PlayedProjectUser(msgItem)));
//            _recentPlayedUserList.Sort((p1, p2) =>
//                                       -p1.LastPlayTime.CompareTo(p2.LastPlayTime));
//            MessengerAsync.Broadcast(EMessengerType.OnProjectRecentPlayedChanged);
//        }

        public void RequestRecentRecordList (
            int startInx,
            int maxCount,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            _recentRecordList.Request(_projectId, startInx, maxCount,
                successCallback, failedCallback);
        }

        public void RequestRecordRankList (
            int startInx,
            int maxCount,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            _recordRankList.Request(_projectId, startInx, maxCount,
                successCallback, failedCallback);
        }
        #endregion


        public class PlayedProjectUser
        {
            private User _user;
            private long _lastPlayTime;
            public User User
            {
                get
                {
                    return _user;
                }

                set
                {
                    _user = value;
                }
            }

            public long LastPlayTime
            {
                get
                {
                    return _lastPlayTime;
                }

                set
                {
                    _lastPlayTime = value;
                }
            }

            //            public PlayedProjectUser(Msg_ProjectRecentPlayedUserData msg)
            //            {
            //                Set(msg);
            //            }

            //            public void Set(Msg_ProjectRecentPlayedUserData msg)
            //            {
            //                _user = UserManager.Instance.OnSyncUserData(msg.UserData);
            //                _lastPlayTime = msg.LastPlayTime;
            //            }
        }
    }
}