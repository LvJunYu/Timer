using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewChatInGame : UIViewResManagedBase
    {
        public EventTriggerListener OpenBtn;
        public EventTriggerListener OpenPannelBtn;
        public Button CloseBtn;
        public USViewChatInGame InGameChat;
        public Toggle QuickChatTog;
        public Toggle ChatHistoryTog;
        public GameObject QuickChatPannel;
        public GameObject ChatHistoryPannel;
        public RectTransform OpenPannel;
        public Button AddNewPreinstallChatBtn;
        public InputField NewPreinstallInputField;
        public GameObject AddNewPannel;
        public RectTransform PannelRtf;
    }
}