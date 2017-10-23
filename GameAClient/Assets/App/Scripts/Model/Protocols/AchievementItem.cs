//  | 成就数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AchievementItem : SyncronisticData {
        #region 字段
        /// <summary>
        /// 成就Id
        /// </summary>
        private int _achievementId;
        /// <summary>
        /// 成就达成时间
        /// </summary>
        private long _createTime;
        #endregion

        #region 属性
        /// <summary>
        /// 成就Id
        /// </summary>
        public int AchievementId { 
            get { return _achievementId; }
            set { if (_achievementId != value) {
                _achievementId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 成就达成时间
        /// </summary>
        public long CreateTime { 
            get { return _createTime; }
            set { if (_createTime != value) {
                _createTime = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_AchievementItem msg)
        {
            if (null == msg) return false;
            _achievementId = msg.AchievementId;     
            _createTime = msg.CreateTime;     
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (AchievementItem obj)
        {
            if (null == obj) return false;
            _achievementId = obj.AchievementId;           
            _createTime = obj.CreateTime;           
            return true;
        }

        public void OnSyncFromParent (Msg_AchievementItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AchievementItem (Msg_AchievementItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AchievementItem () { 
        }
        #endregion
    }
}