// 查询所有GM | 查询所有GM
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class GameMasterList : SyncronisticData<Msg_SC_DAT_GameMasterList> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// ECachedDataState
        /// </summary>
        private int _resultCode;
        /// <summary>
        /// 
        /// </summary>
        private long _updateTime;
        /// <summary>
        /// 
        /// </summary>
        private List<UserInfoDetail> _dataList;

        // cs fields----------------------------------
        /// <summary>
        /// 占位
        /// </summary>
        private int _cs_flag;
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
        public List<UserInfoDetail> DataList { 
            get { return _dataList; }
            set { if (_dataList != value) {
                _dataList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 占位
        /// </summary>
        public int CS_Flag { 
            get { return _cs_flag; }
            set { _cs_flag = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _dataList) {
                    for (int i = 0; i < _dataList.Count; i++) {
                        if (null != _dataList[i] && _dataList[i].IsDirty) {
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
		/// 查询所有GM
		/// </summary>
		/// <param name="flag">占位.</param>
        public void Request (
            int flag,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_flag != flag) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_flag = flag;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_GameMasterList msg = new Msg_CS_DAT_GameMasterList();
                msg.Flag = flag;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_GameMasterList>(
                    SoyHttpApiPath.GameMasterList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_GameMasterList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            _dataList = new List<UserInfoDetail>();
            for (int i = 0; i < msg.DataList.Count; i++) {
                _dataList.Add(new UserInfoDetail(msg.DataList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_GameMasterList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            if (null ==  _dataList) {
                _dataList = new List<UserInfoDetail>();
            }
            _dataList.Clear();
            for (int i = 0; i < msg.DataList.Count; i++) {
                _dataList.Add(new UserInfoDetail(msg.DataList[i]));
            }
            return true;
        } 

        public bool DeepCopy (GameMasterList obj)
        {
            if (null == obj) return false;
            _resultCode = obj.ResultCode;           
            _updateTime = obj.UpdateTime;           
            if (null ==  obj.DataList) return false;
            if (null ==  _dataList) {
                _dataList = new List<UserInfoDetail>();
            }
            _dataList.Clear();
            for (int i = 0; i < obj.DataList.Count; i++){
                _dataList.Add(obj.DataList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_GameMasterList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GameMasterList (Msg_SC_DAT_GameMasterList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GameMasterList () { 
            _dataList = new List<UserInfoDetail>();
            OnCreate();
        }
        #endregion
    }
}