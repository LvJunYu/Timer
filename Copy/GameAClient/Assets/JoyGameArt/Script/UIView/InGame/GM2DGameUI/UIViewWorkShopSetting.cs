using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewWorkShopSetting : UIViewResManagedBase
    {
        public GameObject MobilePannel;
        public GameObject SettingPannel;
        public GameObject WinConditionPannel;
        public Button CloseBtn;
        public Button SureBtn;
        public Button ExitBtn;
        public Toggle Toggle01;
        public Toggle Toggle02;
        public Toggle[] Togs;
        public USViewGameSettingItem PlayBackGroundMusic;
        public USViewGameSettingItem PlaySoundsEffects;
        public Button TestBtn;
        public Button PublishBtn;
        public GameObject PublishBtnDisable;
        public GameObject TestBtnFinished;
        public InputField TitleInputField;
        public InputField DescInputField;
        public UMViewGamePlayItem[] ItemArray;
        public Text LifeShowText;
        public UMViewGameRatingBarItem[] LifeItemArray;

        public GameObject PCPannel;
        public GameObject CommonSettingPanel;
        public GameObject LevelSettingPannel;
        public Button RestoreDefaultBtn;
        public Text SureBtn_2Txt;
        public Text SureBtn_3Txt;
        public Button SureBtn_2;
        public Button ExitBtn_2;
        public USViewGameSettingItem PlayBackGroundMusic_2;
        public USViewGameSettingItem PlaySoundsEffects_2;
        public Button TestBtn_2;
        public Button PublishBtn_2;
        public GameObject PublishBtnDisable_2;
        public GameObject TestBtnFinished_2;
        public InputField TitleInputField_2;
        public InputField DescInputField_2;
        public UMViewGamePlayItem[] ItemArray_2;
        public Text LifeShowText_2;
        public UMViewGameRatingBarItem[] LifeItemArray_2;
        public USViewInputKeySetting[] UsInputKeyViews;
        public Button SureBtn_3;
        public Button ExitBtn_3;
        public Toggle WindowScreenToggle;
        public Toggle FullScreenToggle;
        public Dropdown ResolutionDropdown;
        public GameObject BtnDock1;
        public GameObject BtnDock2;

        public GameObject NetBattleBasicPannel;
        public GameObject NetBattleWinConditionPannel;
        public GameObject NetBattlePlayerSettingPannel;
        public InputField TitleInputField_3;
        public InputField DescInputField_3;
        public Toggle InfiniteLifeTog;
        public USViewSliderSetting LifeCountSetting;
        public USViewSliderSetting ReviveTimeSetting;
        public USViewSliderSetting ReviveProtectTimeSetting;
        public USViewSliderSetting MinPlayerCountSetting;
        public Toggle[] ReviveTypeTogs;
        public Toggle[] HarmTypeTogs;

        public USViewSliderSetting TimeLimitSetting;
        public USViewSliderSetting ArriveScoreSetting;
        public USViewSliderSetting CollectGemScoreSetting;
        public USViewSliderSetting KillMonsterScoreSetting;
        public USViewSliderSetting KillPlayerScoreSetting;
        public USViewSliderSetting WinScoreSetting;
        public USViewGameSettingItem ScoreConditionSetting;
        public Toggle[] WinConditionTogs;

        public USViewSliderSetting MaxHpSetting;
        public USViewSliderSetting JumpAbilitySetting;
        public USViewSliderSetting MoveSpeedSetting;
        public USViewSliderSetting InjuredReduceSetting;
        public USViewSliderSetting CureIncreaseSetting;


        public USViewSliderSetting UsBGMusicSetting_2;
        public USViewSliderSetting UsMusicEffectSetting_2;

//        public USViewSliderSetting UsBGMusicSetting;
//        public USViewSliderSetting UsMusicEffectSetting;
    }
}