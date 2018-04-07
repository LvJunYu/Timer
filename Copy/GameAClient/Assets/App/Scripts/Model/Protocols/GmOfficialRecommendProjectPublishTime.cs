// 获取官方推荐关卡当前发布时间 | 获取官方推荐关卡当前发布时间
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class GmOfficialRecommendProjectPublishTime : SyncronisticData<Msg_SC_DAT_GmOfficialRecommendProjectPublishTime> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 发布时间
        /// </summary>
        private long _publishTime;

        // cs fields----------------------------------
        /// <summary>
        /// 占位
        /// </summary>
        private int _cs_flag;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 发布时间
        /// </summary>
        public long PublishTime { 
            get { return _publishTime; }
            set { if (_publishTime != value) {
                _publishTime = value;
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
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 获取官方推荐关卡当前发布时间
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

                Msg_CS_DAT_GmOfficialRecommendProjectPublishTime msg = new Msg_CS_DAT_GmOfficialRecommendProjectPublishTime();
                msg.Flag = flag;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_GmOfficialRecommendProjectPublishTime>(
                    SoyHttpApiPath.GmOfficialRecommendProjectPublishTime, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_GmOfficialRecommendProjectPublishTime msg)
        {
            if (null == msg) return false;
            _publishTime = msg.PublishTime;           
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_GmOfficialRecommendProjectPublishTime msg)
        {
            if (null == msg) return false;
            _publishTime = msg.PublishTime;           
            return true;
        } 

        public bool DeepCopy (GmOfficialRecommendProjectPublishTime obj)
        {
            if (null == obj) return false;
            _publishTime = obj.PublishTime;           
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_GmOfficialRecommendProjectPublishTime msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmOfficialRecommendProjectPublishTime (Msg_SC_DAT_GmOfficialRecommendProjectPublishTime msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmOfficialRecommendProjectPublishTime () { 
            OnCreate();
        }
        #endregion
    }
}