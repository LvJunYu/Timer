/********************************************************************
** Filename : GameParticleManager
** Author : Dong
** Date : 2016/4/21 星期四 下午 4:28:43
** Summary : GameParticleManager
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using GameA.Game;
using UnityEngine;
using AnimationState = Spine.AnimationState;
using Object = UnityEngine.Object;

namespace SoyEngine
{
    public class GameParticleManager : MonoBehaviour
    {
        public static GameParticleManager Instance;

	    public const float TickPoolInterval = 5;

	    private List<int> _removeBuffer;
	    private Dictionary<int, UnityNativeParticleItem> _sceneParticleItems;


		private Dictionary<string, ParticleItemPool> _particlePoolDic;

	    private Transform _rootParticleParent;

	    private float _lastTickPoolTime;

	    private bool _isDoingTick = false;

		private void Awake()
        {
            Instance = this;

			Init();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

		public UnityNativeParticleItem GetUnityNativeParticleItem (string itemName, Transform parent, ESortingOrder sortingOrder = ESortingOrder.Item)
	    {
            if (string.IsNullOrEmpty (itemName)) return null;
            //#if UNITY_EDITOR
            //itemPrefab = Resources.Load (itemName);
            //if (itemPrefab != null) {
            //    GameObject resGo1 = GameObject.Instantiate (itemPrefab) as GameObject;
            //    UnityNativeParticleItem com1 = new UnityNativeParticleItem ();
            //    com1.Init (resGo1);
            //    com1.SetParent (parent, Vector3.zero);
            //    return com1;
            //}
            //#endif

	        Transform realParent = parent == null ? _rootParticleParent : parent;
			UnityNativeParticleItem com = GetParticleItem(itemName);
	        if (com == null ||com.Trans == null)
	        {
				LogHelper.Error("GetUnityNativeParticleItem failed! itemName is {0}", itemName);
				return null;
	        }
			com.SetData(false,0);
			com.SetParent(realParent, Vector3.zero);
			com.SetSortingOrder (sortingOrder);
			AddToSceneItems(com);
			return com;
	    }

	    public UIParticleItem GetUIParticleItem(string itemName, Transform parent, int groupId)
	    {
			var group = GM2DGUIManager.Instance.UIRoot.GetUIGroup(groupId);
			if (group == null)
			{
				LogHelper.Error("GetUIParticleItem calle but groupId is invalid {0}!", groupId);
				return null;
			}
			var particle = GetUnityNativeParticleItem(itemName, parent);
		    if (parent == null)
		    {
			    return null;
		    }
			particle.SetLayer((int)ELayer.UI);
			UIParticleItem item = new UIParticleItem(particle, group);
		    group.AddSortingLayerItem(item);

			return item;
	    }

        public bool Emit(string itemName, Vector3 pos, Vector3 rotation, Vector3 scale, float lifeTime = ConstDefineGM2D.DefaultParticlePlayTime, ESortingOrder sortingOrder = ESortingOrder.Item)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                return false;
            }

            UnityNativeParticleItem com = GetParticleItem(itemName);
            if (com == null || com.Trans == null)
            {
                LogHelper.Error("Emit failed! itemName is {0}", itemName);
                return false;
            }
            com.SetData(true, lifeTime);
            com.SetParent(null, pos);
            com.Trans.localEulerAngles = rotation;
            com.Trans.localScale = scale;
            com.SetSortingOrder(sortingOrder);
            com.Play(lifeTime);
            AddToSceneItems(com);
            return true;
        }

		public bool Emit(string itemName, Vector3 pos, Vector3 scale, float lifeTime = ConstDefineGM2D.DefaultParticlePlayTime, ESortingOrder sortingOrder = ESortingOrder.Item)
		{
		    return Emit(itemName, pos, Vector3.zero, scale, lifeTime, sortingOrder);
	    }

        public UnityNativeParticleItem Emit(string itemName, Transform parent, float lifeTime = ConstDefineGM2D.DefaultParticlePlayTime)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                return null;
            }
//#if UNITY_EDITOR
//            itemPrefab = Resources.Load (itemName);
//            if (itemPrefab != null) {
//                GameObject resGo1 = Instantiate (itemPrefab) as GameObject;
//                UnityNativeParticleItem com1 = new UnityNativeParticleItem ();
//                com1.Init (resGo1, true, lifeTime);
//                com1.SetParent (parent, Vector3.zero);
//                com1.Play (lifeTime);
//                _sceneParticleItems.Add (com1);
//                return true;
//            }
//#endif

            var com = GetParticleItem(itemName);
			if (com == null || com.Trans == null)
			{
				LogHelper.Error("Emit failed! itemName is {0}", itemName);
				return null;
			}
			com.SetData(true, lifeTime);
            com.SetParent(parent, Vector3.zero);
            com.Play(lifeTime);
			AddToSceneItems(com);
			return com;
        }

	    void Update()
	    {
		    UpdatePlayingParticleItem();
	    }

	    public void ClearAll()
	    {

		}

	    public void RemoveFromSceneItems(UnityNativeParticleItem item)
	    {
		    if (item == null || item.Trans == null)
		    {
				LogHelper.Warning("RemoveFromSceneItems failed! itme is {0}", item==null?"null":item.ItemName);
			    return;
		    }
		    if (_sceneParticleItems.ContainsKey(item.UUID))
		    {
			    if (_isDoingTick)
			    {
					_removeBuffer.Add(item.UUID);
				}
				else
			    {
				    _sceneParticleItems.Remove(item.UUID);
			    }
		    }
	    }
    
	    public static void FreeParticleItem(UnityNativeParticleItem item)
	    {
		    if (item == null || item.Trans == null)
		    {
				LogHelper.Warning("FreeParticleItem called but item is invalid!");
			    return;
		    }
		    if (Instance == null)
		    {
				item.Release();
			}
		    else
			{
				ParticleItemPool pool;
				if (Instance._particlePoolDic.TryGetValue(item.ItemName, out pool))
				{
					pool.Free(item);
				}
				else
				{
					item.Release();
				}
			}
	    }

		#region private

	    private UnityNativeParticleItem GetParticleItem(string itemName)
	    {
		    ParticleItemPool pool;
		    if (!_particlePoolDic.TryGetValue(itemName, out pool))
		    {
				pool = new ParticleItemPool(itemName, _rootParticleParent);
				_particlePoolDic.Add(itemName, pool);
			}
		    return pool.Get();
	    }

		private void Init()
	    {
			_removeBuffer = new List<int>();
			_sceneParticleItems = new Dictionary<int, UnityNativeParticleItem>();
			_particlePoolDic = new Dictionary<string, ParticleItemPool>();
			_rootParticleParent = new GameObject("ParticleRoot").transform;
			CommonTools.SetParent(_rootParticleParent,transform);
		}

	    private void AddToSceneItems(UnityNativeParticleItem item)
	    {
			if (item == null || item.Trans == null)
		    {
				LogHelper.Error("AddToSceneItems failed! item is {0}.",item==null?"null":item.ItemName);
			    return;
		    }
		    if (_sceneParticleItems.ContainsKey(item.UUID))
		    {
                //LogHelper.Error("AddToSceneItems failed item.uuid {0} duplicated,item.ItemName is {1}", item.UUID, item.ItemName);
			    return;
		    }
			_sceneParticleItems.Add(item.UUID, item);
		}

	    private void UpdatePlayingParticleItem()
	    {
		    _isDoingTick = true;
			var sceneItemsEnumerator = _sceneParticleItems.GetEnumerator();
		    while (sceneItemsEnumerator.MoveNext())
		    {
			    var cur = sceneItemsEnumerator.Current.Value;
				if (cur.Trans == null)
				{
					_removeBuffer.Add(cur.UUID);
					continue;
				}
				if (cur.HasBeenDestory)
				{
					_removeBuffer.Add(cur.UUID);
					continue;
				}
				if (cur.NeedToDestroy)
				{
					cur.DestroySelf();
					continue;
				}
				if (cur.IsPlaying && cur.HasFinish)
				{
					cur.Stop();
					continue;
				}
			}

		    for (int i = 0; i < _removeBuffer.Count; i++)
		    {
			    var item = _removeBuffer[i];
			    if (_sceneParticleItems.ContainsKey(item))
			    {
				    _sceneParticleItems.Remove(item);
			    }
		    }
			_removeBuffer.Clear();


			if (Time.realtimeSinceStartup - _lastTickPoolTime > TickPoolInterval)
		    {
			    _lastTickPoolTime = Time.realtimeSinceStartup;

			    var enumerator = _particlePoolDic.GetEnumerator();
			    while (enumerator.MoveNext())
			    {
				    var item = enumerator.Current.Value;
				    item.Tick();
			    }
		    }
		    _isDoingTick = false;
	    }

		#endregion
	}
}
