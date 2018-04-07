// 获取关卡推荐状态 | 获取关卡推荐状态
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class GmProjectOfficialRecommendStatus : SyncronisticData<Msg_SC_DAT_GmProjectOfficialRecommendStatus> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 是否在推荐中
        /// </summary>
        private bool _isOfficialRecommend;
        /// <summary>
        /// 状态
        /// </summary>
        private EOfficialRecommendProjectStatus _status;

        // cs fields----------------------------------
        /// <summary>
        /// 关卡id
        /// </summary>
        private long _cs_projectId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 是否在推荐中
        /// </summary>
        public bool IsOfficialRecommend { 
            get { return _isOfficialRecommend; }
            set { if (_isOfficialRecommend != value) {
                _isOfficialRecommend = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 状态
        /// </summary>
        public EOfficialRecommendProjectStatus Status { 
            get { return _status; }
            set { if (_status != value) {
                _status = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 关卡id
        /// </summary>
        public long CS_ProjectId { 
            get { return _cs_projectId; }
            set { _cs_projectId = value; }
        }

        public override bool IsDirty {
            get {
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 获取关卡推荐状态
		/// </summary>
		/// <param name="projectId">关卡id.</param>
        public void Request (
            long projectId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_projectId != projectId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_projectId = projectId;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_GmProjectOfficialRecommendStatus msg = new Msg_CS_DAT_GmProjectOfficialRecommendStatus();
                msg.ProjectId = projectId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_GmProjectOfficialRecommendStatus>(
                    SoyHttpApiPath.GmProjectOfficialRecommendStatus, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_GmProjectOfficialRecommendStatus msg)
        {
            if (null == msg) return false;
            _isOfficialRecommend = msg.IsOfficialRecommend;           
            _status = msg.Status;           
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_GmProjectOfficialRecommendStatus msg)
        {
            if (null == msg) return false;
            _isOfficialRecommend = msg.IsOfficialRecommend;           
            _status = msg.Status;           
            return true;
        } 

        public bool DeepCopy (GmProjectOfficialRecommendStatus obj)
        {
            if (null == obj) return false;
            _isOfficialRecommend = obj.IsOfficialRecommend;           
            _status = obj.Status;           
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_GmProjectOfficialRecommendStatus msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmProjectOfficialRecommendStatus (Msg_SC_DAT_GmProjectOfficialRecommendStatus msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmProjectOfficialRecommendStatus () { 
            OnCreate();
        }
        #endregion
    }
}