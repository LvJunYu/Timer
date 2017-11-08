using SoyEngine;

namespace GameA
{
    public partial class UserInfoSimple
    {
        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();
            Messenger<long>.Broadcast(EMessengerType.OnUserInfoChanged, _userId);
        }
    }
}