// 获取物体预设数据 | 获取物体预设数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UnitPreinstall : SyncronisticData<Msg_SC_DAT_UnitPreinstall> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 预设Id
        /// </summary>
        private long _preinstallId;
        /// <summary>
        /// 
        /// </summary>
        private long _createTime;
        /// <summary>
        /// 
        /// </summary>
        private long _updateTime;
        /// <summary>
        /// 
        /// </summary>
        private Preinstall _preinstallData;

        // cs fields----------------------------------
        /// <summary>
        /// 预设Id
        /// </summary>
        private long _cs_preinstallId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 预设Id
        /// </summary>
        public long PreinstallId { 
            get { return _preinstallId; }
            set { if (_preinstallId != value) {
                _preinstallId = value;
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
        /// 
        /// </summary>
        public long UpdateTime { 
            get { return _updateTime; }
            set { if (_updateTime != value) {
                _updateTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public Preinstall PreinstallData { 
            get { return _preinstallData; }
            set { if (_preinstallData != value) {
                _preinstallData = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 预设Id
        /// </summary>
        public long CS_PreinstallId { 
            get { return _cs_preinstallId; }
            set { _cs_preinstallId = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _preinstallData && _preinstallData.IsDirty) {
                    return true;
                }
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 获取物体预设数据
		/// </summary>
		/// <param name="preinstallId">预设Id.</param>
        public void Request (
            long preinstallId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_preinstallId != preinstallId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_preinstallId = preinstallId;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_UnitPreinstall msg = new Msg_CS_DAT_UnitPreinstall();
                msg.PreinstallId = preinstallId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UnitPreinstall>(
                    SoyHttpApiPath.UnitPreinstall, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_UnitPreinstall msg)
        {
            if (null == msg) return false;
            _preinstallId = msg.PreinstallId;           
            _createTime = msg.CreateTime;           
            _updateTime = msg.UpdateTime;           
            if (null == _preinstallData) {
                _preinstallData = new Preinstall(msg.PreinstallData);
            } else {
                _preinstallData.OnSyncFromParent(msg.PreinstallData);
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_UnitPreinstall msg)
        {
            if (null == msg) return false;
            _preinstallId = msg.PreinstallId;           
            _createTime = msg.CreateTime;           
            _updateTime = msg.UpdateTime;           
            if(null != msg.PreinstallData){
                if (null == _preinstallData){
                    _preinstallData = new Preinstall(msg.PreinstallData);
                }
                _preinstallData.CopyMsgData(msg.PreinstallData);
            }
            return true;
        } 

        public bool DeepCopy (UnitPreinstall obj)
        {
            if (null == obj) return false;
            _preinstallId = obj.PreinstallId;           
            _createTime = obj.CreateTime;           
            _updateTime = obj.UpdateTime;           
            if(null != obj.PreinstallData){
                if (null == _preinstallData){
                    _preinstallData = new Preinstall();
                }
                _preinstallData.DeepCopy(obj.PreinstallData);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UnitPreinstall msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UnitPreinstall (Msg_SC_DAT_UnitPreinstall msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UnitPreinstall () { 
            _preinstallData = new Preinstall();
            OnCreate();
        }
        #endregion
    }
}