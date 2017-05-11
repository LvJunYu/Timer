/********************************************************************
** Filename : SoyHttpApiPath
** Author : quansiwei
** Date : 16/3/21 下午9:39:21
** Summary : SoyHttpApiPath
***********************************************************************/

using System;
using System.Collections.Generic;

namespace SoyEngine
{
    public static class SoyHttpApiPath
    {
        // CS协议包例子
        public const string DataExample = "test/testAPI";
        // 获取应用全局信息
        public const string AppData = "/appInfo/appData";
        // 设备登录包
        public const string LoginByToken = "/account/loginByToken";
        // 退出登录
        public const string Logout = "/account/logout";
        // 用户简要信息
        public const string UserInfoSimple = "/user/getSimple";
        // 用户详细信息
        public const string UserInfoDetail = "/user/get";
        // 获取等级数据
        public const string UserLevel = "/user/getLevelData";
        // 社交关系统计
        public const string UserRelationStatistic = "/relation/getUserRelationStatistic";
        // 用户与我的关系
        public const string UserRelationWithMe = "/relation/getUserRelationWithMe";
        // 获取关卡数据
        public const string Project = "/project/get";
        // 关卡扩展信息
        public const string ProjectExtend = "/project/getExtend";
        // 创建关卡
        public const string CreateProject = "/project/create";
        // 更新关卡
        public const string UpdateProject = "/project/update";
        // 删除关卡
        public const string DeleteProject = "/project/delete";
        // 工坊关卡
        public const string PersonalProjectList = "/project/getPersonalList";
        // 发布关卡
        public const string PublishProject = "/project/publish";
        // 取消发布
        public const string UnpublishProject = "/project/unpublish";
        // 用户发布的关卡
        public const string UserPublishedProjectList = "/project/getUserPublishedList";
        // 工坊关卡
        public const string Record = "/record/get";
        // 冒险模式关卡列表
        public const string AdventureProjectList = "/adventure/getProjectList";
        // 冒险模式进度
        public const string AdventureUserProgress = "/adventure/getUserProgress";
        // 获取冒险关卡用户数据
        public const string AdventureUserLevelDataDetail = "/adventure/getUserLevelDataDetail";
        // 获取冒险章节用户数据
        public const string AdventureUserSection = "/adventure/getUserSectionData";
        // 获取体力数据
        public const string UserEnergy = "/adventure/getUserEnergyData";
        // 冒险模式用户数据
        public const string AdventureUserData = "/adventure/getUserData";
        // 进入冒险关卡
        public const string PlayAdventureLevel = "/adventure/playLevel";
        // 解锁章节
        public const string UnlockAdventureSection = "/adventure/unlockSection";
        // 提交冒险模式数据
        public const string CommitAdventureLevelResult = "/adventure/commitLevelResult";
        // 用户道具数据
        public const string UserProp = "/home/getUserPropData";
        // 使用道具
        public const string UseProps = "/home/useProps";
        // 家园装饰
        public const string HomePartData = "/home/getUserPartData";
        // 解锁装饰
        public const string UnlockHomePart = "/home/unlockHomePart";
        // 角色正在使用时装数据
        public const string UsingAvatarPart = "/home/getUserUsingAvatarPartData";
        // 角色可以使用的时装数据
        public const string ValidAvatarPart = "/home/getUserValidAvatarPartData";
        // 角色换装
        public const string ChangeAvatarPart = "/home/changeAvatarPart";
        // 时装打折券
        public const string UserAvatarPartDiscountCoupon = "/home/getUserAvatarPartDiscountCouponData";
        // 抽奖券
        public const string UserRaffleTicket = "/home/getUserRaffleTicketData";
        // 购买时装
        public const string BuyAvatarPart = "/home/buyAvatarPart";
        // 转盘抽奖
        public const string Raffle = "/home/raffle";
        // 用户匹配改造数据
        public const string MatchUserData = "/match/getUserData";
        // 改造
        public const string Reform = "/match/reform";
        // 随机改造关卡
        public const string ReselectReformLevel = "/match/reselectReformLevel";
        // 领取改造奖励
        public const string GetReformReward = "/match/getReformReward";
        // 上传改造关卡
        public const string SaveReformProject = "/match/saveReformProject";
        // 发布改造关卡
        public const string PublishReformProject = "/match/publishReformProject";
        // 获取挑战关卡
        public const string GetMatchChallengeProject = "/match/getChallengeProject";
        // 选取挑战关卡
        public const string SelectMatchChallengeProject = "/match/selectChallengeProject";
        // 开始挑战
        public const string PlayMatchChallengeLevel = "/match/playChallengeLevel";
        // 提交匹配挑战关卡数据
        public const string CommitMatchChallengeLevelResult = "/match/commitChallengeLevelResult";
        // 跳过本次挑战
        public const string MatchSkipChallenge = "/match/skipChallenge";
        // 执行GM指令
        public const string ExecuteCommand = "/gm/executeCommand";
    }
}