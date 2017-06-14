/********************************************************************
** Filename : UnityNativeParticleItem  
** Author : ake
** Date : 7/12/2016 10:19:35 AM
** Summary : UnityNativeParticleItem  
***********************************************************************/


using System;
using System.Collections;
using GameA.Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SoyEngine
{
	public class UnityNativeParticleItem:ParticleItemBase
	{
		protected Renderer[] _cachedRenders;

		protected float _lastPlayTime;

		protected bool _autoStop = false;

		protected float _playTime = 0;

		public Action OnFreeEvent;


		public override bool HasFinish
		{
			get
			{
				if(!_autoStop)
				{
					return false;
				}
				return Time.realtimeSinceStartup > _lastPlayTime + _playTime;
			}
		}

		public override void InitGo(GameObject go,string itemName)
		{
			base.InitGo(go, itemName);
			InitRender();
			_cachedGo.SetActiveEx(false);
		}

		public override void SetData(bool autoDestroy , float lifeTime)
		{
			base.SetData(autoDestroy, lifeTime);
			_autoStop = false;
			_playTime = 0;
		}

		public override void Play()
		{
            if (!_isPlaying) {
                _autoStop = false;
                PlayItem ();
                for (int i = 0; i < _cachedRenders.Length; i++)
                {
                    _cachedRenders[i].enabled = true;
                }
                _isPlaying = true;
            }
		}

		public override void Play(float playTime)
		{
            if (!_isPlaying) {
                _autoStop = true;
                _playTime = Mathf.Max (playTime, 0);
                PlayItem ();
                for (int i = 0; i < _cachedRenders.Length; i++)
                {
                    _cachedRenders[i].enabled = true;
                }
                _isPlaying = true;
            }
		}



		public override void Stop()
		{
		    if (_isPlaying)
		    {
                _cachedGo.SetActiveEx(false);
                _isPlaying = false;
		    }
		}

        public void Pause()
        {
            if (_isPlaying)
            {
                for (int i = 0; i < _cachedRenders.Length; i++)
                {
                    _cachedRenders[i].enabled = false;
                }
                _isPlaying = false;
            }
        }

		public override void DestroySelf(float delay = 0f)
		{
			_hasBeenDestroy = true;
			if (_trans == null)
			{
				LogHelper.Warning("DestroySelf called but _trans is null! item is {0}",_itemName);
				return;
			}
            if (GameParticleManager.Instance == null)
		    {
                return;
		    }
		    {
				GameParticleManager.Instance.RemoveFromSceneItems(this);
			}
			if (delay <= 0)
			{
				GameParticleManager.FreeParticleItem(this);
			}
			else
			{
				CoroutineProxy.Instance.StartCoroutine(DelayDestroySelf(delay));
			}
			
		}

		IEnumerator DelayDestroySelf(float value)
		{
			yield return new WaitForSeconds(value);
			if (Trans != null)
			{
				GameParticleManager.FreeParticleItem(this);
			}
		}

		public override void Release()
		{
			OnFreeEvent = null;
			Object.Destroy(_cachedGo);
		}

		public override void OnFree()
		{
			if (OnFreeEvent != null)
			{
				OnFreeEvent();
			}
			OnFreeEvent = null;
		}


		public void SetSortingOrder(ESortingOrder order)
		{
			SetSortingOrder((int) order);
		}

		public void SetSortingOrder(int order)
		{
			if (_cachedRenders != null)
			{
				for (int i = 0; i < _cachedRenders.Length; i++)
				{
					_cachedRenders[i].sortingOrder = order;
				}
			}
		}

		public void SetLayer(int v)
		{
			CommonTools.SetAllLayerIncludeHideObj(_trans, v);
		}

		#region private



		private void PlayItem()
		{
			_lastPlayTime = Time.realtimeSinceStartup;
			_cachedGo.SetActiveEx(true);
		}

		private void InitRender()
		{
			if (_cachedGo != null)
			{
				_cachedRenders = _cachedGo.GetComponentsInChildren<Renderer>(true);
				for (int i = 0; i < _cachedRenders.Length; i++)
				{
					_cachedRenders[i].sortingOrder = (int)ESortingOrder.Bullet;
				}
			}
		}


		#endregion
	}
}