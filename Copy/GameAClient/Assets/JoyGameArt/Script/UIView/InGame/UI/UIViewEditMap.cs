using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewEditMap : UIViewResManagedBase
    {
        public GameObject SceneDock;
        public RectTransform TopBtnRtf;
        public RectTransform RightBtnRtf;
        public Button CreateSceneBtn;
        public Toggle[] SceneToggles;
        public GameObject[] Heads;
        public Button TopAddBtn;
        public Button TopLessBtn;
        public Button RightAddBtn;
        public Button RightLessBtn;
        public Button TopAddDiableBtn;
        public Button TopLessDiableBtn;
        public Button RightAddDiableBtn;
        public Button RightLessDiableBtn;
    }
}