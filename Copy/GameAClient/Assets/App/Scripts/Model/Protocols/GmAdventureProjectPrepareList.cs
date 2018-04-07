// 编辑区冒险模式关卡列表 | 编辑区冒险模式关卡列表
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class GmAdventureProjectPrepareList : SyncronisticData<Msg_SC_DAT_GmAdventureProjectPrepareList> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 版本号
        /// </summary>
        private long _version;
        /// <summary>
        /// 
        /// </summary>
        private int _totalSectionCount;
        /// <summary>
        /// 
        /// </summary>
        private List<GmAdventureSection> _sectionList;

        // cs fields----------------------------------
        /// <summary>
        /// 最小章节
        /// </summary>
        private int _cs_minSection;
        /// <summary>
        /// 最大章节
        /// </summary>
        private int _cs_maxSection;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 版本号
        /// </summary>
        public long Version { 
            get { return _version; }
            set { if (_version != value) {
                _version = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int TotalSectionCount { 
            get { return _totalSectionCount; }
            set { if (_totalSectionCount != value) {
                _totalSectionCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public List<GmAdventureSection> SectionList { 
            get { return _sectionList; }
            set { if (_sectionList != value) {
                _sectionList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 最小章节
        /// </summary>
        public int CS_MinSection { 
            get { return _cs_minSection; }
            set { _cs_minSection = value; }
        }
        /// <summary>
        /// 最大章节
        /// </summary>
        public int CS_MaxSection { 
            get { return _cs_maxSection; }
            set { _cs_maxSection = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _sectionList) {
                    for (int i = 0; i < _sectionList.Count; i++) {
                        if (null != _sectionList[i] && _sectionList[i].IsDirty) {
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
		/// 编辑区冒险模式关卡列表
		/// </summary>
		/// <param name="minSection">最小章节.</param>
		/// <param name="maxSection">最大章节.</param>
        public void Request (
            int minSection,
            int maxSection,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_minSection != minSection) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_maxSection != maxSection) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_minSection = minSection;
                _cs_maxSection = maxSection;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_GmAdventureProjectPrepareList msg = new Msg_CS_DAT_GmAdventureProjectPrepareList();
                msg.MinSection = minSection;
                msg.MaxSection = maxSection;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_GmAdventureProjectPrepareList>(
                    SoyHttpApiPath.GmAdventureProjectPrepareList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_GmAdventureProjectPrepareList msg)
        {
            if (null == msg) return false;
            _version = msg.Version;           
            _totalSectionCount = msg.TotalSectionCount;           
            _sectionList = new List<GmAdventureSection>();
            for (int i = 0; i < msg.SectionList.Count; i++) {
                _sectionList.Add(new GmAdventureSection(msg.SectionList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_GmAdventureProjectPrepareList msg)
        {
            if (null == msg) return false;
            _version = msg.Version;           
            _totalSectionCount = msg.TotalSectionCount;           
            if (null ==  _sectionList) {
                _sectionList = new List<GmAdventureSection>();
            }
            _sectionList.Clear();
            for (int i = 0; i < msg.SectionList.Count; i++) {
                _sectionList.Add(new GmAdventureSection(msg.SectionList[i]));
            }
            return true;
        } 

        public bool DeepCopy (GmAdventureProjectPrepareList obj)
        {
            if (null == obj) return false;
            _version = obj.Version;           
            _totalSectionCount = obj.TotalSectionCount;           
            if (null ==  obj.SectionList) return false;
            if (null ==  _sectionList) {
                _sectionList = new List<GmAdventureSection>();
            }
            _sectionList.Clear();
            for (int i = 0; i < obj.SectionList.Count; i++){
                _sectionList.Add(obj.SectionList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_GmAdventureProjectPrepareList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmAdventureProjectPrepareList (Msg_SC_DAT_GmAdventureProjectPrepareList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmAdventureProjectPrepareList () { 
            _sectionList = new List<GmAdventureSection>();
            OnCreate();
        }
        #endregion
    }
}