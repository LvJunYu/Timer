//  | 推送结构
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class PushData : SyncronisticData<Msg_PushData> {
        #region 字段
        /// <summary>
        /// 成就数据
        /// </summary>
        private AchievementPushData _achievement;
        #endregion

        #region 属性
        /// <summary>
        /// 成就数据
        /// </summary>
        public AchievementPushData Achievement { 
            get { return _achievement; }
            set { if (_achievement != value) {
                _achievement = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_PushData msg)
        {
            if (null == msg) return false;
            if (null == _achievement) {
                _achievement = new AchievementPushData(msg.Achievement);
            } else {
                _achievement.OnSyncFromParent(msg.Achievement);
            }
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_PushData msg)
        {
            if (null == msg) return false;
            if(null != msg.Achievement){
                if (null == _achievement){
                    _achievement = new AchievementPushData(msg.Achievement);
                }
                _achievement.CopyMsgData(msg.Achievement);
            }
            return true;
        } 

        public bool DeepCopy (PushData obj)
        {
            if (null == obj) return false;
            if(null != obj.Achievement){
                if (null == _achievement){
                    _achievement = new AchievementPushData();
                }
                _achievement.DeepCopy(obj.Achievement);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_PushData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public PushData (Msg_PushData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public PushData () { 
            _achievement = new AchievementPushData();
        }
        #endregion
    }
}