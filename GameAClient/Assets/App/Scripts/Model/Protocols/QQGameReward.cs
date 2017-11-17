// QQ蓝钻大厅特权奖励数据 | QQ蓝钻大厅特权奖励数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class QQGameReward : SyncronisticData<Msg_SC_DAT_QQGameReward> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 蓝钻新手
        /// </summary>
        private int _blueBeginner;
        /// <summary>
        /// 蓝钻每日
        /// </summary>
        private List<int> _blueNormalDaily;
        /// <summary>
        /// 蓝钻豪华日常
        /// </summary>
        private int _blueSuperDaily;
        /// <summary>
        /// 蓝钻年费日常
        /// </summary>
        private int _blueYearDaily;
        /// <summary>
        /// 蓝钻成长
        /// </summary>
        private List<int> _blueGrow;
        /// <summary>
        /// 大厅新手
        /// </summary>
        private int _hallBeginner;
        /// <summary>
        /// 大厅成长
        /// </summary>
        private List<int> _hallGrow;
        /// <summary>
        /// 大厅日常
        /// </summary>
        private int _hallDaily;

        // cs fields----------------------------------
        /// <summary>
        /// 占位
        /// </summary>
        private int _cs_flag;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 蓝钻新手
        /// </summary>
        public int BlueBeginner { 
            get { return _blueBeginner; }
            set { if (_blueBeginner != value) {
                _blueBeginner = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 蓝钻每日
        /// </summary>
        public List<int> BlueNormalDaily { 
            get { return _blueNormalDaily; }
            set { if (_blueNormalDaily != value) {
                _blueNormalDaily = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 蓝钻豪华日常
        /// </summary>
        public int BlueSuperDaily { 
            get { return _blueSuperDaily; }
            set { if (_blueSuperDaily != value) {
                _blueSuperDaily = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 蓝钻年费日常
        /// </summary>
        public int BlueYearDaily { 
            get { return _blueYearDaily; }
            set { if (_blueYearDaily != value) {
                _blueYearDaily = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 蓝钻成长
        /// </summary>
        public List<int> BlueGrow { 
            get { return _blueGrow; }
            set { if (_blueGrow != value) {
                _blueGrow = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 大厅新手
        /// </summary>
        public int HallBeginner { 
            get { return _hallBeginner; }
            set { if (_hallBeginner != value) {
                _hallBeginner = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 大厅成长
        /// </summary>
        public List<int> HallGrow { 
            get { return _hallGrow; }
            set { if (_hallGrow != value) {
                _hallGrow = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 大厅日常
        /// </summary>
        public int HallDaily { 
            get { return _hallDaily; }
            set { if (_hallDaily != value) {
                _hallDaily = value;
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
		/// QQ蓝钻大厅特权奖励数据
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

                Msg_CS_DAT_QQGameReward msg = new Msg_CS_DAT_QQGameReward();
                msg.Flag = flag;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_QQGameReward>(
                    SoyHttpApiPath.QQGameReward, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_QQGameReward msg)
        {
            if (null == msg) return false;
            _blueBeginner = msg.BlueBeginner;           
            _blueNormalDaily = msg.BlueNormalDaily;           
            _blueSuperDaily = msg.BlueSuperDaily;           
            _blueYearDaily = msg.BlueYearDaily;           
            _blueGrow = msg.BlueGrow;           
            _hallBeginner = msg.HallBeginner;           
            _hallGrow = msg.HallGrow;           
            _hallDaily = msg.HallDaily;           
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_QQGameReward msg)
        {
            if (null == msg) return false;
            _blueBeginner = msg.BlueBeginner;           
            _blueNormalDaily = msg.BlueNormalDaily;           
            _blueSuperDaily = msg.BlueSuperDaily;           
            _blueYearDaily = msg.BlueYearDaily;           
            _blueGrow = msg.BlueGrow;           
            _hallBeginner = msg.HallBeginner;           
            _hallGrow = msg.HallGrow;           
            _hallDaily = msg.HallDaily;           
            return true;
        } 

        public bool DeepCopy (QQGameReward obj)
        {
            if (null == obj) return false;
            _blueBeginner = obj.BlueBeginner;           
            _blueNormalDaily = obj.BlueNormalDaily;           
            _blueSuperDaily = obj.BlueSuperDaily;           
            _blueYearDaily = obj.BlueYearDaily;           
            _blueGrow = obj.BlueGrow;           
            _hallBeginner = obj.HallBeginner;           
            _hallGrow = obj.HallGrow;           
            _hallDaily = obj.HallDaily;           
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_QQGameReward msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public QQGameReward (Msg_SC_DAT_QQGameReward msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public QQGameReward () { 
            OnCreate();
        }
        #endregion
    }
}