// 获取蓝钻数据 | 获取蓝钻数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class BlueVipData : SyncronisticData<Msg_SC_DAT_BlueVipData> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 是否是蓝钻
        /// </summary>
        private bool _isBlueVip;
        /// <summary>
        /// 是否是超级蓝钻
        /// </summary>
        private bool _isSuperBlueVip;
        /// <summary>
        /// 是否是年费蓝钻
        /// </summary>
        private bool _isBlueYearVip;
        /// <summary>
        /// 蓝钻等级
        /// </summary>
        private int _blueVipLevel;
        /// <summary>
        /// OpopenId，对应QQ号
        /// </summary>
        private string _opopenId;

        // cs fields----------------------------------
        /// <summary>
        /// 用户id
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 是否是蓝钻
        /// </summary>
        public bool IsBlueVip { 
            get { return _isBlueVip; }
            set { if (_isBlueVip != value) {
                _isBlueVip = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 是否是超级蓝钻
        /// </summary>
        public bool IsSuperBlueVip { 
            get { return _isSuperBlueVip; }
            set { if (_isSuperBlueVip != value) {
                _isSuperBlueVip = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 是否是年费蓝钻
        /// </summary>
        public bool IsBlueYearVip { 
            get { return _isBlueYearVip; }
            set { if (_isBlueYearVip != value) {
                _isBlueYearVip = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 蓝钻等级
        /// </summary>
        public int BlueVipLevel { 
            get { return _blueVipLevel; }
            set { if (_blueVipLevel != value) {
                _blueVipLevel = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// OpopenId，对应QQ号
        /// </summary>
        public string OpopenId { 
            get { return _opopenId; }
            set { if (_opopenId != value) {
                _opopenId = value;
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
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 获取蓝钻数据
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

                Msg_CS_DAT_BlueVipData msg = new Msg_CS_DAT_BlueVipData();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_BlueVipData>(
                    SoyHttpApiPath.BlueVipData, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_BlueVipData msg)
        {
            if (null == msg) return false;
            _isBlueVip = msg.IsBlueVip;           
            _isSuperBlueVip = msg.IsSuperBlueVip;           
            _isBlueYearVip = msg.IsBlueYearVip;           
            _blueVipLevel = msg.BlueVipLevel;           
            _opopenId = msg.OpopenId;           
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_BlueVipData msg)
        {
            if (null == msg) return false;
            _isBlueVip = msg.IsBlueVip;           
            _isSuperBlueVip = msg.IsSuperBlueVip;           
            _isBlueYearVip = msg.IsBlueYearVip;           
            _blueVipLevel = msg.BlueVipLevel;           
            _opopenId = msg.OpopenId;           
            return true;
        } 

        public bool DeepCopy (BlueVipData obj)
        {
            if (null == obj) return false;
            _isBlueVip = obj.IsBlueVip;           
            _isSuperBlueVip = obj.IsSuperBlueVip;           
            _isBlueYearVip = obj.IsBlueYearVip;           
            _blueVipLevel = obj.BlueVipLevel;           
            _opopenId = obj.OpopenId;           
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_BlueVipData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public BlueVipData (Msg_SC_DAT_BlueVipData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public BlueVipData () { 
            OnCreate();
        }
        #endregion
    }
}