//  | 奖励条目
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class RewardItem : SyncronisticData
    {
        private const string _goldName = "金币";
        private const string _diamondName = "钻石";
        private const string _playerExpName = "冒险经验";
        private const string _creatorExpName = "工匠经验";
        private const string _fashionCouponName = "时装券";
        private const string _raffleTicketName = "奖券";
        public UnityEngine.Sprite GetSprite () {
            return null;
        }

        public string GetName () {
            switch ((ERewardType)_type) {
            case ERewardType.RT_Gold: {
                    return _goldName;
                }
            case ERewardType.RT_Diamond: {
                    return _diamondName;
                }
            case ERewardType.RT_PlayerExp: {
                    return _playerExpName;
                }
            case ERewardType.RT_CreatorExp: {
                    return _creatorExpName;
                }
            case ERewardType.RT_FashionCoupon: {
                    return _fashionCouponName;
                }
            case ERewardType.RT_RaffleTicket: {
                    return _raffleTicketName;
                }
            }
            return string.Empty;
        }
    }
}