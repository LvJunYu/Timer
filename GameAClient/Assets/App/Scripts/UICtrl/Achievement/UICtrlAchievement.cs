using System;
using SoyEngine;
using System.Collections.Generic;

namespace GameA
{
    /// <summary>
    /// 成就页面
    /// </summary>
    [UIAutoSetup]
    public class UICtrlAchievement : UICtrlAnimationBase<UIViewAchievement>
    {

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpUI;
        }
    }

    public class AchievemrntRecord
    {
        //a、App使用相关
        //打开过游戏的天数总和
        //连续登陆天数
        //App打开时长总和
        
        //b、养成积累相关
        //获得的金币总和
        //冒险家等级
        //匠人等级
        //获得的拼图碎片数量总和
        //拼好的拼图数量
        //装备过的时装种类总数
        
        //c、社交相关
        //关注的好友总数
        //粉丝总数
        //访问好友家园的次数
        //观看录像的次数
        
        //d、游戏行为相关
        //单人模式通关的最高关卡
        //单人模式获得星数总和
        //单人模式通关成功次数
        //单人模式通关失败次数
        //世界关卡通关成功次数
        //世界关卡通关失败次数
        //挑战改造关卡成功的次数
        //发布改造关卡中新增地块的总和
        
        //e、创作相关
        //发布自制关卡的次数
        //自制关卡被玩的总次数
        //自制关卡被玩但闯关失败的总次数
        //自制关卡收到的点赞数总和
        
        //f、游戏内记录相关
        //关卡中走过的总路程
        //杀死的怪物总数
        //拾取的钻石总数
        //死掉总次数
        //坠亡次数
        //被机关杀死次数
        //被怪物杀死次数
        //顶碎的砖块总数
        //踩掉的云朵总数
        //顶出的隐形方块总数
        //拾取地块／机关的总数
        //经过传送门的次数
    }
}