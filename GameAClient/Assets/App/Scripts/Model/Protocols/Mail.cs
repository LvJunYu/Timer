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
        private EMailType _type;
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
        /// <summary>
        /// 乱入战斗数据
        /// </summary>
        private MatchShadowBattleData _shadowBattleData;
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
        public EMailType Type { 
            get { return _type; }
            set { if (_type != value) {
                _type = value;
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
        /// <summary>
        /// 乱入战斗数据
        /// </summary>
        public MatchShadowBattleData ShadowBattleData { 
            get { return _shadowBattleData; }
            set { if (_shadowBattleData != value) {
                _shadowBattleData = value;
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
            _type = msg.Type;     
            _title = msg.Title;     
            _content = msg.Content;     
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
            if (null == _shadowBattleData) {
                _shadowBattleData = new MatchShadowBattleData(msg.ShadowBattleData);
            } else {
                _shadowBattleData.OnSyncFromParent(msg.ShadowBattleData);
            }
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
            _type = msg.Type;           
            _title = msg.Title;           
            _content = msg.Content;           
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
            if(null != msg.ShadowBattleData){
                if (null == _shadowBattleData){
                    _shadowBattleData = new MatchShadowBattleData(msg.ShadowBattleData);
                }
                _shadowBattleData.CopyMsgData(msg.ShadowBattleData);
            }
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
            _type = obj.Type;           
            _title = obj.Title;           
            _content = obj.Content;           
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
            if(null != obj.ShadowBattleData){
                if (null == _shadowBattleData){
                    _shadowBattleData = new MatchShadowBattleData();
                }
                _shadowBattleData.DeepCopy(obj.ShadowBattleData);
            }
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
            _shadowBattleData = new MatchShadowBattleData();
        }
        #endregion
    }
}