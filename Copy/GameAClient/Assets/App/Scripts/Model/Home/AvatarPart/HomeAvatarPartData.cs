/********************************************************************
** Filename : HomeAvatarPartData.cs
** Author : quan
** Date : 3/27/2017 5:16 PM
** Summary : HomeAvatarPartData.cs
***********************************************************************/

using System;
using SoyEngine.Proto;
using SoyEngine;
using System.Collections.Generic;

namespace GameA
{
    public class HomeAvatarPartData
    {
        private long _userId;
		private bool _inited = false;
//        private Dictionary<string, Item> _avatarPartDict = new Dictionary<string, Item>();
		private Dictionary<EAvatarPart, Dictionary<long, Item>> _avatarPartDic = new Dictionary<EAvatarPart, Dictionary<long, Item>>(); 
        private Dictionary<EAvatarPart, Item> _avatarPartUsingDict = new Dictionary<EAvatarPart, Item>();

        public long UserId
        {
            get { return _userId; }
        }

		public bool Inited {
			get {
				return _inited;
			}
		}

		public int HeadPartId {
			get {
				if (_avatarPartUsingDict.ContainsKey (EAvatarPart.AP_Head)) {
					return (int)_avatarPartUsingDict [EAvatarPart.AP_Head].Id;
				} else {
					return -1;
				}
			}
		}
		public int UpperPartId {
			get {
				if (_avatarPartUsingDict.ContainsKey (EAvatarPart.AP_Upper)) {
					return (int)_avatarPartUsingDict [EAvatarPart.AP_Upper].Id;
				} else {
					return -1;
				}
			}
		}
		public int LowerPartId {
			get {
				if (_avatarPartUsingDict.ContainsKey (EAvatarPart.AP_Lower)) {
					return (int)_avatarPartUsingDict [EAvatarPart.AP_Lower].Id;
				} else {
					return -1;
				}
			}
		}
		public int AppendagePartId {
			get {
				if (_avatarPartUsingDict.ContainsKey (EAvatarPart.AP_Appendage)) {
					return (int)_avatarPartUsingDict [EAvatarPart.AP_Appendage].Id;
				} else {
					return -1;
				}
			}
		}

        public HomeAvatarPartData(long userId)
        {
            _userId = userId;
			_avatarPartDic.Add (EAvatarPart.AP_Head, new Dictionary<long, Item>());
			_avatarPartDic.Add (EAvatarPart.AP_Upper, new Dictionary<long, Item>());
			_avatarPartDic.Add (EAvatarPart.AP_Lower, new Dictionary<long, Item>());
			_avatarPartDic.Add (EAvatarPart.AP_Appendage, new Dictionary<long, Item>());
        }

		public void LoadUsingData(Action successAction,
                              Action<ENetResultCode> failedAction)
        {
//            LoadData(EUsingState.US_Using, EExpirationState.ES_Unexpired, ret =>
//            {
//                _avatarPartUsingDict.Clear();
//                for (int i = 0; i < ret.ItemDataList.Count; i++)
//                {
//                    Msg_AvatarPartItem msgItem = ret.ItemDataList[i];
//                    Item item = new Item((EAvatarPart)msgItem.Type, msgItem.Id);
//                    if (!_avatarPartUsingDict.ContainsKey(item.AvatarPartType))
//                    {
//                        _avatarPartUsingDict.Add(item.AvatarPartType, item);
//                    }
//                    else
//                    {
//                        LogHelper.Error("Using type duplication");
//                    }
//					Dictionary<long, Item> targetDic;
//					if (_avatarPartDic.TryGetValue(item.AvatarPartType, out targetDic)) {
//						targetDic[item.Id] = item;
//					} else {
//						LogHelper.Error("Unrecognized avatar type when loadUsingData {0}", item);
//					}
//                }
//					_inited = true;
//                if (successAction != null)
//                {
//                    successAction.Invoke();
//                }
//            }, failedAction);
        }

		public void LoadUnexpiredData(Action successAction,
                              Action<ENetResultCode> failedAction)
        {
//            LoadData(EUsingState.US_All, EExpirationState.ES_Unexpired, ret =>
//            {
//				_avatarPartUsingDict.Clear();
//				_avatarPartDic[EAvatarPart.AP_Head].Clear();
//				_avatarPartDic[EAvatarPart.AP_Upper].Clear();
//				_avatarPartDic[EAvatarPart.AP_Lower].Clear();
//				_avatarPartDic[EAvatarPart.AP_Appendage].Clear();
//                for (int i = 0; i < ret.ItemDataList.Count; i++)
//                {
//                    Msg_AvatarPartItem msgItem = ret.ItemDataList[i];
//                    Item item = new Item((EAvatarPart)msgItem.Type, msgItem.Id);
//					Dictionary<long, Item> targetDic;
//					if (_avatarPartDic.TryGetValue(item.AvatarPartType, out targetDic)) {
//						targetDic[item.Id] = item;
//					} else {
//						LogHelper.Error("Unrecognized avatar type when LoadUnexpiredData {0}", item);
//					}
//					if (item.Using) {
//						_avatarPartUsingDict[item.AvatarPartType] = item;
//					}
//                }
//					_inited = true;
//                if (successAction != null)
//                {
//                    successAction.Invoke();
//                }
//            }, failedAction);
        }

//        private void LoadData(EUsingState usingState,
//                              EExpirationState expirationState,
//                              Action<Msg_SC_DAT_AvatarPart> successAction,
//                              Action<ENetResultCode> failedAction)
//        {
//            Msg_CS_DAT_AvatarPart msg = new Msg_CS_DAT_AvatarPart();
//            msg.UserId = _userId;
//            msg.UsingState = usingState;
//            msg.ExpirationState = expirationState;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_AvatarPart>(
//				SoyHttpApiPath.AvatarPart,
//                msg,
//                successAction,
//                (errCode, errMsg) => {
//                    if (failedAction != null)
//                    {
//                        failedAction.Invoke(errCode);
//                }
//            });
//        }

		public void SendChangeAvatarPart (EAvatarPart type, long newId,
			Action successAction,
			Action<ENetResultCode> failedAction) {
			if (_inited == false) return;
			Msg_CS_CMD_ChangeAvatarPart msg = new Msg_CS_CMD_ChangeAvatarPart ();
			msg.Type = type;
			msg.NewId = newId;
			NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ChangeAvatarPart> (
				SoyHttpApiPath.ChangeAvatarPart,
				msg,
				(retMsg) => {
					if (retMsg.ResultCode == (int)EChangeAvatarPartCode.CAPC_Success) {
						Item newEquipedItem;
						if (!_avatarPartDic[type].TryGetValue(newId, out newEquipedItem)) {
							LogHelper.Error("Can't find equiped avatar part {0}", type);
						} else {
							Item oldEquipedItem;
							if (_avatarPartUsingDict.TryGetValue(type, out oldEquipedItem)) {
								oldEquipedItem.Using = false;
							}
							newEquipedItem.Using = true;
							if (successAction != null) {
								successAction();
							}
						}
					} else {
						if (failedAction != null)
						{
							failedAction.Invoke(ENetResultCode.NR_None);
						}
					}
				},
				(errCode, errMsg) => {
					if (failedAction != null)
					{
						failedAction.Invoke(errCode);
					}
				}
			);
		}

        public class Item
        {
//            private string _key;
            private EAvatarPart _avatarPartType;
            private long _id;
            private bool _using;
            private long _expirationTime;

//            public string Key
//            {
//                get
//                {
//                    return _key;
//                }
//            }
//
            public EAvatarPart AvatarPartType
            {
                get
                {
                    return _avatarPartType;
                }
            }

            public long Id
            {
                get
                {
                    return _id;
                }
            }

            public bool Using
            {
                get
                {
                    return _using;
                }
				set {
					_using = value;
				}
            }

            public long ExpirationTime
            {
                get
                {
                    return _expirationTime;
                }
            }

            public Item(EAvatarPart avatarPartType, long id)
            {
                _avatarPartType = avatarPartType;
                _id = id;
//                _key = string.Format("{0}_{1}", (int)avatarPartType, id);
            }

            public void Set(Msg_AvatarPartItem msg)
            {
                _using = msg.Using;
                _expirationTime = msg.ExpirationTime;
            }

			public override string ToString ()
        	{
        		return string.Format ("[Item: _avatarPartType={0}, _id={1}, _using={2}, _expirationTime={3}]", _avatarPartType, _id, _using, _expirationTime);
        	}
        	
        }
    }
}
