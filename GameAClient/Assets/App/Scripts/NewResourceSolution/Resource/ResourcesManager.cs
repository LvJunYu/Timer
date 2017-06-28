using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;

namespace NewResourceSolution
{
	public class ResourcesManager : Singleton<ResourcesManager>
    {
        #region fields
        private static readonly int s_maxScenary = 20;

        /// <summary>
        /// 自动执行unload操作最小时间间隔
        /// </summary>
        private static long s_minimumAutoUnloadActionInternal = 20000;

        /// <summary>
        /// 资源目录
        /// </summary>
        private CHRuntimeResManifest _manifest;


        //        private AssetCache _assetCache;

        /// <summary>
        /// 目前正在使用的资源应用情景蒙版
        /// </summary>
        private int _inUseScenaryMask;

        /// <summary>
        /// 上次进行unload操作的时间
        /// </summary>
        private long _lastUnloadActionTime;


        #endregion

        #region properties
        #endregion

        #region public methods

        public UnityEngine.Object GetPrefab(EResType resType, string name, int scenary = 0)
        {
            return GetAsset<UnityEngine.Object>(resType, name, scenary, true, true, LocalizationManager.Instance.CurrentLocale);
        }

        public bool TryGetTexture(string name, out UnityEngine.Texture texture, int scenary = 0)
        {
            texture = GetAsset<UnityEngine.Texture>(EResType.Texture, name, scenary, false, false);
            if (null != texture)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryGetSprite(string name, out UnityEngine.Sprite sprite, int scenary = 0)
        {
            sprite = GetAsset<UnityEngine.Sprite>(EResType.Sprite, name, scenary, false, false);
            if (null != sprite)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public UnityEngine.Texture GetTexture (string name, int scenary = 0)
        {
            return GetAsset<UnityEngine.Texture>(EResType.Texture, name, scenary, true, false);
        }

        public UnityEngine.Sprite GetSprite (string name, int scenary = 0)
        {
            return GetAsset<UnityEngine.Sprite>(EResType.Sprite, name, scenary, true, false);
        }

        public UnityEngine.AudioClip GetAudio (string name, int scenary = 0)
        {
            return GetAsset<UnityEngine.AudioClip>(EResType.AudioClip, name, scenary, true, false);
        }

        public string GetJson (string name, int scenary = 0)
        {
            var textAsset = GetAsset<UnityEngine.TextAsset>(EResType.Json, name, scenary, true, false);
            if (null != textAsset)
            {
                return textAsset.text;
            }
            return string.Empty;
        }

        public T GetAsset<T> (
            EResType resType,
            string name,
            int scenary,
            bool logWhenError = true,
            bool isLocaleRes = false,
            ELocale locale = ELocale.WW
            ) where T : UnityEngine.Object
        {
            #if UNITY_EDITOR
            if (!RuntimeConfig.Instance.UseAssetBundleRes)
            {
                //var textAsset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(ResPathUtility.GetEditorDebugResPath(EResType.Table, name)) as UnityEngine.TextAsset;
                //return textAsset.text;

                //var obj = UnityEditor.AssetDatabase.LoadMainAssetAtPath(ResPathUtility.GetEditorDebugResPath(resType, name));
                //if (null == obj)
                //{
                //    obj = UnityEditor.AssetDatabase.LoadMainAssetAtPath(ResPathUtility.GetEditorDebugResPath(resType, name, true, ELocale.WW));
                //}
                return null;
            }
            else
            #endif
            {
                return _manifest.GetAsset(name, scenary, logWhenError, isLocaleRes, locale) as T;
            }
        }

        public void Init ()
        {
            _inUseScenaryMask = 0;
            _lastUnloadActionTime = 0;

            // 读取本地版本
            if (UnityTools.TryGetObjectFromLocal<CHRuntimeResManifest> (ResDefine.CHResManifestFileName, out _manifest))
            {
                _manifest.FileLocation = EFileLocation.Persistent;
                LogHelper.Info ("Res in pesistant, ver: {0}", _manifest.Ver);
            }
            else
            {
                // 不存在则读取包内版本
                UnityTools.TryGetObjectFromStreamingAssets<CHRuntimeResManifest> (ResDefine.CHResManifestFileName, out _manifest);
                if (null != _manifest)
                {
                    _manifest.FileLocation = EFileLocation.StreamingAsset;
                    LogHelper.Info ("Res in package, ver: {0}", _manifest.Ver);
                }
            }
            if (null != _manifest)
            {
                _manifest.MapBundles();

                if (!_manifest.Init())
                {
                    // 这里出错，游戏将无法正常运行
                    return;
                }
            }
            else
            {
                LogHelper.Error ("Init resourcesManager failed, can't find any manifest.");
                // todo 通知玩家，退出程序
                return;
            }

            //            if (null == _assetCache)
            //            {
            //              _assetCache = new AssetCache ();
            //            }
            //            if (null)


            //          _assetCache.Init (_manifest);
        }

        public void Update ()
        {
            // 定时进行清理asset缓存，以使内存中资源的大小维持在建议大小以内
            if (_manifest.CachedAssetTotalSize > CHRuntimeResManifest.s_SuggestedAssetMemorySize)
            {
                long now = DateTimeUtil.GetNowTicks () / 10000;
                if (now - _lastUnloadActionTime > s_minimumAutoUnloadActionInternal)
                {
                    _manifest.UnloadUnusedAssets (0);
                }
            }
        }

        /// <summary>
        /// 卸载某一特定使用场景的asset
        /// </summary>
        /// <param name="scenary">Scenary.</param>
        public void UnloadScenary (int scenary)
        {
            int maskToUnload = 1 << scenary;
            _inUseScenaryMask &= ~maskToUnload;
            _manifest.UnloadUnusedAssets (maskToUnload);
        }

        public void ForceUnloadScenary (int scenary)
        {
            int maskToUnload = 1 << scenary;
            _inUseScenaryMask &= ~maskToUnload;
            _manifest.ForceUnloadUnusedAssets (maskToUnload);
        }
        #endregion

        #region private methods

        #endregion

        #region update&download
        public void CheckApplicationAndResourcesVersion ()
        {
            CoroutineManager.StartCoroutine(VersionUpdater.CheckVerInternal(_manifest));
        }
        #endregion
    }
}