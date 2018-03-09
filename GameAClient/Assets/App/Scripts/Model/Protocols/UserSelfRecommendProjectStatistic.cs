// 用户自荐槽数据统计 | 用户自荐槽数据统计
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserSelfRecommendProjectStatistic : SyncronisticData<Msg_SC_DAT_UserSelfRecommendProjectStatistic> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 已经使用的槽位数
        /// </summary>
        private int _currentUsedCount;
        /// <summary>
        /// 总的槽位数
        /// </summary>
        private int _totalCount;

        // cs fields----------------------------------
        /// <summary>
        /// 关卡
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 已经使用的槽位数
        /// </summary>
        public int CurrentUsedCount { 
            get { return _currentUsedCount; }
            set { if (_currentUsedCount != value) {
                _currentUsedCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 总的槽位数
        /// </summary>
        public int TotalCount { 
            get { return _totalCount; }
            set { if (_totalCount != value) {
                _totalCount = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 关卡
        /// </summary>
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }

        public override bool IsDirty {
            get {
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 用户自荐槽数据统计
		/// </summary>
		/// <param name="userId">关卡.</param>
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

                Msg_CS_DAT_UserSelfRecommendProjectStatistic msg = new Msg_CS_DAT_UserSelfRecommendProjectStatistic();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserSelfRecommendProjectStatistic>(
                    SoyHttpApiPath.UserSelfRecommendProjectStatistic, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_UserSelfRecommendProjectStatistic msg)
        {
            if (null == msg) return false;
            _currentUsedCount = msg.CurrentUsedCount;           
            _totalCount = msg.TotalCount;           
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_UserSelfRecommendProjectStatistic msg)
        {
            if (null == msg) return false;
            _currentUsedCount = msg.CurrentUsedCount;           
            _totalCount = msg.TotalCount;           
            return true;
        } 

        public bool DeepCopy (UserSelfRecommendProjectStatistic obj)
        {
            if (null == obj) return false;
            _currentUsedCount = obj.CurrentUsedCount;           
            _totalCount = obj.TotalCount;           
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserSelfRecommendProjectStatistic msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserSelfRecommendProjectStatistic (Msg_SC_DAT_UserSelfRecommendProjectStatistic msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserSelfRecommendProjectStatistic () { 
            OnCreate();
        }
        #endregion
    }
}