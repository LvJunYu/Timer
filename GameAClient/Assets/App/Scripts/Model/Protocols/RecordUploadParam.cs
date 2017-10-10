//  | 上传录像附加数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class RecordUploadParam : SyncronisticData {
        #region 字段
        /// <summary>
        /// 
        /// </summary>
        private bool _success;
        /// <summary>
        /// 
        /// </summary>
        private byte[] _deadPos;
        /// <summary>
        /// 录像使用帧数
        /// </summary>
        private int _usedTime;
        /// <summary>
        /// 最终得分
        /// </summary>
        private int _score;
        /// <summary>
        /// 奖分道具数
        /// </summary>
        private int _scoreItemCount;
        /// <summary>
        /// 击杀怪物数
        /// </summary>
        private int _killMonsterCount;
        /// <summary>
        /// 剩余时间数
        /// </summary>
        private int _leftTime;
        /// <summary>
        /// 剩余生命
        /// </summary>
        private int _leftLife;
        /// <summary>
        /// 星1标志
        /// </summary>
        private bool _star1Flag;
        /// <summary>
        /// 星2标志
        /// </summary>
        private bool _star2Flag;
        /// <summary>
        /// 星3标志
        /// </summary>
        private bool _star3Flag;
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public bool Success { 
            get { return _success; }
            set { if (_success != value) {
                _success = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] DeadPos { 
            get { return _deadPos; }
            set { if (_deadPos != value) {
                _deadPos = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 录像使用帧数
        /// </summary>
        public int UsedTime { 
            get { return _usedTime; }
            set { if (_usedTime != value) {
                _usedTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 最终得分
        /// </summary>
        public int Score { 
            get { return _score; }
            set { if (_score != value) {
                _score = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 奖分道具数
        /// </summary>
        public int ScoreItemCount { 
            get { return _scoreItemCount; }
            set { if (_scoreItemCount != value) {
                _scoreItemCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 击杀怪物数
        /// </summary>
        public int KillMonsterCount { 
            get { return _killMonsterCount; }
            set { if (_killMonsterCount != value) {
                _killMonsterCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 剩余时间数
        /// </summary>
        public int LeftTime { 
            get { return _leftTime; }
            set { if (_leftTime != value) {
                _leftTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 剩余生命
        /// </summary>
        public int LeftLife { 
            get { return _leftLife; }
            set { if (_leftLife != value) {
                _leftLife = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 星1标志
        /// </summary>
        public bool Star1Flag { 
            get { return _star1Flag; }
            set { if (_star1Flag != value) {
                _star1Flag = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 星2标志
        /// </summary>
        public bool Star2Flag { 
            get { return _star2Flag; }
            set { if (_star2Flag != value) {
                _star2Flag = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 星3标志
        /// </summary>
        public bool Star3Flag { 
            get { return _star3Flag; }
            set { if (_star3Flag != value) {
                _star3Flag = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_RecordUploadParam msg)
        {
            if (null == msg) return false;
            _success = msg.Success;     
            _deadPos = msg.DeadPos;     
            _usedTime = msg.UsedTime;     
            _score = msg.Score;     
            _scoreItemCount = msg.ScoreItemCount;     
            _killMonsterCount = msg.KillMonsterCount;     
            _leftTime = msg.LeftTime;     
            _leftLife = msg.LeftLife;     
            _star1Flag = msg.Star1Flag;     
            _star2Flag = msg.Star2Flag;     
            _star3Flag = msg.Star3Flag;     
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (RecordUploadParam obj)
        {
            if (null == obj) return false;
            return true;
        }

        public void OnSyncFromParent (Msg_RecordUploadParam msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public RecordUploadParam (Msg_RecordUploadParam msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public RecordUploadParam () { 
        }
        #endregion
    }
}