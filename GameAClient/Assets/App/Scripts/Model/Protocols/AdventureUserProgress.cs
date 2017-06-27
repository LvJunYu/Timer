// 冒险模式进度 | 冒险模式进度
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AdventureUserProgress : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        private long _userId;
        /// <summary>
        /// 完成的章节
        /// </summary>
        private int _completeSection;
        /// <summary>
        /// 完成的关卡
        /// </summary>
        private int _completeLevel;
        /// <summary>
        /// 鼓励点数
        /// </summary>
        private int _encouragePoint;
        /// <summary>
        /// 章节钥匙数
        /// </summary>
        private int _sectionKeyCount;
        /// <summary>
        /// 章节解锁进度
        /// </summary>
        private int _sectionUnlockProgress;

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
        /// 完成的章节
        /// </summary>
        public int CompleteSection { 
            get { return _completeSection; }
            set { if (_completeSection != value) {
                _completeSection = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 完成的关卡
        /// </summary>
        public int CompleteLevel { 
            get { return _completeLevel; }
            set { if (_completeLevel != value) {
                _completeLevel = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 鼓励点数
        /// </summary>
        public int EncouragePoint { 
            get { return _encouragePoint; }
            set { if (_encouragePoint != value) {
                _encouragePoint = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 章节钥匙数
        /// </summary>
        public int SectionKeyCount { 
            get { return _sectionKeyCount; }
            set { if (_sectionKeyCount != value) {
                _sectionKeyCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 章节解锁进度
        /// </summary>
        public int SectionUnlockProgress { 
            get { return _sectionUnlockProgress; }
            set { if (_sectionUnlockProgress != value) {
                _sectionUnlockProgress = value;
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
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 冒险模式进度
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

                Msg_CS_DAT_AdventureUserProgress msg = new Msg_CS_DAT_AdventureUserProgress();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_AdventureUserProgress>(
                    SoyHttpApiPath.AdventureUserProgress, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_AdventureUserProgress msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            _completeSection = msg.CompleteSection;           
            _completeLevel = msg.CompleteLevel;           
            _encouragePoint = msg.EncouragePoint;           
            _sectionKeyCount = msg.SectionKeyCount;           
            _sectionUnlockProgress = msg.SectionUnlockProgress;           
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_AdventureUserProgress msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdventureUserProgress (Msg_SC_DAT_AdventureUserProgress msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdventureUserProgress () { 
            OnCreate();
        }
        #endregion
    }
}