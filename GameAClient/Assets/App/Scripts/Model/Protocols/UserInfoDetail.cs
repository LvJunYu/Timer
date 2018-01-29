// 用户详细信息 | 用户详细信息
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserInfoDetail : SyncronisticData<Msg_SC_DAT_UserInfoDetail> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 简要信息
        /// </summary>
        private UserInfoSimple _userInfoSimple;
        /// <summary>
        /// 短id
        /// </summary>
        private long _shortId;
        /// <summary>
        /// 
        /// </summary>
        private string _userName;
        /// <summary>
        /// 
        /// </summary>
        private string _phoneNum;
        /// <summary>
        /// 生日
        /// </summary>
        private long _birthDay;
        /// <summary>
        /// 国家
        /// </summary>
        private string _country;
        /// <summary>
        /// 省份
        /// </summary>
        private string _province;
        /// <summary>
        /// 城市
        /// </summary>
        private string _city;
        /// <summary>
        /// 签名
        /// </summary>
        private string _profile;
        /// <summary>
        /// 更新时间
        /// </summary>
        private long _updateTime;
        /// <summary>
        /// 账号类型
        /// </summary>
        private int _roleType;
        /// <summary>
        /// 社交关系统计
        /// </summary>
        private UserRelationStatistic _relationStatistic;
        /// <summary>
        /// 登录次数
        /// </summary>
        private int _loginCount;
        /// <summary>
        /// 最后登录时间
        /// </summary>
        private long _lastLoginTime;
        /// <summary>
        /// 留言次数
        /// </summary>
        private int _messageCount;

        // cs fields----------------------------------
        /// <summary>
        /// 用户id
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 简要信息
        /// </summary>
        public UserInfoSimple UserInfoSimple { 
            get { return _userInfoSimple; }
            set { if (_userInfoSimple != value) {
                _userInfoSimple = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 短id
        /// </summary>
        public long ShortId { 
            get { return _shortId; }
            set { if (_shortId != value) {
                _shortId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public string UserName { 
            get { return _userName; }
            set { if (_userName != value) {
                _userName = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public string PhoneNum { 
            get { return _phoneNum; }
            set { if (_phoneNum != value) {
                _phoneNum = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 生日
        /// </summary>
        public long BirthDay { 
            get { return _birthDay; }
            set { if (_birthDay != value) {
                _birthDay = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { 
            get { return _country; }
            set { if (_country != value) {
                _country = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 省份
        /// </summary>
        public string Province { 
            get { return _province; }
            set { if (_province != value) {
                _province = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 城市
        /// </summary>
        public string City { 
            get { return _city; }
            set { if (_city != value) {
                _city = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 签名
        /// </summary>
        public string Profile { 
            get { return _profile; }
            set { if (_profile != value) {
                _profile = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 更新时间
        /// </summary>
        public long UpdateTime { 
            get { return _updateTime; }
            set { if (_updateTime != value) {
                _updateTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 账号类型
        /// </summary>
        public int RoleType { 
            get { return _roleType; }
            set { if (_roleType != value) {
                _roleType = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 社交关系统计
        /// </summary>
        public UserRelationStatistic RelationStatistic { 
            get { return _relationStatistic; }
            set { if (_relationStatistic != value) {
                _relationStatistic = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 登录次数
        /// </summary>
        public int LoginCount { 
            get { return _loginCount; }
            set { if (_loginCount != value) {
                _loginCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public long LastLoginTime { 
            get { return _lastLoginTime; }
            set { if (_lastLoginTime != value) {
                _lastLoginTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 留言次数
        /// </summary>
        public int MessageCount { 
            get { return _messageCount; }
            set { if (_messageCount != value) {
                _messageCount = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 用户id
        /// </summary>
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _userInfoSimple && _userInfoSimple.IsDirty) {
                    return true;
                }
                if (null != _relationStatistic && _relationStatistic.IsDirty) {
                    return true;
                }
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 用户详细信息
		/// </summary>
		/// <param name="userId">用户id.</param>
        public void Request (
            long userId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_userId != userId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_userId = userId;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_UserInfoDetail msg = new Msg_CS_DAT_UserInfoDetail();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserInfoDetail>(
                    SoyHttpApiPath.UserInfoDetail, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_UserInfoDetail msg)
        {
            if (null == msg) return false;
            if (null == _userInfoSimple) {
                _userInfoSimple = new UserInfoSimple(msg.UserInfoSimple);
            } else {
                _userInfoSimple.OnSyncFromParent(msg.UserInfoSimple);
            }
            _shortId = msg.ShortId;           
            _userName = msg.UserName;           
            _phoneNum = msg.PhoneNum;           
            _birthDay = msg.BirthDay;           
            _country = msg.Country;           
            _province = msg.Province;           
            _city = msg.City;           
            _profile = msg.Profile;           
            _updateTime = msg.UpdateTime;           
            _roleType = msg.RoleType;           
            if (null == _relationStatistic) {
                _relationStatistic = new UserRelationStatistic(msg.RelationStatistic);
            } else {
                _relationStatistic.OnSyncFromParent(msg.RelationStatistic);
            }
            _loginCount = msg.LoginCount;           
            _lastLoginTime = msg.LastLoginTime;           
            _messageCount = msg.MessageCount;           
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_UserInfoDetail msg)
        {
            if (null == msg) return false;
            if(null != msg.UserInfoSimple){
                if (null == _userInfoSimple){
                    _userInfoSimple = new UserInfoSimple(msg.UserInfoSimple);
                }
                _userInfoSimple.CopyMsgData(msg.UserInfoSimple);
            }
            _shortId = msg.ShortId;           
            _userName = msg.UserName;           
            _phoneNum = msg.PhoneNum;           
            _birthDay = msg.BirthDay;           
            _country = msg.Country;           
            _province = msg.Province;           
            _city = msg.City;           
            _profile = msg.Profile;           
            _updateTime = msg.UpdateTime;           
            _roleType = msg.RoleType;           
            if(null != msg.RelationStatistic){
                if (null == _relationStatistic){
                    _relationStatistic = new UserRelationStatistic(msg.RelationStatistic);
                }
                _relationStatistic.CopyMsgData(msg.RelationStatistic);
            }
            _loginCount = msg.LoginCount;           
            _lastLoginTime = msg.LastLoginTime;           
            _messageCount = msg.MessageCount;           
            return true;
        } 

        public bool DeepCopy (UserInfoDetail obj)
        {
            if (null == obj) return false;
            if(null != obj.UserInfoSimple){
                if (null == _userInfoSimple){
                    _userInfoSimple = new UserInfoSimple();
                }
                _userInfoSimple.DeepCopy(obj.UserInfoSimple);
            }
            _shortId = obj.ShortId;           
            _userName = obj.UserName;           
            _phoneNum = obj.PhoneNum;           
            _birthDay = obj.BirthDay;           
            _country = obj.Country;           
            _province = obj.Province;           
            _city = obj.City;           
            _profile = obj.Profile;           
            _updateTime = obj.UpdateTime;           
            _roleType = obj.RoleType;           
            if(null != obj.RelationStatistic){
                if (null == _relationStatistic){
                    _relationStatistic = new UserRelationStatistic();
                }
                _relationStatistic.DeepCopy(obj.RelationStatistic);
            }
            _loginCount = obj.LoginCount;           
            _lastLoginTime = obj.LastLoginTime;           
            _messageCount = obj.MessageCount;           
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserInfoDetail msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserInfoDetail (Msg_SC_DAT_UserInfoDetail msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserInfoDetail () { 
            _userInfoSimple = new UserInfoSimple();
            _relationStatistic = new UserRelationStatistic();
            OnCreate();
        }
        #endregion
    }
}