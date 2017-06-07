/********************************************************************

** Filename : GameAudioManager
** Author : ake
** Date : 2016/4/14 20:58:17
** Summary : GameAudioManager
***********************************************************************/

using System;
using System.Collections.Generic;
using GameA.Game;
using UnityEngine;

namespace SoyEngine
{
	public enum EAudioType
	{
		Music,
		SoundsEffects,
	}

    public class AudioItem
    {
        public string AudioName;
        public EAudioType AudioType;
        public AudioSource AudioSource;
	    public Action OnRecycle;
    }

	public class GameAudioManager:MonoBehaviour
	{
		public const int SoyAudioItemBufferSize = 10;
		public const float CheckToRemoveInvalidAudioItemInterval = 0.2f;

		private static GameAudioManager _instance;

		public GameSettingData _setting;

		private Dictionary<string, AudioItem> _playingAudioEffect;

		private Stack<AudioItem> _audioItemPool;
		private AudioItem[] _audioItemBuffer;
		private float _lastCheckTime = 0;
		private GameObject _cachedAudioGo;

		public static GameAudioManager Instance
		{
			get
			{
				return _instance;
			}
		}

		void Awake()
		{
			_instance = this;
			_playingAudioEffect = new Dictionary<string, AudioItem>();
			_setting = GM2DGame.Instance.Settings;
			_audioItemPool = new Stack<AudioItem>();
			_audioItemBuffer = new AudioItem[SoyAudioItemBufferSize];
			InitGo();
		}

		void OnDestroy()
		{
		    _instance = null;
		}

		public void PlayMusic(string audioName)
		{
            if (string.IsNullOrEmpty(audioName))
            {
                return;
            }
			PlayAudio(EAudioType.Music, audioName, true);
		}

        public void PlaySoundsEffects(string audioName, bool loop = false)
		{
		    if (string.IsNullOrEmpty(audioName))
		    {
		        return ;
		    }
			PlayAudio(EAudioType.SoundsEffects, audioName, loop);
		}

        public bool IsPlaying(string audioName)
        {
            AudioItem audioItem;
            if (!_playingAudioEffect.TryGetValue(audioName, out audioItem))
            {
                return false;
            }
            return true;
        }

	    public void Stop(string audioName)
		{
            if (string.IsNullOrEmpty(audioName))
            {
                return;
            }
			StopPlayAudioItem(audioName);
		}

	    public void StopAll()
	    {
	        var enumerator = _playingAudioEffect.GetEnumerator();
	        while (enumerator.MoveNext())
	        {
	            var item = enumerator.Current.Value;
	            if (item.AudioSource != null)
	            {
	                item.AudioSource.Stop();
	            }
	        }
	    }

		public void OnSettingChanged()
		{
			var enumerator = _playingAudioEffect.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var item = enumerator.Current.Value;
				item.AudioSource.volume = GetPlayVolume(item.AudioType);
			}
		}


		#region  private 

		private void Update()
		{
			//检查所有正在播放的音效
			RemoveInvalidAudioItem();
		}

        private void PlayAudio(EAudioType type, string audioName, bool loop)
		{
			if (!CheckCanPlay(type))
			{
				return;
			}
			float volume = GetPlayVolume(type);
			AudioItem audioItem;
			if (!_playingAudioEffect.TryGetValue(audioName, out audioItem))
			{
				AudioClip clip;
				if (!GameResourceManager.Instance.TryGetAudioClipByName(audioName, out clip))
				{
					LogHelper.Error("Audio {0} load failed!", audioName);
					return;
				}
				audioItem = GetAudioItem();
				audioItem.AudioSource.clip = clip;
				_playingAudioEffect.Add(audioName,audioItem);
			}
			audioItem.AudioSource.volume = volume;
			audioItem.AudioSource.loop = loop;
			audioItem.AudioType = type;
			audioItem.AudioName = audioName;
			audioItem.AudioSource.Play();
            return;
		}

        private void StopPlayAudioItem(string audioName)
		{
			AudioItem audioItem;
			if (!_playingAudioEffect.TryGetValue(audioName, out audioItem))
			{
				return;
			}
			RecycleAudioItem(audioItem);
            return;
		}

		/// <summary>
		/// 需要保证item安全性
		/// </summary>
		/// <param name="audioItem"></param>
		private void RecycleAudioItem(AudioItem audioItem)
		{
			audioItem.AudioSource.Stop();
			audioItem.AudioSource.clip = null;
			audioItem.AudioSource.enabled = false;

			if (audioItem.OnRecycle != null)
			{
				audioItem.OnRecycle();
				audioItem.OnRecycle = null;
			}
			_playingAudioEffect.Remove(audioItem.AudioName);
			_audioItemPool.Push(audioItem);
		}

		private AudioItem GetAudioItem()
		{
			AudioItem res;
			if (_audioItemPool.Count > 0)
			{
				res = _audioItemPool.Pop();
				res.AudioSource.enabled = true;
			}
			else
			{
				res = new AudioItem();
				res.AudioSource = _cachedAudioGo.AddComponent<AudioSource>();
			}
			return res;
		}

		private bool CheckCanPlay(EAudioType type)
		{
			if (type == EAudioType.Music || type == EAudioType.SoundsEffects)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private float GetPlayVolume(EAudioType type)
		{
			if (type == EAudioType.Music && _setting.PlayMusic)
			{
				return 0.6f;
			}
			else if (type == EAudioType.SoundsEffects && _setting.PlaySoundsEffects)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		private void ClearBuffer()
		{
			for (int i = 0; i < SoyAudioItemBufferSize; i++)
			{
				_audioItemBuffer[i] = null;
			}
		}

		private void RemoveInvalidAudioItem()
		{
			if (Time.realtimeSinceStartup - _lastCheckTime < CheckToRemoveInvalidAudioItemInterval)
			{
				return;
			}
			_lastCheckTime = Time.realtimeSinceStartup;
			int needToRemoveItemCount = 0;
			var enumerator = _playingAudioEffect.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (needToRemoveItemCount >= SoyAudioItemBufferSize)
				{
					break;
				}
				var item = enumerator.Current.Value;
				if (!item.AudioSource.isPlaying)
				{
					_audioItemBuffer[needToRemoveItemCount] = item;
					needToRemoveItemCount ++;
				}
			}
			for (int i = 0; i < needToRemoveItemCount; i++)
			{
				RecycleAudioItem(_audioItemBuffer[i]);
			}
			ClearBuffer();
		}

		private void InitGo()
		{
			_cachedAudioGo = new GameObject("AudioGo");
			CommonTools.SetParent(_cachedAudioGo.transform, gameObject.transform);
			_cachedAudioGo.AddComponent<AudioListener>();
		}

        public void ClearAll ()
        {
        }

		#endregion

	}
}
