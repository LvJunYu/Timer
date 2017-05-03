// 冒险模式进度 | 冒险模式进度
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AdventureUserProgress : SyncronisticData 
    {
        #region 字段
        // sc fields----------------------------------
        // 用户
        private long _userId;
        // 完成的章节
        private int _completeSection;
        // 完成的关卡
        private int _completeLevel;
        // 鼓励点数
        private int _encouragePoint;
        // 章节钥匙数
        private int _sectionKeyCount;
        // 章节解锁进度
        private int _sectionUnlockProgress;

        // cs fields----------------------------------
        // 用户
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        // 用户
        public long UserId { 
            get { return _userId; }
            set { if (_userId != value) {
                _userId = value;
                SetDirty();
            }}
        }
        // 完成的章节
        public int CompleteSection { 
            get { return _completeSection; }
            set { if (_completeSection != value) {
                _completeSection = value;
                SetDirty();
            }}
        }
        // 完成的关卡
        public int CompleteLevel { 
            get { return _completeLevel; }
            set { if (_completeLevel != value) {
                _completeLevel = value;
                SetDirty();
            }}
        }
        // 鼓励点数
        public int EncouragePoint { 
            get { return _encouragePoint; }
            set { if (_encouragePoint != value) {
                _encouragePoint = value;
                SetDirty();
            }}
        }
        // 章节钥匙数
        public int SectionKeyCount { 
            get { return _sectionKeyCount; }
            set { if (_sectionKeyCount != value) {
                _sectionKeyCount = value;
                SetDirty();
            }}
        }
        // 章节解锁进度
        public int SectionUnlockProgress { 
            get { return _sectionUnlockProgress; }
            set { if (_sectionUnlockProgress != value) {
                _sectionUnlockProgress = value;
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
		/// 冒险模式进度
		/// </summary>
		/// <param name="userId">用户.</param>
        public void Request (
            long userId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
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
        }
        #endregion
    }
}