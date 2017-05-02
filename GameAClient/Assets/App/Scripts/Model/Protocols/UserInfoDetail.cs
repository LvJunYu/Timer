// 用户详细信息 | 用户详细信息
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserInfoDetail : SyncronisticData 
    {
        #region 字段
        // sc fields----------------------------------
        // 简要信息
        private UserInfoSimple _userInfoSimple;
        // 
        private string _userName;
        // 
        private string _phoneNum;
        // 生日
        private long _birthDay;
        // 国家
        private string _country;
        // 省份
        private string _province;
        // 城市
        private string _city;
        // 签名
        private string _profile;
        // 更新时间
        private long _updateTime;
        // 账号类型
        private int _roleType;
        // 社交关系统计
        private UserRelationStatistic _relationStatistic;

        // cs fields----------------------------------
        // 用户id
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        // 简要信息
        public UserInfoSimple UserInfoSimple { 
            get { return _userInfoSimple; }
            set { if (_userInfoSimple != value) {
                _userInfoSimple = value;
                SetDirty();
            }}
        }
        // 
        public string UserName { 
            get { return _userName; }
            set { if (_userName != value) {
                _userName = value;
                SetDirty();
            }}
        }
        // 
        public string PhoneNum { 
            get { return _phoneNum; }
            set { if (_phoneNum != value) {
                _phoneNum = value;
                SetDirty();
            }}
        }
        // 生日
        public long BirthDay { 
            get { return _birthDay; }
            set { if (_birthDay != value) {
                _birthDay = value;
                SetDirty();
            }}
        }
        // 国家
        public string Country { 
            get { return _country; }
            set { if (_country != value) {
                _country = value;
                SetDirty();
            }}
        }
        // 省份
        public string Province { 
            get { return _province; }
            set { if (_province != value) {
                _province = value;
                SetDirty();
            }}
        }
        // 城市
        public string City { 
            get { return _city; }
            set { if (_city != value) {
                _city = value;
                SetDirty();
            }}
        }
        // 签名
        public string Profile { 
            get { return _profile; }
            set { if (_profile != value) {
                _profile = value;
                SetDirty();
            }}
        }
        // 更新时间
        public long UpdateTime { 
            get { return _updateTime; }
            set { if (_updateTime != value) {
                _updateTime = value;
                SetDirty();
            }}
        }
        // 账号类型
        public int RoleType { 
            get { return _roleType; }
            set { if (_roleType != value) {
                _roleType = value;
                SetDirty();
            }}
        }
        // 社交关系统计
        public UserRelationStatistic RelationStatistic { 
            get { return _relationStatistic; }
            set { if (_relationStatistic != value) {
                _relationStatistic = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        // 用户id
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

        public bool OnSync (Msg_SC_DAT_UserInfoDetail msg)
        {
            if (null == msg) return false;
            if (null == _userInfoSimple) {
                _userInfoSimple = new UserInfoSimple(msg.UserInfoSimple);
            } else {
                _userInfoSimple.OnSyncFromParent(msg.UserInfoSimple);
            }
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
            OnSyncPartial();
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
        }
        #endregion
    }
}