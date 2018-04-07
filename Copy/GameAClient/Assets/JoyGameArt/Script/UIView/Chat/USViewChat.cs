using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class USViewChat : USViewBase
    {
        public Toggle[] ChatTypeTagArray;
        public Button ChatTypeBtn;
        public Button SendContentBtn;
        public InputField ChatInput;
        public ScrollRect ChatScrollRect;
        public RectTransform ChatContentDock;
        public RectTransform PoolDock;
        public RectTransform ChatFriendsBgRtf;
        public RectTransform ChatFriendsGridRtf;
        public RectTransform BtnsDockRtf;
        public RectTransform BtnsGridRtf;
        public Button CloseChatFriendDockBtn;
        public Button CloseBtnsDockBtn;
        public Text ChatFriendTxt;
        public Button ChatFriendBtn;
        public Button CheckInfoBtn;
        public Button PrivateChatBtn;
        public Button FollowBtn;
        public GameObject PrivateChatRedPoint;
    }
}