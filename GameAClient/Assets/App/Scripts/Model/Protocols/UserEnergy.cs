// 获取体力数据 | 获取体力数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserEnergy : SyncronisticData 
    {
        #region 字段
        // sc fields----------------------------------
        // 当前体力值
        private int _energy;
        // 体力加速结束时间
        private long _energyBoostingEndTime;
        // 体力最后刷新时间
        private long _energyLastRefreshTime;
        // 体力上限
        private int _energyCapacity;

        // cs fields----------------------------------
        // 用户
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        // 当前体力值
        public int Energy { 
            get { return _energy; }
            set { if (_energy != value) {
                _energy = value;
                SetDirty();
            }}
        }
        // 体力加速结束时间
        public long EnergyBoostingEndTime { 
            get { return _energyBoostingEndTime; }
            set { if (_energyBoostingEndTime != value) {
                _energyBoostingEndTime = value;
                SetDirty();
            }}
        }
        // 体力最后刷新时间
        public long EnergyLastRefreshTime { 
            get { return _energyLastRefreshTime; }
            set { if (_energyLastRefreshTime != value) {
                _energyLastRefreshTime = value;
                SetDirty();
            }}
        }
        // 体力上限
        public int EnergyCapacity { 
            get { return _energyCapacity; }
            set { if (_energyCapacity != value) {
                _energyCapacity = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        // 用户
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
		/// 获取体力数据
		/// </summary>
		/// <param name="userId">用户.</param>
        public void Request (
            long userId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            OnRequest (successCallback, failedCallback);

            Msg_CS_DAT_UserEnergy msg = new Msg_CS_DAT_UserEnergy();
            msg.UserId = userId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserEnergy>(
                SoyHttpApiPath.UserEnergy, msg, ret => {
                    if (OnSync(ret)) {
                        OnSyncSucceed(); 
                    }
                }, (failedCode, failedMsg) => {
                    OnSyncFailed(failedCode, failedMsg);
            });
        }

        public bool OnSync (Msg_SC_DAT_UserEnergy msg)
        {
            if (null == msg) return false;
            _energy = msg.Energy;           
            _energyBoostingEndTime = msg.EnergyBoostingEndTime;           
            _energyLastRefreshTime = msg.EnergyLastRefreshTime;           
            _energyCapacity = msg.EnergyCapacity;           
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserEnergy msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserEnergy (Msg_SC_DAT_UserEnergy msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserEnergy () { 
        }
        #endregion
    }
}