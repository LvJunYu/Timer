using UnityEngine;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewTrain : UIViewBase
    {
        public Button CloseBtn;
        public RectTransform PropertyListRTF;
        public RectTransform InfoListRTF;
        public Text OwnedTrainPointTxt;
        public GameObject[] GradeImgs;
        public GameObject IsTraining;
        public Text TrainingTxt;
        public Text ValueDescTxt;
        public Text RemainTimeTxt;
        public Text FinishCostTxt;
        public Slider TrainingSlider;
        public Button FinishImmediatelyBtn;
        public Material MapMaterial;
        public Transform[] MapOutPoints;
        public Transform[] MapInPoints;
    }
}
