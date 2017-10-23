//  | 排行榜
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class WorldRankItem : SyncronisticData {
        #region 字段
        /// <summary>
        /// 用户数据
        /// </summary>
        private UserInfoSimple _userInfo;
        /// <summary>
        /// 数量
        /// </summary>
        private int _count;
        #endregion

        #region 属性
        /// <summary>
        /// 用户数据
        /// </summary>
        public UserInfoSimple UserInfo { 
            get { return _userInfo; }
            set { if (_userInfo != value) {
                _userInfo = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { 
            get { return _count; }
            set { if (_count != value) {
                _count = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_WorldRankItem msg)
        {
            if (null == msg) return false;
            if (null == _userInfo) {
                _userInfo = new UserInfoSimple(msg.UserInfo);
            } else {
                _userInfo.OnSyncFromParent(msg.UserInfo);
            }
            _count = msg.Count;     
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (WorldRankItem obj)
        {
            if (null == obj) return false;
            if(null != obj.UserInfo){
                if (null == _userInfo){
                    _userInfo = new UserInfoSimple();
                }
                _userInfo.DeepCopy(obj.UserInfo);
            }
            _count = obj.Count;           
            return true;
        }

        public void OnSyncFromParent (Msg_WorldRankItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WorldRankItem (Msg_WorldRankItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WorldRankItem () { 
            _userInfo = new UserInfoSimple();
        }
        #endregion
    }
}