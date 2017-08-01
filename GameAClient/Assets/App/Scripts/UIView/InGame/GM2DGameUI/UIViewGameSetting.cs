/********************************************************************
** Filename : UIViewGameSetting  
** Author : ake
** Date : 6/8/2016 7:21:27 PM
** Summary : UIViewGameSetting  
***********************************************************************/


using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
	public class UIViewGameSetting:UIViewBase
	{
//		public Text Title;

//		public UMViewAudioItem MusicItem;
//		public UMViewAudioItem SoundsEffects;
//		public UMViewAudioItem ShowRunTimeShadowItem;
//		public UMViewAudioItem ShowEditShadowItem;

//		public Button ButtonExit;
//		public Text ButtonExitText;
        public USViewGameSettingItem ShowShadow;
        public USViewGameSettingItem ShowRoute;
	    public GameObject BtnGroup1;
	    public GameObject BtnGroup2;
        public Text NickName;
        public Button ReturnBtn;
        public RawImage UserHeadAvatar;
        /// <summary>
        /// Ä¬ÈÏÍ¼Æ¬
        /// </summary>
		public Texture DefaultUserHeadTexture;
        //		public Text ButtonReturnToGameText;

        public Button RestartBtn;
//		public Text ButtonRestartText;

//		public UIRectTransformStatus UIStateController;

		public Button ExitBtn;

        public Button LoginOut;
        public Button ChangePwd;



    }
}