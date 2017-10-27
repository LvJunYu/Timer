// 用户简要信息 | 用户简要信息
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserInfoSimple : SyncronisticData<Msg_SC_DAT_UserInfoSimple> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 用户ID
        /// </summary>
        private long _userId;
        /// <summary>
        /// 昵称
        /// </summary>
        private string _nickName;
        /// <summary>
        /// 头像
        /// </summary>
        private string _headImgUrl;
        /// <summary>
        /// 性别
        /// </summary>
        private ESex _sex;
        /// <summary>
        /// 和我的关系
        /// </summary>
        private UserRelationWithMe _relationWithMe;
        /// <summary>
        /// 等级数据
        /// </summary>
        private UserLevel _levelData;

        // cs fields----------------------------------
        /// <summary>
        /// 用户id
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { 
            get { return _userId; }
            set { if (_userId != value) {
                _userId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { 
            get { return _nickName; }
            set { if (_nickName != value) {
                _nickName = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 头像
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
        /// 和我的关系
        /// </summary>
        public UserRelationWithMe RelationWithMe { 
            get { return _relationWithMe; }
            set { if (_relationWithMe != value) {
                _relationWithMe = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 等级数据
        /// </summary>
        public UserLevel LevelData { 
            get { return _levelData; }
            set { if (_levelData != value) {
                _levelData = value;
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
            if (_isRequesting) {
                if (_cs_userId != userId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_userId = userId;
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
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_UserInfoSimple msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            _nickName = msg.NickName;           
            _headImgUrl = msg.HeadImgUrl;           
            _sex = msg.Sex;           
            if(null != msg.RelationWithMe){
                if (null == _relationWithMe){
                    _relationWithMe = new UserRelationWithMe(msg.RelationWithMe);
                }
                _relationWithMe.CopyMsgData(msg.RelationWithMe);
            }
            if(null != msg.LevelData){
                if (null == _levelData){
                    _levelData = new UserLevel(msg.LevelData);
                }
                _levelData.CopyMsgData(msg.LevelData);
            }
            return true;
        } 

        public bool DeepCopy (UserInfoSimple obj)
        {
            if (null == obj) return false;
            _userId = obj.UserId;           
            _nickName = obj.NickName;           
            _headImgUrl = obj.HeadImgUrl;           
            _sex = obj.Sex;           
            if(null != obj.RelationWithMe){
                if (null == _relationWithMe){
                    _relationWithMe = new UserRelationWithMe();
                }
                _relationWithMe.DeepCopy(obj.RelationWithMe);
            }
            if(null != obj.LevelData){
                if (null == _levelData){
                    _levelData = new UserLevel();
                }
                _levelData.DeepCopy(obj.LevelData);
            }
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
            OnCreate();
        }
        #endregion
    }
}