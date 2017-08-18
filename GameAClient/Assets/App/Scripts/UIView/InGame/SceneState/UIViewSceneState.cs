/********************************************************************
** Filename : UIViewSceneState  
** Author : ake
** Date : 4/28/2016 6:05:26 PM
** Summary : UIViewSceneState  
***********************************************************************/

using UnityEngine;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewSceneState:UIViewBase
    {
        public GameObject LevelInfoDock;
        public GameObject SpaceDock;
        public Text SectionText;
        public GameObject NormalLevelDock;
        public Text NormalLevelText;
        public GameObject BonusLevelDock;
        public Text BonusLevelText;
        public UIViewSceneStateItem HpItem;
        public UIViewSceneStateItem CollectionItem;
        public UIViewSceneStateItem EnemyItem;
        public UIViewSceneStateItem ArriveItem;
        public UIViewSceneStateItem TimeLimitItem;
        public UIViewSceneStateItem KeyItem;
        public UIViewSceneStateItem ScoreItem;

        public Image HP;
        public Image MP;

        public Image SpeedUpCDImg;
        public Text SpeedUpCDText;
        public GameObject SpeedUpReady;

        public GameObject StarConditions;
        public Text[] StarConditionText;
        public Image[] StarConditionStar;


        // 临时引导界面
        public GameObject HelpPage;
	}
}