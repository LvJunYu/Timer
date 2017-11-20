//  | 邮件
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class Mail : SyncronisticData<Msg_Mail> {
        #region 字段
        /// <summary>
        /// Id
        /// </summary>
        private long _id;
        /// <summary>
        /// 
        /// </summary>
        private UserInfoSimple _userInfo;
        /// <summary>
        /// 
        /// </summary>
        private EMailType _mailType;
        /// <summary>
        /// 
        /// </summary>
        private EMailFuncType _funcType;
        /// <summary>
        /// 
        /// </summary>
        private string _title;
        /// <summary>
        /// 
        /// </summary>
        private string _content;
        /// <summary>
        /// 
        /// </summary>
        private long _contentId;
        /// <summary>
        /// 
        /// </summary>
        private Reward _attachItemList;
        /// <summary>
        /// 
        /// </summary>
        private bool _readFlag;
        /// <summary>
        /// 
        /// </summary>
        private long _readTime;
        /// <summary>
        /// 
        /// </summary>
        private bool _receiptedFlag;
        /// <summary>
        /// 
        /// </summary>
        private long _receiptedTime;
        /// <summary>
        /// 
        /// </summary>
        private long _createTime;
        #endregion

        #region 属性
        /// <summary>
        /// Id
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
        public UserInfoSimple UserInfo { 
            get { return _userInfo; }
            set { if (_userInfo != value) {
                _userInfo = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public EMailType MailType { 
            get { return _mailType; }
            set { if (_mailType != value) {
                _mailType = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public EMailFuncType FuncType { 
            get { return _funcType; }
            set { if (_funcType != value) {
                _funcType = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public string Title { 
            get { return _title; }
            set { if (_title != value) {
                _title = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public string Content { 
            get { return _content; }
            set { if (_content != value) {
                _content = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long ContentId { 
            get { return _contentId; }
            set { if (_contentId != value) {
                _contentId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public Reward AttachItemList { 
            get { return _attachItemList; }
            set { if (_attachItemList != value) {
                _attachItemList = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public bool ReadFlag { 
            get { return _readFlag; }
            set { if (_readFlag != value) {
                _readFlag = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long ReadTime { 
            get { return _readTime; }
            set { if (_readTime != value) {
                _readTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public bool ReceiptedFlag { 
            get { return _receiptedFlag; }
            set { if (_receiptedFlag != value) {
                _receiptedFlag = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long ReceiptedTime { 
            get { return _receiptedTime; }
            set { if (_receiptedTime != value) {
                _receiptedTime = value;
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
        #endregion

        #region 方法
        public bool OnSync (Msg_Mail msg)
        {
            if (null == msg) return false;
            _id = msg.Id;     
            if (null == _userInfo) {
                _userInfo = new UserInfoSimple(msg.UserInfo);
            } else {
                _userInfo.OnSyncFromParent(msg.UserInfo);
            }
            _mailType = msg.MailType;     
            _funcType = msg.FuncType;     
            _title = msg.Title;     
            _content = msg.Content;     
            _contentId = msg.ContentId;     
            if (null == _attachItemList) {
                _attachItemList = new Reward(msg.AttachItemList);
            } else {
                _attachItemList.OnSyncFromParent(msg.AttachItemList);
            }
            _readFlag = msg.ReadFlag;     
            _readTime = msg.ReadTime;     
            _receiptedFlag = msg.ReceiptedFlag;     
            _receiptedTime = msg.ReceiptedTime;     
            _createTime = msg.CreateTime;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_Mail msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
            if(null != msg.UserInfo){
                if (null == _userInfo){
                    _userInfo = new UserInfoSimple(msg.UserInfo);
                }
                _userInfo.CopyMsgData(msg.UserInfo);
            }
            _mailType = msg.MailType;           
            _funcType = msg.FuncType;           
            _title = msg.Title;           
            _content = msg.Content;           
            _contentId = msg.ContentId;           
            if(null != msg.AttachItemList){
                if (null == _attachItemList){
                    _attachItemList = new Reward(msg.AttachItemList);
                }
                _attachItemList.CopyMsgData(msg.AttachItemList);
            }
            _readFlag = msg.ReadFlag;           
            _readTime = msg.ReadTime;           
            _receiptedFlag = msg.ReceiptedFlag;           
            _receiptedTime = msg.ReceiptedTime;           
            _createTime = msg.CreateTime;           
            return true;
        } 

        public bool DeepCopy (Mail obj)
        {
            if (null == obj) return false;
            _id = obj.Id;           
            if(null != obj.UserInfo){
                if (null == _userInfo){
                    _userInfo = new UserInfoSimple();
                }
                _userInfo.DeepCopy(obj.UserInfo);
            }
            _mailType = obj.MailType;           
            _funcType = obj.FuncType;           
            _title = obj.Title;           
            _content = obj.Content;           
            _contentId = obj.ContentId;           
            if(null != obj.AttachItemList){
                if (null == _attachItemList){
                    _attachItemList = new Reward();
                }
                _attachItemList.DeepCopy(obj.AttachItemList);
            }
            _readFlag = obj.ReadFlag;           
            _readTime = obj.ReadTime;           
            _receiptedFlag = obj.ReceiptedFlag;           
            _receiptedTime = obj.ReceiptedTime;           
            _createTime = obj.CreateTime;           
            return true;
        }

        public void OnSyncFromParent (Msg_Mail msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Mail (Msg_Mail msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Mail () { 
            _userInfo = new UserInfoSimple();
            _attachItemList = new Reward();
        }
        #endregion
    }
}