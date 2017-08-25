//  | 奖励条目

using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public partial class RewardItem
    {
        private const string _goldName = "金币";
        private const string _diamondName = "钻石";
        private const string _playerExpName = "冒险经验";
        private const string _creatorExpName = "工匠经验";
        private const string _fashionCouponName = "时装券";
        private const string _defaultRaffleTicketName = "低级抽奖券";
        private const string _randomUnitName = "随机改造地块";

        private const string _goldSprite = "icon_gold_1";
        private const string _diamondSprite = "icon_diam_1";
        private const string _playerExpSprite = "icon_exp_240";
        private const string _creatorExpSprite = "icon_exp_240";
        private const string _fashionCouponSprite = "icon_one_240";
        private const string _raffleTicketSprite1 = "icon_one_240";
        private const string _raffleTicketSprite2 = "icon_two_240";
        private const string _raffleTicketSprite3 = "icon_three_240";
        private const string _raffleTicketSprite4 = "icon_worker_240";
        private const string _randomUnitSprite = "icon_gift_1_240";


        public Sprite GetSprite () {
            switch ((ERewardType)_type) {
            case ERewardType.RT_Gold: {
                    return ResourcesManager.Instance.GetSprite (_goldSprite);
                }
            case ERewardType.RT_Diamond: {
                    return ResourcesManager.Instance.GetSprite (_diamondSprite);
                }
            case ERewardType.RT_PlayerExp: {
                    return ResourcesManager.Instance.GetSprite (_playerExpSprite);
                }
            case ERewardType.RT_CreatorExp: {
                    return ResourcesManager.Instance.GetSprite (_creatorExpSprite);
                }
            case ERewardType.RT_FashionCoupon: {
                    return ResourcesManager.Instance.GetSprite (_fashionCouponSprite);
                }
            case ERewardType.RT_RaffleTicket: {
                    switch (Id) {
                        case 1:
                        return ResourcesManager.Instance.GetSprite (_raffleTicketSprite1);
                        case 2:
                        return ResourcesManager.Instance.GetSprite (_raffleTicketSprite2);
                        case 3:
                        return ResourcesManager.Instance.GetSprite (_raffleTicketSprite3);
                        case 4:
                        return ResourcesManager.Instance.GetSprite (_raffleTicketSprite4);
                    }
                    return ResourcesManager.Instance.GetSprite (_raffleTicketSprite1);
                }
            case ERewardType.RT_BoostItem:
                {
                    var table = TableManager.Instance.GetBoostItem ((int)_id);
                    if (null == table)
                        return null;
                    return ResourcesManager.Instance.GetSprite (table.Icon);
                }
            case ERewardType.RT_ReformUnit:
                {
                    var table = TableManager.Instance.GetUnit ((int)_id);
                    if (null == table)
                        return null;
                    return ResourcesManager.Instance.GetSprite (table.Icon);
                }
            case ERewardType.RT_RandomReformUnit:
                {
                    return ResourcesManager.Instance.GetSprite (_randomUnitSprite);
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
                    var table = TableManager.Instance.GetTurntable ((int)Id);
                    if (null == table) {
                        // todo error handle
                        return _defaultRaffleTicketName;
                    }
                    return table.Name;
                }
            case ERewardType.RT_BoostItem:
                {
                    var table = TableManager.Instance.GetBoostItem ((int)_id);
                    if (null == table)
                        return string.Empty;
                    return table.Name;
                }
            case ERewardType.RT_ReformUnit:
                {
                    var table = TableManager.Instance.GetUnit ((int)_id);
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

        /// <summary>
        /// 将奖励数据临时实现到本地数据
        /// </summary>
        public void AddToLocal ()
        {
            switch ((ERewardType)_type)
            {
            case ERewardType.RT_Gold:
                GameATools.LocalAddGold((int)_count);
                break;
            case ERewardType.RT_Diamond:
                GameATools.LocalAddDiamond((int)_count);
                break;
            case ERewardType.RT_PlayerExp:
                GameATools.LocalAddPlayerExp((int)_count);
                break;
            case ERewardType.RT_CreatorExp:
                GameATools.LocalAddCreatorExp((int)_count);
                break;
            case ERewardType.RT_FashionCoupon:
                break;
            case ERewardType.RT_RaffleTicket:
                LocalUser.Instance.RaffleTicket.AddTicketsInRaffleDictionary((int)_id, (int) _count);
                // todo 这里不要引用界面，应该发消息通知界面更新
                LocalUser.Instance.RaffleTicket.Request(LocalUser.Instance.UserGuid, () => {
                    SocialGUIManager.Instance.GetUI<UICtrlLottery>().RefreshRaffleCount();
                }, code => {
                    LogHelper.Error("Network error when get RaffleCount, {0}", code);
                });
                break;
            case ERewardType.RT_BoostItem:
                var allBoostItem = LocalUser.Instance.UserProp.ItemDataList;
                bool found = false;
                for (int i = 0; i < allBoostItem.Count; i++) {
                    if (allBoostItem [i].Type == Id) {
                        PropItem item = allBoostItem [i];
                        item.Count += (int)_count;
                        allBoostItem [i] = item;
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    PropItem newItem = new PropItem ();
                    newItem.Type = (int)_id;
                    newItem.Count = (int)_count;
                    allBoostItem.Add (newItem);
                }
                LocalUser.Instance.UserProp.ItemDataList = allBoostItem;
                LocalUser.Instance.LoadPropData(null, null);
                break;
            case ERewardType.RT_RandomReformUnit:
                LocalUser.Instance.MatchUserData.Request (LocalUser.Instance.UserGuid, null, null);
                break;
            case ERewardType.RT_ReformUnit:
                LocalUser.Instance.MatchUserData.Request (LocalUser.Instance.UserGuid, null, null);
                break;
            }
        }
    }
}