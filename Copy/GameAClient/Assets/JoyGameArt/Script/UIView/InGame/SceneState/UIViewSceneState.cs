/********************************************************************
** Filename : UIViewSceneState  
** Author : ake
** Date : 4/28/2016 6:05:26 PM
** Summary : UIViewSceneState  
***********************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewSceneState : UIViewResManagedBase

    {
        public GameObject LevelInfoDock;
        public Text SectionText;
        public GameObject NormalLevelDock;
        public Text NormalLevelText;
        public GameObject BonusLevelDock;
        public Text BonusLevelText;
        public GameObject LifeRoot;
        public Text LifeText;
        public GameObject CollectionRoot;
        public Text CollectionText;
        public RectTransform CollectionIconRtf;
        public GameObject EnemyRoot;
        public Text EnemyText;
        public RectTransform LeftTimeRoot;
        public Text LeftTimeText;
        public RectTransform CollectionLifeIconRtf;
        public GameObject KeyRoot;
        public Text KeyText;
        public GameObject ScoreRoot;
        public Text ScoreText;
        public GameObject EditModeSpace;
        public GameObject ConditionsRoot;
        public RectTransform ConditionsItemRoot;
        public GameObject HelpPage;
        public Text CountDownTxt;

        public Text ReviveTxt;
        public GameObject ReviveDock;
        public GameObject StandaloneObj;
        public GameObject MultiObj;
        public Text TimeLimit;
        public Text TimeOverCondition;
        public Text WinScoreCondition;
        public Text ArriveScore;
        public Text CollectGemScore;
        public Text KillMonsterScore;
        public Text KillPlayerScore;
        public UsViewNpcTask[] TaskGroup;
        public GameObject TaskPanel;

        public Text MutilPlayerDetail;
    }
}