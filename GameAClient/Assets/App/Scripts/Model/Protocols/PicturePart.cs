//  | 拼图碎片
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class PicturePart : SyncronisticData {
        #region 字段
        /// <summary>
        /// 拼图Id
        /// </summary>
        private long _pictureId;
        /// <summary>
        /// 下标
        /// </summary>
        private int _pictureInx;
        /// <summary>
        /// 数量
        /// </summary>
        private int _totalCount;
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
        /// 下标
        /// </summary>
        public int PictureInx { 
            get { return _pictureInx; }
            set { if (_pictureInx != value) {
                _pictureInx = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 数量
        /// </summary>
        public int TotalCount { 
            get { return _totalCount; }
            set { if (_totalCount != value) {
                _totalCount = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_PicturePart msg)
        {
            if (null == msg) return false;
            _pictureId = msg.PictureId;     
            _pictureInx = msg.PictureInx;     
            _totalCount = msg.TotalCount;     
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_PicturePart msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public PicturePart (Msg_PicturePart msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public PicturePart () { 
        }
        #endregion
    }
}