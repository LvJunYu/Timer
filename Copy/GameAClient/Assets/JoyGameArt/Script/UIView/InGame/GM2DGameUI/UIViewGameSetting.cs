/********************************************************************
** Filename : UIViewGameSetting  
** Author : ake
** Date : 6/8/2016 7:21:27 PM
** Summary : UIViewGameSetting  
***********************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewGameSetting : UIViewResManagedBase
    {
        public GameObject MobilePanel;
        public USViewGameSettingItem PlayBackGroundMusic;
        public USViewGameSettingItem PlaySoundsEffects;
        public USViewGameSettingItem ShowShadow;
        public USViewGameSettingItem ShowRoute;
        public GameObject BtnGroup1;
        public GameObject BtnGroup2;
        public Text NickName;
        public Button ReturnBtn;
        public RawImage UserHeadAvatar;
        public Texture DefaultUserHeadTexture;
        public Button RestartBtn;
        public Button ExitBtn;
        public Button LoginOut;
        public Button ChangePwdBtn;
        public Button BindingBtn;
        public GameObject PCPanel;
        public USViewGameSettingItem PlayBackGroundMusic_2;
        public USViewGameSettingItem PlaySoundsEffects_2;
        public USViewGameSettingItem ShowShadow_2;
        public USViewGameSettingItem ShowRoute_2;
        public GameObject BtnGroup1_2;
        public GameObject BtnGroup2_2;
        public Button ReturnBtn_2;
        public Button RestartBtn_2;
        public Button ExitBtn_2;
        public Button RestoreDefaultBtn;
        public Button OKBtn;
        public Button RestoreDefaultBtn_2;
        public Button OKBtn_2;
        public Toggle WindowScreenToggle;
        public Toggle FullScreenToggle;
        public Dropdown ResolutionDropdown;

        public USViewInputKeySetting[] UsInputKeyViews;
        public Button PCLogoutBtn;

        public USViewSliderSetting UsBGMusicSetting;
        public USViewSliderSetting UsMusicEffectSetting;
    }
}