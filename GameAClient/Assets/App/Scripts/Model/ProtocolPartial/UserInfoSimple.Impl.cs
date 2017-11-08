using SoyEngine;

namespace GameA
{
    public partial class UserInfoSimple
    {
        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();
            Messenger.Broadcast(EMessengerType.OnUserInfoChanged);
        }
    }
}