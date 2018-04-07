/********************************************************************
** Filename : UICtrlSingleMode
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
	public class UMViewlLevel : UMViewResManagedBase
    {
		public GameObject Active;
		public GameObject Disactive;
        public Button Current;
		public Button LevelBtn;
		public RectTransform[] StarLightAry;
        public RectTransform[] StarDarkAry;
		public Text LevelTitle;
        public Text StartText;
        public Text StartText2;
	    public Image IslandImage;
	    public Image LightImage;
	    public Texture HeadDefaltTexture;
	    public RawImage[] FriendHeadImgs;
	    public Button[] FriendHeadButtonBtns;
    }
}
