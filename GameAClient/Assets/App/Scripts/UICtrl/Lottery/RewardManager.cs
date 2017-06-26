using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameA.Game;
using Spine;
using Spine.Unity;


namespace GameA
{
    public class RewardManager
    {
        private const string _goldName = "金币";
        private const string _diamondName = "钻石";
        private const string _playerExpName = "冒险经验";
        private const string _creatorExpName = "工匠经验";
        private const string _fashionCouponName = "时装券";
        private const string _defaultRaffleTicketName = "低级抽奖券";

        private const string _goldSprite = "icon_lottery_1";
        private const string _diamondSprite = "icon_lottery_1";
        private const string _playerExpSprite = "icon_lottery_1";
        private const string _creatorExpSprite = "icon_lottery_1";
        private const string _fashionCouponSprite = "icon_lottery_1";
        private const string _raffleTicketSprite1 = "icon_lottery_1";
        private const string _raffleTicketSprite2 = "icon_lottery_1";
        private const string _raffleTicketSprite3 = "icon_lottery_1";
        private const string _raffleTicketSprite4 = "icon_lottery_1";

        public UnityEngine.Sprite GetSprite(int LotteryTicketType, int rewardNum)
        {
            // todo update api
//            if (GameResourceManager.Instance == null) return null;
//            switch ((ERewardType)GetTableRewardFromDic(JudgeRewardKey(LotteryTicketType, rewardNum)).Type1)
//            {
//                case ERewardType.RT_Gold:
//                    {
//                        return GameResourceManager.Instance.GetSpriteByName(_goldSprite);
//                    }
//                case ERewardType.RT_Diamond:
//                    {
//                        return GameResourceManager.Instance.GetSpriteByName(_diamondSprite);
//                    }
//                case ERewardType.RT_PlayerExp:
//                    {
//                        return GameResourceManager.Instance.GetSpriteByName(_playerExpSprite);
//                    }
//                case ERewardType.RT_CreatorExp:
//                    {
//                        return GameResourceManager.Instance.GetSpriteByName(_creatorExpSprite);
//                    }
//                case ERewardType.RT_FashionCoupon:
//                    {
//                        return GameResourceManager.Instance.GetSpriteByName(_fashionCouponSprite);
//                    }
//                case ERewardType.RT_RaffleTicket:
//                    {
//                        switch (GetTableRewardFromDic(JudgeRewardKey(LotteryTicketType, rewardNum)).SubType1)
//                        {
//                            case 1:
//                                return GameResourceManager.Instance.GetSpriteByName(_raffleTicketSprite1);
//                            case 2:
//                                return GameResourceManager.Instance.GetSpriteByName(_raffleTicketSprite2);
//                            case 3:
//                                return GameResourceManager.Instance.GetSpriteByName(_raffleTicketSprite3);
//                            case 4:
//                                return GameResourceManager.Instance.GetSpriteByName(_raffleTicketSprite4);
//                        }
//                        return GameResourceManager.Instance.GetSpriteByName(_raffleTicketSprite1);
//                    }
//            }
            return null;
        }
        private Table_Reward GetTableRewardFromDic(int TurntableUnitReward) //根据key找到对应的奖励
        {
            // var TurntableUnit = TableManager.Instance.Table_TurntableDic[LotteryTicketType];
            return TableManager.Instance.Table_RewardDic[TurntableUnitReward];
        }
        private int JudgeRewardKey(int LotteryTicketType, int rewardNum)//根据抽奖类型和奖励号拿到对应在字典中的key
        {
            var TurntableUnit = TableManager.Instance.Table_TurntableDic[LotteryTicketType];
            switch (rewardNum)
            {
                case 1:
                    {
                        return TurntableUnit.Reward1;
                    }
                case 2:
                    {
                        return TurntableUnit.Reward2;
                    }
                case 3:
                    {
                        return TurntableUnit.Reward3;
                    }
                case 4:
                    {
                        return TurntableUnit.Reward4;
                    }
                case 5:
                    {
                        return TurntableUnit.Reward5;
                    }
                case 6:
                    {
                        return TurntableUnit.Reward6;
                    }
                case 7:
                    {
                        return TurntableUnit.Reward7;
                    }
                case 8:
                    {
                        return TurntableUnit.Reward8;
                    }
                default:
                    return 0;
            }
        }

        public int GetCount(int LotteryTicketType, int rewardNum)
        {
  
            return GetTableRewardFromDic(JudgeRewardKey(LotteryTicketType, rewardNum)).Value1;
        }

        public string GetName(int LotteryTicketType, int rewardNum)
        {
            switch ((ERewardType)GetTableRewardFromDic(JudgeRewardKey(LotteryTicketType, rewardNum)).Type1)
            {
                case ERewardType.RT_Gold:
                    {
                        return _goldName;
                    }
                case ERewardType.RT_Diamond:
                    {
                        return _diamondName;
                    }
                case ERewardType.RT_PlayerExp:
                    {
                        return _playerExpName;
                    }
                case ERewardType.RT_CreatorExp:
                    {
                        return _creatorExpName;
                    }
                case ERewardType.RT_FashionCoupon:
                    {
                        return _fashionCouponName;
                    }
                case ERewardType.RT_RaffleTicket:
                    {
                        var table = Game.TableManager.Instance.GetTurntable((int)LotteryTicketType);
                        if (null == table)
                        {
                            // todo error handle
                            return _defaultRaffleTicketName;
                        }
                        return table.Name;
                    }
            }
            return string.Empty;
        }

    }
}