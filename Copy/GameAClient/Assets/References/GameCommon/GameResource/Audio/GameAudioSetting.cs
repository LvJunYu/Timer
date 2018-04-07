/********************************************************************

** Filename : GameAudioSetting
** Author : ake
** Date : 2016/4/14 20:59:19
** Summary : GameAudioSetting
***********************************************************************/

using System;
using UnityEngine;

namespace SoyEngine
{
	[Serializable]
	public class GameAudioSetting
	{
		public bool PlayMusic = true;
		public bool PlaySoundsEffects = true;

		private const int Flag1 = 1 << 1;
		private const int Flag2 = 1 << 2;


		private const string SaveKey = "AudioKey";


		public GameAudioSetting()
		{
			Load();
		}

		public void Save()
		{
			int tmp1 = !PlayMusic ? 0 : Flag1;
			int tmp2 = !PlaySoundsEffects ? 0 : Flag2;
			int value = tmp1 | tmp2;
			PlayerPrefs.SetInt(SaveKey, value);
		}

		public void Load()
		{
			if (PlayerPrefs.HasKey(SaveKey))
			{
				int value = PlayerPrefs.GetInt(SaveKey);
				PlayMusic = (Flag1 & value) != 0;
				PlaySoundsEffects = (Flag2 & value) != 0;
			}
			else
			{
				PlayMusic = true;
				PlaySoundsEffects = true;
			}
		}
	}
}
