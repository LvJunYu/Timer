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
        private int _rewardType = 0;

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

        public int GetCountInRaffleDictionary(int _id)
        {

            if (_RaffleTicketDict.ContainsKey(_id))
            {
                return (int)_RaffleTicketDict[_id];
            }
            else
            {
                return -1;
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
                    if (resultCode == ERaffleCode.RC_Success)
                    {
                        SuccessfullyUseRaffleTicket(re.RewardId, (int)selectedTicketNum);
                        successCallback(this._rewardType);
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

        private void SuccessfullyUseRaffleTicket(long currentRewardId, int selectedTicketNum)
        {
            var raffleUnit = TableManager.Instance.Table_TurntableDic[selectedTicketNum];
            if (currentRewardId == raffleUnit.Reward1)
            {
                this._rewardType = 1;
            }
            else if (currentRewardId == raffleUnit.Reward2)
            {
                this._rewardType = 2;
            }
            else if (currentRewardId == raffleUnit.Reward3)
            {
                this._rewardType = 3;
            }
            else if (currentRewardId == raffleUnit.Reward4)
            {
                this._rewardType = 4;
            }
            else if (currentRewardId == raffleUnit.Reward5)

            {
                this._rewardType = 5;

            }
            else if (currentRewardId == raffleUnit.Reward6)
            {
                this._rewardType = 6;
            }
            else if (currentRewardId == raffleUnit.Reward7)
            {
                this._rewardType = 7;
            }
            else if (currentRewardId == raffleUnit.Reward8)
            {
                this._rewardType = 8;
            }
            return;
        }

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
