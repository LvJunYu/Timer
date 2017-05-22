/********************************************************************
** Filename : UIViewPublish
** Author : quan
** Date : 2015/7/2 16:30:24
** Summary : UIViewPublish
***********************************************************************/

using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewPublish : UIViewBase
    {
        public Text TitleText;
        public InputField NameInputField;
        public InputField DesInputField;
        public RawImage CoverImage;
        public Button CoverBtn;

        public Text DownloadPriceText;
        public Button DownloadPriceBtn;

        public Text PublishCountText;

        public Button SaveBtn;
        public Button PublishBtn;
        public Button CancleBtn;

	    public Text NameHeadingText;
	    public Text DesHeadingText;

	    public Text SaveBtnText;
	    public Text PublishBtnText;

	    public Button ButtonTagArder;
	    public Text ButtonTagArderText;
		public Button ButtonTagRiddle;
		public Text ButtonTagRiddleText;
		public Button ButtonTagHighpoint;
		public Text ButtonTagHighpointText;
		public Button ButtonUploadRecord;
		public Text ButtonUploadRecordText;
	    public GameObject UploadSelectedFlag;
    }
}