//  | 编辑编辑区冒险模式关卡
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class GmUpdateAdventureProjectPrepare : SyncronisticData<Msg_GmUpdateAdventureProjectPrepare> {
        #region 字段
        /// <summary>
        /// 章节
        /// </summary>
        private int _section;
        /// <summary>
        /// 
        /// </summary>
        private EAdventureProjectType _projectType;
        /// <summary>
        /// 
        /// </summary>
        private int _level;
        /// <summary>
        /// 
        /// </summary>
        private long _projectId;
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
        /// 
        /// </summary>
        public EAdventureProjectType ProjectType { 
            get { return _projectType; }
            set { if (_projectType != value) {
                _projectType = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int Level { 
            get { return _level; }
            set { if (_level != value) {
                _level = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long ProjectId { 
            get { return _projectId; }
            set { if (_projectId != value) {
                _projectId = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_GmUpdateAdventureProjectPrepare msg)
        {
            if (null == msg) return false;
            _section = msg.Section;     
            _projectType = msg.ProjectType;     
            _level = msg.Level;     
            _projectId = msg.ProjectId;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_GmUpdateAdventureProjectPrepare msg)
        {
            if (null == msg) return false;
            _section = msg.Section;           
            _projectType = msg.ProjectType;           
            _level = msg.Level;           
            _projectId = msg.ProjectId;           
            return true;
        } 

        public bool DeepCopy (GmUpdateAdventureProjectPrepare obj)
        {
            if (null == obj) return false;
            _section = obj.Section;           
            _projectType = obj.ProjectType;           
            _level = obj.Level;           
            _projectId = obj.ProjectId;           
            return true;
        }

        public void OnSyncFromParent (Msg_GmUpdateAdventureProjectPrepare msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmUpdateAdventureProjectPrepare (Msg_GmUpdateAdventureProjectPrepare msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmUpdateAdventureProjectPrepare () { 
        }
        #endregion
    }
}