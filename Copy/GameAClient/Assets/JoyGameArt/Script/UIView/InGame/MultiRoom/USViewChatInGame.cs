using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class USViewChatInGame : USViewBase
    {
        public Toggle CampTog;
        public Toggle RoomTog;
        public Button ChatTypeBtn;
        public Button SendBtn;
        public InputField ChatInput;
        public ScrollRect ChatScrollRect;
        public RectTransform ChatContentDock;
        public RectTransform PoolDock;
        
        public RectTransform RawChatContentDock;
        public RectTransform RawChatPannel;
    }
}