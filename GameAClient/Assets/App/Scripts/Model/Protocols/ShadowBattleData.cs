// 匹配乱入对决 | 匹配乱入对决
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ShadowBattleData : SyncronisticData<Msg_SC_DAT_ShadowBattleData> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 乱入对决Id
        /// </summary>
        private long _id;
        /// <summary>
        /// 录像数据
        /// </summary>
        private Record _record;
        /// <summary>
        /// 过关奖励
        /// </summary>
        private Reward _reward;
        /// <summary>
        /// 原始挑战者
        /// </summary>
        private UserInfoSimple _originPlayer;

        // cs fields----------------------------------
        /// <summary>
        /// BattleId
        /// </summary>
        private long _cs_id;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 乱入对决Id
        /// </summary>
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 录像数据
        /// </summary>
        public Record Record { 
            get { return _record; }
            set { if (_record != value) {
                _record = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 过关奖励
        /// </summary>
        public Reward Reward { 
            get { return _reward; }
            set { if (_reward != value) {
                _reward = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 原始挑战者
        /// </summary>
        public UserInfoSimple OriginPlayer { 
            get { return _originPlayer; }
            set { if (_originPlayer != value) {
                _originPlayer = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// BattleId
        /// </summary>
        public long CS_Id { 
            get { return _cs_id; }
            set { _cs_id = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _record && _record.IsDirty) {
                    return true;
                }
                if (null != _reward && _reward.IsDirty) {
                    return true;
                }
                if (null != _originPlayer && _originPlayer.IsDirty) {
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
		/// <param name="id">BattleId.</param>
        public void Request (
            long id,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_id != id) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_id = id;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_ShadowBattleData msg = new Msg_CS_DAT_ShadowBattleData();
                msg.Id = id;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_ShadowBattleData>(
                    SoyHttpApiPath.ShadowBattleData, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_ShadowBattleData msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
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
            if (null == _originPlayer) {
                _originPlayer = new UserInfoSimple(msg.OriginPlayer);
            } else {
                _originPlayer.OnSyncFromParent(msg.OriginPlayer);
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_ShadowBattleData msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
            if(null != msg.Record){
                if (null == _record){
                    _record = new Record(msg.Record);
                }
                _record.CopyMsgData(msg.Record);
            }
            if(null != msg.Reward){
                if (null == _reward){
                    _reward = new Reward(msg.Reward);
                }
                _reward.CopyMsgData(msg.Reward);
            }
            if(null != msg.OriginPlayer){
                if (null == _originPlayer){
                    _originPlayer = new UserInfoSimple(msg.OriginPlayer);
                }
                _originPlayer.CopyMsgData(msg.OriginPlayer);
            }
            return true;
        } 

        public bool DeepCopy (ShadowBattleData obj)
        {
            if (null == obj) return false;
            _id = obj.Id;           
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
            if(null != obj.OriginPlayer){
                if (null == _originPlayer){
                    _originPlayer = new UserInfoSimple();
                }
                _originPlayer.DeepCopy(obj.OriginPlayer);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_ShadowBattleData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ShadowBattleData (Msg_SC_DAT_ShadowBattleData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ShadowBattleData () { 
            _record = new Record();
            _reward = new Reward();
            _originPlayer = new UserInfoSimple();
            OnCreate();
        }
        #endregion
    }
}