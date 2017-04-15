/********************************************************************
** Filename : HomePartData.cs
** Author : quan
** Date : 3/27/2017 4:28 PM
** Summary : HomePartData.cs
***********************************************************************/

using System;
using SoyEngine.Proto;
using SoyEngine;
using System.Collections.Generic;

namespace GameA
{
	public partial class HomePartData : SyncronisticData {
//        private long _userId;
        private Dictionary<EHomePart, Item> _homePartDict = new Dictionary<EHomePart, Item>();
        public HomePartData(long userId)
        {
            _userId = userId;
        }

        public void LoadData(Action successCallback, Action<ENetResultCode> failedCallback)
        {
            Msg_CS_DAT_HomePartData msg = new Msg_CS_DAT_HomePartData();
            msg.UserId = _userId;
			NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_HomePartData>(SoyHttpApiPath.HomePartData, msg, ret =>
            {
                Set(ret);
                if (null != successCallback)
                {
                    successCallback.Invoke();
                }
            }, (errorCode, errorMsg) =>
            {
                if (null != failedCallback)
                {
                    failedCallback.Invoke(errorCode);
                }
            });
        }

        public void Set(Msg_SC_DAT_HomePartData msg)
        {
            for (int i = 0; i < msg.ItemDataList.Count; i++)
            {
                Msg_HomePartItem msgItem = msg.ItemDataList[i];
                Item item;
                if (!_homePartDict.TryGetValue((EHomePart)msgItem.Type, out item))
                {
                    item = new Item((EHomePart) msgItem.Type);
                    _homePartDict.Add(item.HomePartType, item);
                }
                item.Set(msgItem);
            }
        }

        public class Item
        {
            private EHomePart _homePartType;
            private long _id;
            private EHomePartLockState _state;
            private int _level;

            public EHomePart HomePartType
            {
                get
                {
                    return _homePartType;
                }
            }

            public long Id
            {
                get
                {
                    return _id;
                }
            }

            public EHomePartLockState State
            {
                get
                {
                    return _state;
                }
            }

            public int Level
            {
                get
                {
                    return _level;
                }
            }

            public Item(EHomePart homePartType)
            {
                _homePartType = homePartType;
            }

            public void Set(Msg_HomePartItem msg)
            {
                _id = msg.Id;
                _state = (EHomePartLockState)msg.State;
                _level = msg.Level;
            }
        }
    }
}
