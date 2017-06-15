// 抽奖券 | 抽奖券
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;
using GameA.Game;

namespace GameA
{
    public partial class UserRaffleTicket : SyncronisticData
    {
        //private Dictionary<ERaffleTicketType, RaffleItem> _RaffleTicketDict = new Dictionary<ERaffleTicketType, RaffleItem>();
        private Dictionary<long, int> _RaffleTicketDict = new Dictionary<long, int>();
        //private int _rewardType = 0;
        private Reward _reward =new Reward();
        //public void LoadData(Action successCallback, Action<ENetResultCode> failedCallback)
        //{
        //    Msg_CS_DAT_UserRaffleTicket msg = new Msg_CS_DAT_UserRaffleTicket();
        //    msg.UserId = _userId;
        //    NetworkManager.AppHttpClient.SendWithCb<Msg_CS_DAT_UserRaffleTicket>(SoyHttpApiPath.UserRaffleTicket, msg, ret =>
        //    {
        //        //Set(ret);
        //        if (null != successCallback)
        //        {
        //            successCallback.Invoke();
        //        }
        //    }, (errorCode, errorMsg) =>
        //    {
        //        if (null != failedCallback)
        //        {
        //            failedCallback.Invoke(errorCode);
        //        }
        //    });
        //}
        public Reward GetReward
        {
            set { _reward = value; }
            get { return _reward; }

        }

        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();
            _RaffleTicketDict.Clear();
            for (int i = 0; i < _itemDataList.Count; i++)
            {

                _RaffleTicketDict.Add(_itemDataList[i].Id, _itemDataList[i].Count);


                //_RaffleTicketDict.Add(RaffleItem.ERaffleTicketType, RaffleItem);
            }
        }

        public int GetCountInRaffleDictionary(int id)
        {

            if (_RaffleTicketDict.ContainsKey(id))
            {
                return (int)_RaffleTicketDict[ id];
            }
            else
            {
                return -1;
            }

        }

        public void  AddTicketsInRaffleDictionary(int id,int number)
        {

            if (_RaffleTicketDict.ContainsKey(id))
            {
                 _RaffleTicketDict[id]+= number;
            }


        }



        public void UseRaffleTicket(long selectedTicketNum, Action<long> successCallback, Action<ENetResultCode> failedCallback)
        {
            this._RaffleTicketDict[selectedTicketNum]--;
            RemoteCommands.Raffle(
                selectedTicketNum,
                (re) =>
               
                
                {
                    ERaffleCode resultCode = (ERaffleCode)re.ResultCode;
                    List<Msg_RewardItem> RewardList = re.Reward.ItemList;
                    
                    if (resultCode == ERaffleCode.RC_Success)
                    {
                        //SuccessfullyUseRaffleTicket(re.RewardId, (int)selectedTicketNum);
                        foreach (var item in RewardList)
                        {
                            GiveReward(item);
                        }
                        successCallback(ReturnRewardOnPanel(re.RewardId, (int)selectedTicketNum));
                        _reward.OnSync(re.Reward);
                    }
                    else
                    {
                        UnsuccessfullyUseRaffleTicket();
                    }
                },
                code => {
                    LogHelper.Error("Network error when use Raffle, {0}", code);
                    UnsuccessfullyUseRaffleTicket();
                    this._RaffleTicketDict[selectedTicketNum]++;
                });
        }

        //         internal void UseRaffleTicket(int selectedTicketNum, Action<long> rotateThePanel, object p)
        //         {
        //             throw new NotImplementedException();
        //         }

        private long ReturnRewardOnPanel(long currentRewardId, int selectedTicketNum)
        {
            long rewardType = 0;
            var raffleUnit = TableManager.Instance.Table_TurntableDic[selectedTicketNum];
            if (currentRewardId == raffleUnit.Reward1)
            {
              rewardType = 1;
            }
            else if (currentRewardId == raffleUnit.Reward2)
            {
                rewardType = 2;
            }
            else if (currentRewardId == raffleUnit.Reward3)
            {
                rewardType = 3;
            }
            else if (currentRewardId == raffleUnit.Reward4)
            {
                rewardType = 4;
            }
            else if (currentRewardId == raffleUnit.Reward5)

            {
                rewardType = 5;

            }
            else if (currentRewardId == raffleUnit.Reward6)
            {
                rewardType = 6;
            }
            else if (currentRewardId == raffleUnit.Reward7)
            {
                rewardType = 7;
            }
            else if (currentRewardId == raffleUnit.Reward8)
            {
                rewardType = 8;
            }
            return rewardType;
        }

        //private void GetReward(Msg_Reward reward)
        //{

        //    _reward = reward;
        //}

        private void GiveReward(Msg_RewardItem rewardItem)
        {
            //var raffleUnit =TableManager.Instance.Table_RewardDic[(int)currentRewardId];
            switch (rewardItem.Type)
            {
              
        //public enum ERewardType
        //{
        //    RT_None = 0,
        //    RT_Gold = 1,
        //    RT_Diamond = 2,
        //    RT_PlayerExp = 3,
        //    RT_CreatorExp = 4,
        //    RT_FashionCoupon = 5,
        //    RT_RaffleTicket = 6,
        //    RT_BoostItem = 7,
        //    RT_RandomReformUnit = 8   ,
        //    RT_ReformUnit = 9
        //}
                case 0:
                    break;
                case 1:
                    GameATools.LocalAddGold((int)rewardItem.Count);
                    break;
                case 2:
                    GameATools.LocalAddDiamond((int)rewardItem.Count);
                    break;
                case 3:
                    GameATools.LocalAddPlayerExp((int)rewardItem.Count);
                    break;
                case 4:
                    GameATools.LocalAddCreatorExp((int)rewardItem.Count);
                    break;
                case 5:
                    break;
                case 6:
                    AddTicketsInRaffleDictionary((int)rewardItem.Id, (int) rewardItem.Count);
                    LocalUser.Instance.RaffleTicket.Request(LocalUser.Instance.UserGuid, () => {
                        SocialGUIManager.Instance.GetUI<UICtrlLottery>().RefreshRaffleCount();

                    }, code => {
                        LogHelper.Error("Network error when get RaffleCount, {0}", code);
                    });
                    break;
                case 7:
                    MatchUnitData();
                    break;
                case 8:
                    MatchUnitData();
                    break;
                case 9:
                    MatchUnitData();
                    break;
            }
        }

        private void MatchUnitData()
        {
            Msg_CS_DAT_MatchUserData msg = new Msg_CS_DAT_MatchUserData();
             LocalUser.Instance.MatchUserData.Request(LocalUser.Instance.UserGuid, () => {
             }, code => {
                 LogHelper.Error("Network error when refresh MatchUnitData, {0}", code);
             });

        }

        //private void RewardMoney(int moneyAmount)
        //{
        //    LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin += moneyAmount;
        //}
        //private void RewardDiamond(int diamondAmount)
        //{
        //    LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond += diamondAmount;
        //}
        //private void RewardPlayerExp(int playerExpAmount)
        //{
        //    LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerExp += playerExpAmount;
        //}
        //private void RewardCreatorExp(int creatorExpAmount)
        //{
        //    LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorExp += creatorExpAmount;
        //}
        private void UnsuccessfullyUseRaffleTicket()
        {

        }



        //public void Set(Msg_SC_DAT_UserRaffleTicket msg)
        //{
        //    for (int i = 0; i < msg.ItemDataList.Count; i++)
        //    {
        //        Msg_RaffleTicketItem msgItem = msg.ItemDataList[i];
        //        RaffleItem item;
        //        if (!_RaffleTicketDict.TryGetValue((ERaffleTicket)msgItem.Type, out item))
        //        {
        //            item = new RaffleItem((ERaffleTicket)msgItem.Type);
        //            _RaffleTicketDict.Add(item.ERaffleTicketType, item);
        //        }
        //        item.Set(msgItem);
        //    }
        //}
        //    public enum ERaffleTicketType
        //    {
        //        newcomerticket = 1,
        //        primaryticket = 2,
        //        middleticket = 3,
        //        advancedticket = 4,
        //        masterticket = 5,

        //    };

        //    public class RaffleItem
        //    {
        //        private ERaffleTicketType _EraffleTicketType;
        //        private long _id;
        //        private int _count;

        //        public ERaffleTicketType ERaffleTicketType
        //        {
        //            get
        //            {
        //                return _EraffleTicketType;
        //            }
        //        }

        //        public long Id
        //        {
        //            get
        //            {
        //                return _id;
        //            }
        //        }


        //        public int Count
        //        {
        //            get
        //            {
        //                return _count;
        //            }
        //        }

        //        public RaffleItem(ERaffleTicketType RaffleTicketType)
        //        {
        //            _EraffleTicketType = ERaffleTicketType;
        //        }

        //        public void Set(Msg_RaffleTicketItem msg)
        //        {
        //            _id = msg.Id;
        //            _EraffleTicketType = msg.Type;
        //            _count = msg.Count;
        //        }

    }
}
