//  | 抽奖券
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class RaffleTicketItem : SyncronisticData<Msg_RaffleTicketItem> {
        #region 字段
        /// <summary>
        /// 类型
        /// </summary>
        private int _type;
        /// <summary>
        /// 
        /// </summary>
        private long _id;
        /// <summary>
        /// 
        /// </summary>
        private int _count;
        #endregion

        #region 属性
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { 
            get { return _type; }
            set { if (_type != value) {
                _type = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
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
        public int Count { 
            get { return _count; }
            set { if (_count != value) {
                _count = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_RaffleTicketItem msg)
        {
            if (null == msg) return false;
            _type = msg.Type;     
            _id = msg.Id;     
            _count = msg.Count;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_RaffleTicketItem msg)
        {
            if (null == msg) return false;
            _type = msg.Type;           
            _id = msg.Id;           
            _count = msg.Count;           
            return true;
        } 

        public bool DeepCopy (RaffleTicketItem obj)
        {
            if (null == obj) return false;
            _type = obj.Type;           
            _id = obj.Id;           
            _count = obj.Count;           
            return true;
        }

        public void OnSyncFromParent (Msg_RaffleTicketItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public RaffleTicketItem (Msg_RaffleTicketItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public RaffleTicketItem () { 
        }
        #endregion
    }
}