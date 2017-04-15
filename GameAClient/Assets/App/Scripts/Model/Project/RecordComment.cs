///********************************************************************
//** Filename : RecordComment.cs
//** Author : quan
//** Date : 1/9/2017 3:26 PM
//** Summary : RecordComment.cs
//***********************************************************************/
//using System;
//using SoyEngine.Proto;
//using SoyEngine;
//using UnityEngine;
//
//namespace GameA
//{
//    public class RecordComment
//    {
//        #region field
//        private long _guid;
//        private User _userInfo;
//        private string _comment;
//        private int _timePos;
//        private Vector2 _pos;
//        private ERecordCommentStyle _style;
//        private long _recordId;
//        private long _createTime;
//        #endregion field
//
//        #region property
//        public long Guid
//        {
//            get
//            {
//                return _guid;
//            }
//
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
//                return _userInfo;
//            }
//
//            set
//            {
//                _userInfo = value;
//            }
//        }
//
//        public string Comment
//        {
//            get
//            {
//                return _comment;
//            }
//
//            set
//            {
//                _comment = value;
//            }
//        }
//
//        public int TimePos
//        {
//            get
//            {
//                return _timePos;
//            }
//
//            set
//            {
//                _timePos = value;
//            }
//        }
//
//        public Vector2 Pos
//        {
//            get
//            {
//                return _pos;
//            }
//
//            set
//            {
//                _pos = value;
//            }
//        }
//
//        public ERecordCommentStyle Style
//        {
//            get
//            {
//                return _style;
//            }
//
//            set
//            {
//                _style = value;
//            }
//        }
//
//        public long RecordId
//        {
//            get
//            {
//                return _recordId;
//            }
//
//            set
//            {
//                _recordId = value;
//            }
//        }
//
//        public long CreateTime
//        {
//            get
//            {
//                return _createTime;
//            }
//
//            set
//            {
//                _createTime = value;
//            }
//        }
//        #endregion property
//        public RecordComment()
//        {
//        }
//
//        public RecordComment(Msg_RecordComment msg)
//        {
//            Set(msg);
//        }
//
//        public void Set(Msg_RecordComment msg)
//        {
//            _guid = msg.RecordCommentId;
//            _comment = msg.Comment;
//            _recordId = msg.RecordId;
//            _timePos = msg.TimePos;
//            _style = (ERecordCommentStyle)msg.Style;
//            _pos = new Vector2(msg.PosX, msg.PosY);
//            _createTime = msg.CreateTime;
//            if (msg.UserInfo != null)
//            {
//                _userInfo = UserManager.Instance.OnSyncUserData(msg.UserInfo);
//            }
//        }
//    }
//}
//
