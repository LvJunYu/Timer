using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewMatrixDetail : UIViewBase
    {
        public Button EditButtonRightResource;
        public Button CancelButtonRightResource;

        public Image TopCover;
        public Text NameText;
        public Text Summary;
        public Button CreateBtn;
        public Button CreateXiuXianBtn;
        public Button CreateJieMiBtn;
        public Button CreateJiXianBtn;
        public Button CloseCategoryMaskBtn;
        public Button CloseCategoryMaskBtnBigger;
		/// <summary>
		/// 覆蓋界面
		/// </summary>
        public GameObject CategaryMask;
        public Image CreateBtnSprite;
        public UISoyPersonalProjectList SoyPersonalProjectList;
        public GameObject UnloginDock;
        public Button LoginBtn;
        public Button RegisterBtn;

		public Button ReturnBtn;
    }
}
