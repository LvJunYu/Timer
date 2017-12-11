// 获取预设列表 | 获取预设列表
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UnitPreinstallList : SyncronisticData<Msg_SC_DAT_UnitPreinstallList> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 预设列表
        /// </summary>
        private List<UnitPreinstall> _preinstallList;

        // cs fields----------------------------------
        /// <summary>
        /// 物体ID
        /// </summary>
        private int _cs_unitId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 预设列表
        /// </summary>
        public List<UnitPreinstall> PreinstallList { 
            get { return _preinstallList; }
            set { if (_preinstallList != value) {
                _preinstallList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 物体ID
        /// </summary>
        public int CS_UnitId { 
            get { return _cs_unitId; }
            set { _cs_unitId = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _preinstallList) {
                    for (int i = 0; i < _preinstallList.Count; i++) {
                        if (null != _preinstallList[i] && _preinstallList[i].IsDirty) {
                            return true;
                        }
                    }
                }
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 获取预设列表
		/// </summary>
		/// <param name="unitId">物体ID.</param>
        public void Request (
            int unitId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_unitId != unitId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_unitId = unitId;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_UnitPreinstallList msg = new Msg_CS_DAT_UnitPreinstallList();
                msg.UnitId = unitId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UnitPreinstallList>(
                    SoyHttpApiPath.UnitPreinstallList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_UnitPreinstallList msg)
        {
            if (null == msg) return false;
            _preinstallList = new List<UnitPreinstall>();
            for (int i = 0; i < msg.PreinstallList.Count; i++) {
                _preinstallList.Add(new UnitPreinstall(msg.PreinstallList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_UnitPreinstallList msg)
        {
            if (null == msg) return false;
            if (null ==  _preinstallList) {
                _preinstallList = new List<UnitPreinstall>();
            }
            _preinstallList.Clear();
            for (int i = 0; i < msg.PreinstallList.Count; i++) {
                _preinstallList.Add(new UnitPreinstall(msg.PreinstallList[i]));
            }
            return true;
        } 

        public bool DeepCopy (UnitPreinstallList obj)
        {
            if (null == obj) return false;
            if (null ==  obj.PreinstallList) return false;
            if (null ==  _preinstallList) {
                _preinstallList = new List<UnitPreinstall>();
            }
            _preinstallList.Clear();
            for (int i = 0; i < obj.PreinstallList.Count; i++){
                _preinstallList.Add(obj.PreinstallList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UnitPreinstallList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UnitPreinstallList (Msg_SC_DAT_UnitPreinstallList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UnitPreinstallList () { 
            _preinstallList = new List<UnitPreinstall>();
            OnCreate();
        }
        #endregion
    }
}