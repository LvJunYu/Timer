// SC包例子 | CS协议包例子
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class DataExample : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 关卡 id
        /// </summary>
        private long _id;
        /// <summary>
        /// 名字
        /// </summary>
        private string _name;
        /// <summary>
        /// 分数
        /// </summary>
        private float _score;
        /// <summary>
        /// 关卡内容
        /// </summary>
        private List<Struct> _struct;

        // cs fields----------------------------------
        /// <summary>
        /// 关卡 id
        /// </summary>
        private long _cs_id;
        /// <summary>
        /// 名字
        /// </summary>
        private string _cs_name;
        /// <summary>
        /// 类型
        /// </summary>
        private ESomeEnum _cs_type;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 关卡 id
        /// </summary>
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { 
            get { return _name; }
            set { if (_name != value) {
                _name = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 分数
        /// </summary>
        public float Score { 
            get { return _score; }
            set { if (_score != value) {
                _score = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 关卡内容
        /// </summary>
        public List<Struct> Struct { 
            get { return _struct; }
            set { if (_struct != value) {
                _struct = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 关卡 id
        /// </summary>
        public long CS_Id { 
            get { return _cs_id; }
            set { _cs_id = value; }
        }
        /// <summary>
        /// 名字
        /// </summary>
        public string CS_Name { 
            get { return _cs_name; }
            set { _cs_name = value; }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public ESomeEnum CS_Type { 
            get { return _cs_type; }
            set { _cs_type = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _struct) {
                    for (int i = 0; i < _struct.Count; i++) {
                        if (null != _struct[i] && _struct[i].IsDirty) {
                            return true;
                        }
                    }
                }
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// CS协议包例子
		/// </summary>
		/// <param name="id">关卡 id.</param>
		/// <param name="name">名字.</param>
		/// <param name="type">类型.</param>
        public void Request (
            long id,
            string name,
            ESomeEnum type,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_id != id) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_name != name) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_type != type) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_id = id;
                _cs_name = name;
                _cs_type = type;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_DataExample msg = new Msg_CS_DAT_DataExample();
                msg.Id = id;
                msg.Name = name;
                msg.Type = type;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_DataExample>(
                    SoyHttpApiPath.DataExample, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_DataExample msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
            _name = msg.Name;           
            _score = msg.Score;           
            _struct = new List<Struct>();
            for (int i = 0; i < msg.Struct.Count; i++) {
                _struct.Add(new Struct(msg.Struct[i]));
            }
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_DataExample msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public DataExample (Msg_SC_DAT_DataExample msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public DataExample () { 
            _struct = new List<Struct>();
        }
        #endregion
    }
}