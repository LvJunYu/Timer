//  | 用户自荐关卡列表
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserSelfRecommendProject : SyncronisticData<Msg_UserSelfRecommendProject> {
        #region 字段
        /// <summary>
        /// 下标
        /// </summary>
        private int _slotInx;
        /// <summary>
        /// 关卡数据
        /// </summary>
        private Project _projectData;
        #endregion

        #region 属性
        /// <summary>
        /// 下标
        /// </summary>
        public int SlotInx { 
            get { return _slotInx; }
            set { if (_slotInx != value) {
                _slotInx = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 关卡数据
        /// </summary>
        public Project ProjectData { 
            get { return _projectData; }
            set { if (_projectData != value) {
                _projectData = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_UserSelfRecommendProject msg)
        {
            if (null == msg) return false;
            _slotInx = msg.SlotInx;     
            if (null == _projectData) {
                _projectData = new Project(msg.ProjectData);
            } else {
                _projectData.OnSyncFromParent(msg.ProjectData);
            }
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_UserSelfRecommendProject msg)
        {
            if (null == msg) return false;
            _slotInx = msg.SlotInx;           
            if(null != msg.ProjectData){
                if (null == _projectData){
                    _projectData = new Project(msg.ProjectData);
                }
                _projectData.CopyMsgData(msg.ProjectData);
            }
            return true;
        } 

        public bool DeepCopy (UserSelfRecommendProject obj)
        {
            if (null == obj) return false;
            _slotInx = obj.SlotInx;           
            if(null != obj.ProjectData){
                if (null == _projectData){
                    _projectData = new Project();
                }
                _projectData.DeepCopy(obj.ProjectData);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_UserSelfRecommendProject msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserSelfRecommendProject (Msg_UserSelfRecommendProject msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserSelfRecommendProject () { 
            _projectData = new Project();
        }
        #endregion
    }
}