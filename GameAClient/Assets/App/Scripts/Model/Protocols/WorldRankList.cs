// 排行榜 | 排行榜

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class WorldRankList : SyncronisticData<Msg_SC_DAT_WorldRankList>
    {
        #region 字段

        // sc fields----------------------------------
        /// <summary>
        /// ECachedDataState
        /// </summary>
        private int _resultCode;

        /// <summary>
        /// 
        /// </summary>
        private long _updateTime;

        /// <summary>
        /// 
        /// </summary>
        private List<WorldRankItem> _rankList;

        // cs fields----------------------------------
        /// <summary>
        /// 类型
        /// </summary>
        private EWorldRankType _cs_type;

        /// <summary>
        /// 时间段
        /// </summary>
        private ERankTimeBucket _cs_timeBucket;

        /// <summary>
        /// 
        /// </summary>
        private int _cs_startInx;

        /// <summary>
        /// 
        /// </summary>
        private int _cs_maxCount;

        #endregion

        #region 属性

        // sc properties----------------------------------
        /// <summary>
        /// ECachedDataState
        /// </summary>
        public int ResultCode
        {
            get { return _resultCode; }
            set
            {
                if (_resultCode != value)
                {
                    _resultCode = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long UpdateTime
        {
            get { return _updateTime; }
            set
            {
                if (_updateTime != value)
                {
                    _updateTime = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<WorldRankItem> RankList
        {
            get { return _rankList; }
            set
            {
                if (_rankList != value)
                {
                    _rankList = value;
                    SetDirty();
                }
            }
        }

        // cs properties----------------------------------
        /// <summary>
        /// 类型
        /// </summary>
        public EWorldRankType CS_Type
        {
            get { return _cs_type; }
            set { _cs_type = value; }
        }

        /// <summary>
        /// 时间段
        /// </summary>
        public ERankTimeBucket CS_TimeBucket
        {
            get { return _cs_timeBucket; }
            set { _cs_timeBucket = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CS_StartInx
        {
            get { return _cs_startInx; }
            set { _cs_startInx = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CS_MaxCount
        {
            get { return _cs_maxCount; }
            set { _cs_maxCount = value; }
        }

        public override bool IsDirty
        {
            get
            {
                if (null != _rankList)
                {
                    for (int i = 0; i < _rankList.Count; i++)
                    {
                        if (null != _rankList[i] && _rankList[i].IsDirty)
                        {
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
        /// 排行榜
        /// </summary>
        /// <param name="type">类型.</param>
        /// <param name="timeBucket">时间段.</param>
        /// <param name="startInx">.</param>
        /// <param name="maxCount">.</param>
        public void Request(
            EWorldRankType type,
            ERankTimeBucket timeBucket,
            int startInx,
            int maxCount,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting)
            {
                if (_cs_type != type)
                {
                    if (null != failedCallback) failedCallback.Invoke(ENetResultCode.NR_None);
                    return;
                }
                if (_cs_timeBucket != timeBucket)
                {
                    if (null != failedCallback) failedCallback.Invoke(ENetResultCode.NR_None);
                    return;
                }
                if (_cs_startInx != startInx)
                {
                    if (null != failedCallback) failedCallback.Invoke(ENetResultCode.NR_None);
                    return;
                }
                if (_cs_maxCount != maxCount)
                {
                    if (null != failedCallback) failedCallback.Invoke(ENetResultCode.NR_None);
                    return;
                }
                OnRequest(successCallback, failedCallback);
            }
            else
            {
                _cs_type = type;
                _cs_timeBucket = timeBucket;
                _cs_startInx = startInx;
                _cs_maxCount = maxCount;
                OnRequest(successCallback, failedCallback);

                Msg_CS_DAT_WorldRankList msg = new Msg_CS_DAT_WorldRankList();
                msg.Type = type;
                msg.TimeBucket = timeBucket;
                msg.StartInx = startInx;
                msg.MaxCount = maxCount;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_WorldRankList>(
                    SoyHttpApiPath.WorldRankList, msg, ret =>
                    {
                        if (OnSync(ret))
                        {
                            OnSyncSucceed();
                        }
                    }, (failedCode, failedMsg) => { OnSyncFailed(failedCode, failedMsg); });
            }
        }

        public bool OnSync(Msg_SC_DAT_WorldRankList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;
            _updateTime = msg.UpdateTime;
            _rankList = new List<WorldRankItem>();
            for (int i = 0; i < msg.RankList.Count; i++)
            {
                _rankList.Add(new WorldRankItem(msg.RankList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData(Msg_SC_DAT_WorldRankList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;
            _updateTime = msg.UpdateTime;
            if (null == _rankList)
            {
                _rankList = new List<WorldRankItem>();
            }
            _rankList.Clear();
            for (int i = 0; i < msg.RankList.Count; i++)
            {
                _rankList.Add(new WorldRankItem(msg.RankList[i]));
            }
            return true;
        }

        public bool DeepCopy(WorldRankList obj)
        {
            if (null == obj) return false;
            _resultCode = obj.ResultCode;
            _updateTime = obj.UpdateTime;
            if (null == obj.RankList) return false;
            if (null == _rankList)
            {
                _rankList = new List<WorldRankItem>();
            }
            _rankList.Clear();
            for (int i = 0; i < obj.RankList.Count; i++)
            {
                _rankList.Add(obj.RankList[i]);
            }
            return true;
        }

        public void OnSyncFromParent(Msg_SC_DAT_WorldRankList msg)
        {
            if (OnSync(msg))
            {
                OnSyncSucceed();
            }
        }

        public WorldRankList(Msg_SC_DAT_WorldRankList msg)
        {
            if (OnSync(msg))
            {
                OnSyncSucceed();
            }
        }

        public WorldRankList()
        {
            _rankList = new List<WorldRankItem>();
            OnCreate();
        }

        #endregion
    }
}