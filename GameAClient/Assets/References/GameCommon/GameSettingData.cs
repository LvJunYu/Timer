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
		
		private bool _playMusic = true;
		private bool _playSoundsEffects = true;
		private bool _showPlayModeShadow = true;
		private bool _showEditModeShadow = true;

		public bool PlayMusic
		{
			get { return _playMusic; }
			set
			{
				if (_playMusic != value)
				{
					_playMusic = value;
					Messenger.Broadcast(EMessengerType.OnGameSettingChanged);
				}
			}
		}

		public bool PlaySoundsEffects
		{
			get { return _playSoundsEffects; }
			set
			{
				if (_playSoundsEffects != value)
				{
					_playSoundsEffects = value;
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

		private const int PlayMusicFlag = 1 << 1;
		private const int PlaySoundsEffectsFlag = 1 << 2;
		private const int ShowPlayModeShadowFlag = 1 << 3;
		private const int ShowEidtModeShadowFlag = 1 << 4;

		private const string SaveKey = "GameSettingKey";


		private GameSettingData()
		{
			Load();
		}

		public void Save()
		{
			int tmp1 = !_playMusic ? 0 : PlayMusicFlag;
			int tmp2 = !_playSoundsEffects ? 0 : PlaySoundsEffectsFlag;
			int tmp3 = !_showPlayModeShadow ? 0 : ShowPlayModeShadowFlag;
			int tmp4 = !_showEditModeShadow ? 0 : ShowEidtModeShadowFlag;
			int value = tmp1 | tmp2 | tmp3|tmp4;
			PlayerPrefs.SetInt(SaveKey, value);
		}

		public void Load()
		{
			if (PlayerPrefs.HasKey(SaveKey))
			{
				int value = PlayerPrefs.GetInt(SaveKey);
				_playMusic = (PlayMusicFlag & value) != 0;
				_playSoundsEffects = (PlaySoundsEffectsFlag & value) != 0;
				_showPlayModeShadow = (ShowPlayModeShadowFlag & value) != 0;
				_showEditModeShadow = (ShowEidtModeShadowFlag & value) != 0;
			}
			else
			{
				_playMusic = true;
				_playSoundsEffects = true;
				_showPlayModeShadow = true;
				_showEditModeShadow = true;
			}
		}
	}
}
