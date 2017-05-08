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
        // 最后更新时间
        private long _updateTime;
        // 
        private long _projectId;
        // 
        private UserInfoSimple _userInfo;
        // 名称
        private string _name;
        // 简介
        private string _summary;
        // 关卡截图资源路径
        private string _iconPath;
        // 关卡文件资源路径
        private string _resPath;
        // 创建时间
        private long _createTime;
        // 
        private ELocalDataState _localDataState;
        // 
        private EProjectStatus _projectStatus;
        // 
        private long _localUpdateTime;
        // 
        private int _programVersion;
        // 
        private int _resourcesVersion;
        // 是否有效
        private bool _isValid;
        // 收藏次数等信息
        private ProjectExtend _extendData;
        // 用户收藏等信息
        private ProjectUserData _projectUserData;
        // 是否已经过关
        private bool _passFlag;
        // 过关录像使用时间
        private float _recordUsedTime;
        // 改造原始section
        private int _targetSection;
        // 改造原始level
        private int _targetLevel;
        // 地图宽
        private int _mapWidth;
        // 地图高
        private int _mapHeight;
        // 总地块数
        private int _totalUnitCount;
        // 添加地块数量
        private int _addCount;
        // 删除地块数量
        private int _deleteCount;
        // 修改地块数量
        private int _modifyCount;

        // cs fields----------------------------------
        // 关卡Id
        private long _cs_projectId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        // 最后更新时间
        public long UpdateTime { 
            get { return _updateTime; }
            set { if (_updateTime != value) {
                _updateTime = value;
                SetDirty();
            }}
        }
        // 
        public long ProjectId { 
            get { return _projectId; }
            set { if (_projectId != value) {
                _projectId = value;
                SetDirty();
            }}
        }
        // 
        public UserInfoSimple UserInfo { 
            get { return _userInfo; }
            set { if (_userInfo != value) {
                _userInfo = value;
                SetDirty();
            }}
        }
        // 名称
        public string Name { 
            get { return _name; }
            set { if (_name != value) {
                _name = value;
                SetDirty();
            }}
        }
        // 简介
        public string Summary { 
            get { return _summary; }
            set { if (_summary != value) {
                _summary = value;
                SetDirty();
            }}
        }
        // 关卡截图资源路径
        public string IconPath { 
            get { return _iconPath; }
            set { if (_iconPath != value) {
                _iconPath = value;
                SetDirty();
            }}
        }
        // 关卡文件资源路径
        public string ResPath { 
            get { return _resPath; }
            set { if (_resPath != value) {
                _resPath = value;
                SetDirty();
            }}
        }
        // 创建时间
        public long CreateTime { 
            get { return _createTime; }
            set { if (_createTime != value) {
                _createTime = value;
                SetDirty();
            }}
        }
        // 
        public ELocalDataState LocalDataState { 
            get { return _localDataState; }
            set { if (_localDataState != value) {
                _localDataState = value;
                SetDirty();
            }}
        }
        // 
        public EProjectStatus ProjectStatus { 
            get { return _projectStatus; }
            set { if (_projectStatus != value) {
                _projectStatus = value;
                SetDirty();
            }}
        }
        // 
        public long LocalUpdateTime { 
            get { return _localUpdateTime; }
            set { if (_localUpdateTime != value) {
                _localUpdateTime = value;
                SetDirty();
            }}
        }
        // 
        public int ProgramVersion { 
            get { return _programVersion; }
            set { if (_programVersion != value) {
                _programVersion = value;
                SetDirty();
            }}
        }
        // 
        public int ResourcesVersion { 
            get { return _resourcesVersion; }
            set { if (_resourcesVersion != value) {
                _resourcesVersion = value;
                SetDirty();
            }}
        }
        // 是否有效
        public bool IsValid { 
            get { return _isValid; }
            set { if (_isValid != value) {
                _isValid = value;
                SetDirty();
            }}
        }
        // 收藏次数等信息
        public ProjectExtend ExtendData { 
            get { return _extendData; }
            set { if (_extendData != value) {
                _extendData = value;
                SetDirty();
            }}
        }
        // 用户收藏等信息
        public ProjectUserData ProjectUserData { 
            get { return _projectUserData; }
            set { if (_projectUserData != value) {
                _projectUserData = value;
                SetDirty();
            }}
        }
        // 是否已经过关
        public bool PassFlag { 
            get { return _passFlag; }
            set { if (_passFlag != value) {
                _passFlag = value;
                SetDirty();
            }}
        }
        // 过关录像使用时间
        public float RecordUsedTime { 
            get { return _recordUsedTime; }
            set { if (_recordUsedTime != value) {
                _recordUsedTime = value;
                SetDirty();
            }}
        }
        // 改造原始section
        public int TargetSection { 
            get { return _targetSection; }
            set { if (_targetSection != value) {
                _targetSection = value;
                SetDirty();
            }}
        }
        // 改造原始level
        public int TargetLevel { 
            get { return _targetLevel; }
            set { if (_targetLevel != value) {
                _targetLevel = value;
                SetDirty();
            }}
        }
        // 地图宽
        public int MapWidth { 
            get { return _mapWidth; }
            set { if (_mapWidth != value) {
                _mapWidth = value;
                SetDirty();
            }}
        }
        // 地图高
        public int MapHeight { 
            get { return _mapHeight; }
            set { if (_mapHeight != value) {
                _mapHeight = value;
                SetDirty();
            }}
        }
        // 总地块数
        public int TotalUnitCount { 
            get { return _totalUnitCount; }
            set { if (_totalUnitCount != value) {
                _totalUnitCount = value;
                SetDirty();
            }}
        }
        // 添加地块数量
        public int AddCount { 
            get { return _addCount; }
            set { if (_addCount != value) {
                _addCount = value;
                SetDirty();
            }}
        }
        // 删除地块数量
        public int DeleteCount { 
            get { return _deleteCount; }
            set { if (_deleteCount != value) {
                _deleteCount = value;
                SetDirty();
            }}
        }
        // 修改地块数量
        public int ModifyCount { 
            get { return _modifyCount; }
            set { if (_modifyCount != value) {
                _modifyCount = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        // 关卡Id
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