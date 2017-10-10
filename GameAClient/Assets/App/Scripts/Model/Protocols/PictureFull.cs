//  | 拼图数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class PictureFull : SyncronisticData {
        #region 字段
        /// <summary>
        /// 拼图Id
        /// </summary>
        private long _pictureId;
        /// <summary>
        /// 等级
        /// </summary>
        private int _level;
        /// <summary>
        /// 是否正在使用
        /// </summary>
        private bool _isUsing;
        /// <summary>
        /// 槽位
        /// </summary>
        private int _slot;
        #endregion

        #region 属性
        /// <summary>
        /// 拼图Id
        /// </summary>
        public long PictureId { 
            get { return _pictureId; }
            set { if (_pictureId != value) {
                _pictureId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 等级
        /// </summary>
        public int Level { 
            get { return _level; }
            set { if (_level != value) {
                _level = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 是否正在使用
        /// </summary>
        public bool IsUsing { 
            get { return _isUsing; }
            set { if (_isUsing != value) {
                _isUsing = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 槽位
        /// </summary>
        public int Slot { 
            get { return _slot; }
            set { if (_slot != value) {
                _slot = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_PictureFull msg)
        {
            if (null == msg) return false;
            _pictureId = msg.PictureId;     
            _level = msg.Level;     
            _isUsing = msg.IsUsing;     
            _slot = msg.Slot;     
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (PictureFull obj)
        {
            if (null == obj) return false;
            return true;
        }

        public void OnSyncFromParent (Msg_PictureFull msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public PictureFull (Msg_PictureFull msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public PictureFull () { 
        }
        #endregion
    }
}