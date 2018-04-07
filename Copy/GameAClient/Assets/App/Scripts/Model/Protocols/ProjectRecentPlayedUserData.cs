//  | 最近玩过的用户
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ProjectRecentPlayedUserData : SyncronisticData<Msg_ProjectRecentPlayedUserData> {
        #region 字段
        /// <summary>
        /// 
        /// </summary>
        private UserInfoSimple _userData;
        /// <summary>
        /// 
        /// </summary>
        private long _lastPlayTime;
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public UserInfoSimple UserData { 
            get { return _userData; }
            set { if (_userData != value) {
                _userData = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long LastPlayTime { 
            get { return _lastPlayTime; }
            set { if (_lastPlayTime != value) {
                _lastPlayTime = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_ProjectRecentPlayedUserData msg)
        {
            if (null == msg) return false;
            if (null == _userData) {
                _userData = new UserInfoSimple(msg.UserData);
            } else {
                _userData.OnSyncFromParent(msg.UserData);
            }
            _lastPlayTime = msg.LastPlayTime;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_ProjectRecentPlayedUserData msg)
        {
            if (null == msg) return false;
            if(null != msg.UserData){
                if (null == _userData){
                    _userData = new UserInfoSimple(msg.UserData);
                }
                _userData.CopyMsgData(msg.UserData);
            }
            _lastPlayTime = msg.LastPlayTime;           
            return true;
        } 

        public bool DeepCopy (ProjectRecentPlayedUserData obj)
        {
            if (null == obj) return false;
            if(null != obj.UserData){
                if (null == _userData){
                    _userData = new UserInfoSimple();
                }
                _userData.DeepCopy(obj.UserData);
            }
            _lastPlayTime = obj.LastPlayTime;           
            return true;
        }

        public void OnSyncFromParent (Msg_ProjectRecentPlayedUserData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectRecentPlayedUserData (Msg_ProjectRecentPlayedUserData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectRecentPlayedUserData () { 
            _userData = new UserInfoSimple();
        }
        #endregion
    }
}