using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoyEngine;

namespace NewResourceSolution
{
    /// <summary>
    /// 运行时使用的Manifest
    /// </summary>
    public class CHRuntimeResManifest : CHResManifest
    {
        #region fields
        /// <summary>
        /// 缓存asset占用内存建议大小，进行unload操作时，总占用大小小于该值即停止unload操作
        /// </summary>
        public static long s_SuggestedAssetMemorySize = 20 * ResDefine.KiloByte * ResDefine.KiloByte;
        /// <summary>
        /// 缓存asset占用内存警报大小，所有缓存总大小超过该值后，开始进行unload操作
        /// </summary>
        public static long s_WarningAssetMemorySize = 40 * ResDefine.KiloByte * ResDefine.KiloByte;

        /// <summary>
        /// asset缓存词典，key是assetBundleName，全小写
        /// </summary>
        private Dictionary<string, CHResBundle> _cachedBundleDic = new Dictionary<string, CHResBundle> ();
        /// <summary>
        /// 缓存asset所占用的总大小
        /// </summary>
        private long _cachedAssetsTotalSize;

        /// <summary>
        /// 进行unload时使用的记录需要清理的asset的列表缓存
        /// </summary>
        private List<string> _assetToUnload = new List<string>();
        /// <summary>
        /// 加载bundle用的列表缓存
        /// </summary>
        private List<CHResBundle> _bundleToLoad = new List<CHResBundle>();
        /// <summary>
        /// 加载bundle过程中需要修改scenaryMask的所有bundle缓存
        /// </summary>
        private List<CHResBundle> _bundleToAddScenary = new List<CHResBundle>();
        /// <summary>
        /// ResourcesManager的manifest缓存
        /// </summary>
        private CHRuntimeResManifest _manifest;
        /// <summary>
        /// unity 资源目录
        /// </summary>
        private UnityEngine.AssetBundleManifest _unityManifest;

        private Dictionary<string, string> _assetName2BundleName = new Dictionary<string, string>();
        private Dictionary<string, CHResBundle> _bundleName2Bundle = new Dictionary<string, CHResBundle>();
        #endregion

        #region properties
        /// <summary>
        /// 缓存asset所占用的总大小
        /// </summary>
        public long CachedAssetTotalSize
        {
            get
            {
                return this._cachedAssetsTotalSize;
            }
        }
        #endregion

        #region methods
        public CHRuntimeResManifest(Version version) : base(version) {}

        public void MapBundles ()
        {
            _assetName2BundleName.Clear ();
            _bundleName2Bundle.Clear ();
            for (int i = 0; i < _bundles.Count; i++)
            {
                _bundles[i].IsLocaleRes = _bundles[i].AssetBundleName.ToCharArray()[0] == ResDefine.LocaleResBundleNameFirstChar;
                if (_bundles[i].IsLocaleRes)
                {
                    _bundles[i].LocaleName = _bundles[i].AssetBundleName.Substring(1).Split(ResDefine.ReplaceSplashCharInAssetBundleName)[0].ToUpper();
                }
                for (int j = 0; j < _bundles[i].AssetNames.Length; j++)
                {
                    string assetNameWithLocaleName = _bundles[i].IsLocaleRes ?
                        StringUtil.Format(StringFormat.TwoLevelPath, _bundles[i].LocaleName, _bundles[i].AssetNames[j]) :
                        _bundles[i].AssetNames[j];
                    if (!_assetName2BundleName.ContainsKey(assetNameWithLocaleName))
                    {
                        //                      LogHelper.Info("Regist assetName {0} with {1}", assetNameWithLocaleName, _bundles[i].AssetBundleName);
                        _assetName2BundleName.Add(assetNameWithLocaleName, _bundles[i].AssetBundleName);
                    }
                    else
                    {
                        LogHelper.Error("Asset name dumplicate in manifest, name: {0}", _bundles[i].AssetNames[j]);
                    }
                }
                if (!_bundleName2Bundle.ContainsKey(_bundles[i].AssetBundleName))
                {
                    _bundleName2Bundle.Add(_bundles[i].AssetBundleName, _bundles[i]);
                }
                else
                {
                    LogHelper.Error("Bundle name dumplicate in manifest, name: {0}", _bundles[i].AssetBundleName);
                }
            }
        }

        public bool Init ()
        {
            _cachedBundleDic.Clear ();
            _cachedAssetsTotalSize = 0;

            CHResBundle unityManifestBundle = GetBundleByBundleName(ResDefine.UnityManifestBundleName);
            if (null == unityManifestBundle)
            {
                LogHelper.Error("No unity manifest in CHResManifest, resourcesManager init failed.");
                return false;
            }
            var manifestAssetBundle = AssetBundle.LoadFromFile(unityManifestBundle.GetFilePath(unityManifestBundle.FileLocation));
            if (null == manifestAssetBundle)
            {
                LogHelper.Error("Load manifest bundle failed, resourcesManager init failed.");
                return false;
            }
            _unityManifest = manifestAssetBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
            if (null == _unityManifest)
            {
                LogHelper.Error("Load AssetBundleManifest in manifest bundle failed, resourcesManager init failed.");
                return false;
            }
            manifestAssetBundle.Unload(false);
            return true;
        }

        public UnityEngine.Object GetAsset (
            string assetName,
            int scenary,
            bool logWhenError,
            bool isLocaleRes = false,
            ELocale locale = ELocale.WW)
        {
            string assetNameWithLocaleName = isLocaleRes ?
                StringUtil.Format(StringFormat.TwoLevelPath, LocaleDefine.LocaleNames[(int)locale], assetName) :
                assetName;
            string bundleName = GetBundleNameByAssetName(assetNameWithLocaleName, isLocaleRes, locale);
            if (string.IsNullOrEmpty(bundleName))
            {
                if (logWhenError)
                {
                    LogHelper.Error("No bundle contains asset: {0}", assetName);
                }
                return null;
            }
            CHResBundle bundle;
            UnityEngine.Object asset;
            if (_cachedBundleDic.TryGetValue(bundleName, out bundle))
            {
                if (bundle.AssetDic.TryGetValue(assetName, out asset))
                {
                    bundle.ScenaryMask &= scenary;
                    return asset;
                }
            }
            // try load
            bundle = CacheBundleAndDependencies(bundleName, scenary, logWhenError);
            if (null != bundle)
            {
                if (bundle.AssetDic.TryGetValue(assetName, out asset))
                {
                    return asset;
                }
            }
            return null;
        }

        /// <summary>
        /// 清理不再使用的asset缓存，考虑在建议asset缓存大小下尽量多缓存asset，如果清理完毕后剩余的缓存大小仍然超过警报大小，则进行警报
        /// </summary>
        public void UnloadUnusedAssets (int scenaryMask2Unload)
        {
            if (_cachedAssetsTotalSize < s_SuggestedAssetMemorySize)
                return;
            InternalUnloadUnusedAssets (scenaryMask2Unload, false);
        }
        /// <summary>
        /// 强制清理不再使用的asset缓存，不考虑在建议asset缓存大小下尽量多缓存asset
        /// </summary>
        public void ForceUnloadUnusedAssets (int scenaryMask2Unload)
        {
            InternalUnloadUnusedAssets (scenaryMask2Unload, true);
        }


        /// <summary>
        /// 清除所有asset缓存，不管该缓存是否还有使用
        /// </summary>
        public void ClearAllAssetCache ()
        {
            _cachedBundleDic.Clear ();
            _cachedAssetsTotalSize = 0;
            UnityEngine.Resources.UnloadUnusedAssets ();
        }

        private void InternalUnloadUnusedAssets (int scenaryMask2Unload, bool force)
        {
            _assetToUnload.Clear ();
            var itor = _cachedBundleDic.GetEnumerator ();
            while (itor.MoveNext ())
            {
                CHResBundle bundle = itor.Current.Value;
                if (0 != scenaryMask2Unload)
                {
                    bundle.ScenaryMask &= ~scenaryMask2Unload;
                }
                if (0 == bundle.ScenaryMask)
                {
                    _assetToUnload.Add (itor.Current.Key);
                    bundle.AssetDic.Clear();
                    _cachedAssetsTotalSize -= bundle.AssetsTotalSize;
                    if (!force && _cachedAssetsTotalSize < s_SuggestedAssetMemorySize)
                    {
                        break;
                    }
                }
            }
            for (int i = 0; i < _assetToUnload.Count; i++)
            {
                _cachedBundleDic.Remove (_assetToUnload [i]);
                LogHelper.Info ("___________________Unload bundle {0}", _assetToUnload[i]);
            }
            LogHelper.Info ("___________________Unloaded {0} bundles", _assetToUnload.Count);
            UnityEngine.Resources.UnloadUnusedAssets ();
            if (_cachedAssetsTotalSize > s_WarningAssetMemorySize)
            {
                // todo 发出警报
            }
        }


        /// <summary>
        /// 缓存bundle和他的依赖
        /// </summary>
        /// <param name="bundle">Bundle.</param>
        /// <param name="scenary">Scenary.</param>
        private CHResBundle CacheBundleAndDependencies (string bundleName, int scenary, bool logWhenError)
        {
            CHResBundle bundle = GetBundleByBundleName(bundleName);
            if (null == bundle)
            {
                if (logWhenError)
                {
                    LogHelper.Error("Load bundle <{0}> failed", bundleName);
                }
                return null;
            }
            return CacheBundleAndDependencies(bundle, scenary, logWhenError);
        }
        /// <summary>
        /// 缓存bundle和他的依赖
        /// </summary>
        /// <param name="bundle">Bundle.</param>
        /// <param name="scenary">Scenary.</param>
        private CHResBundle CacheBundleAndDependencies (CHResBundle bundle, int scenary, bool logWhenError)
        {
            LogHelper.Info("CacheBundleAndDependencies, bundleName: {0}", bundle.AssetBundleName);
            // 如果资源不在本地，也不是非压缩且在streamingAsset中， 也不是adam资源且在streamingAsset中
            if (EFileLocation.Persistent != bundle.FileLocation &&
                !(EFileLocation.StreamingAsset == bundle.FileLocation &&
                    (_adamBundleNameList.Contains(bundle.AssetBundleName) || 
                        EAssetBundleCompressType.NoCompress == bundle.CompressType))
            )
            {
                if (logWhenError)
                {
                    LogHelper.Error("Bundle <{0}> not downloaded", bundle.AssetBundleName);
                }
                return null;
            }
            _bundleToLoad.Clear();
            _bundleToAddScenary.Clear();
            _bundleToLoad.Add(bundle);
            string[] dependencies = _unityManifest.GetAllDependencies(bundle.AssetBundleName);
            //          LogHelper.Info("Dependencies cnt: {0}", dependencies.Length);
            for (int i = 0; i < dependencies.Length; i++)
            {
                //              LogHelper.Info("Dependence {0}: {1}", i, dependencies[i]);
                CHResBundle dependenceBundle = null;
                if (_cachedBundleDic.TryGetValue(dependencies[i], out dependenceBundle))
                {
                    if ((dependenceBundle.ScenaryMask & scenary) == 0)
                    {
                        _bundleToAddScenary.Add(dependenceBundle);
                    }
                    LogHelper.Info ("Bundle {0} already cached, asset cnt {1}", dependenceBundle.AssetBundleName, dependenceBundle.AssetDic.Count);
                    var itor = dependenceBundle.AssetDic.GetEnumerator ();
                    while (itor.MoveNext ()) {
                        LogHelper.Info ("asset in cached bundle: name {0}, {1}", itor.Current.Key, itor.Current.Value);
                    }
                    continue;
                }
                dependenceBundle = GetBundleByBundleName(dependencies[i]);
                if (null == dependenceBundle)
                {
                    if (logWhenError)
                    {
                        LogHelper.Error("Load bundle <{0}> failed", dependencies[0]);
                    }
                    return null;
                }
                if (EFileLocation.Persistent != dependenceBundle.FileLocation &&
                    !(EFileLocation.StreamingAsset == dependenceBundle.FileLocation &&
                        (_adamBundleNameList.Contains(dependenceBundle.AssetBundleName) ||
                            EAssetBundleCompressType.NoCompress == dependenceBundle.CompressType))
                )
                {
                    if (logWhenError)
                    {
                        LogHelper.Error("Bundle <{0}> not downloaded", dependenceBundle.AssetBundleName);
                    }
                    return null;
                }
                _bundleToLoad.Add(dependenceBundle);
                LogHelper.Info("load dependence bundle: {0}", dependenceBundle.AssetBundleName);
            }
            bool anyError = false;
            for (int i = 0; i < _bundleToLoad.Count; i++)
            {
                //              LogHelper.Info("bundle to load [{0}]: {1}", i, _bundleToLoad[i].AssetBundleName);
                AssetBundle assetBundle = AssetBundle.LoadFromFile(_bundleToLoad[i].GetFilePath(_bundleToLoad[i].FileLocation));
                if (null == assetBundle)
                {
                    if (logWhenError)
                    {
                        LogHelper.Error("Load assetbundle {0} failed.", _bundleToLoad[i].AssetBundleName);
                    }
                    anyError = true;
                    break;
                }
                _bundleToLoad[i].AssetDic.Clear();
                for (int j = 0; j < _bundleToLoad[i].AssetNames.Length; j++)
                {
                    UnityEngine.Object asset = assetBundle.LoadAsset(_bundleToLoad[i].AssetNames[j]);
                    if (null == asset)
                    {
                        if (logWhenError)
                        {
                            LogHelper.Error("Load asset {0} from asset bundle {1} failed.", _bundleToLoad[i].AssetNames[j], _bundleToLoad[i].AssetBundleName);
                        }
                        anyError = true;
                        break;
                    }
                    _bundleToLoad[i].AssetDic.Add(_bundleToLoad[i].AssetNames[j], asset);
                }
                _bundleToAddScenary.Add(_bundleToLoad[i]);
                _cachedBundleDic.Add(_bundleToLoad[i].AssetBundleName, _bundleToLoad[i]);
                assetBundle.Unload(false);
            }
            if (anyError)
            {
                // undo all actions in this method
                for (int i = 0; i < _bundleToLoad.Count; i++)
                {
                    CHResBundle cachedBundle;
                    if (_cachedBundleDic.TryGetValue(_bundleToLoad[i].AssetBundleName, out cachedBundle))
                    {
                        cachedBundle.AssetDic.Clear();
                        _cachedBundleDic.Remove(cachedBundle.AssetBundleName);
                    }
                }
                return null;
            }
            for (int i = 0; i < _bundleToAddScenary.Count; i++)
            {
                _bundleToAddScenary[i].ScenaryMask &= scenary;
            }
            LogHelper.Info("CacheBundleAndDependencies done, current cache cnt: {0}", _cachedBundleDic.Count);
            return bundle;
        }

        #endregion

        #region get bundle methods

        private string GetBundleNameByAssetName (string assetNameWithLocaleName, bool isLocaleRes = false, ELocale locale = ELocale.WW)
        {
            if (null == _assetName2BundleName || _assetName2BundleName.Count == 0)
            {
                LogHelper.Error("Manifest not mapped when call GetBundleNameByAssetName({0})", assetNameWithLocaleName);
                return null;
            }
            string bundleName = string.Empty;
            _assetName2BundleName.TryGetValue(assetNameWithLocaleName, out bundleName);
            return bundleName;
        }

        private CHResBundle GetBundleByAssetName (string assetNameWithLocaleName, bool isLocaleRes = false, ELocale locale = ELocale.WW)
        {
            if (null == _assetName2BundleName || _assetName2BundleName.Count == 0 ||
                null == _bundleName2Bundle || _bundleName2Bundle.Count == 0)
            {
                LogHelper.Error("Manifest not mapped when call GetBundleByAssetName({0})", assetNameWithLocaleName);
                return null;
            }
            string bundleName = GetBundleNameByAssetName(assetNameWithLocaleName, isLocaleRes, locale);
            if (string.IsNullOrEmpty(bundleName))
            {
                return null;
            }
            CHResBundle bundle = null;
            _bundleName2Bundle.TryGetValue(bundleName, out bundle);
            return bundle;
        }

        public CHResBundle GetBundleByBundleName (string bundleName)
        {
            if (null == _bundleName2Bundle || _bundleName2Bundle.Count == 0)
            {
                LogHelper.Error("Manifest not mapped when call GetBundleByBundleName({0})", bundleName);
                return null;
            }
            CHResBundle bundle;
            _bundleName2Bundle.TryGetValue(bundleName, out bundle);
            return bundle;
        }
        #endregion
    }
}