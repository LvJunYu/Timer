//  | 编辑区冒险模式关卡列表
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class GmAdventureSection : SyncronisticData<Msg_GmAdventureSection> {
        #region 字段
        /// <summary>
        /// 章节
        /// </summary>
        private int _section;
        /// <summary>
        /// 普通关卡列表
        /// </summary>
        private List<GmAdventurePrepareProject> _normalProjectList;
        /// <summary>
        /// 奖励关卡列表
        /// </summary>
        private List<GmAdventurePrepareProject> _bonusProjectList;
        #endregion

        #region 属性
        /// <summary>
        /// 章节
        /// </summary>
        public int Section { 
            get { return _section; }
            set { if (_section != value) {
                _section = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 普通关卡列表
        /// </summary>
        public List<GmAdventurePrepareProject> NormalProjectList { 
            get { return _normalProjectList; }
            set { if (_normalProjectList != value) {
                _normalProjectList = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 奖励关卡列表
        /// </summary>
        public List<GmAdventurePrepareProject> BonusProjectList { 
            get { return _bonusProjectList; }
            set { if (_bonusProjectList != value) {
                _bonusProjectList = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_GmAdventureSection msg)
        {
            if (null == msg) return false;
            _section = msg.Section;     
            _normalProjectList = new List<GmAdventurePrepareProject>();
            for (int i = 0; i < msg.NormalProjectList.Count; i++) {
                _normalProjectList.Add(new GmAdventurePrepareProject(msg.NormalProjectList[i]));
            }
            _bonusProjectList = new List<GmAdventurePrepareProject>();
            for (int i = 0; i < msg.BonusProjectList.Count; i++) {
                _bonusProjectList.Add(new GmAdventurePrepareProject(msg.BonusProjectList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_GmAdventureSection msg)
        {
            if (null == msg) return false;
            _section = msg.Section;           
            if (null ==  _normalProjectList) {
                _normalProjectList = new List<GmAdventurePrepareProject>();
            }
            _normalProjectList.Clear();
            for (int i = 0; i < msg.NormalProjectList.Count; i++) {
                _normalProjectList.Add(new GmAdventurePrepareProject(msg.NormalProjectList[i]));
            }
            if (null ==  _bonusProjectList) {
                _bonusProjectList = new List<GmAdventurePrepareProject>();
            }
            _bonusProjectList.Clear();
            for (int i = 0; i < msg.BonusProjectList.Count; i++) {
                _bonusProjectList.Add(new GmAdventurePrepareProject(msg.BonusProjectList[i]));
            }
            return true;
        } 

        public bool DeepCopy (GmAdventureSection obj)
        {
            if (null == obj) return false;
            _section = obj.Section;           
            if (null ==  obj.NormalProjectList) return false;
            if (null ==  _normalProjectList) {
                _normalProjectList = new List<GmAdventurePrepareProject>();
            }
            _normalProjectList.Clear();
            for (int i = 0; i < obj.NormalProjectList.Count; i++){
                _normalProjectList.Add(obj.NormalProjectList[i]);
            }
            if (null ==  obj.BonusProjectList) return false;
            if (null ==  _bonusProjectList) {
                _bonusProjectList = new List<GmAdventurePrepareProject>();
            }
            _bonusProjectList.Clear();
            for (int i = 0; i < obj.BonusProjectList.Count; i++){
                _bonusProjectList.Add(obj.BonusProjectList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_GmAdventureSection msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmAdventureSection (Msg_GmAdventureSection msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmAdventureSection () { 
            _normalProjectList = new List<GmAdventurePrepareProject>();
            _bonusProjectList = new List<GmAdventurePrepareProject>();
        }
        #endregion
    }
}