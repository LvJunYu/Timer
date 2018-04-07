/********************************************************************
** Filename : GameSettingData  
** Author : ake
** Date : 12/14/2016 2:44:55 PM
** Summary : GameSettingData  
***********************************************************************/


using System;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [Serializable]
    public class GameSettingData
    {
        public static readonly GameSettingData Instance = new GameSettingData();

        private int _playMusic = 5;
        private int _playSoundsEffects = 5;
        private bool _showPlayModeShadow = true;
        private bool _showEditModeShadow = true;

        public int PlayMusic
        {
            get { return _playMusic; }
            set
            {
                if (_playMusic != value)
                {
                    _playMusic = Mathf.Clamp(value, 0, 10);
                    Messenger.Broadcast(EMessengerType.OnGameSettingChanged);
                }
            }
        }

        public int PlaySoundsEffects
        {
            get { return _playSoundsEffects; }
            set
            {
                if (_playSoundsEffects != value)
                {
                    _playSoundsEffects = Mathf.Clamp(value, 0, 10);
                    Messenger.Broadcast(EMessengerType.OnGameSettingChanged);
                }
            }
        }

        public bool ShowPlayModeShadow
        {
            get { return _showPlayModeShadow; }
            set { _showPlayModeShadow = value; }
        }

        public bool ShowEditModeShadow
        {
            get { return _showEditModeShadow; }
            set { _showEditModeShadow = value; }
        }

        private const int PlayMusicFlag = 240;
        private const int PlaySoundsEffectsFlag = 3840;
        private const int ShowPlayModeShadowFlag = 1 << 3;
        private const int ShowEidtModeShadowFlag = 1 << 4;

        private const string SaveKey = "GameSettingKey";


        private GameSettingData()
        {
            Load();
        }

        public void Save()
        {
            int tmp1 = (_playMusic << 4) & PlayMusicFlag;
            int tmp2 = (_playSoundsEffects << 8) & PlaySoundsEffectsFlag;
            int tmp3 = !_showPlayModeShadow ? 0 : ShowPlayModeShadowFlag;
            int tmp4 = !_showEditModeShadow ? 0 : ShowEidtModeShadowFlag;
            int value = tmp1 | tmp2 | tmp3 | tmp4;
            PlayerPrefs.SetInt(SaveKey, value);
        }

        public void Load()
        {
            if (PlayerPrefs.HasKey(SaveKey))
            {
                int value = PlayerPrefs.GetInt(SaveKey);
                _playMusic = (PlayMusicFlag & value) >> 4;
                _playSoundsEffects = (PlaySoundsEffectsFlag & value) >> 8;
                _showPlayModeShadow = (ShowPlayModeShadowFlag & value) != 0;
                _showEditModeShadow = (ShowEidtModeShadowFlag & value) != 0;
            }
            else
            {
                _playMusic = 5;
                _playSoundsEffects = 5;
                _showPlayModeShadow = true;
                _showEditModeShadow = true;
            }
        }
    }
}