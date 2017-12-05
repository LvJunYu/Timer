//  | 联机对战数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class NetBattleData : SyncronisticData<Msg_NetBattleData> {
        #region 字段
        /// <summary>
        /// 
        /// </summary>
        private long _projectId;
        /// <summary>
        /// 对战类型
        /// </summary>
        private ENetBattleType _netBattleType;
        /// <summary>
        /// 时间限制
        /// </summary>
        private int _timeLimit;
        /// <summary>
        /// 胜利条件
        /// </summary>
        private int _winCondition;
        /// <summary>
        /// 队伍人数
        /// </summary>
        private List<int> _teamCount;
        /// <summary>
        /// 最大人数
        /// </summary>
        private int _maxUserCount;
        #endregion

        #region 属性
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
        /// <summary>
        /// 对战类型
        /// </summary>
        public ENetBattleType NetBattleType { 
            get { return _netBattleType; }
            set { if (_netBattleType != value) {
                _netBattleType = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 时间限制
        /// </summary>
        public int TimeLimit { 
            get { return _timeLimit; }
            set { if (_timeLimit != value) {
                _timeLimit = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 胜利条件
        /// </summary>
        public int WinCondition { 
            get { return _winCondition; }
            set { if (_winCondition != value) {
                _winCondition = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 队伍人数
        /// </summary>
        public List<int> TeamCount { 
            get { return _teamCount; }
            set { if (_teamCount != value) {
                _teamCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 最大人数
        /// </summary>
        public int MaxUserCount { 
            get { return _maxUserCount; }
            set { if (_maxUserCount != value) {
                _maxUserCount = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_NetBattleData msg)
        {
            if (null == msg) return false;
            _projectId = msg.ProjectId;     
            _netBattleType = msg.NetBattleType;     
            _timeLimit = msg.TimeLimit;     
            _winCondition = msg.WinCondition;     
            _teamCount = msg.TeamCount;     
            _maxUserCount = msg.MaxUserCount;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_NetBattleData msg)
        {
            if (null == msg) return false;
            _projectId = msg.ProjectId;           
            _netBattleType = msg.NetBattleType;           
            _timeLimit = msg.TimeLimit;           
            _winCondition = msg.WinCondition;           
            _teamCount = msg.TeamCount;           
            _maxUserCount = msg.MaxUserCount;           
            return true;
        } 

        public bool DeepCopy (NetBattleData obj)
        {
            if (null == obj) return false;
            _projectId = obj.ProjectId;           
            _netBattleType = obj.NetBattleType;           
            _timeLimit = obj.TimeLimit;           
            _winCondition = obj.WinCondition;           
            _teamCount = obj.TeamCount;           
            _maxUserCount = obj.MaxUserCount;           
            return true;
        }

        public void OnSyncFromParent (Msg_NetBattleData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NetBattleData (Msg_NetBattleData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NetBattleData () { 
        }
        #endregion
    }
}