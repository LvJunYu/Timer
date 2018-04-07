//  | 获取编辑区官方推荐关卡
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class WorldOfficialRecommendPrepareProject : SyncronisticData<Msg_WorldOfficialRecommendPrepareProject> {
        #region 字段
        /// <summary>
        /// 推荐Id
        /// </summary>
        private long _id;
        /// <summary>
        /// 
        /// </summary>
        private Project _projectData;
        /// <summary>
        /// 
        /// </summary>
        private long _lastUpdateTime;
        /// <summary>
        /// 
        /// </summary>
        private UserInfoSimple _lastUpdateUser;
        /// <summary>
        /// 
        /// </summary>
        private EWorldOfficialRecommendPrepareProjectStatus _status;
        #endregion

        #region 属性
        /// <summary>
        /// 推荐Id
        /// </summary>
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public Project ProjectData { 
            get { return _projectData; }
            set { if (_projectData != value) {
                _projectData = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long LastUpdateTime { 
            get { return _lastUpdateTime; }
            set { if (_lastUpdateTime != value) {
                _lastUpdateTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public UserInfoSimple LastUpdateUser { 
            get { return _lastUpdateUser; }
            set { if (_lastUpdateUser != value) {
                _lastUpdateUser = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public EWorldOfficialRecommendPrepareProjectStatus Status { 
            get { return _status; }
            set { if (_status != value) {
                _status = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_WorldOfficialRecommendPrepareProject msg)
        {
            if (null == msg) return false;
            _id = msg.Id;     
            if (null == _projectData) {
                _projectData = new Project(msg.ProjectData);
            } else {
                _projectData.OnSyncFromParent(msg.ProjectData);
            }
            _lastUpdateTime = msg.LastUpdateTime;     
            if (null == _lastUpdateUser) {
                _lastUpdateUser = new UserInfoSimple(msg.LastUpdateUser);
            } else {
                _lastUpdateUser.OnSyncFromParent(msg.LastUpdateUser);
            }
            _status = msg.Status;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_WorldOfficialRecommendPrepareProject msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
            if(null != msg.ProjectData){
                if (null == _projectData){
                    _projectData = new Project(msg.ProjectData);
                }
                _projectData.CopyMsgData(msg.ProjectData);
            }
            _lastUpdateTime = msg.LastUpdateTime;           
            if(null != msg.LastUpdateUser){
                if (null == _lastUpdateUser){
                    _lastUpdateUser = new UserInfoSimple(msg.LastUpdateUser);
                }
                _lastUpdateUser.CopyMsgData(msg.LastUpdateUser);
            }
            _status = msg.Status;           
            return true;
        } 

        public bool DeepCopy (WorldOfficialRecommendPrepareProject obj)
        {
            if (null == obj) return false;
            _id = obj.Id;           
            if(null != obj.ProjectData){
                if (null == _projectData){
                    _projectData = new Project();
                }
                _projectData.DeepCopy(obj.ProjectData);
            }
            _lastUpdateTime = obj.LastUpdateTime;           
            if(null != obj.LastUpdateUser){
                if (null == _lastUpdateUser){
                    _lastUpdateUser = new UserInfoSimple();
                }
                _lastUpdateUser.DeepCopy(obj.LastUpdateUser);
            }
            _status = obj.Status;           
            return true;
        }

        public void OnSyncFromParent (Msg_WorldOfficialRecommendPrepareProject msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WorldOfficialRecommendPrepareProject (Msg_WorldOfficialRecommendPrepareProject msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WorldOfficialRecommendPrepareProject () { 
            _projectData = new Project();
            _lastUpdateUser = new UserInfoSimple();
        }
        #endregion
    }
}