//   /********************************************************************
//   ** Filename : ProjectComment.cs
//   ** Author : quan
//   ** Date : 2016/6/6 12:36
//   ** Summary : ProjectComment.cs
//   ***********************************************************************/
//using System;
//using SoyEngine.Proto;
//using SoyEngine;
//
//namespace GameA
//{
//    public class ProjectComment
//    {
//        #region field
//        private long _guid;
//        private User _userInfo;
//        private User _targetUserInfo;
//        private string _comment;
//        private long _projectGuid;
//        private long _createTime;
//        #endregion field
//
//        #region property
//        public long Guid
//        {
//            get
//            {
//                return this._guid;
//            }
//            set
//            {
//                _guid = value;
//            }
//        }
//
//        public User UserInfo
//        {
//            get
//            {
//                return this._userInfo;
//            }
//            set
//            {
//                _userInfo = value;
//            }
//        }
//
//        public User TargetUserInfo
//        {
//            get
//            {
//                return this._targetUserInfo;
//            }
//            set
//            {
//                _targetUserInfo = value;
//            }
//        }
//
//        public string Comment
//        {
//            get
//            {
//                return this._comment;
//            }
//            set
//            {
//                _comment = value;
//            }
//        }
//
//        public long ProjectGuid
//        {
//            get
//            {
//                return this._projectGuid;
//            }
//            set
//            {
//                _projectGuid = value;
//            }
//        }
//
//        public long CreateTime
//        {
//            get
//            {
//                return this._createTime;
//            }
//            set
//            {
//                _createTime = value;
//            }
//        }
//        #endregion property
//        public ProjectComment()
//        {
//        }
//        public ProjectComment(Msg_ProjectComment msg)
//        {
//            Set(msg);
//        }
//
//        public void Set(Msg_ProjectComment msg)
//        {
//            _guid = msg.Guid;
//            _comment = msg.Comment;
//            _projectGuid = msg.ProjectGuid;
//            _createTime = msg.CreateTime;
//            if (msg.UserInfo != null)
//            {
//                _userInfo = UserManager.Instance.OnSyncUserData(msg.UserInfo);
//            }
//            if (msg.TargetUserInfo != null)
//            {
//                _targetUserInfo = UserManager.Instance.OnSyncUserData(msg.TargetUserInfo);
//            }
//        }
//    }
//}
//
