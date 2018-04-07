using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewQQHall: UIViewResManagedBase
    {
       
        public Button CloseButton;
        public Button ColltionButtonNewPlayer;
        public Button ColltionEveryDayPlayer;
        public Button[] MenuButtonAry;
        public Button[] MenuSelectedButtonAry;
        public GameObject[] AwardPanel;
        public UITabGroup TabGroup;
        public GridDataScroller GridDataScroller;
        public RectTransform NewPlayerRect;
        public RectTransform EveryDayRect;

    }
}