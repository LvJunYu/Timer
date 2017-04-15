/********************************************************************
** Filename : ParticleItemPool  
** Author : ake
** Date : 9/8/2016 10:49:55 AM
** Summary : ParticleItemPool  
***********************************************************************/


using System.Collections.Generic;
using UnityEngine;

namespace SoyEngine
{
	public class ParticleItemPool
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

		private Stack<UnityNativeParticleItem> _freeParticleItems;
		private Transform _poolRoot;

		private float _lastRequestTime;

		public ParticleItemPool(string name,Transform parent)
		{
			_poolName = name;
			_freeParticleItems = new Stack<UnityNativeParticleItem>();
			_poolRoot = new GameObject(name + "_pool").transform;
			CommonTools.SetParent(_poolRoot, parent);
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
            #if UNITY_EDITOR
            var itemPrefab1 = Resources.Load (_poolName);
            if (itemPrefab1 != null) {
                GameObject resGo1 = Object.Instantiate (itemPrefab1) as GameObject;
                UnityNativeParticleItem com1 = new UnityNativeParticleItem ();
                com1.InitGo (resGo1, _poolName);
                return com1;
            }
            #endif
            var itemPrefab = GameResourceManager.Instance.LoadMainAssetObject(_poolName);
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
