//  | 获取通知数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class NotificationDataItem : SyncronisticData<Msg_NotificationDataItem> {
        #region 字段
        /// <summary>
        /// ENotificationDataType
        /// </summary>
        private ENotificationDataType _type;
        /// <summary>
        /// 
        /// </summary>
        private long _id;
        /// <summary>
        /// 
        /// </summary>
        private long _createTime;
        /// <summary>
        /// 
        /// </summary>
        private int _count;
        /// <summary>
        /// 时间发起者
        /// </summary>
        private UserInfoSimple _sender;
        /// <summary>
        /// 关卡评论回复
        /// </summary>
        private ProjectCommentReply _projectCommentReply;
        /// <summary>
        /// 留言板回复
        /// </summary>
        private UserMessageReply _userMessageReply;
        /// <summary>
        /// 关卡信息
        /// </summary>
        private Project _projectData;
        #endregion

        #region 属性
        /// <summary>
        /// ENotificationDataType
        /// </summary>
        public ENotificationDataType Type { 
            get { return _type; }
            set { if (_type != value) {
                _type = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long CreateTime { 
            get { return _createTime; }
            set { if (_createTime != value) {
                _createTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int Count { 
            get { return _count; }
            set { if (_count != value) {
                _count = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 时间发起者
        /// </summary>
        public UserInfoSimple Sender { 
            get { return _sender; }
            set { if (_sender != value) {
                _sender = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 关卡评论回复
        /// </summary>
        public ProjectCommentReply ProjectCommentReply { 
            get { return _projectCommentReply; }
            set { if (_projectCommentReply != value) {
                _projectCommentReply = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 留言板回复
        /// </summary>
        public UserMessageReply UserMessageReply { 
            get { return _userMessageReply; }
            set { if (_userMessageReply != value) {
                _userMessageReply = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 关卡信息
        /// </summary>
        public Project ProjectData { 
            get { return _projectData; }
            set { if (_projectData != value) {
                _projectData = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_NotificationDataItem msg)
        {
            if (null == msg) return false;
            _type = msg.Type;     
            _id = msg.Id;     
            _createTime = msg.CreateTime;     
            _count = msg.Count;     
            if (null == _sender) {
                _sender = new UserInfoSimple(msg.Sender);
            } else {
                _sender.OnSyncFromParent(msg.Sender);
            }
            if (null == _projectCommentReply) {
                _projectCommentReply = new ProjectCommentReply(msg.ProjectCommentReply);
            } else {
                _projectCommentReply.OnSyncFromParent(msg.ProjectCommentReply);
            }
            if (null == _userMessageReply) {
                _userMessageReply = new UserMessageReply(msg.UserMessageReply);
            } else {
                _userMessageReply.OnSyncFromParent(msg.UserMessageReply);
            }
            if (null == _projectData) {
                _projectData = new Project(msg.ProjectData);
            } else {
                _projectData.OnSyncFromParent(msg.ProjectData);
            }
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_NotificationDataItem msg)
        {
            if (null == msg) return false;
            _type = msg.Type;           
            _id = msg.Id;           
            _createTime = msg.CreateTime;           
            _count = msg.Count;           
            if(null != msg.Sender){
                if (null == _sender){
                    _sender = new UserInfoSimple(msg.Sender);
                }
                _sender.CopyMsgData(msg.Sender);
            }
            if(null != msg.ProjectCommentReply){
                if (null == _projectCommentReply){
                    _projectCommentReply = new ProjectCommentReply(msg.ProjectCommentReply);
                }
                _projectCommentReply.CopyMsgData(msg.ProjectCommentReply);
            }
            if(null != msg.UserMessageReply){
                if (null == _userMessageReply){
                    _userMessageReply = new UserMessageReply(msg.UserMessageReply);
                }
                _userMessageReply.CopyMsgData(msg.UserMessageReply);
            }
            if(null != msg.ProjectData){
                if (null == _projectData){
                    _projectData = new Project(msg.ProjectData);
                }
                _projectData.CopyMsgData(msg.ProjectData);
            }
            return true;
        } 

        public bool DeepCopy (NotificationDataItem obj)
        {
            if (null == obj) return false;
            _type = obj.Type;           
            _id = obj.Id;           
            _createTime = obj.CreateTime;           
            _count = obj.Count;           
            if(null != obj.Sender){
                if (null == _sender){
                    _sender = new UserInfoSimple();
                }
                _sender.DeepCopy(obj.Sender);
            }
            if(null != obj.ProjectCommentReply){
                if (null == _projectCommentReply){
                    _projectCommentReply = new ProjectCommentReply();
                }
                _projectCommentReply.DeepCopy(obj.ProjectCommentReply);
            }
            if(null != obj.UserMessageReply){
                if (null == _userMessageReply){
                    _userMessageReply = new UserMessageReply();
                }
                _userMessageReply.DeepCopy(obj.UserMessageReply);
            }
            if(null != obj.ProjectData){
                if (null == _projectData){
                    _projectData = new Project();
                }
                _projectData.DeepCopy(obj.ProjectData);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_NotificationDataItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NotificationDataItem (Msg_NotificationDataItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NotificationDataItem () { 
            _sender = new UserInfoSimple();
            _projectCommentReply = new ProjectCommentReply();
            _userMessageReply = new UserMessageReply();
            _projectData = new Project();
        }
        #endregion
    }
}