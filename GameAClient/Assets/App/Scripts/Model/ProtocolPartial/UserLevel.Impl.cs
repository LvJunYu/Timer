// 取等级数据 | 获取等级数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserLevel : SyncronisticData {
        protected override void OnSyncPartial ()
        {
            base.OnSyncPartial ();
            if (LocalUser.Instance.UserGuid == CS_UserId) {
                Messenger.Broadcast (EMessengerType.OnGoldChanged);
                Messenger.Broadcast (EMessengerType.OnDiamondChanged);
            }
        }
    }
}