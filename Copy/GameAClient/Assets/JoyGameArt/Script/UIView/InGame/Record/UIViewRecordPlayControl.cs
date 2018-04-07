using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewRecordPlayControl : UIViewResManagedBase
    {
        public Button GlobalButton;
        public GameObject ContentDock;
        public Button BackButton;
        public Text TitleText;
        public Text CurrentTimeText;
        public Image ProgressFgMaskImage;
        public Text TotalTimeText;
        public Button PlayButton;
        public Button PauseButton;
        public Button DecreaseSpeedButton;
        public Text SpeedText;
        public Button IncreaseSpeedButton;
    }
}