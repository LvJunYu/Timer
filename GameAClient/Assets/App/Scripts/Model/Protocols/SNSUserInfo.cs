//  | 社交资料
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class SNSUserInfo : SyncronisticData {
        #region 字段
        /// <summary>
        /// 平台id
        /// </summary>
        private string _pid;
        /// <summary>
        /// 昵称
        /// </summary>
        private string _userNickName;
        /// <summary>
        /// 
        /// </summary>
        private string _headImgUrl;
        /// <summary>
        /// 性别
        /// </summary>
        private ESex _sex;
        /// <summary>
        /// 生日
        /// </summary>
        private long _birthDay;
        /// <summary>
        /// 省份
        /// </summary>
        private string _province;
        /// <summary>
        /// 城市
        /// </summary>
        private string _city;
        /// <summary>
        /// 国家
        /// </summary>
        private string _country;
        /// <summary>
        /// token
        /// </summary>
        private string _accessToken;
        /// <summary>
        /// token
        /// </summary>
        private string _refreshToken;
        /// <summary>
        /// 
        /// </summary>
        private string _additionalId;
        #endregion

        #region 属性
        /// <summary>
        /// 平台id
        /// </summary>
        public string Pid { 
            get { return _pid; }
            set { if (_pid != value) {
                _pid = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 昵称
        /// </summary>
        public string UserNickName { 
            get { return _userNickName; }
            set { if (_userNickName != value) {
                _userNickName = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public string HeadImgUrl { 
            get { return _headImgUrl; }
            set { if (_headImgUrl != value) {
                _headImgUrl = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 性别
        /// </summary>
        public ESex Sex { 
            get { return _sex; }
            set { if (_sex != value) {
                _sex = value;
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
        /// token
        /// </summary>
        public string AccessToken { 
            get { return _accessToken; }
            set { if (_accessToken != value) {
                _accessToken = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// token
        /// </summary>
        public string RefreshToken { 
            get { return _refreshToken; }
            set { if (_refreshToken != value) {
                _refreshToken = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public string AdditionalId { 
            get { return _additionalId; }
            set { if (_additionalId != value) {
                _additionalId = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_SNSUserInfo msg)
        {
            if (null == msg) return false;
            _pid = msg.Pid;     
            _userNickName = msg.UserNickName;     
            _headImgUrl = msg.HeadImgUrl;     
            _sex = msg.Sex;     
            _birthDay = msg.BirthDay;     
            _province = msg.Province;     
            _city = msg.City;     
            _country = msg.Country;     
            _accessToken = msg.AccessToken;     
            _refreshToken = msg.RefreshToken;     
            _additionalId = msg.AdditionalId;     
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SNSUserInfo msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public SNSUserInfo (Msg_SNSUserInfo msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public SNSUserInfo () { 
        }
        #endregion
    }
}