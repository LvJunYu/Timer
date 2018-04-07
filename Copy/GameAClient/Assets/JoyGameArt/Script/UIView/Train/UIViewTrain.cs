using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewTrain : UIViewResManagedBase
    {
        public RectTransform PanelRtf;
        public RectTransform MaskRtf;
        public Button CloseBtn;
        public RectTransform PropertyListRTF;
        public RectTransform InfoListRTF;
        public Text OwnedTrainPointTxt;
        public GameObject IsTraining;
        public Text TrainingTxt;
        public Text ValueDescTxt;
        public Text RemainTimeTxt;
        public Text FinishCostTxt;
        public Slider TrainingSlider;
        public Button FinishImmediatelyBtn;
        public Button UpgradeGradeBtn;
        public Material MapMaterial;
        public Transform MapImg;
        public GameObject[] GradeImgs;
        public Transform[] MapOutPoints;
        public Transform[] MapInPoints;
        public GameObject[] Animations;
    }
}