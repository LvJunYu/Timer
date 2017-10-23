//  | 冒险模式关卡列表
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AdventureSection : SyncronisticData {
        #region 字段
        /// <summary>
        /// 章节
        /// </summary>
        private int _section;
        /// <summary>
        /// 普通关卡列表
        /// </summary>
        private List<Project> _normalProjectList;
        /// <summary>
        /// 奖励关卡列表
        /// </summary>
        private List<Project> _bonusProjectList;
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
        public List<Project> NormalProjectList { 
            get { return _normalProjectList; }
            set { if (_normalProjectList != value) {
                _normalProjectList = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 奖励关卡列表
        /// </summary>
        public List<Project> BonusProjectList { 
            get { return _bonusProjectList; }
            set { if (_bonusProjectList != value) {
                _bonusProjectList = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_AdventureSection msg)
        {
            if (null == msg) return false;
            _section = msg.Section;     
            _normalProjectList = new List<Project>();
            for (int i = 0; i < msg.NormalProjectList.Count; i++) {
                _normalProjectList.Add(new Project(msg.NormalProjectList[i]));
            }
            _bonusProjectList = new List<Project>();
            for (int i = 0; i < msg.BonusProjectList.Count; i++) {
                _bonusProjectList.Add(new Project(msg.BonusProjectList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (AdventureSection obj)
        {
            if (null == obj) return false;
            _section = obj.Section;           
            if (null ==  obj.NormalProjectList) return false;
            if (null ==  _normalProjectList) {
                _normalProjectList = new List<Project>();
            }
            _normalProjectList.Clear();
            for (int i = 0; i < obj.NormalProjectList.Count; i++){
                _normalProjectList.Add(obj.NormalProjectList[i]);
            }
            if (null ==  obj.BonusProjectList) return false;
            if (null ==  _bonusProjectList) {
                _bonusProjectList = new List<Project>();
            }
            _bonusProjectList.Clear();
            for (int i = 0; i < obj.BonusProjectList.Count; i++){
                _bonusProjectList.Add(obj.BonusProjectList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_AdventureSection msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdventureSection (Msg_AdventureSection msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdventureSection () { 
            _normalProjectList = new List<Project>();
            _bonusProjectList = new List<Project>();
        }
        #endregion
    }
}