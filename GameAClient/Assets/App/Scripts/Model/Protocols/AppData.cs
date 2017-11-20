// 获取应用全局信息 | 获取应用全局信息
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AppData : SyncronisticData<Msg_SC_DAT_AppData> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 应用版本号
        /// </summary>
        private string _imageUrlRoot;
        /// <summary>
        /// 资源版本号
        /// </summary>
        private string _fileUrlRoot;
        /// <summary>
        /// 资源根路径
        /// </summary>
        private string _gameResRoot;
        /// <summary>
        /// 最新版本号
        /// </summary>
        private string _newestAppVersion;
        /// <summary>
        /// 服务器时间
        /// </summary>
        private long _serverTime;
        /// <summary>
        /// api是否兼容
        /// </summary>
        private bool _aPISupport;

        // cs fields----------------------------------
        /// <summary>
        /// 占位
        /// </summary>
        private int _cs_flag;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 应用版本号
        /// </summary>
        public string ImageUrlRoot { 
            get { return _imageUrlRoot; }
            set { if (_imageUrlRoot != value) {
                _imageUrlRoot = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 资源版本号
        /// </summary>
        public string FileUrlRoot { 
            get { return _fileUrlRoot; }
            set { if (_fileUrlRoot != value) {
                _fileUrlRoot = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 资源根路径
        /// </summary>
        public string GameResRoot { 
            get { return _gameResRoot; }
            set { if (_gameResRoot != value) {
                _gameResRoot = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 最新版本号
        /// </summary>
        public string NewestAppVersion { 
            get { return _newestAppVersion; }
            set { if (_newestAppVersion != value) {
                _newestAppVersion = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 服务器时间
        /// </summary>
        public long ServerTime { 
            get { return _serverTime; }
            set { if (_serverTime != value) {
                _serverTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// api是否兼容
        /// </summary>
        public bool APISupport { 
            get { return _aPISupport; }
            set { if (_aPISupport != value) {
                _aPISupport = value;
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
		/// 获取应用全局信息
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

                Msg_CS_DAT_AppData msg = new Msg_CS_DAT_AppData();
                msg.Flag = flag;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_AppData>(
                    SoyHttpApiPath.AppData, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_AppData msg)
        {
            if (null == msg) return false;
            _imageUrlRoot = msg.ImageUrlRoot;           
            _fileUrlRoot = msg.FileUrlRoot;           
            _gameResRoot = msg.GameResRoot;           
            _newestAppVersion = msg.NewestAppVersion;           
            _serverTime = msg.ServerTime;           
            _aPISupport = msg.APISupport;           
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_AppData msg)
        {
            if (null == msg) return false;
            _imageUrlRoot = msg.ImageUrlRoot;           
            _fileUrlRoot = msg.FileUrlRoot;           
            _gameResRoot = msg.GameResRoot;           
            _newestAppVersion = msg.NewestAppVersion;           
            _serverTime = msg.ServerTime;           
            _aPISupport = msg.APISupport;           
            return true;
        } 

        public bool DeepCopy (AppData obj)
        {
            if (null == obj) return false;
            _imageUrlRoot = obj.ImageUrlRoot;           
            _fileUrlRoot = obj.FileUrlRoot;           
            _gameResRoot = obj.GameResRoot;           
            _newestAppVersion = obj.NewestAppVersion;           
            _serverTime = obj.ServerTime;           
            _aPISupport = obj.APISupport;           
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_AppData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AppData (Msg_SC_DAT_AppData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AppData () { 
            OnCreate();
        }
        #endregion
    }
}