// 冒险模式好友最高关数据 | 冒险模式好友最高关数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AllFriendsAdvProgressData : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 最好关卡好友数据列表
        /// </summary>
        private List<AdvProgressData> _advProgressDataList;

        // cs fields----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 最好关卡好友数据列表
        /// </summary>
        public List<AdvProgressData> AdvProgressDataList { 
            get { return _advProgressDataList; }
            set { if (_advProgressDataList != value) {
                _advProgressDataList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _advProgressDataList) {
                    for (int i = 0; i < _advProgressDataList.Count; i++) {
                        if (null != _advProgressDataList[i] && _advProgressDataList[i].IsDirty) {
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
		/// 冒险模式好友最高关数据
		/// </summary>
		/// <param name="userId">用户.</param>
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

                Msg_CS_DAT_AllFriendsAdvProgressData msg = new Msg_CS_DAT_AllFriendsAdvProgressData();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_AllFriendsAdvProgressData>(
                    SoyHttpApiPath.AllFriendsAdvProgressData, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_AllFriendsAdvProgressData msg)
        {
            if (null == msg) return false;
            _advProgressDataList = new List<AdvProgressData>();
            for (int i = 0; i < msg.AdvProgressDataList.Count; i++) {
                _advProgressDataList.Add(new AdvProgressData(msg.AdvProgressDataList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (AllFriendsAdvProgressData obj)
        {
            if (null == obj) return false;
            if (null ==  obj.AdvProgressDataList) return false;
            if (null ==  _advProgressDataList) {
                _advProgressDataList = new List<AdvProgressData>();
            }
            _advProgressDataList.Clear();
            for (int i = 0; i < obj.AdvProgressDataList.Count; i++){
                _advProgressDataList.Add(obj.AdvProgressDataList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_AllFriendsAdvProgressData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AllFriendsAdvProgressData (Msg_SC_DAT_AllFriendsAdvProgressData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AllFriendsAdvProgressData () { 
            _advProgressDataList = new List<AdvProgressData>();
            OnCreate();
        }
        #endregion
    }
}