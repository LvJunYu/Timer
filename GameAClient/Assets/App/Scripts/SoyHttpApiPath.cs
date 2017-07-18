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
        // 登录
        public const string Login = "/account/login";
        // 游客登录
        public const string LoginAsGuest = "/account/loginAsGuest";
        // 设备登录包
        public const string LoginByToken = "/account/loginByToken";
        // 忘记密码
        public const string ForgetPassword = "/account/forgetPassword";
        // 修改密码
        public const string ChangePassword = "/account/changePassword";
        // 注册
        public const string Register = "/account/register";
        // 绑定账号
        public const string AccountBind = "/account/bind";
        // 请求验证码
        public const string GetVerificationCode = "/account/getVerificationCode";
        // 退出登录
        public const string Logout = "/account/logout";
        // 用户简要信息
        public const string UserInfoSimple = "/user/getSimple";
        // 用户详细信息
        public const string UserInfoDetail = "/user/get";
        // 获取等级数据
        public const string UserLevel = "/user/getLevelData";
        // 用户详细信息
        public const string UpdateUserInfo = "/user/updateInfo";
        // 获取应用全局信息
        public const string AppData = "/appInfo/appData";
        // 社交关系统计
        public const string UserRelationStatistic = "/relation/getUserRelationStatistic";
        // 用户与我的关系
        public const string UserRelationWithMe = "/relation/getUserRelationWithMe";
        // 更新关注状态
        public const string UpdateFollowState = "/relation/updateFollowState";
        // 更新关注状态
        public const string UpdateBlockState = "/relation/updateBlockState";
        // 获取社交用户列表
        public const string RelationUserList = "/relation/getUserList";
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
        public const string PublishWorldProject = "/world/publishProject";
        // 取消发布
        public const string UnpublishWorldProject = "/world/unpublishProject";
        // 用户发布的关卡
        public const string UserPublishedWorldProjectList = "/world/getUserPublishedProjectList";
        // Msg_CS_CMD_PlayWorldProject
        public const string PlayWorldProject = "/world/playProject";
        // 提交过关世界关卡数据
        public const string CommitWorldProjectResult = "/world/commitProjectResult";
        // 获取世界关卡评论列表
        public const string WorldProjectCommentList = "/world/getProjectCommentList";
        // 提交世界关卡评论
        public const string PostWorldProjectComment = "/world/postProjectComment";
        // 修改关卡顶踩
        public const string UpdateWorldProjectLike = "/world/updateProjectLike";
        // 修改关卡收藏状态
        public const string UpdateWorldProjectFavorite = "/world/updateProjectFavorite";
        // 删除收藏关卡
        public const string DeleteWorldProjectFavorite = "/world/deleteProjectFavorite";
        // 用户收藏的关卡
        public const string UserFavoriteWorldProjectList = "/world/getUserFavoriteProjectList";
        // 最近玩过
        public const string UserWorldProjectPlayHistoryList = "/world/getUserProjectPlayHistoryList";
        // 最近录像
        public const string WorldProjectRecentRecordList = "/world/getProjectRecentRecordList";
        // 高分排行榜
        public const string WorldProjectRecordRankList = "/world/getProjectRecordRankList";
        // 最新关卡
        public const string WorldNewestProjectList = "/world/getNewestProjectList";
        // 录像
        public const string Record = "/record/get";
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
        // Msg_CS_DAT_UserWorkshopUnitData
        public const string UserWorkshopUnitData = "/home/getUserWorkshopUnitData";
        // 冒险模式关卡列表
        public const string AdventureProjectList = "/adventure/getProjectList";
        // 冒险模式进度
        public const string AdventureUserProgress = "/adventure/getUserProgress";
        // 获取冒险关卡用户数据
        public const string AdventureUserLevelDataDetail = "/adventure/getUserLevelDataDetail";
        // 请求单人模式关卡排行
        public const string AdventureLevelRankList = "/adventure/getLevelRankList";
        // 获取冒险章节用户数据
        public const string AdventureUserSection = "/adventure/getUserSectionData";
        // 获取体力数据
        public const string UserEnergy = "/adventure/getUserEnergyData";
        // 购买体力
        public const string BuyEnergy = "/adventure/buyEnergy";
        // 冒险模式用户数据
        public const string AdventureUserData = "/adventure/getUserData";
        // 进入冒险关卡
        public const string PlayAdventureLevel = "/adventure/playLevel";
        // 解锁章节
        public const string UnlockAdventureSection = "/adventure/unlockSection";
        // 提交冒险模式数据
        public const string CommitAdventureLevelResult = "/adventure/commitLevelResult";
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
        // 发送邮件
        public const string SendMail = "/mail/send";
        // 查询未读邮件数量
        public const string MailStatistic = "/mail/statistic";
        // 查询邮件
        public const string MailList = "/mail/list";
        // 标记已读
        public const string MarkMailRead = "/mail/markRead";
        // 领取附件
        public const string ReceiptMailAttach = "/mail/receiptAttach";
        // 领取附件
        public const string DeleteMail = "/mail/delete";
        // 执行GM指令
        public const string ExecuteCommand = "/gm/executeCommand";
    }
}