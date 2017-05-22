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
		public Text Title;

		public UMViewAudioItem MusicItem;
		public UMViewAudioItem SoundsEffects;
		public UMViewAudioItem ShowRunTimeShadowItem;
		public UMViewAudioItem ShowEditShadowItem;

		public Button ButtonExit;
		public Text ButtonExitText;

		public Button ButtonReturnToGame;
		public Text ButtonReturnToGameText;

		public Button ButtonRestart;
		public Text ButtonRestartText;

		public UIRectTransformStatus UIStateController;

		public Button ButtonClose;

	}
}