// protocol command msgs

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public class RemoteCommands {
        public static bool IsRequstingLoginByToken {
            get { return _isRequstingLoginByToken; }
        }
        private static bool _isRequstingLoginByToken = false;
        /// <summary>
		/// 设备登录包
		/// </summary>
		/// <param name="appVersion">应用版本号</param>
		/// <param name="name">资源版本号</param>
		/// <param name="devicePlatform">设备类型</param>
        public static void LoginByToken (
            string appVersion,
            string name,
            EPhoneType devicePlatform,
            Action<Msg_SC_CMD_LoginByToken> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingLoginByToken) {
                return;
            }
            _isRequstingLoginByToken = true;
            Msg_CS_CMD_LoginByToken msg = new Msg_CS_CMD_LoginByToken();
            // 设备登录包
            msg.AppVersion = appVersion;
            msg.Name = name;
            msg.DevicePlatform = devicePlatform;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_LoginByToken>(
                SoyHttpApiPath.LoginByToken, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingLoginByToken = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "LoginByToken", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingLoginByToken = false;
                },
                form
            );
        }

        public static bool IsRequstingLogout {
            get { return _isRequstingLogout; }
        }
        private static bool _isRequstingLogout = false;
        /// <summary>
		/// 退出登录
		/// </summary>
		/// <param name="flag">占位</param>
        public static void Logout (
            int flag,
            Action<Msg_SC_CMD_Logout> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingLogout) {
                return;
            }
            _isRequstingLogout = true;
            Msg_CS_CMD_Logout msg = new Msg_CS_CMD_Logout();
            // 退出登录
            msg.Flag = flag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_Logout>(
                SoyHttpApiPath.Logout, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingLogout = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "Logout", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingLogout = false;
                },
                form
            );
        }

        public static bool IsRequstingUpdateUserInfo {
            get { return _isRequstingUpdateUserInfo; }
        }
        private static bool _isRequstingUpdateUserInfo = false;
        /// <summary>
		/// 用户详细信息
		/// </summary>
		/// <param name="data">用户信息</param>
        public static void UpdateUserInfo (
            Msg_SC_DAT_UserInfoDetail data,
            Action<Msg_SC_CMD_UpdateUserInfo> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpdateUserInfo) {
                return;
            }
            _isRequstingUpdateUserInfo = true;
            Msg_CS_CMD_UpdateUserInfo msg = new Msg_CS_CMD_UpdateUserInfo();
            // 用户详细信息
            msg.Data = data;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateUserInfo>(
                SoyHttpApiPath.UpdateUserInfo, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpdateUserInfo = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateUserInfo", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpdateUserInfo = false;
                },
                form
            );
        }

        public static bool IsRequstingUpdateFollowState {
            get { return _isRequstingUpdateFollowState; }
        }
        private static bool _isRequstingUpdateFollowState = false;
        /// <summary>
		/// 更新关注状态
		/// </summary>
		/// <param name="userId">用户id</param>
		/// <param name="followFlag"></param>
        public static void UpdateFollowState (
            long userId,
            bool followFlag,
            Action<Msg_SC_CMD_UpdateFollowState> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpdateFollowState) {
                return;
            }
            _isRequstingUpdateFollowState = true;
            Msg_CS_CMD_UpdateFollowState msg = new Msg_CS_CMD_UpdateFollowState();
            // 更新关注状态
            msg.UserId = userId;
            msg.FollowFlag = followFlag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateFollowState>(
                SoyHttpApiPath.UpdateFollowState, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpdateFollowState = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateFollowState", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpdateFollowState = false;
                },
                form
            );
        }

        public static bool IsRequstingUpdateBlockState {
            get { return _isRequstingUpdateBlockState; }
        }
        private static bool _isRequstingUpdateBlockState = false;
        /// <summary>
		/// 更新关注状态
		/// </summary>
		/// <param name="userId">用户id</param>
		/// <param name="blockFlag"></param>
        public static void UpdateBlockState (
            long userId,
            bool blockFlag,
            Action<Msg_SC_CMD_UpdateBlockState> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpdateBlockState) {
                return;
            }
            _isRequstingUpdateBlockState = true;
            Msg_CS_CMD_UpdateBlockState msg = new Msg_CS_CMD_UpdateBlockState();
            // 更新关注状态
            msg.UserId = userId;
            msg.BlockFlag = blockFlag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateBlockState>(
                SoyHttpApiPath.UpdateBlockState, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpdateBlockState = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateBlockState", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpdateBlockState = false;
                },
                form
            );
        }

        public static bool IsRequstingCreateProject {
            get { return _isRequstingCreateProject; }
        }
        private static bool _isRequstingCreateProject = false;
        /// <summary>
		/// 创建关卡
		/// </summary>
		/// <param name="name"></param>
		/// <param name="summary"></param>
		/// <param name="programVersion"></param>
		/// <param name="resourceVersion"></param>
		/// <param name="passFlag"></param>
		/// <param name="recordUsedTime"></param>
		/// <param name="timeLimit">时间限制</param>
		/// <param name="winCondition">胜利条件</param>
        public static void CreateProject (
            string name,
            string summary,
            int programVersion,
            int resourceVersion,
            bool passFlag,
            float recordUsedTime,
            int timeLimit,
            int winCondition,
            Action<Msg_SC_CMD_CreateProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingCreateProject) {
                return;
            }
            _isRequstingCreateProject = true;
            Msg_CS_CMD_CreateProject msg = new Msg_CS_CMD_CreateProject();
            // 创建关卡
            msg.Name = name;
            msg.Summary = summary;
            msg.ProgramVersion = programVersion;
            msg.ResourceVersion = resourceVersion;
            msg.PassFlag = passFlag;
            msg.RecordUsedTime = recordUsedTime;
            msg.TimeLimit = timeLimit;
            msg.WinCondition = winCondition;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_CreateProject>(
                SoyHttpApiPath.CreateProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingCreateProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "CreateProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingCreateProject = false;
                },
                form
            );
        }

        public static bool IsRequstingUpdateProject {
            get { return _isRequstingUpdateProject; }
        }
        private static bool _isRequstingUpdateProject = false;
        /// <summary>
		/// 更新关卡
		/// </summary>
		/// <param name="projectId">关卡Id</param>
		/// <param name="name"></param>
		/// <param name="summary"></param>
		/// <param name="programVersion"></param>
		/// <param name="resourceVersion"></param>
		/// <param name="passFlag"></param>
		/// <param name="recordUsedTime"></param>
		/// <param name="timeLimit">时间限制</param>
		/// <param name="winCondition">胜利条件</param>
        public static void UpdateProject (
            long projectId,
            string name,
            string summary,
            int programVersion,
            int resourceVersion,
            bool passFlag,
            float recordUsedTime,
            int timeLimit,
            int winCondition,
            Action<Msg_SC_CMD_UpdateProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpdateProject) {
                return;
            }
            _isRequstingUpdateProject = true;
            Msg_CS_CMD_UpdateProject msg = new Msg_CS_CMD_UpdateProject();
            // 更新关卡
            msg.ProjectId = projectId;
            msg.Name = name;
            msg.Summary = summary;
            msg.ProgramVersion = programVersion;
            msg.ResourceVersion = resourceVersion;
            msg.PassFlag = passFlag;
            msg.RecordUsedTime = recordUsedTime;
            msg.TimeLimit = timeLimit;
            msg.WinCondition = winCondition;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateProject>(
                SoyHttpApiPath.UpdateProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpdateProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpdateProject = false;
                },
                form
            );
        }

        public static bool IsRequstingDeleteProject {
            get { return _isRequstingDeleteProject; }
        }
        private static bool _isRequstingDeleteProject = false;
        /// <summary>
		/// 删除关卡
		/// </summary>
		/// <param name="projectId">关卡Id</param>
        public static void DeleteProject (
            List<long> projectId,
            Action<Msg_SC_CMD_DeleteProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingDeleteProject) {
                return;
            }
            _isRequstingDeleteProject = true;
            Msg_CS_CMD_DeleteProject msg = new Msg_CS_CMD_DeleteProject();
            // 删除关卡
            msg.ProjectId.AddRange(projectId);
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_DeleteProject>(
                SoyHttpApiPath.DeleteProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingDeleteProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "DeleteProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingDeleteProject = false;
                },
                form
            );
        }

        public static bool IsRequstingPublishWorldProject {
            get { return _isRequstingPublishWorldProject; }
        }
        private static bool _isRequstingPublishWorldProject = false;
        /// <summary>
		/// 发布关卡
		/// </summary>
		/// <param name="personalProjectId"></param>
		/// <param name="name"></param>
		/// <param name="summary"></param>
		/// <param name="programVersion"></param>
		/// <param name="resourceVersion"></param>
		/// <param name="recordUsedTime"></param>
		/// <param name="timeLimit">时间限制</param>
		/// <param name="winCondition">胜利条件</param>
        public static void PublishWorldProject (
            long personalProjectId,
            string name,
            string summary,
            int programVersion,
            int resourceVersion,
            float recordUsedTime,
            int timeLimit,
            int winCondition,
            Action<Msg_SC_CMD_PublishWorldProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingPublishWorldProject) {
                return;
            }
            _isRequstingPublishWorldProject = true;
            Msg_CS_CMD_PublishWorldProject msg = new Msg_CS_CMD_PublishWorldProject();
            // 发布关卡
            msg.PersonalProjectId = personalProjectId;
            msg.Name = name;
            msg.Summary = summary;
            msg.ProgramVersion = programVersion;
            msg.ResourceVersion = resourceVersion;
            msg.RecordUsedTime = recordUsedTime;
            msg.TimeLimit = timeLimit;
            msg.WinCondition = winCondition;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_PublishWorldProject>(
                SoyHttpApiPath.PublishWorldProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingPublishWorldProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "PublishWorldProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingPublishWorldProject = false;
                },
                form
            );
        }

        public static bool IsRequstingUnpublishWorldProject {
            get { return _isRequstingUnpublishWorldProject; }
        }
        private static bool _isRequstingUnpublishWorldProject = false;
        /// <summary>
		/// 取消发布
		/// </summary>
		/// <param name="projectId">关卡Id</param>
        public static void UnpublishWorldProject (
            List<long> projectId,
            Action<Msg_SC_CMD_UnpublishWorldProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUnpublishWorldProject) {
                return;
            }
            _isRequstingUnpublishWorldProject = true;
            Msg_CS_CMD_UnpublishWorldProject msg = new Msg_CS_CMD_UnpublishWorldProject();
            // 取消发布
            msg.ProjectId.AddRange(projectId);
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UnpublishWorldProject>(
                SoyHttpApiPath.UnpublishWorldProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUnpublishWorldProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UnpublishWorldProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUnpublishWorldProject = false;
                },
                form
            );
        }

        public static bool IsRequstingPlayWorldProject {
            get { return _isRequstingPlayWorldProject; }
        }
        private static bool _isRequstingPlayWorldProject = false;
        /// <summary>
		/// Msg_CS_CMD_PlayWorldProject
		/// </summary>
		/// <param name="projectId">关卡Id</param>
        public static void PlayWorldProject (
            long projectId,
            Action<Msg_SC_CMD_PlayWorldProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingPlayWorldProject) {
                return;
            }
            _isRequstingPlayWorldProject = true;
            Msg_CS_CMD_PlayWorldProject msg = new Msg_CS_CMD_PlayWorldProject();
            // Msg_CS_CMD_PlayWorldProject
            msg.ProjectId = projectId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_PlayWorldProject>(
                SoyHttpApiPath.PlayWorldProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingPlayWorldProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "PlayWorldProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingPlayWorldProject = false;
                },
                form
            );
        }

        public static bool IsRequstingCommitWorldProjectResult {
            get { return _isRequstingCommitWorldProjectResult; }
        }
        private static bool _isRequstingCommitWorldProjectResult = false;
        /// <summary>
		/// 提交过关世界关卡数据
		/// </summary>
		/// <param name="token">关卡Id</param>
		/// <param name="success"></param>
		/// <param name="deadPos"></param>
		/// <param name="usedTime"></param>
		/// <param name="score">最终得分</param>
		/// <param name="scoreItemCount">奖分道具数</param>
		/// <param name="killMonsterCount">击杀怪物数</param>
		/// <param name="leftTime">剩余时间数</param>
		/// <param name="leftLife">剩余生命</param>
        public static void CommitWorldProjectResult (
            long token,
            bool success,
            byte[] deadPos,
            float usedTime,
            int score,
            int scoreItemCount,
            int killMonsterCount,
            int leftTime,
            int leftLife,
            Action<Msg_SC_CMD_CommitWorldProjectResult> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingCommitWorldProjectResult) {
                return;
            }
            _isRequstingCommitWorldProjectResult = true;
            Msg_CS_CMD_CommitWorldProjectResult msg = new Msg_CS_CMD_CommitWorldProjectResult();
            // 提交过关世界关卡数据
            msg.Token = token;
            msg.Success = success;
            msg.DeadPos = deadPos;
            msg.UsedTime = usedTime;
            msg.Score = score;
            msg.ScoreItemCount = scoreItemCount;
            msg.KillMonsterCount = killMonsterCount;
            msg.LeftTime = leftTime;
            msg.LeftLife = leftLife;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_CommitWorldProjectResult>(
                SoyHttpApiPath.CommitWorldProjectResult, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingCommitWorldProjectResult = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "CommitWorldProjectResult", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingCommitWorldProjectResult = false;
                },
                form
            );
        }

        public static bool IsRequstingPostWorldProjectComment {
            get { return _isRequstingPostWorldProjectComment; }
        }
        private static bool _isRequstingPostWorldProjectComment = false;
        /// <summary>
		/// 提交世界关卡评论
		/// </summary>
		/// <param name="projectId">关卡Id</param>
		/// <param name="targetUserId"></param>
		/// <param name="comment"></param>
        public static void PostWorldProjectComment (
            long projectId,
            long targetUserId,
            string comment,
            Action<Msg_SC_CMD_PostWorldProjectComment> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingPostWorldProjectComment) {
                return;
            }
            _isRequstingPostWorldProjectComment = true;
            Msg_CS_CMD_PostWorldProjectComment msg = new Msg_CS_CMD_PostWorldProjectComment();
            // 提交世界关卡评论
            msg.ProjectId = projectId;
            msg.TargetUserId = targetUserId;
            msg.Comment = comment;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_PostWorldProjectComment>(
                SoyHttpApiPath.PostWorldProjectComment, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingPostWorldProjectComment = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "PostWorldProjectComment", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingPostWorldProjectComment = false;
                },
                form
            );
        }

        public static bool IsRequstingUpdateWorldProjectLike {
            get { return _isRequstingUpdateWorldProjectLike; }
        }
        private static bool _isRequstingUpdateWorldProjectLike = false;
        /// <summary>
		/// 修改关卡顶踩
		/// </summary>
		/// <param name="projectId">关卡Id</param>
		/// <param name="likeState"></param>
        public static void UpdateWorldProjectLike (
            long projectId,
            EProjectLikeState likeState,
            Action<Msg_SC_CMD_UpdateWorldProjectLike> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpdateWorldProjectLike) {
                return;
            }
            _isRequstingUpdateWorldProjectLike = true;
            Msg_CS_CMD_UpdateWorldProjectLike msg = new Msg_CS_CMD_UpdateWorldProjectLike();
            // 修改关卡顶踩
            msg.ProjectId = projectId;
            msg.LikeState = likeState;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateWorldProjectLike>(
                SoyHttpApiPath.UpdateWorldProjectLike, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpdateWorldProjectLike = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateWorldProjectLike", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpdateWorldProjectLike = false;
                },
                form
            );
        }

        public static bool IsRequstingUpdateWorldProjectFavorite {
            get { return _isRequstingUpdateWorldProjectFavorite; }
        }
        private static bool _isRequstingUpdateWorldProjectFavorite = false;
        /// <summary>
		/// 修改关卡收藏状态
		/// </summary>
		/// <param name="projectId">关卡Id</param>
		/// <param name="favoriteFlag"></param>
        public static void UpdateWorldProjectFavorite (
            long projectId,
            bool favoriteFlag,
            Action<Msg_SC_CMD_UpdateWorldProjectFavorite> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpdateWorldProjectFavorite) {
                return;
            }
            _isRequstingUpdateWorldProjectFavorite = true;
            Msg_CS_CMD_UpdateWorldProjectFavorite msg = new Msg_CS_CMD_UpdateWorldProjectFavorite();
            // 修改关卡收藏状态
            msg.ProjectId = projectId;
            msg.FavoriteFlag = favoriteFlag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateWorldProjectFavorite>(
                SoyHttpApiPath.UpdateWorldProjectFavorite, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpdateWorldProjectFavorite = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateWorldProjectFavorite", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpdateWorldProjectFavorite = false;
                },
                form
            );
        }

        public static bool IsRequstingDeleteWorldProjectFavorite {
            get { return _isRequstingDeleteWorldProjectFavorite; }
        }
        private static bool _isRequstingDeleteWorldProjectFavorite = false;
        /// <summary>
		/// 删除收藏关卡
		/// </summary>
		/// <param name="projectId">关卡Id</param>
        public static void DeleteWorldProjectFavorite (
            List<long> projectId,
            Action<Msg_SC_CMD_DeleteWorldProjectFavorite> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingDeleteWorldProjectFavorite) {
                return;
            }
            _isRequstingDeleteWorldProjectFavorite = true;
            Msg_CS_CMD_DeleteWorldProjectFavorite msg = new Msg_CS_CMD_DeleteWorldProjectFavorite();
            // 删除收藏关卡
            msg.ProjectId.AddRange(projectId);
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_DeleteWorldProjectFavorite>(
                SoyHttpApiPath.DeleteWorldProjectFavorite, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingDeleteWorldProjectFavorite = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "DeleteWorldProjectFavorite", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingDeleteWorldProjectFavorite = false;
                },
                form
            );
        }

        public static bool IsRequstingBuyEnergy {
            get { return _isRequstingBuyEnergy; }
        }
        private static bool _isRequstingBuyEnergy = false;
        /// <summary>
		/// 购买体力
		/// </summary>
		/// <param name="energy">购买数量</param>
        public static void BuyEnergy (
            int energy,
            Action<Msg_SC_CMD_BuyEnergy> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingBuyEnergy) {
                return;
            }
            _isRequstingBuyEnergy = true;
            Msg_CS_CMD_BuyEnergy msg = new Msg_CS_CMD_BuyEnergy();
            // 购买体力
            msg.Energy = energy;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_BuyEnergy>(
                SoyHttpApiPath.BuyEnergy, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingBuyEnergy = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "BuyEnergy", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingBuyEnergy = false;
                },
                form
            );
        }

        public static bool IsRequstingPlayAdventureLevel {
            get { return _isRequstingPlayAdventureLevel; }
        }
        private static bool _isRequstingPlayAdventureLevel = false;
        /// <summary>
		/// 进入冒险关卡
		/// </summary>
		/// <param name="section">章节id</param>
		/// <param name="projectType">关卡类型</param>
		/// <param name="level">关卡id</param>
        public static void PlayAdventureLevel (
            int section,
            EAdventureProjectType projectType,
            int level,
            Action<Msg_SC_CMD_PlayAdventureLevel> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingPlayAdventureLevel) {
                return;
            }
            _isRequstingPlayAdventureLevel = true;
            Msg_CS_CMD_PlayAdventureLevel msg = new Msg_CS_CMD_PlayAdventureLevel();
            // 进入冒险关卡
            msg.Section = section;
            msg.ProjectType = projectType;
            msg.Level = level;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_PlayAdventureLevel>(
                SoyHttpApiPath.PlayAdventureLevel, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingPlayAdventureLevel = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "PlayAdventureLevel", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingPlayAdventureLevel = false;
                },
                form
            );
        }

        public static bool IsRequstingUnlockAdventureSection {
            get { return _isRequstingUnlockAdventureSection; }
        }
        private static bool _isRequstingUnlockAdventureSection = false;
        /// <summary>
		/// 解锁章节
		/// </summary>
		/// <param name="section">章节</param>
        public static void UnlockAdventureSection (
            int section,
            Action<Msg_SC_CMD_UnlockAdventureSection> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUnlockAdventureSection) {
                return;
            }
            _isRequstingUnlockAdventureSection = true;
            Msg_CS_CMD_UnlockAdventureSection msg = new Msg_CS_CMD_UnlockAdventureSection();
            // 解锁章节
            msg.Section = section;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UnlockAdventureSection>(
                SoyHttpApiPath.UnlockAdventureSection, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUnlockAdventureSection = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UnlockAdventureSection", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUnlockAdventureSection = false;
                },
                form
            );
        }

        public static bool IsRequstingCommitAdventureLevelResult {
            get { return _isRequstingCommitAdventureLevelResult; }
        }
        private static bool _isRequstingCommitAdventureLevelResult = false;
        /// <summary>
		/// 提交冒险模式数据
		/// </summary>
		/// <param name="token">令牌</param>
		/// <param name="success">是否过关</param>
		/// <param name="usedTime">使用的时间</param>
		/// <param name="star1Flag">星1标志</param>
		/// <param name="star2Flag">星2标志</param>
		/// <param name="star3Flag">星3标志</param>
		/// <param name="score">最终得分</param>
		/// <param name="scoreItemCount">奖分道具数</param>
		/// <param name="killMonsterCount">击杀怪物数</param>
		/// <param name="leftTime">剩余时间数</param>
		/// <param name="leftLife">剩余生命</param>
        public static void CommitAdventureLevelResult (
            long token,
            bool success,
            float usedTime,
            bool star1Flag,
            bool star2Flag,
            bool star3Flag,
            int score,
            int scoreItemCount,
            int killMonsterCount,
            int leftTime,
            int leftLife,
            Action<Msg_SC_CMD_CommitAdventureLevelResult> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingCommitAdventureLevelResult) {
                return;
            }
            _isRequstingCommitAdventureLevelResult = true;
            Msg_CS_CMD_CommitAdventureLevelResult msg = new Msg_CS_CMD_CommitAdventureLevelResult();
            // 提交冒险模式数据
            msg.Token = token;
            msg.Success = success;
            msg.UsedTime = usedTime;
            msg.Star1Flag = star1Flag;
            msg.Star2Flag = star2Flag;
            msg.Star3Flag = star3Flag;
            msg.Score = score;
            msg.ScoreItemCount = scoreItemCount;
            msg.KillMonsterCount = killMonsterCount;
            msg.LeftTime = leftTime;
            msg.LeftLife = leftLife;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_CommitAdventureLevelResult>(
                SoyHttpApiPath.CommitAdventureLevelResult, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingCommitAdventureLevelResult = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "CommitAdventureLevelResult", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingCommitAdventureLevelResult = false;
                },
                form
            );
        }

        public static bool IsRequstingUseProps {
            get { return _isRequstingUseProps; }
        }
        private static bool _isRequstingUseProps = false;
        /// <summary>
		/// 使用道具
		/// </summary>
		/// <param name="token">令牌</param>
		/// <param name="itemDataList"></param>
        public static void UseProps (
            long token,
            List<Msg_PropItem> itemDataList,
            Action<Msg_SC_CMD_UseProps> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUseProps) {
                return;
            }
            _isRequstingUseProps = true;
            Msg_CS_CMD_UseProps msg = new Msg_CS_CMD_UseProps();
            // 使用道具
            msg.Token = token;
            msg.ItemDataList.AddRange(itemDataList);
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UseProps>(
                SoyHttpApiPath.UseProps, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUseProps = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UseProps", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUseProps = false;
                },
                form
            );
        }

        public static bool IsRequstingUnlockHomePart {
            get { return _isRequstingUnlockHomePart; }
        }
        private static bool _isRequstingUnlockHomePart = false;
        /// <summary>
		/// 解锁装饰
		/// </summary>
		/// <param name="type">部位</param>
		/// <param name="id"></param>
        public static void UnlockHomePart (
            EHomePart type,
            long id,
            Action<Msg_SC_CMD_UnlockHomePart> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUnlockHomePart) {
                return;
            }
            _isRequstingUnlockHomePart = true;
            Msg_CS_CMD_UnlockHomePart msg = new Msg_CS_CMD_UnlockHomePart();
            // 解锁装饰
            msg.Type = type;
            msg.Id = id;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UnlockHomePart>(
                SoyHttpApiPath.UnlockHomePart, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUnlockHomePart = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UnlockHomePart", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUnlockHomePart = false;
                },
                form
            );
        }

        public static bool IsRequstingChangeAvatarPart {
            get { return _isRequstingChangeAvatarPart; }
        }
        private static bool _isRequstingChangeAvatarPart = false;
        /// <summary>
		/// 角色换装
		/// </summary>
		/// <param name="type">部位</param>
		/// <param name="newId"></param>
        public static void ChangeAvatarPart (
            EAvatarPart type,
            long newId,
            Action<Msg_SC_CMD_ChangeAvatarPart> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingChangeAvatarPart) {
                return;
            }
            _isRequstingChangeAvatarPart = true;
            Msg_CS_CMD_ChangeAvatarPart msg = new Msg_CS_CMD_ChangeAvatarPart();
            // 角色换装
            msg.Type = type;
            msg.NewId = newId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ChangeAvatarPart>(
                SoyHttpApiPath.ChangeAvatarPart, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingChangeAvatarPart = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ChangeAvatarPart", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingChangeAvatarPart = false;
                },
                form
            );
        }

        public static bool IsRequstingBuyAvatarPart {
            get { return _isRequstingBuyAvatarPart; }
        }
        private static bool _isRequstingBuyAvatarPart = false;
        /// <summary>
		/// 购买时装
		/// </summary>
		/// <param name="buyList">购买的数据列表</param>
		/// <param name="putOn">购买成功是否立即穿上</param>
        public static void BuyAvatarPart (
            List<Msg_BuyAvatarPartItem> buyList,
            bool putOn,
            Action<Msg_SC_CMD_BuyAvatarPart> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingBuyAvatarPart) {
                return;
            }
            _isRequstingBuyAvatarPart = true;
            Msg_CS_CMD_BuyAvatarPart msg = new Msg_CS_CMD_BuyAvatarPart();
            // 购买时装
            msg.BuyList.AddRange(buyList);
            msg.PutOn = putOn;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_BuyAvatarPart>(
                SoyHttpApiPath.BuyAvatarPart, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingBuyAvatarPart = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "BuyAvatarPart", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingBuyAvatarPart = false;
                },
                form
            );
        }

        public static bool IsRequstingRaffle {
            get { return _isRequstingRaffle; }
        }
        private static bool _isRequstingRaffle = false;
        /// <summary>
		/// 转盘抽奖
		/// </summary>
		/// <param name="id">抽奖券Id</param>
        public static void Raffle (
            long id,
            Action<Msg_SC_CMD_Raffle> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingRaffle) {
                return;
            }
            _isRequstingRaffle = true;
            Msg_CS_CMD_Raffle msg = new Msg_CS_CMD_Raffle();
            // 转盘抽奖
            msg.Id = id;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_Raffle>(
                SoyHttpApiPath.Raffle, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingRaffle = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "Raffle", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingRaffle = false;
                },
                form
            );
        }

        public static bool IsRequstingReform {
            get { return _isRequstingReform; }
        }
        private static bool _isRequstingReform = false;
        /// <summary>
		/// 改造
		/// </summary>
		/// <param name="flag">占位</param>
        public static void Reform (
            int flag,
            Action<Msg_SC_CMD_Reform> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingReform) {
                return;
            }
            _isRequstingReform = true;
            Msg_CS_CMD_Reform msg = new Msg_CS_CMD_Reform();
            // 改造
            msg.Flag = flag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_Reform>(
                SoyHttpApiPath.Reform, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingReform = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "Reform", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingReform = false;
                },
                form
            );
        }

        public static bool IsRequstingReselectReformLevel {
            get { return _isRequstingReselectReformLevel; }
        }
        private static bool _isRequstingReselectReformLevel = false;
        /// <summary>
		/// 随机改造关卡
		/// </summary>
		/// <param name="curReformSection">当前改造关卡所属章节</param>
		/// <param name="curReformLevel">当前改造关卡所属关卡</param>
        public static void ReselectReformLevel (
            int curReformSection,
            int curReformLevel,
            Action<Msg_SC_CMD_ReselectReformLevel> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingReselectReformLevel) {
                return;
            }
            _isRequstingReselectReformLevel = true;
            Msg_CS_CMD_ReselectReformLevel msg = new Msg_CS_CMD_ReselectReformLevel();
            // 随机改造关卡
            msg.CurReformSection = curReformSection;
            msg.CurReformLevel = curReformLevel;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ReselectReformLevel>(
                SoyHttpApiPath.ReselectReformLevel, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingReselectReformLevel = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ReselectReformLevel", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingReselectReformLevel = false;
                },
                form
            );
        }

        public static bool IsRequstingGetReformReward {
            get { return _isRequstingGetReformReward; }
        }
        private static bool _isRequstingGetReformReward = false;
        /// <summary>
		/// 领取改造奖励
		/// </summary>
		/// <param name="rewardLevel">改造奖励级别</param>
        public static void GetReformReward (
            int rewardLevel,
            Action<Msg_SC_CMD_GetReformReward> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingGetReformReward) {
                return;
            }
            _isRequstingGetReformReward = true;
            Msg_CS_CMD_GetReformReward msg = new Msg_CS_CMD_GetReformReward();
            // 领取改造奖励
            msg.RewardLevel = rewardLevel;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_GetReformReward>(
                SoyHttpApiPath.GetReformReward, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingGetReformReward = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "GetReformReward", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingGetReformReward = false;
                },
                form
            );
        }

        public static bool IsRequstingSaveReformProject {
            get { return _isRequstingSaveReformProject; }
        }
        private static bool _isRequstingSaveReformProject = false;
        /// <summary>
		/// 上传改造关卡
		/// </summary>
		/// <param name="projectId">关卡Id</param>
		/// <param name="programVersion"></param>
		/// <param name="resourceVersion"></param>
		/// <param name="passFlag">是否已经通过</param>
		/// <param name="recordUsedTime">过关使用时间</param>
		/// <param name="uploadParam">上传参数</param>
        public static void SaveReformProject (
            long projectId,
            int programVersion,
            int resourceVersion,
            bool passFlag,
            float recordUsedTime,
            Msg_ProjectUploadParam uploadParam,
            Action<Msg_SC_CMD_SaveReformProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingSaveReformProject) {
                return;
            }
            _isRequstingSaveReformProject = true;
            Msg_CS_CMD_SaveReformProject msg = new Msg_CS_CMD_SaveReformProject();
            // 上传改造关卡
            msg.ProjectId = projectId;
            msg.ProgramVersion = programVersion;
            msg.ResourceVersion = resourceVersion;
            msg.PassFlag = passFlag;
            msg.RecordUsedTime = recordUsedTime;
            msg.UploadParam = uploadParam;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_SaveReformProject>(
                SoyHttpApiPath.SaveReformProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingSaveReformProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "SaveReformProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingSaveReformProject = false;
                },
                form
            );
        }

        public static bool IsRequstingPublishReformProject {
            get { return _isRequstingPublishReformProject; }
        }
        private static bool _isRequstingPublishReformProject = false;
        /// <summary>
		/// 发布改造关卡
		/// </summary>
		/// <param name="personalProjectId"></param>
		/// <param name="programVersion"></param>
		/// <param name="resourceVersion"></param>
		/// <param name="recordUsedTime"></param>
		/// <param name="uploadParam">上传参数</param>
        public static void PublishReformProject (
            long personalProjectId,
            int programVersion,
            int resourceVersion,
            float recordUsedTime,
            Msg_ProjectUploadParam uploadParam,
            Action<Msg_SC_CMD_PublishReformProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingPublishReformProject) {
                return;
            }
            _isRequstingPublishReformProject = true;
            Msg_CS_CMD_PublishReformProject msg = new Msg_CS_CMD_PublishReformProject();
            // 发布改造关卡
            msg.PersonalProjectId = personalProjectId;
            msg.ProgramVersion = programVersion;
            msg.ResourceVersion = resourceVersion;
            msg.RecordUsedTime = recordUsedTime;
            msg.UploadParam = uploadParam;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_PublishReformProject>(
                SoyHttpApiPath.PublishReformProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingPublishReformProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "PublishReformProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingPublishReformProject = false;
                },
                form
            );
        }

        public static bool IsRequstingGetMatchChallengeProject {
            get { return _isRequstingGetMatchChallengeProject; }
        }
        private static bool _isRequstingGetMatchChallengeProject = false;
        /// <summary>
		/// 获取挑战关卡
		/// </summary>
		/// <param name="flag">占位符</param>
        public static void GetMatchChallengeProject (
            int flag,
            Action<Msg_SC_CMD_GetMatchChallengeProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingGetMatchChallengeProject) {
                return;
            }
            _isRequstingGetMatchChallengeProject = true;
            Msg_CS_CMD_GetMatchChallengeProject msg = new Msg_CS_CMD_GetMatchChallengeProject();
            // 获取挑战关卡
            msg.Flag = flag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_GetMatchChallengeProject>(
                SoyHttpApiPath.GetMatchChallengeProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingGetMatchChallengeProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "GetMatchChallengeProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingGetMatchChallengeProject = false;
                },
                form
            );
        }

        public static bool IsRequstingSelectMatchChallengeProject {
            get { return _isRequstingSelectMatchChallengeProject; }
        }
        private static bool _isRequstingSelectMatchChallengeProject = false;
        /// <summary>
		/// 选取挑战关卡
		/// </summary>
		/// <param name="challengeType">选择类型</param>
		/// <param name="change">花钱切换</param>
        public static void SelectMatchChallengeProject (
            EChallengeProjectType challengeType,
            bool change,
            Action<Msg_SC_CMD_SelectMatchChallengeProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingSelectMatchChallengeProject) {
                return;
            }
            _isRequstingSelectMatchChallengeProject = true;
            Msg_CS_CMD_SelectMatchChallengeProject msg = new Msg_CS_CMD_SelectMatchChallengeProject();
            // 选取挑战关卡
            msg.ChallengeType = challengeType;
            msg.Change = change;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_SelectMatchChallengeProject>(
                SoyHttpApiPath.SelectMatchChallengeProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingSelectMatchChallengeProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "SelectMatchChallengeProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingSelectMatchChallengeProject = false;
                },
                form
            );
        }

        public static bool IsRequstingPlayMatchChallengeLevel {
            get { return _isRequstingPlayMatchChallengeLevel; }
        }
        private static bool _isRequstingPlayMatchChallengeLevel = false;
        /// <summary>
		/// 开始挑战
		/// </summary>
		/// <param name="projectId">关卡Id</param>
        public static void PlayMatchChallengeLevel (
            long projectId,
            Action<Msg_SC_CMD_PlayMatchChallengeLevel> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingPlayMatchChallengeLevel) {
                return;
            }
            _isRequstingPlayMatchChallengeLevel = true;
            Msg_CS_CMD_PlayMatchChallengeLevel msg = new Msg_CS_CMD_PlayMatchChallengeLevel();
            // 开始挑战
            msg.ProjectId = projectId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_PlayMatchChallengeLevel>(
                SoyHttpApiPath.PlayMatchChallengeLevel, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingPlayMatchChallengeLevel = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "PlayMatchChallengeLevel", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingPlayMatchChallengeLevel = false;
                },
                form
            );
        }

        public static bool IsRequstingCommitMatchChallengeLevelResult {
            get { return _isRequstingCommitMatchChallengeLevelResult; }
        }
        private static bool _isRequstingCommitMatchChallengeLevelResult = false;
        /// <summary>
		/// 提交匹配挑战关卡数据
		/// </summary>
		/// <param name="token">令牌</param>
		/// <param name="success">是否过关</param>
		/// <param name="usedTime">使用的时间</param>
        public static void CommitMatchChallengeLevelResult (
            long token,
            bool success,
            float usedTime,
            Action<Msg_SC_CMD_CommitMatchChallengeLevelResult> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingCommitMatchChallengeLevelResult) {
                return;
            }
            _isRequstingCommitMatchChallengeLevelResult = true;
            Msg_CS_CMD_CommitMatchChallengeLevelResult msg = new Msg_CS_CMD_CommitMatchChallengeLevelResult();
            // 提交匹配挑战关卡数据
            msg.Token = token;
            msg.Success = success;
            msg.UsedTime = usedTime;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_CommitMatchChallengeLevelResult>(
                SoyHttpApiPath.CommitMatchChallengeLevelResult, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingCommitMatchChallengeLevelResult = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "CommitMatchChallengeLevelResult", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingCommitMatchChallengeLevelResult = false;
                },
                form
            );
        }

        public static bool IsRequstingMatchSkipChallenge {
            get { return _isRequstingMatchSkipChallenge; }
        }
        private static bool _isRequstingMatchSkipChallenge = false;
        /// <summary>
		/// 跳过本次挑战
		/// </summary>
		/// <param name="flag">占位</param>
        public static void MatchSkipChallenge (
            int flag,
            Action<Msg_SC_CMD_MatchSkipChallenge> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingMatchSkipChallenge) {
                return;
            }
            _isRequstingMatchSkipChallenge = true;
            Msg_CS_CMD_MatchSkipChallenge msg = new Msg_CS_CMD_MatchSkipChallenge();
            // 跳过本次挑战
            msg.Flag = flag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_MatchSkipChallenge>(
                SoyHttpApiPath.MatchSkipChallenge, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingMatchSkipChallenge = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "MatchSkipChallenge", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingMatchSkipChallenge = false;
                },
                form
            );
        }

        public static bool IsRequstingExecuteCommand {
            get { return _isRequstingExecuteCommand; }
        }
        private static bool _isRequstingExecuteCommand = false;
        /// <summary>
		/// 执行GM指令
		/// </summary>
		/// <param name="userId">用户</param>
		/// <param name="command"></param>
        public static void ExecuteCommand (
            long userId,
            string command,
            Action<Msg_SC_CMD_ExecuteCommand> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingExecuteCommand) {
                return;
            }
            _isRequstingExecuteCommand = true;
            Msg_CS_CMD_ExecuteCommand msg = new Msg_CS_CMD_ExecuteCommand();
            // 执行GM指令
            msg.UserId = userId;
            msg.Command = command;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ExecuteCommand>(
                SoyHttpApiPath.ExecuteCommand, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingExecuteCommand = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ExecuteCommand", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingExecuteCommand = false;
                },
                form
            );
        }

    }
}