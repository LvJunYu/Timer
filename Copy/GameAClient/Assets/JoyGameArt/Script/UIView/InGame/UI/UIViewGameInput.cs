using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewGameInput : UIViewResManagedBase
    {
        public GameObject MobilePannel;
        public JoyStickEx JoyStickEx;
        public PushButton AssistBtn;
        public PushButton JumpBtn;
        public Image JumpBtnIcon;
        public USViewSkillBtn[] USViewSkillBtns;

        public GameObject PcPannel;
        public USViewSkillBtn[] USViewSkillBtns_PC;
        public Text AssistInputKeyTxt;
        public GameObject AssistBtn_PC;
    }
}