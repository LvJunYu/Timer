// 匹配乱入对决 | 匹配乱入对决
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class MatchShadowBattleData : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// ECachedDataState
        /// </summary>
        private int _resultCode;
        /// <summary>
        /// 
        /// </summary>
        private UserInfoSimple _userData;
        /// <summary>
        /// 
        /// </summary>
        private Project _project;
        /// <summary>
        /// 
        /// </summary>
        private Record _record;
        /// <summary>
        /// 
        /// </summary>
        private Reward _reward;

        // cs fields----------------------------------
        /// <summary>
        /// 用户Id
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// ECachedDataState
        /// </summary>
        public int ResultCode { 
            get { return _resultCode; }
            set { if (_resultCode != value) {
                _resultCode = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public UserInfoSimple UserData { 
            get { return _userData; }
            set { if (_userData != value) {
                _userData = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public Project Project { 
            get { return _project; }
            set { if (_project != value) {
                _project = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public Record Record { 
            get { return _record; }
            set { if (_record != value) {
                _record = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public Reward Reward { 
            get { return _reward; }
            set { if (_reward != value) {
                _reward = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 用户Id
        /// </summary>
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _userData && _userData.IsDirty) {
                    return true;
                }
                if (null != _project && _project.IsDirty) {
                    return true;
                }
                if (null != _record && _record.IsDirty) {
                    return true;
                }
                if (null != _reward && _reward.IsDirty) {
                    return true;
                }
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 匹配乱入对决
		/// </summary>
		/// <param name="userId">用户Id.</param>
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

                Msg_CS_DAT_MatchShadowBattleData msg = new Msg_CS_DAT_MatchShadowBattleData();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_MatchShadowBattleData>(
                    SoyHttpApiPath.MatchShadowBattleData, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_MatchShadowBattleData msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            if (null == _userData) {
                _userData = new UserInfoSimple(msg.UserData);
            } else {
                _userData.OnSyncFromParent(msg.UserData);
            }
            if (null == _project) {
                _project = new Project(msg.Project);
            } else {
                _project.OnSyncFromParent(msg.Project);
            }
            if (null == _record) {
                _record = new Record(msg.Record);
            } else {
                _record.OnSyncFromParent(msg.Record);
            }
            if (null == _reward) {
                _reward = new Reward(msg.Reward);
            } else {
                _reward.OnSyncFromParent(msg.Reward);
            }
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (MatchShadowBattleData obj)
        {
            if (null == obj) return false;
            _resultCode = obj.ResultCode;           
            if(null != obj.UserData){
                if (null == _userData){
                    _userData = new UserInfoSimple();
                }
                _userData.DeepCopy(obj.UserData);
            }
            if(null != obj.Project){
                if (null == _project){
                    _project = new Project();
                }
                _project.DeepCopy(obj.Project);
            }
            if(null != obj.Record){
                if (null == _record){
                    _record = new Record();
                }
                _record.DeepCopy(obj.Record);
            }
            if(null != obj.Reward){
                if (null == _reward){
                    _reward = new Reward();
                }
                _reward.DeepCopy(obj.Reward);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_MatchShadowBattleData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public MatchShadowBattleData (Msg_SC_DAT_MatchShadowBattleData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public MatchShadowBattleData () { 
            _userData = new UserInfoSimple();
            _project = new Project();
            _record = new Record();
            _reward = new Reward();
            OnCreate();
        }
        #endregion
    }
}