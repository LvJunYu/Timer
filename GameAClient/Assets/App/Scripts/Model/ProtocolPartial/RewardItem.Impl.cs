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
        private const string _defaultRaffleTicketName = "低级抽奖券";
        private const string _randomUnitName = "随机改造地块";

        private const string _goldSprite = "icon_gold_240";
        private const string _diamondSprite = "icon_diam_240";
        private const string _playerExpSprite = "icon_one_240";
        private const string _creatorExpSprite = "icon_one_240";
        private const string _fashionCouponSprite = "icon_one_240";
        private const string _raffleTicketSprite1 = "icon_one_240";
        private const string _raffleTicketSprite2 = "icon_two_240";
        private const string _raffleTicketSprite3 = "icon_three_240";
        private const string _raffleTicketSprite4 = "icon_worker_240";
        private const string _randomUnitSprite = "icon_gift_1_240";


        public UnityEngine.Sprite GetSprite () {
            if (GameResourceManager.Instance == null) return null;
            switch ((ERewardType)_type) {
            case ERewardType.RT_Gold: {
                    return GameResourceManager.Instance.GetSpriteByName (_goldSprite);
                }
            case ERewardType.RT_Diamond: {
                    return GameResourceManager.Instance.GetSpriteByName (_diamondSprite);
                }
            case ERewardType.RT_PlayerExp: {
                    return GameResourceManager.Instance.GetSpriteByName (_playerExpSprite);
                }
            case ERewardType.RT_CreatorExp: {
                    return GameResourceManager.Instance.GetSpriteByName (_creatorExpSprite);
                }
            case ERewardType.RT_FashionCoupon: {
                    return GameResourceManager.Instance.GetSpriteByName (_fashionCouponSprite);
                }
            case ERewardType.RT_RaffleTicket: {
                    switch (Id) {
                        case 1:
                            return GameResourceManager.Instance.GetSpriteByName (_raffleTicketSprite1);
                        case 2:
                            return GameResourceManager.Instance.GetSpriteByName (_raffleTicketSprite2);
                        case 3:
                            return GameResourceManager.Instance.GetSpriteByName (_raffleTicketSprite3);
                        case 4:
                            return GameResourceManager.Instance.GetSpriteByName (_raffleTicketSprite4);
                    }
                    return GameResourceManager.Instance.GetSpriteByName (_raffleTicketSprite1);
                }
            case ERewardType.RT_BoostItem:
                {
                    var table = Game.TableManager.Instance.GetBoostItem ((int)_id);
                    if (null == table)
                        return null;
                    return GameResourceManager.Instance.GetSpriteByName (table.Icon);
                }
            case ERewardType.RT_ReformUnit:
                {
                    var table = Game.TableManager.Instance.GetUnit ((int)_id);
                    if (null == table)
                        return null;
                    return GameResourceManager.Instance.GetSpriteByName (table.Icon);
                }
            case ERewardType.RT_RandomReformUnit:
                {
                    return GameResourceManager.Instance.GetSpriteByName (_randomUnitSprite);
                }
            }
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
                    var table = Game.TableManager.Instance.GetTurntable ((int)Id);
                    if (null == table) {
                        // todo error handle
                        return _defaultRaffleTicketName;
                    }
                    return table.Name;
                }
            case ERewardType.RT_BoostItem:
                {
                    var table = Game.TableManager.Instance.GetBoostItem ((int)_id);
                    if (null == table)
                        return string.Empty;
                    return table.Name;
                }
            case ERewardType.RT_ReformUnit:
                {
                    var table = Game.TableManager.Instance.GetUnit ((int)_id);
                    if (null == table)
                        return null;
                    return table.Name;
                }
            case ERewardType.RT_RandomReformUnit:
                {
                    return _randomUnitName;
                }
            }
            return string.Empty;
        }
    }
}