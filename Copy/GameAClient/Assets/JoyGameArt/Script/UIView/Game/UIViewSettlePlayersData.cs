/********************************************************************
** Filename : UIViewGameFinish  
** Author : ake
** Date : 4/29/2016 8:06:30 PM
** Summary : UIViewGameFinish  
***********************************************************************/


using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewSettlePlayersData : UIViewResManagedBase
    {
        public Button ExitBtn;
        public Button ReplayBtn;
        public RectTransform WinContentTrans;
        public RectTransform LoseContentTrans;
        public Text TileText;
        public Image WinTileImage;
        public Image FailTileImage;
        public GameObject DataPanel;
        public GameObject AniPanel;
        public Image MoveLight;
        public Image LeftLight;
        public Image RightLight;
        public RawImage[] PlayGroup;
        public Text[] PlayNameGroup;
        public Image MvpImage;
        public HorizontalLayoutGroup PlayersLayoutGroup;
        public ContentSizeFitter PlayersContentSizeFitter;
        public RawImage[] LightsImage;
        public Image AllLightImage;
        public GameObject[] CoorepationObj;
        public GameObject[] BattleObj;
        public GameObject CoorepationWinObj;
        public GameObject CoorepationFailObj;
        public SkeletonAnimation[] PlayerAvatarAnimation;
    }
}