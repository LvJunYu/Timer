/********************************************************************
** Filename : ParticleItemPool  
** Author : ake
** Date : 9/8/2016 10:49:55 AM
** Summary : ParticleItemPool  
***********************************************************************/


using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SoyEngine
{
	public class ParticleItemPool : IDisposable
	{
		public string PoolName
		{
			get { return _poolName; }
		}
		/// <summary>
		/// 最大闲置回收事件
		/// </summary>
		public const float MaxUnusedRecoveryDelay = 10;

		private string _poolName;

		private Stack<UnityNativeParticleItem> _freeParticleItems = new Stack<UnityNativeParticleItem>();
		private Transform _poolRoot;

		private float _lastRequestTime;

		public ParticleItemPool(string name,Transform parent)
		{
			_poolName = name;
			_poolRoot = new GameObject(name + "_pool").transform;
			CommonTools.SetParent(_poolRoot, parent);
		}

        public void Dispose()
        {
            foreach (UnityNativeParticleItem unityNativeParticleItem in _freeParticleItems)
            {
                unityNativeParticleItem.Release();
            }
            _freeParticleItems.Clear();
            if (_poolRoot != null)
            {
                Object.Destroy(_poolRoot.gameObject);
            }
        }


	    public int Tick()
		{
			if (_freeParticleItems.Count == 0)
			{
				return 0;
			}
			if (CheckAutoRecovery())
			{
				var item = _freeParticleItems.Pop();
				item.Release();
				_lastRequestTime = Time.realtimeSinceStartup;
				return 1;
			}
			return 0;
		}

		public UnityNativeParticleItem Get()
		{
			UnityNativeParticleItem item;
			if (_freeParticleItems.Count == 0)
			{
				item = CreateItem();
			}
			else
			{
				item =_freeParticleItems.Pop();
			}
			_lastRequestTime = Time.realtimeSinceStartup;

			return item;
		}

		public void Free(UnityNativeParticleItem item)
		{
			if (item != null && item.Trans != null)
			{
				item.Stop();
				item.SetParent(_poolRoot,Vector3.zero);
				item.OnFree();
				_freeParticleItems.Push(item);
			}
		}


		#region

		private bool CheckAutoRecovery()
		{
			return Time.realtimeSinceStartup - _lastRequestTime > MaxUnusedRecoveryDelay;
		}


		private UnityNativeParticleItem CreateItem()
		{
            var itemPrefab = NewResourceSolution.ResourcesManager.Instance.GetPrefab(NewResourceSolution.EResType.ParticlePrefab,  _poolName, 0);
			if (itemPrefab == null)
			{
				LogHelper.Error("GameResourceManager.Instance.LoadMainAssetObject({0}) is null!", _poolName);
				return null;
			}
			GameObject resGo = Object.Instantiate(itemPrefab) as GameObject;
			UnityNativeParticleItem com = new UnityNativeParticleItem();
			com.InitGo(resGo,_poolName);
			return com;
		}

		#endregion

	}
}
