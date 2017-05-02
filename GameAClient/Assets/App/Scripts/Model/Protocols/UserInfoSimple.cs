// 用户简要信息 | 用户简要信息
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserInfoSimple : SyncronisticData 
    {
        #region 字段
        // sc fields----------------------------------
        // 用户ID
        private long _userId;
        // 昵称
        private string _nickName;
        // 头像
        private string _headImgUrl;
        // 性别
        private ESex _sex;
        // 和我的关系
        private UserRelationWithMe _relationWithMe;
        // 等级数据
        private UserLevel _levelData;

        // cs fields----------------------------------
        // 用户id
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        // 用户ID
        public long UserId { 
            get { return _userId; }
            set { if (_userId != value) {
                _userId = value;
                SetDirty();
            }}
        }
        // 昵称
        public string NickName { 
            get { return _nickName; }
            set { if (_nickName != value) {
                _nickName = value;
                SetDirty();
            }}
        }
        // 头像
        public string HeadImgUrl { 
            get { return _headImgUrl; }
            set { if (_headImgUrl != value) {
                _headImgUrl = value;
                SetDirty();
            }}
        }
        // 性别
        public ESex Sex { 
            get { return _sex; }
            set { if (_sex != value) {
                _sex = value;
                SetDirty();
            }}
        }
        // 和我的关系
        public UserRelationWithMe RelationWithMe { 
            get { return _relationWithMe; }
            set { if (_relationWithMe != value) {
                _relationWithMe = value;
                SetDirty();
            }}
        }
        // 等级数据
        public UserLevel LevelData { 
            get { return _levelData; }
            set { if (_levelData != value) {
                _levelData = value;
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
                if (null != _relationWithMe && _relationWithMe.IsDirty) {
                    return true;
                }
                if (null != _levelData && _levelData.IsDirty) {
                    return true;
                }
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 用户简要信息
		/// </summary>
		/// <param name="userId">用户id.</param>
        public void Request (
            long userId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            OnRequest (successCallback, failedCallback);

            Msg_CS_DAT_UserInfoSimple msg = new Msg_CS_DAT_UserInfoSimple();
            msg.UserId = userId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserInfoSimple>(
                SoyHttpApiPath.UserInfoSimple, msg, ret => {
                    if (OnSync(ret)) {
                        OnSyncSucceed(); 
                    }
                }, (failedCode, failedMsg) => {
                    OnSyncFailed(failedCode, failedMsg);
            });
        }

        public bool OnSync (Msg_SC_DAT_UserInfoSimple msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            _nickName = msg.NickName;           
            _headImgUrl = msg.HeadImgUrl;           
            _sex = msg.Sex;           
            if (null == _relationWithMe) {
                _relationWithMe = new UserRelationWithMe(msg.RelationWithMe);
            } else {
                _relationWithMe.OnSyncFromParent(msg.RelationWithMe);
            }
            if (null == _levelData) {
                _levelData = new UserLevel(msg.LevelData);
            } else {
                _levelData.OnSyncFromParent(msg.LevelData);
            }
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserInfoSimple msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserInfoSimple (Msg_SC_DAT_UserInfoSimple msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserInfoSimple () { 
            _relationWithMe = new UserRelationWithMe();
            _levelData = new UserLevel();
        }
        #endregion
    }
}