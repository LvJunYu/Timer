using SoyEngine;

namespace GameA
{
    public partial class UserLevel
    {
        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();
            if (LocalUser.Instance.UserGuid == CS_UserId)
            {
                Messenger.Broadcast(EMessengerType.OnGoldChanged);
                Messenger.Broadcast(EMessengerType.OnDiamondChanged);
            }
        }
    }
}