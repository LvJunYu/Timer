// 匹配乱入对决 | 匹配乱入对决
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class MatchShadowBattleData : SyncronisticData<Msg_SC_DAT_MatchShadowBattleData> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// EMatchShadowBattleDataCode
        /// </summary>
        private int _resultCode;
        /// <summary>
        /// 
        /// </summary>
        private Project _project;
        /// <summary>
        /// 
        /// </summary>
        private long _token;
        /// <summary>
        /// 
        /// </summary>
        private byte[] _deadPos;
        /// <summary>
        /// 乱入对决
        /// </summary>
        private ShadowBattleData _shadowBattleData;

        // cs fields----------------------------------
        /// <summary>
        /// 用户Id
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// EMatchShadowBattleDataCode
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
        public long Token { 
            get { return _token; }
            set { if (_token != value) {
                _token = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] DeadPos { 
            get { return _deadPos; }
            set { if (_deadPos != value) {
                _deadPos = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 乱入对决
        /// </summary>
        public ShadowBattleData ShadowBattleData { 
            get { return _shadowBattleData; }
            set { if (_shadowBattleData != value) {
                _shadowBattleData = value;
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
                if (null != _project && _project.IsDirty) {
                    return true;
                }
                if (null != _shadowBattleData && _shadowBattleData.IsDirty) {
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
            if (null == _project) {
                _project = new Project(msg.Project);
            } else {
                _project.OnSyncFromParent(msg.Project);
            }
            _token = msg.Token;           
            _deadPos = msg.DeadPos;           
            if (null == _shadowBattleData) {
                _shadowBattleData = new ShadowBattleData(msg.ShadowBattleData);
            } else {
                _shadowBattleData.OnSyncFromParent(msg.ShadowBattleData);
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_MatchShadowBattleData msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            if(null != msg.Project){
                if (null == _project){
                    _project = new Project(msg.Project);
                }
                _project.CopyMsgData(msg.Project);
            }
            _token = msg.Token;           
            _deadPos = msg.DeadPos;           
            if(null != msg.ShadowBattleData){
                if (null == _shadowBattleData){
                    _shadowBattleData = new ShadowBattleData(msg.ShadowBattleData);
                }
                _shadowBattleData.CopyMsgData(msg.ShadowBattleData);
            }
            return true;
        } 

        public bool DeepCopy (MatchShadowBattleData obj)
        {
            if (null == obj) return false;
            _resultCode = obj.ResultCode;           
            if(null != obj.Project){
                if (null == _project){
                    _project = new Project();
                }
                _project.DeepCopy(obj.Project);
            }
            _token = obj.Token;           
            _deadPos = obj.DeadPos;           
            if(null != obj.ShadowBattleData){
                if (null == _shadowBattleData){
                    _shadowBattleData = new ShadowBattleData();
                }
                _shadowBattleData.DeepCopy(obj.ShadowBattleData);
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
            _project = new Project();
            _shadowBattleData = new ShadowBattleData();
            OnCreate();
        }
        #endregion
    }
}