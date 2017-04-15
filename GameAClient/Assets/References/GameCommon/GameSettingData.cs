/********************************************************************
** Filename : GameSettingData  
** Author : ake
** Date : 12/14/2016 2:44:55 PM
** Summary : GameSettingData  
***********************************************************************/


using System;
using UnityEngine;

namespace SoyEngine
{
	[Serializable]
	public class GameSettingData
	{
		public bool PlayMusic = true;
		public bool PlaySoundsEffects = true;
		public bool ShowPlayModeShadow = true;
		public bool ShowEidtModeShadow = true;

		private const int PlayMusicFlag = 1 << 1;
		private const int PlaySoundsEffectsFlag = 1 << 2;
		private const int ShowPlayModeShadowFlag = 1 << 3;
		private const int ShowEidtModeShadowFlag = 1 << 4;

		private const string SaveKey = "GameSettingKey";


		public GameSettingData()
		{
			Load();
		}

		public void Save()
		{
			int tmp1 = !PlayMusic ? 0 : PlayMusicFlag;
			int tmp2 = !PlaySoundsEffects ? 0 : PlaySoundsEffectsFlag;
			int tmp3 = !ShowPlayModeShadow ? 0 : ShowPlayModeShadowFlag;
			int tmp4 = !ShowEidtModeShadow ? 0 : ShowEidtModeShadowFlag;
			int value = tmp1 | tmp2 | tmp3|tmp4;
			PlayerPrefs.SetInt(SaveKey, value);
		}

		public void Load()
		{
			if (PlayerPrefs.HasKey(SaveKey))
			{
				int value = PlayerPrefs.GetInt(SaveKey);
				PlayMusic = (PlayMusicFlag & value) != 0;
				PlaySoundsEffects = (PlaySoundsEffectsFlag & value) != 0;
				ShowPlayModeShadow = (ShowPlayModeShadowFlag & value) != 0;
				ShowEidtModeShadow = (ShowEidtModeShadowFlag & value) != 0;
			}
			else
			{
				PlayMusic = true;
				PlaySoundsEffects = true;
				ShowPlayModeShadow = true;
				ShowEidtModeShadow = true;
			}
		}
	}
}
