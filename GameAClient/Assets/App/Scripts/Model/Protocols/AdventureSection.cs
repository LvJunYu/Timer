//  | 冒险模式关卡列表
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AdventureSection : SyncronisticData {
        #region 字段
        // 章节
        private int _section;
        // 普通关卡列表
        private List<Project> _normalProjectList;
        // 奖励关卡列表
        private List<Project> _bonusProjectList;
        #endregion

        #region 属性
        // 章节
        public int Section { 
            get { return _section; }
            set { if (_section != value) {
                _section = value;
                SetDirty();
            }}
        }
        // 普通关卡列表
        public List<Project> NormalProjectList { 
            get { return _normalProjectList; }
            set { if (_normalProjectList != value) {
                _normalProjectList = value;
                SetDirty();
            }}
        }
        // 奖励关卡列表
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