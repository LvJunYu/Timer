// 角色训练数据 | 角色训练数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserTrainProperty : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        private long _userId;
        /// <summary>
        /// 培养点数
        /// </summary>
        private int _trainPoint;
        /// <summary>
        /// 训练属性列表
        /// </summary>
        private List<TrainProperty> _itemDataList;
        /// <summary>
        /// 当前阶层等级
        /// </summary>
        private int _grade;

        // cs fields----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        public long UserId { 
            get { return _userId; }
            set { if (_userId != value) {
                _userId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 培养点数
        /// </summary>
        public int TrainPoint { 
            get { return _trainPoint; }
            set { if (_trainPoint != value) {
                _trainPoint = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 训练属性列表
        /// </summary>
        public List<TrainProperty> ItemDataList { 
            get { return _itemDataList; }
            set { if (_itemDataList != value) {
                _itemDataList = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 当前阶层等级
        /// </summary>
        public int Grade { 
            get { return _grade; }
            set { if (_grade != value) {
                _grade = value;
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
                if (null != _itemDataList) {
                    for (int i = 0; i < _itemDataList.Count; i++) {
                        if (null != _itemDataList[i] && _itemDataList[i].IsDirty) {
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
		/// 角色训练数据
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

                Msg_CS_DAT_UserTrainProperty msg = new Msg_CS_DAT_UserTrainProperty();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserTrainProperty>(
                    SoyHttpApiPath.UserTrainProperty, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_UserTrainProperty msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            _trainPoint = msg.TrainPoint;           
            _itemDataList = new List<TrainProperty>();
            for (int i = 0; i < msg.ItemDataList.Count; i++) {
                _itemDataList.Add(new TrainProperty(msg.ItemDataList[i]));
            }
            _grade = msg.Grade;           
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserTrainProperty msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserTrainProperty (Msg_SC_DAT_UserTrainProperty msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserTrainProperty () { 
            _itemDataList = new List<TrainProperty>();
            OnCreate();
        }
        #endregion
    }
}