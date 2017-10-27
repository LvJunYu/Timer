// 冒险模式好友最高关数据 | 冒险模式好友最高关数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class FriendsAdvProgressData : SyncronisticData<Msg_SC_DAT_FriendsAdvProgressData> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 
        /// </summary>
        private int _resultCode;
        /// <summary>
        /// 
        /// </summary>
        private long _updateTime;
        /// <summary>
        /// 进度数据列表
        /// </summary>
        private List<AdvProgressData> _advProgressDataList;

        // cs fields----------------------------------
        /// <summary>
        /// 起始章节
        /// </summary>
        private int _cs_startSection;
        /// <summary>
        /// 起始关卡
        /// </summary>
        private int _cs_startLevel;
        /// <summary>
        /// 结束章节
        /// </summary>
        private int _cs_endSection;
        /// <summary>
        /// 结束关卡
        /// </summary>
        private int _cs_endLevel;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 
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
        /// 进度数据列表
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
        /// 起始章节
        /// </summary>
        public int CS_StartSection { 
            get { return _cs_startSection; }
            set { _cs_startSection = value; }
        }
        /// <summary>
        /// 起始关卡
        /// </summary>
        public int CS_StartLevel { 
            get { return _cs_startLevel; }
            set { _cs_startLevel = value; }
        }
        /// <summary>
        /// 结束章节
        /// </summary>
        public int CS_EndSection { 
            get { return _cs_endSection; }
            set { _cs_endSection = value; }
        }
        /// <summary>
        /// 结束关卡
        /// </summary>
        public int CS_EndLevel { 
            get { return _cs_endLevel; }
            set { _cs_endLevel = value; }
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
		/// <param name="startSection">起始章节.</param>
		/// <param name="startLevel">起始关卡.</param>
		/// <param name="endSection">结束章节.</param>
		/// <param name="endLevel">结束关卡.</param>
        public void Request (
            int startSection,
            int startLevel,
            int endSection,
            int endLevel,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_startSection != startSection) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_startLevel != startLevel) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_endSection != endSection) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_endLevel != endLevel) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_startSection = startSection;
                _cs_startLevel = startLevel;
                _cs_endSection = endSection;
                _cs_endLevel = endLevel;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_FriendsAdvProgressData msg = new Msg_CS_DAT_FriendsAdvProgressData();
                msg.StartSection = startSection;
                msg.StartLevel = startLevel;
                msg.EndSection = endSection;
                msg.EndLevel = endLevel;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_FriendsAdvProgressData>(
                    SoyHttpApiPath.FriendsAdvProgressData, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_FriendsAdvProgressData msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            _advProgressDataList = new List<AdvProgressData>();
            for (int i = 0; i < msg.AdvProgressDataList.Count; i++) {
                _advProgressDataList.Add(new AdvProgressData(msg.AdvProgressDataList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_FriendsAdvProgressData msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            if (null ==  _advProgressDataList) {
                _advProgressDataList = new List<AdvProgressData>();
            }
            _advProgressDataList.Clear();
            for (int i = 0; i < msg.AdvProgressDataList.Count; i++) {
                _advProgressDataList.Add(new AdvProgressData(msg.AdvProgressDataList[i]));
            }
            return true;
        } 

        public bool DeepCopy (FriendsAdvProgressData obj)
        {
            if (null == obj) return false;
            _resultCode = obj.ResultCode;           
            _updateTime = obj.UpdateTime;           
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

        public void OnSyncFromParent (Msg_SC_DAT_FriendsAdvProgressData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public FriendsAdvProgressData (Msg_SC_DAT_FriendsAdvProgressData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public FriendsAdvProgressData () { 
            _advProgressDataList = new List<AdvProgressData>();
            OnCreate();
        }
        #endregion
    }
}