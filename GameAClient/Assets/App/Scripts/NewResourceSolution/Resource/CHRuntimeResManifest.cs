﻿using System;
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
		private List<string> _bundleToCache = new List<string>();
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
//						LogHelper.Info("Regist assetName {0} with {1}", assetNameWithLocaleName, _bundles[i].AssetBundleName);
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

		/// <summary>
		/// 将一个没制作的（预期使用WW资源的）asset的bundle索引重定向到ww的bundle
		/// </summary>
		/// <returns><c>true</c>, if undefined locale asset was redirected, <c>false</c> otherwise.</returns>
		/// <param name="assetName">Asset name.</param>
		/// <param name="locale">Locale.</param>
		public bool RedirectUndefinedLocaleAsset (string assetName, ELocale locale)
		{
			// 查找有没有WW资源
			string assetNameWithLocaleWW = StringUtil.Format(StringFormat.TwoLevelPath, LocaleDefine.LocaleNames[(int)ELocale.WW], assetName);
			string wwBundleName = GetBundleNameByAssetName(assetNameWithLocaleWW);
			if (string.IsNullOrEmpty(wwBundleName))
			{
				return false;
			}
			string assetNameWithLocaleName = StringUtil.Format(StringFormat.TwoLevelPath, LocaleDefine.LocaleNames[(int)locale], assetName);
			_assetName2BundleName.Add(assetNameWithLocaleName, wwBundleName);
			return true;
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
			string bundleName = GetBundleNameByAssetName(assetNameWithLocaleName);
			if (string.IsNullOrEmpty(bundleName))
			{
				// 如果请求的是本地化资源但不是ww资源，则尝试重定向
				if (isLocaleRes && ELocale.WW != locale)
				{
					if (!RedirectUndefinedLocaleAsset(assetName, locale))
					{
						if (logWhenError)
						{
							LogHelper.Error("No bundle contains asset: {0}", assetName);
						}
						return null;
					}
					else
					{
						bundleName = GetBundleNameByAssetName(assetNameWithLocaleName);
					}
				}
				else
				{
	                if (logWhenError)
	                {
	                    LogHelper.Error("No bundle contains asset: {0}", assetName);
	                }
					return null;
				}
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
//			if (_cachedAssetsTotalSize < s_SuggestedAssetMemorySize)
//				return;
			InternalUnloadUnusedAssets (scenaryMask2Unload, false);
		}
		/// <summary>
		/// 强制清理不再使用的asset缓存，不考虑在建议asset缓存大小下尽量多缓存asset
		/// </summary>
//		public void ForceUnloadUnusedAssets (int scenaryMask2Unload)
//		{
//			InternalUnloadUnusedAssets (scenaryMask2Unload, true);
//		}


		/// <summary>
		/// 清除所有asset缓存，不管该缓存是否还有使用
		/// </summary>
		public void ClearAllAssetCache ()
		{
            var itor = _cachedBundleDic.GetEnumerator ();
            while (itor.MoveNext ())
            {
                CHResBundle bundle = itor.Current.Value;
                bundle.UncacheAll ();
            }

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
                    bundle.Uncache (scenaryMask2Unload);
				}
				if (0 == bundle.ScenaryMask)
				{
					_assetToUnload.Add (itor.Current.Key);
                    _cachedAssetsTotalSize -= bundle.Size;
//					if (!force && _cachedAssetsTotalSize < s_SuggestedAssetMemorySize)
//					{
//						break;
//					}
				}
			}
			for (int i = 0; i < _assetToUnload.Count; i++)
			{
				_cachedBundleDic.Remove (_assetToUnload [i]);
			}
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
//			LogHelper.Info("CacheBundleAndDependencies, bundleName: {0}", bundle.AssetBundleName);
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
            _bundleToCache.Clear();
			bool anyError = false;
			GetBundleDependenciesQueue(bundle.AssetBundleName);
			_bundleToCache.Add(bundle.AssetBundleName);
//			string[] dependencies = _unityManifest.GetAllDependencies(bundle.AssetBundleName);
//			LogHelper.Info("Dependencies cnt: {0}", dependencies.Length);
			for (int i = 0; i < _bundleToCache.Count; i++)
			{
				//				LogHelper.Info("Dependence {0}: {1}", i, dependencies[i]);
                // 判断是否已缓存
				CHResBundle dependenceBundle = null;
				if (_cachedBundleDic.TryGetValue(_bundleToCache[i], out dependenceBundle))
				{
					if ((dependenceBundle.ScenaryMask & scenary) == 0)
					{
						if (!dependenceBundle.Cache (scenary, logWhenError))
						{
							anyError = true;
						}
					}
                   	continue;
				}
				dependenceBundle = GetBundleByBundleName(_bundleToCache[i]);
                // 判断bundle数据是否存在
				if (null == dependenceBundle)
				{
                    if (logWhenError)
                    {
                        LogHelper.Error("Load bundle <{0}> failed", _bundleToCache[0]);
                    }
					anyError = true;
					continue;
				}
                // 判断文件是否存在
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
					anyError = true;
					continue;
				}
				if (!dependenceBundle.Cache (scenary, logWhenError))
				{
					anyError = true;
				}
				_cachedAssetsTotalSize += dependenceBundle.Size;
				_cachedBundleDic.Add(dependenceBundle.AssetBundleName, dependenceBundle);
			}
			
//			for (int i = 0; i < _bundleToCache.Count; i++)
//			{
////				LogHelper.Info("bundle to load [{0}]: {1}", i, _bundleToLoad[i].AssetBundleName);
//                if (!_bundleToCache [i].Cache (scenary, logWhenError))
//                {
//                    anyError = true;
//                }
//                _cachedAssetsTotalSize += _bundleToCache [i].Size;
//				_cachedBundleDic.Add(_bundleToCache[i].AssetBundleName, _bundleToCache[i]);
//			}
			if (anyError)
			{
				// undo all actions in this method
				for (int i = 0; i < _bundleToCache.Count; i++)
				{
					CHResBundle dependenceBundle = null;
					if (_cachedBundleDic.TryGetValue(_bundleToCache[i], out dependenceBundle))
					{
						dependenceBundle.Uncache (scenary);
						if (0 == dependenceBundle.ScenaryMask)
						{
							_cachedAssetsTotalSize -= dependenceBundle.Size;
							_cachedBundleDic.Remove(dependenceBundle.AssetBundleName);
						}
					}
				}
				return null;
			}
			return bundle;
		}

	    /// <summary>
	    /// 得到bundle载入顺序列表
	    /// </summary>
	    private void GetBundleDependenciesQueue(string bundleName)
	    {
		    string[] dependencies = _unityManifest.GetDirectDependencies(bundleName);
		    for (int i = 0; i < dependencies.Length; i++)
		    {
			    if (!_bundleToCache.Contains(dependencies[i]))
			    {
				    GetBundleDependenciesQueue(dependencies[i]);
				    _bundleToCache.Add(dependencies[i]);
			    }
		    }
	    }

		#endregion

		#region get bundle methods

		private string GetBundleNameByAssetName (string assetNameWithLocaleName)
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

		private CHResBundle GetBundleByAssetName (string assetNameWithLocaleName)
		{
			if (null == _assetName2BundleName || _assetName2BundleName.Count == 0 ||
				null == _bundleName2Bundle || _bundleName2Bundle.Count == 0)
			{
				LogHelper.Error("Manifest not mapped when call GetBundleByAssetName({0})", assetNameWithLocaleName);
				return null;
			}
			string bundleName = GetBundleNameByAssetName(assetNameWithLocaleName);
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