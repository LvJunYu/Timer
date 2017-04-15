/********************************************************************
** Filename : ParticleItemBase  
** Author : ake
** Date : 7/12/2016 10:16:34 AM
** Summary : ParticleItemBase  
***********************************************************************/


using UnityEngine;

namespace SoyEngine
{
	public abstract class ParticleItemBase
	{
		protected GameObject _cachedGo;
		protected Transform _trans;

		protected bool _isPlaying = false;
        protected bool _isPause = false;

		protected float _createTime = 0;
		protected bool _autoDestroy = false;
		protected float _lifeTime = 0;

		protected bool _hasBeenDestroy = false;
		protected string _itemName;
		protected int _uuid;

		public int UUID
		{
			get { return _uuid; }
		}

		public string ItemName
		{
			get { return _itemName; }
		}
		public bool IsPlaying
		{
			get { return _isPlaying; }
		}

        public bool IsPause {
            get { return _isPause; }
        }

		public bool NeedToDestroy
		{
			get
			{
				if (!_autoDestroy)
				{
					return false;
				}
				return Time.realtimeSinceStartup > _createTime + _lifeTime;
			}
		}

		public bool HasBeenDestory
		{
			get { return _hasBeenDestroy; }
		}

		public virtual bool HasFinish
		{
			get
			{
				return false;
			}
		}

		public Transform Trans
		{
			get { return _trans; }
		}

		public virtual void InitGo(GameObject go,string itemName)
		{
			_cachedGo = go;
			_trans = _cachedGo.transform;
			_itemName = itemName;
			if (_cachedGo != null)
			{
				_uuid = _cachedGo.GetInstanceID();
			}
		}

		public void SetParent(Transform parent, Vector3 pos)
		{
			if (_trans == null)
			{
				LogHelper.Error("SetParent called but _trans is null! item is {0}",_itemName);
				return;
			}
			CommonTools.SetParent(_trans,parent);
			_trans.localPosition = pos;
		}
		public abstract void Play(float playTime);

		public abstract void Play();

		public abstract void Stop();

        public abstract void DestroySelf (float delay = 0f);

		public abstract void Release();
		public abstract void OnFree();

		public virtual void SetData(bool autoDestroy, float lifeTime)
		{
			_createTime = Time.realtimeSinceStartup;
			_autoDestroy = autoDestroy;
			_lifeTime = Mathf.Max(0, lifeTime);
			_hasBeenDestroy = false;
			_isPlaying = false;
			_isPause = false;
		}
	}
}
