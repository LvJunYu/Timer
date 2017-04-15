// protocol command msgs

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public class RemoteCommands {
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
            Action<Msg_SC_CMD_LoginByToken> successCallback, Action<ENetResultCode> failedCallback) {

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
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "LoginByToken", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

        /// <summary>
		/// 退出登录
		/// </summary>
		/// <param name="flag">占位</param>
        public static void Logout (
            int flag,
            Action<Msg_SC_CMD_Logout> successCallback, Action<ENetResultCode> failedCallback) {

            Msg_CS_CMD_Logout msg = new Msg_CS_CMD_Logout();
            // 退出登录
            msg.Flag = flag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_Logout>(
                SoyHttpApiPath.Logout, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "Logout", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

        /// <summary>
		/// 创建关卡
		/// </summary>
		/// <param name="name"></param>
		/// <param name="summary"></param>
		/// <param name="programVersion"></param>
		/// <param name="resourceVersion"></param>
		/// <param name="passFlag"></param>
		/// <param name="recordUsedTime"></param>
        public static void CreateProject (
            string name,
            string summary,
            int programVersion,
            int resourceVersion,
            bool passFlag,
            float recordUsedTime,
            Action<Msg_SC_CMD_CreateProject> successCallback, Action<ENetResultCode> failedCallback) {

            Msg_CS_CMD_CreateProject msg = new Msg_CS_CMD_CreateProject();
            // 创建关卡
            msg.Name = name;
            msg.Summary = summary;
            msg.ProgramVersion = programVersion;
            msg.ResourceVersion = resourceVersion;
            msg.PassFlag = passFlag;
            msg.RecordUsedTime = recordUsedTime;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_CreateProject>(
                SoyHttpApiPath.CreateProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "CreateProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

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
        public static void UpdateProject (
            long projectId,
            string name,
            string summary,
            int programVersion,
            int resourceVersion,
            bool passFlag,
            float recordUsedTime,
            Action<Msg_SC_CMD_UpdateProject> successCallback, Action<ENetResultCode> failedCallback) {

            Msg_CS_CMD_UpdateProject msg = new Msg_CS_CMD_UpdateProject();
            // 更新关卡
            msg.ProjectId = projectId;
            msg.Name = name;
            msg.Summary = summary;
            msg.ProgramVersion = programVersion;
            msg.ResourceVersion = resourceVersion;
            msg.PassFlag = passFlag;
            msg.RecordUsedTime = recordUsedTime;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateProject>(
                SoyHttpApiPath.UpdateProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

        /// <summary>
		/// 删除关卡
		/// </summary>
		/// <param name="projectId">关卡Id</param>
        public static void DeleteProject (
            List<long> projectId,
            Action<Msg_SC_CMD_DeleteProject> successCallback, Action<ENetResultCode> failedCallback) {

            Msg_CS_CMD_DeleteProject msg = new Msg_CS_CMD_DeleteProject();
            // 删除关卡
            msg.ProjectId.AddRange(projectId);
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_DeleteProject>(
                SoyHttpApiPath.DeleteProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "DeleteProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

        /// <summary>
		/// 发布关卡
		/// </summary>
		/// <param name="personalProjectId"></param>
		/// <param name="name"></param>
		/// <param name="summary"></param>
		/// <param name="programVersion"></param>
		/// <param name="resourceVersion"></param>
		/// <param name="recordUsedTime"></param>
        public static void PublishProject (
            long personalProjectId,
            string name,
            string summary,
            int programVersion,
            int resourceVersion,
            float recordUsedTime,
            Action<Msg_SC_CMD_PublishProject> successCallback, Action<ENetResultCode> failedCallback) {

            Msg_CS_CMD_PublishProject msg = new Msg_CS_CMD_PublishProject();
            // 发布关卡
            msg.PersonalProjectId = personalProjectId;
            msg.Name = name;
            msg.Summary = summary;
            msg.ProgramVersion = programVersion;
            msg.ResourceVersion = resourceVersion;
            msg.RecordUsedTime = recordUsedTime;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_PublishProject>(
                SoyHttpApiPath.PublishProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "PublishProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

        /// <summary>
		/// 取消发布
		/// </summary>
		/// <param name="projectId">关卡Id</param>
        public static void UnpublishProject (
            List<long> projectId,
            Action<Msg_SC_CMD_UnpublishProject> successCallback, Action<ENetResultCode> failedCallback) {

            Msg_CS_CMD_UnpublishProject msg = new Msg_CS_CMD_UnpublishProject();
            // 取消发布
            msg.ProjectId.AddRange(projectId);
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UnpublishProject>(
                SoyHttpApiPath.UnpublishProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UnpublishProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

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
            Action<Msg_SC_CMD_PlayAdventureLevel> successCallback, Action<ENetResultCode> failedCallback) {

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
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "PlayAdventureLevel", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

        /// <summary>
		/// 解锁章节
		/// </summary>
		/// <param name="section">章节</param>
        public static void UnlockAdventureSection (
            int section,
            Action<Msg_SC_CMD_UnlockAdventureSection> successCallback, Action<ENetResultCode> failedCallback) {

            Msg_CS_CMD_UnlockAdventureSection msg = new Msg_CS_CMD_UnlockAdventureSection();
            // 解锁章节
            msg.Section = section;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UnlockAdventureSection>(
                SoyHttpApiPath.UnlockAdventureSection, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UnlockAdventureSection", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

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
            Action<Msg_SC_CMD_CommitAdventureLevelResult> successCallback, Action<ENetResultCode> failedCallback) {

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
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "CommitAdventureLevelResult", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

        /// <summary>
		/// 使用道具
		/// </summary>
		/// <param name="token">令牌</param>
		/// <param name="itemDataList"></param>
        public static void UseProps (
            long token,
            List<Msg_PropItem> itemDataList,
            Action<Msg_SC_CMD_UseProps> successCallback, Action<ENetResultCode> failedCallback) {

            Msg_CS_CMD_UseProps msg = new Msg_CS_CMD_UseProps();
            // 使用道具
            msg.Token = token;
            msg.ItemDataList.AddRange(itemDataList);
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UseProps>(
                SoyHttpApiPath.UseProps, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UseProps", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

        /// <summary>
		/// 解锁装饰
		/// </summary>
		/// <param name="type">部位</param>
		/// <param name="id"></param>
        public static void UnlockHomePart (
            EHomePart type,
            long id,
            Action<Msg_SC_CMD_UnlockHomePart> successCallback, Action<ENetResultCode> failedCallback) {

            Msg_CS_CMD_UnlockHomePart msg = new Msg_CS_CMD_UnlockHomePart();
            // 解锁装饰
            msg.Type = type;
            msg.Id = id;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UnlockHomePart>(
                SoyHttpApiPath.UnlockHomePart, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UnlockHomePart", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

        /// <summary>
		/// 角色换装
		/// </summary>
		/// <param name="type">部位</param>
		/// <param name="newId"></param>
        public static void ChangeAvatarPart (
            EAvatarPart type,
            long newId,
            Action<Msg_SC_CMD_ChangeAvatarPart> successCallback, Action<ENetResultCode> failedCallback) {

            Msg_CS_CMD_ChangeAvatarPart msg = new Msg_CS_CMD_ChangeAvatarPart();
            // 角色换装
            msg.Type = type;
            msg.NewId = newId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ChangeAvatarPart>(
                SoyHttpApiPath.ChangeAvatarPart, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ChangeAvatarPart", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

        /// <summary>
		/// 购买时装
		/// </summary>
		/// <param name="partType">类型</param>
		/// <param name="partId">部件id</param>
		/// <param name="durationType">购买时长</param>
		/// <param name="currencyType"></param>
		/// <param name="discountCouponId">Msg_AvatarPartDiscountCouponItem 的id</param>
        public static void BuyAvatarPart (
            EAvatarPart partType,
            long partId,
            EBuyAvatarPartDurationType durationType,
            ECurrencyType currencyType,
            long discountCouponId,
            Action<Msg_SC_CMD_BuyAvatarPart> successCallback, Action<ENetResultCode> failedCallback) {

            Msg_CS_CMD_BuyAvatarPart msg = new Msg_CS_CMD_BuyAvatarPart();
            // 购买时装
            msg.PartType = partType;
            msg.PartId = partId;
            msg.DurationType = durationType;
            msg.CurrencyType = currencyType;
            msg.DiscountCouponId = discountCouponId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_BuyAvatarPart>(
                SoyHttpApiPath.BuyAvatarPart, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "BuyAvatarPart", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

        /// <summary>
		/// 转盘抽奖
		/// </summary>
		/// <param name="id">抽奖券Id</param>
        public static void Raffle (
            long id,
            Action<Msg_SC_CMD_Raffle> successCallback, Action<ENetResultCode> failedCallback) {

            Msg_CS_CMD_Raffle msg = new Msg_CS_CMD_Raffle();
            // 转盘抽奖
            msg.Id = id;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_Raffle>(
                SoyHttpApiPath.Raffle, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "Raffle", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

        /// <summary>
		/// 清空用户数据
		/// </summary>
		/// <param name="userId">用户</param>
        public static void ClearUserAll (
            long userId,
            Action<Msg_SC_CMD_ClearUserAll> successCallback, Action<ENetResultCode> failedCallback) {

            Msg_CS_CMD_ClearUserAll msg = new Msg_CS_CMD_ClearUserAll();
            // 清空用户数据
            msg.UserId = userId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ClearUserAll>(
                SoyHttpApiPath.ClearUserAll, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ClearUserAll", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
            });
        }

    }
}