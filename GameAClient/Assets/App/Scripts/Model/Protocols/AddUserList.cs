// 添加好友列表 | 添加好友列表
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AddUserList : SyncronisticData<Msg_SC_DAT_AddUserList> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 结果
        /// </summary>
        private int _resultCode;
        /// <summary>
        /// 更新时间
        /// </summary>
        private long _updataTime;
        /// <summary>
        /// 用户列表
        /// </summary>
        private List<UserInfoSimple> _dataList;

        // cs fields----------------------------------
        /// <summary>
        /// 用户id
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 结果
        /// </summary>
        public int ResultCode { 
            get { return _resultCode; }
            set { if (_resultCode != value) {
                _resultCode = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 更新时间
        /// </summary>
        public long UpdataTime { 
            get { return _updataTime; }
            set { if (_updataTime != value) {
                _updataTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        public List<UserInfoSimple> DataList { 
            get { return _dataList; }
            set { if (_dataList != value) {
                _dataList = value;
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
		/// 添加好友列表
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

                Msg_CS_DAT_AddUserList msg = new Msg_CS_DAT_AddUserList();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_AddUserList>(
                    SoyHttpApiPath.AddUserList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_AddUserList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updataTime = msg.UpdataTime;           
            _dataList = new List<UserInfoSimple>();
            for (int i = 0; i < msg.DataList.Count; i++) {
                _dataList.Add(new UserInfoSimple(msg.DataList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_AddUserList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updataTime = msg.UpdataTime;           
            if (null ==  _dataList) {
                _dataList = new List<UserInfoSimple>();
            }
            _dataList.Clear();
            for (int i = 0; i < msg.DataList.Count; i++) {
                _dataList.Add(new UserInfoSimple(msg.DataList[i]));
            }
            return true;
        } 

        public bool DeepCopy (AddUserList obj)
        {
            if (null == obj) return false;
            _resultCode = obj.ResultCode;           
            _updataTime = obj.UpdataTime;           
            if (null ==  obj.DataList) return false;
            if (null ==  _dataList) {
                _dataList = new List<UserInfoSimple>();
            }
            _dataList.Clear();
            for (int i = 0; i < obj.DataList.Count; i++){
                _dataList.Add(obj.DataList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_AddUserList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AddUserList (Msg_SC_DAT_AddUserList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AddUserList () { 
            _dataList = new List<UserInfoSimple>();
            OnCreate();
        }
        #endregion
    }
}