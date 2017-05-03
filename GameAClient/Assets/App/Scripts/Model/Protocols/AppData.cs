// 获取应用全局信息 | 获取应用全局信息
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AppData : SyncronisticData 
    {
        #region 字段
        // sc fields----------------------------------
        // 应用版本号
        private string _imageUrlRoot;
        // 资源版本号
        private string _fileUrlRoot;
        // 资源根路径
        private string _gameResRoot;
        // 最新版本号
        private string _newestAppVersion;
        // 服务器时间
        private long _serverTime;
        // api是否兼容
        private bool _aPISupport;

        // cs fields----------------------------------
        // 占位
        private int _cs_flag;
        #endregion

        #region 属性
        // sc properties----------------------------------
        // 应用版本号
        public string ImageUrlRoot { 
            get { return _imageUrlRoot; }
            set { if (_imageUrlRoot != value) {
                _imageUrlRoot = value;
                SetDirty();
            }}
        }
        // 资源版本号
        public string FileUrlRoot { 
            get { return _fileUrlRoot; }
            set { if (_fileUrlRoot != value) {
                _fileUrlRoot = value;
                SetDirty();
            }}
        }
        // 资源根路径
        public string GameResRoot { 
            get { return _gameResRoot; }
            set { if (_gameResRoot != value) {
                _gameResRoot = value;
                SetDirty();
            }}
        }
        // 最新版本号
        public string NewestAppVersion { 
            get { return _newestAppVersion; }
            set { if (_newestAppVersion != value) {
                _newestAppVersion = value;
                SetDirty();
            }}
        }
        // 服务器时间
        public long ServerTime { 
            get { return _serverTime; }
            set { if (_serverTime != value) {
                _serverTime = value;
                SetDirty();
            }}
        }
        // api是否兼容
        public bool APISupport { 
            get { return _aPISupport; }
            set { if (_aPISupport != value) {
                _aPISupport = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        // 占位
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

        public bool OnSync (Msg_SC_DAT_AppData msg)
        {
            if (null == msg) return false;
            _imageUrlRoot = msg.ImageUrlRoot;           
            _fileUrlRoot = msg.FileUrlRoot;           
            _gameResRoot = msg.GameResRoot;           
            _newestAppVersion = msg.NewestAppVersion;           
            _serverTime = msg.ServerTime;           
            _aPISupport = msg.APISupport;           
            OnSyncPartial();
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
        }
        #endregion
    }
}