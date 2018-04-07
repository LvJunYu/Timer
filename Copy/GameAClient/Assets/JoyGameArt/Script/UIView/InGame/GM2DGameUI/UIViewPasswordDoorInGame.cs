using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewPasswordDoorInGame : UIViewResManagedBase
    {
        public RectTransform UpRtf;
        public RectTransform DownRtf;
        public RectTransform PannelRtf;
        public GameObject NormalLightObj;
        public GameObject WrongLightObj;
        public GameObject CorrectLightObj;
        public Button CloseBtn;
        public AnimationCurve ShakeCure;
    }
}