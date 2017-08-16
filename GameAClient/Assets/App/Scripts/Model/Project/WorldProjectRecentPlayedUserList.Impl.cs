using System;
using SoyEngine.Proto;

namespace GameA
{
    public partial class WorldProjectRecentPlayedUserList
    {
        public void Request(Action successCallback, Action<ENetResultCode> failCallback)
        {
            Request(_cs_projectId, 0, 5, successCallback, failCallback);
        }
    }
}