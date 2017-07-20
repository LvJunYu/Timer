/********************************************************************
** Filename : GameParticleManager
** Author : Dong
** Date : 2016/4/21 星期四 下午 4:28:43
** Summary : GameParticleManager
***********************************************************************/

using System;
using System.Collections.Generic;
using GameA;
using GameA.Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SoyEngine
{
    public class GameParticleManager : IDisposable
    {
        public const float TickPoolInterval = 5;
        private static GameParticleManager _instance;

        private readonly Dictionary<string, ParticleItemPool> _particlePoolDic =
            new Dictionary<string, ParticleItemPool>();

        private readonly List<int> _removeBuffer = new List<int>();

        private readonly Dictionary<int, UnityNativeParticleItem> _sceneParticleItems =
            new Dictionary<int, UnityNativeParticleItem>();

        private bool _isDoingTick;
        private float _lastTickPoolTime;
        private Transform _rootParticleParent;

        public static GameParticleManager Instance
        {
            get { return _instance ?? (_instance = new GameParticleManager()); }
        }

        public void Dispose()
        {
            foreach (UnityNativeParticleItem unityNativeParticleItem in _sceneParticleItems.Values)
            {
                unityNativeParticleItem.Release();
            }
            _sceneParticleItems.Clear();
            foreach (ParticleItemPool particleItemPool in _particlePoolDic.Values)
            {
                particleItemPool.Dispose();
            }
            _particlePoolDic.Clear();
            if (_rootParticleParent != null)
            {
                Object.Destroy(_rootParticleParent.gameObject);
                _rootParticleParent = null;
            }
            _instance = null;
        }

        public void Init()
        {
            _rootParticleParent = new GameObject("ParticleRoot").transform;
        }

        public UnityNativeParticleItem GetUnityNativeParticleItem(string itemName, Transform parent,
            ESortingOrder sortingOrder = ESortingOrder.Item)
        {
            if (string.IsNullOrEmpty(itemName)) return null;

            Transform realParent = parent == null ? _rootParticleParent : parent;
            UnityNativeParticleItem com = GetParticleItem(itemName);
            if (com == null || com.Trans == null)
            {
                LogHelper.Error("GetUnityNativeParticleItem failed! itemName is {0}", itemName);
                return null;
            }
            com.SetData(false, 0);
            com.SetParent(realParent, Vector3.zero);
            com.SetSortingOrder(sortingOrder);
            AddToSceneItems(com);
            return com;
        }

        public UIParticleItem GetUIParticleItem(string itemName, Transform parent, int groupId)
        {
            SoyUIGroup group = SocialGUIManager.Instance.UIRoot.GetUIGroup(groupId);
            if (group == null)
            {
                LogHelper.Error("GetUIParticleItem calle but groupId is invalid {0}!", groupId);
                return null;
            }
            UnityNativeParticleItem particle = GetUnityNativeParticleItem(itemName, parent);
            if (parent == null)
            {
                return null;
            }
            particle.SetLayer((int) ELayer.UI);
            var item = new UIParticleItem(particle, group);
            group.AddSortingLayerItem(item);

            return item;
        }

        public bool Emit(string itemName, Vector3 pos, Vector3 rotation, Vector3 scale,
            float lifeTime = ConstDefineGM2D.DefaultParticlePlayTime, ESortingOrder sortingOrder = ESortingOrder.Item)
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

        public bool Emit(string itemName, Vector3 pos, Vector3 scale,
            float lifeTime = ConstDefineGM2D.DefaultParticlePlayTime, ESortingOrder sortingOrder = ESortingOrder.Item)
        {
            return Emit(itemName, pos, Vector3.zero, scale, lifeTime, sortingOrder);
        }

        public bool Emit(string itemName, Vector3 pos, float lifeTime = ConstDefineGM2D.DefaultParticlePlayTime,
            ESortingOrder sortingOrder = ESortingOrder.Item)
        {
            return Emit(itemName, pos, Vector3.zero, Vector3.one, lifeTime, sortingOrder);
        }

        public UnityNativeParticleItem Emit(string itemName, Transform parent,
            float lifeTime = ConstDefineGM2D.DefaultParticlePlayTime)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                return null;
            }

            UnityNativeParticleItem com = GetParticleItem(itemName);
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

        public void Update()
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
                LogHelper.Warning("RemoveFromSceneItems failed! itme is {0}", item == null ? "null" : item.ItemName);
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

        public static void FreeSpineObject(SpineObject item)
        {
            if (item != null)
            {
                PoolFactory<SpineObject>.Free(item);
            }
        }

        public static void FreeParticleItem(UnityNativeParticleItem item)
        {
            if (item == null || item.Trans == null)
            {
//                LogHelper.Warning("FreeParticleItem called but item is invalid!");
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

        internal SpineObject EmitLoop(string path, Vector3 pos)
        {
            SpineObject so;
            if (GM2DTools.TryGetSpineObject(path, out so))
            {
                so.Trans.position = pos;
                so.SkeletonAnimation.state.SetAnimation(0, "Run", true);
            }
            return so;
        }

        internal SpineObject EmitLoop(string path, Transform parent)
        {
            SpineObject so;
            if (GM2DTools.TryGetSpineObject(path, out so))
            {
                so.Trans.SetParent(parent, false);
                if (so.SkeletonAnimation == null || so.SkeletonAnimation.state == null)
                {
                    LogHelper.Error("EmitLoop Failed, {0}", path);
                }
                else
                {
                    so.SkeletonAnimation.state.SetAnimation(0, "Run", true);
                }
            }
            return so;
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

        private void AddToSceneItems(UnityNativeParticleItem item)
        {
            if (item == null || item.Trans == null)
            {
                LogHelper.Error("AddToSceneItems failed! item is {0}.", item == null ? "null" : item.ItemName);
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
            Dictionary<int, UnityNativeParticleItem>.Enumerator sceneItemsEnumerator =
                _sceneParticleItems.GetEnumerator();
            while (sceneItemsEnumerator.MoveNext())
            {
                UnityNativeParticleItem cur = sceneItemsEnumerator.Current.Value;
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
                }
            }

            for (int i = 0; i < _removeBuffer.Count; i++)
            {
                int item = _removeBuffer[i];
                if (_sceneParticleItems.ContainsKey(item))
                {
                    _sceneParticleItems.Remove(item);
                }
            }
            _removeBuffer.Clear();


            if (Time.realtimeSinceStartup - _lastTickPoolTime > TickPoolInterval)
            {
                _lastTickPoolTime = Time.realtimeSinceStartup;

                Dictionary<string, ParticleItemPool>.Enumerator enumerator = _particlePoolDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ParticleItemPool item = enumerator.Current.Value;
                    item.Tick();
                }
            }
            _isDoingTick = false;
        }

        #endregion
    }
}