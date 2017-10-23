//  | 上传关卡附加数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ProjectUploadParam : SyncronisticData {
        #region 字段
        /// <summary>
        /// 添加的地块信息
        /// </summary>
        private List<UnitDataItem> _usedUnitDataList;
        /// <summary>
        /// 地图宽度
        /// </summary>
        private int _mapWidth;
        /// <summary>
        /// 地图高度
        /// </summary>
        private int _mapHeight;
        /// <summary>
        /// 总物体数量
        /// </summary>
        private int _totalUnitCount;
        /// <summary>
        /// 添加地块数量
        /// </summary>
        private int _addCount;
        /// <summary>
        /// 删除地块数量
        /// </summary>
        private int _deleteCount;
        /// <summary>
        /// 修改地块数量
        /// </summary>
        private int _modifyCount;
        /// <summary>
        /// 改造率
        /// </summary>
        private float _reformRate;
        /// <summary>
        /// 发布过关重开次数
        /// </summary>
        private int _recordRestartCount;
        /// <summary>
        /// 发布过关使用命数
        /// </summary>
        private int _recordUsedLifeCount;
        /// <summary>
        /// 这次改造总共操作次数
        /// </summary>
        private int _operateCount;
        /// <summary>
        /// 这次改造总共操作时间
        /// </summary>
        private int _totalOperateTime;
        #endregion

        #region 属性
        /// <summary>
        /// 添加的地块信息
        /// </summary>
        public List<UnitDataItem> UsedUnitDataList { 
            get { return _usedUnitDataList; }
            set { if (_usedUnitDataList != value) {
                _usedUnitDataList = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 地图宽度
        /// </summary>
        public int MapWidth { 
            get { return _mapWidth; }
            set { if (_mapWidth != value) {
                _mapWidth = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 地图高度
        /// </summary>
        public int MapHeight { 
            get { return _mapHeight; }
            set { if (_mapHeight != value) {
                _mapHeight = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 总物体数量
        /// </summary>
        public int TotalUnitCount { 
            get { return _totalUnitCount; }
            set { if (_totalUnitCount != value) {
                _totalUnitCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 添加地块数量
        /// </summary>
        public int AddCount { 
            get { return _addCount; }
            set { if (_addCount != value) {
                _addCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 删除地块数量
        /// </summary>
        public int DeleteCount { 
            get { return _deleteCount; }
            set { if (_deleteCount != value) {
                _deleteCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 修改地块数量
        /// </summary>
        public int ModifyCount { 
            get { return _modifyCount; }
            set { if (_modifyCount != value) {
                _modifyCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 改造率
        /// </summary>
        public float ReformRate { 
            get { return _reformRate; }
            set { if (_reformRate != value) {
                _reformRate = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 发布过关重开次数
        /// </summary>
        public int RecordRestartCount { 
            get { return _recordRestartCount; }
            set { if (_recordRestartCount != value) {
                _recordRestartCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 发布过关使用命数
        /// </summary>
        public int RecordUsedLifeCount { 
            get { return _recordUsedLifeCount; }
            set { if (_recordUsedLifeCount != value) {
                _recordUsedLifeCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 这次改造总共操作次数
        /// </summary>
        public int OperateCount { 
            get { return _operateCount; }
            set { if (_operateCount != value) {
                _operateCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 这次改造总共操作时间
        /// </summary>
        public int TotalOperateTime { 
            get { return _totalOperateTime; }
            set { if (_totalOperateTime != value) {
                _totalOperateTime = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_ProjectUploadParam msg)
        {
            if (null == msg) return false;
            _usedUnitDataList = new List<UnitDataItem>();
            for (int i = 0; i < msg.UsedUnitDataList.Count; i++) {
                _usedUnitDataList.Add(new UnitDataItem(msg.UsedUnitDataList[i]));
            }
            _mapWidth = msg.MapWidth;     
            _mapHeight = msg.MapHeight;     
            _totalUnitCount = msg.TotalUnitCount;     
            _addCount = msg.AddCount;     
            _deleteCount = msg.DeleteCount;     
            _modifyCount = msg.ModifyCount;     
            _reformRate = msg.ReformRate;     
            _recordRestartCount = msg.RecordRestartCount;     
            _recordUsedLifeCount = msg.RecordUsedLifeCount;     
            _operateCount = msg.OperateCount;     
            _totalOperateTime = msg.TotalOperateTime;     
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (ProjectUploadParam obj)
        {
            if (null == obj) return false;
            if (null ==  obj.UsedUnitDataList) return false;
            if (null ==  _usedUnitDataList) {
                _usedUnitDataList = new List<UnitDataItem>();
            }
            _usedUnitDataList.Clear();
            for (int i = 0; i < obj.UsedUnitDataList.Count; i++){
                _usedUnitDataList.Add(obj.UsedUnitDataList[i]);
            }
            _mapWidth = obj.MapWidth;           
            _mapHeight = obj.MapHeight;           
            _totalUnitCount = obj.TotalUnitCount;           
            _addCount = obj.AddCount;           
            _deleteCount = obj.DeleteCount;           
            _modifyCount = obj.ModifyCount;           
            _reformRate = obj.ReformRate;           
            _recordRestartCount = obj.RecordRestartCount;           
            _recordUsedLifeCount = obj.RecordUsedLifeCount;           
            _operateCount = obj.OperateCount;           
            _totalOperateTime = obj.TotalOperateTime;           
            return true;
        }

        public void OnSyncFromParent (Msg_ProjectUploadParam msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectUploadParam (Msg_ProjectUploadParam msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectUploadParam () { 
            _usedUnitDataList = new List<UnitDataItem>();
        }
        #endregion
    }
}