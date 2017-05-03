// 获取关卡数据 | 获取关卡数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class Project : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 最后更新时间
        /// </summary>
        private long _updateTime;
        /// <summary>
        /// 
        /// </summary>
        private long _projectId;
        /// <summary>
        /// 
        /// </summary>
        private UserInfoSimple _userInfo;
        /// <summary>
        /// 
        /// </summary>
        private string _name;
        /// <summary>
        /// 
        /// </summary>
        private string _summary;
        /// <summary>
        /// 
        /// </summary>
        private string _iconPath;
        /// <summary>
        /// 
        /// </summary>
        private string _resPath;
        /// <summary>
        /// 
        /// </summary>
        private long _createTime;
        /// <summary>
        /// 
        /// </summary>
        private ELocalDataState _localDataState;
        /// <summary>
        /// 
        /// </summary>
        private EProjectStatus _projectStatus;
        /// <summary>
        /// 
        /// </summary>
        private long _localUpdateTime;
        /// <summary>
        /// 
        /// </summary>
        private int _programVersion;
        /// <summary>
        /// 
        /// </summary>
        private int _resourcesVersion;
        /// <summary>
        /// 是否有效
        /// </summary>
        private bool _isValid;
        /// <summary>
        /// 收藏次数等信息
        /// </summary>
        private ProjectExtend _extendData;
        /// <summary>
        /// 用户收藏等信息
        /// </summary>
        private ProjectUserData _projectUserData;
        /// <summary>
        /// 是否已经过关
        /// </summary>
        private bool _passFlag;
        /// <summary>
        /// 过关录像使用时间
        /// </summary>
        private float _recordUsedTime;
        /// <summary>
        /// 改造原始section
        /// </summary>
        private int _targetSection;
        /// <summary>
        /// 改造原始level
        /// </summary>
        private int _targetLevel;
        /// <summary>
        /// 地图宽
        /// </summary>
        private int _mapWidth;
        /// <summary>
        /// 地图高
        /// </summary>
        private int _mapHeight;
        /// <summary>
        /// 总地块数
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

        // cs fields----------------------------------
        /// <summary>
        /// 关卡Id
        /// </summary>
        private long _cs_projectId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public long UpdateTime { 
            get { return _updateTime; }
            set { if (_updateTime != value) {
                _updateTime = value;
                SetDirty();
            }}
        }
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
        /// 
        /// </summary>
        public UserInfoSimple UserInfo { 
            get { return _userInfo; }
            set { if (_userInfo != value) {
                _userInfo = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name { 
            get { return _name; }
            set { if (_name != value) {
                _name = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public string Summary { 
            get { return _summary; }
            set { if (_summary != value) {
                _summary = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public string IconPath { 
            get { return _iconPath; }
            set { if (_iconPath != value) {
                _iconPath = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public string ResPath { 
            get { return _resPath; }
            set { if (_resPath != value) {
                _resPath = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long CreateTime { 
            get { return _createTime; }
            set { if (_createTime != value) {
                _createTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public ELocalDataState LocalDataState { 
            get { return _localDataState; }
            set { if (_localDataState != value) {
                _localDataState = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public EProjectStatus ProjectStatus { 
            get { return _projectStatus; }
            set { if (_projectStatus != value) {
                _projectStatus = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long LocalUpdateTime { 
            get { return _localUpdateTime; }
            set { if (_localUpdateTime != value) {
                _localUpdateTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int ProgramVersion { 
            get { return _programVersion; }
            set { if (_programVersion != value) {
                _programVersion = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int ResourcesVersion { 
            get { return _resourcesVersion; }
            set { if (_resourcesVersion != value) {
                _resourcesVersion = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { 
            get { return _isValid; }
            set { if (_isValid != value) {
                _isValid = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 收藏次数等信息
        /// </summary>
        public ProjectExtend ExtendData { 
            get { return _extendData; }
            set { if (_extendData != value) {
                _extendData = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 用户收藏等信息
        /// </summary>
        public ProjectUserData ProjectUserData { 
            get { return _projectUserData; }
            set { if (_projectUserData != value) {
                _projectUserData = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 是否已经过关
        /// </summary>
        public bool PassFlag { 
            get { return _passFlag; }
            set { if (_passFlag != value) {
                _passFlag = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 过关录像使用时间
        /// </summary>
        public float RecordUsedTime { 
            get { return _recordUsedTime; }
            set { if (_recordUsedTime != value) {
                _recordUsedTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 改造原始section
        /// </summary>
        public int TargetSection { 
            get { return _targetSection; }
            set { if (_targetSection != value) {
                _targetSection = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 改造原始level
        /// </summary>
        public int TargetLevel { 
            get { return _targetLevel; }
            set { if (_targetLevel != value) {
                _targetLevel = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 地图宽
        /// </summary>
        public int MapWidth { 
            get { return _mapWidth; }
            set { if (_mapWidth != value) {
                _mapWidth = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 地图高
        /// </summary>
        public int MapHeight { 
            get { return _mapHeight; }
            set { if (_mapHeight != value) {
                _mapHeight = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 总地块数
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
        
        // cs properties----------------------------------
        /// <summary>
        /// 关卡Id
        /// </summary>
        public long CS_ProjectId { 
            get { return _cs_projectId; }
            set { _cs_projectId = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _userInfo && _userInfo.IsDirty) {
                    return true;
                }
                if (null != _extendData && _extendData.IsDirty) {
                    return true;
                }
                if (null != _projectUserData && _projectUserData.IsDirty) {
                    return true;
                }
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 获取关卡数据
		/// </summary>
		/// <param name="projectId">关卡Id.</param>
        public void Request (
            long projectId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            OnRequest (successCallback, failedCallback);

            Msg_CS_DAT_Project msg = new Msg_CS_DAT_Project();
            msg.ProjectId = projectId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_Project>(
                SoyHttpApiPath.Project, msg, ret => {
                    if (OnSync(ret)) {
                        OnSyncSucceed(); 
                    }
                }, (failedCode, failedMsg) => {
                    OnSyncFailed(failedCode, failedMsg);
            });
        }

        public bool OnSync (Msg_SC_DAT_Project msg)
        {
            if (null == msg) return false;
            _updateTime = msg.UpdateTime;           
            _projectId = msg.ProjectId;           
            if (null == _userInfo) {
                _userInfo = new UserInfoSimple(msg.UserInfo);
            } else {
                _userInfo.OnSyncFromParent(msg.UserInfo);
            }
            _name = msg.Name;           
            _summary = msg.Summary;           
            _iconPath = msg.IconPath;           
            _resPath = msg.ResPath;           
            _createTime = msg.CreateTime;           
            _localDataState = msg.LocalDataState;           
            _projectStatus = msg.ProjectStatus;           
            _localUpdateTime = msg.LocalUpdateTime;           
            _programVersion = msg.ProgramVersion;           
            _resourcesVersion = msg.ResourcesVersion;           
            _isValid = msg.IsValid;           
            if (null == _extendData) {
                _extendData = new ProjectExtend(msg.ExtendData);
            } else {
                _extendData.OnSyncFromParent(msg.ExtendData);
            }
            if (null == _projectUserData) {
                _projectUserData = new ProjectUserData(msg.ProjectUserData);
            } else {
                _projectUserData.OnSyncFromParent(msg.ProjectUserData);
            }
            _passFlag = msg.PassFlag;           
            _recordUsedTime = msg.RecordUsedTime;           
            _targetSection = msg.TargetSection;           
            _targetLevel = msg.TargetLevel;           
            _mapWidth = msg.MapWidth;           
            _mapHeight = msg.MapHeight;           
            _totalUnitCount = msg.TotalUnitCount;           
            _addCount = msg.AddCount;           
            _deleteCount = msg.DeleteCount;           
            _modifyCount = msg.ModifyCount;           
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_Project msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Project (Msg_SC_DAT_Project msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Project () { 
            _userInfo = new UserInfoSimple();
            _extendData = new ProjectExtend();
            _projectUserData = new ProjectUserData();
        }
        #endregion
    }
}