/********************************************************************
** Filename : RandomAudioPlayer  
** Author : ake
** Date : 5/31/2016 1:58:30 PM
** Summary : RandomAudioPlayer  
***********************************************************************/


using GameA.Game;
using SoyEngine;
using UnityEngine;
using PlayMode = GameA.Game.PlayMode;

namespace SoyEngine
{
	public class RandomAudioPlayer
	{
		protected const int NoneValue = -1;
		private string[] _audioNameArray;
		private bool _isEnable = false;
		private int _randomMaxValue;
		private string _name;

		private int _lastValue = NoneValue;
		private int _lastTriggerFrame = 0;

		public RandomAudioPlayer(string name)
		{
			_name = name;
		}

		public void SetRandomAudioNameArray(string[] array)
		{
			if (array == null || array.Length == 0)
			{
				_isEnable = false;
				return;
			}
			_randomMaxValue = array.Length;
			_audioNameArray = array;
			_lastValue = NoneValue;
			_isEnable = true;
		}

		public void Play()
		{
			if (!_isEnable)
			{
				//LogHelper.Warning("RandomAudioPlayer {0} .Play called but _isEnable is false!",_name);
				return;
			}
            if (GameRun.Instance.LogicFrameCnt == _lastTriggerFrame)
			{
				return;
			}
            _lastTriggerFrame = GameRun.Instance.LogicFrameCnt;
			Stop();
			int randomValue = Random.Range(0, _randomMaxValue);
			GameAudioManager.Instance.PlaySoundsEffects(_audioNameArray[randomValue]);
			_lastValue = randomValue;
		}

		public void Stop()
		{
			if (!_isEnable)
			{
				LogHelper.Warning("RandomAudioPlayer {0} .Stop called but _isEnable is false!", _name);
				return;
			}
			if (_lastValue == NoneValue)
			{
				return;
			}
			else
			{
				GameAudioManager.Instance.Stop(_audioNameArray[_lastValue]);
			}
		}

	}
}