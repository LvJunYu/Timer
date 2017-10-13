using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using Object = UnityEngine.Object;

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
		
	    private HashSet<string> _separateTextureBundleNameSet = new HashSet<string>();
		/// <summary>
		/// ResourcesManager的manifest缓存
		/// </summary>
		private CHRuntimeResManifest _manifest;
		/// <summary>
		/// unity 资源目录
		/// </summary>
		private AssetBundleManifest _unityManifest;

	    
	    private Dictionary<string, string>[] _assetName2BundleNameDictAry = new Dictionary<string, string>[(int) EResType.Max];
	    private Dictionary<string, CHResBundle> _bundleName2BundleDict = new Dictionary<string, CHResBundle>();
	    
		#endregion

		#region properties
		/// <summary>
		/// 缓存asset所占用的总大小
		/// </summary>
		public long CachedAssetTotalSize
		{
			get
			{
				return _cachedAssetsTotalSize;
			}
		}

	    public Dictionary<string, CHResBundle> BundleName2BundleDict
	    {
		    get { return _bundleName2BundleDict; }
	    }

	    #endregion

		#region methods
	    public CHRuntimeResManifest(){}
        public CHRuntimeResManifest(Version version) : base(version) {}

		public void MapBundles ()
		{
			_bundleName2BundleDict.Clear();
			for (int i = 0; i < _assetName2BundleNameDictAry.Length; i++)
			{
				if (_assetName2BundleNameDictAry[i] == null)
				{
					_assetName2BundleNameDictAry[i] = new Dictionary<string, string>();
				}
				else
				{
					_assetName2BundleNameDictAry[i].Clear();
				}
			}
			for (int i = 0; i < _bundles.Count; i++)
			{
				_bundles[i].IsLocaleRes = _bundles[i].AssetBundleName[0] == ResDefine.LocaleResBundleNameFirstChar;
				if (_bundles[i].IsLocaleRes)
				{
					_bundles[i].LocaleName = _bundles[i].AssetBundleName.Substring(1).Split(ResDefine.ReplaceSplashCharInAssetBundleName)[0].ToUpper();
				}
				for (int j = 0; j < _bundles[i].AssetNames.Length; j++)
				{
                    string assetNameWithLocaleName = _bundles[i].IsLocaleRes ?
						StringUtil.Format(StringFormat.TwoLevelPath, _bundles[i].LocaleName, _bundles[i].AssetNames[j]) :
						_bundles[i].AssetNames[j];
					if (!_assetName2BundleNameDictAry[(int) _bundles[i].ResType].ContainsKey(assetNameWithLocaleName))
					{
//						LogHelper.Info("Regist assetName {0} with {1}", assetNameWithLocaleName, _bundles[i].AssetBundleName);
						_assetName2BundleNameDictAry[(int) _bundles[i].ResType].Add(assetNameWithLocaleName, _bundles[i].AssetBundleName);
					}
					else
					{
						LogHelper.Error("Asset name dumplicate in manifest, name: {0}", _bundles[i].AssetNames[j]);
					}
				}
				if (!_bundleName2BundleDict.ContainsKey(_bundles[i].AssetBundleName))
                {
	                _bundleName2BundleDict.Add(_bundles[i].AssetBundleName, _bundles[i]);
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
	    /// <param name="resType"></param>
	    /// <param name="assetName">Asset name.</param>
	    /// <param name="locale">Locale.</param>
	    public bool RedirectUndefinedLocaleAsset (EResType resType, string assetName, ELocale locale)
		{
			// 查找有没有WW资源
			string assetNameWithLocaleWW = StringUtil.Format(StringFormat.TwoLevelPath, LocaleDefine.LocaleNames[(int)ELocale.WW], assetName);
			string wwBundleName = GetBundleNameByAssetName(resType, assetNameWithLocaleWW);
			if (string.IsNullOrEmpty(wwBundleName))
			{
				return false;
			}
			string assetNameWithLocaleName = StringUtil.Format(StringFormat.TwoLevelPath, LocaleDefine.LocaleNames[(int)locale], assetName);
			_assetName2BundleNameDictAry[(int) resType].Add(assetNameWithLocaleName, wwBundleName);
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

		public Object GetAsset (
			EResType resType,
            string assetName,
            int scenary,
			bool withTexDependency,
            bool logWhenError,
			bool isLocaleRes = false,
            ELocale locale = ELocale.WW)
		{
            string assetNameWithLocaleName = isLocaleRes ?
                StringUtil.Format(StringFormat.TwoLevelPath, LocaleDefine.LocaleNames[(int)locale], assetName) :
                    assetName;
			string bundleName = GetBundleNameByAssetName(resType, assetNameWithLocaleName);
			if (string.IsNullOrEmpty(bundleName))
			{
				// 如果请求的是本地化资源但不是ww资源，则尝试重定向
				if (isLocaleRes && ELocale.WW != locale)
				{
					if (!RedirectUndefinedLocaleAsset(resType, assetName, locale))
					{
						if (logWhenError)
						{
							LogHelper.Error("No bundle contains asset: {0}", assetName);
						}
						return null;
					}
					bundleName = GetBundleNameByAssetName(resType, assetNameWithLocaleName);
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
			Object asset;
			if (!_cachedBundleDic.TryGetValue(bundleName, out bundle))
			{
				// try load
				bundle = CacheBundleAndDependencies(bundleName, scenary, withTexDependency, logWhenError);
			}
			if (null != bundle)
			{
				if (bundle.TryGetAsset(assetName, out asset, scenary, logWhenError))
				{
					return asset;
				}
			}
			return null;
		}

		/// <summary>
		/// 清理不再使用的asset缓存，考虑在建议asset缓存大小下尽量多缓存asset，如果清理完毕后剩余的缓存大小仍然超过警报大小，则进行警报
		/// </summary>
		public void UnloadUnusedAssets (long scenaryMask2Unload, long resTypeMask = -1L)
		{
//			if (_cachedAssetsTotalSize < s_SuggestedAssetMemorySize)
//				return;
			InternalUnloadUnusedAssets (scenaryMask2Unload, resTypeMask);
		}
		/// <summary>
		/// 强制清理不再使用的asset缓存，不考虑在建议asset缓存大小下尽量多缓存asset
		/// </summary>
//		public void ForceUnloadUnusedAssets (int scenaryMask2Unload)
//		{
//			InternalUnloadUnusedAssets (scenaryMask2Unload, true);
//		}
		
	    public void ForceUnloadByAssetName(EResType resType, string assetName)
		{
			var bundleName = GetBundleNameByAssetName(resType, assetName);
			CHResBundle bundle;
			if (!_cachedBundleDic.TryGetValue(bundleName, out bundle))
			{
				return;
			}
			_cachedAssetsTotalSize -= bundle.Size;
			bundle.UncacheAll();
			_cachedBundleDic.Remove(bundleName);
		}

	    public void SetupTexture(EResType resType, string assetName, string materialName, int scenary)
	    {
		    var bundleName = GetBundleNameByAssetName(resType, assetName);
		    CHResBundle bundle;
		    if (!_cachedBundleDic.TryGetValue(bundleName, out bundle))
		    {
			    return;
		    }
		    if (bundle.MaterialDependencies == null)
		    {
			    LogHelper.Error("SetupTexture Failed, MaterialDependencies is Null, resType: {0}, assetName: {1}", resType,
				    assetName);
			    return;
		    }
		    var md = Array.Find(bundle.MaterialDependencies, dependency => dependency.MaterialName == materialName);
		    if (md == null)
		    {
			    LogHelper.Error("SetupTexture Failed, Material is not exsit, resType: {0}, assetName: {1}, matName: {2}",
				    resType, assetName, materialName);
			    return;
		    }
		    Object obj;
		    if (!bundle.TryGetAsset(materialName, out obj, scenary, true))
		    {
			    LogHelper.Error("SetupTexture Failed, Material obj is not in bundle, resType: {0}, assetName: {1}, matName: {2}",
				    resType, assetName, materialName);
			    return;
		    }
		    Material mat = obj as Material;
		    if (mat == null)
		    {
			    LogHelper.Error("SetupTexture Failed, Material is not in bundle, resType: {0}, assetName: {1}, matName: {2}",
				    resType, assetName, materialName);
			    return;
		    }
		    for (int i = 0; i < md.TextureProperties.Length; i++)
		    {
			    var tp = md.TextureProperties[i];
			    var o = GetAsset(EResType.Texture, tp.TextureName, scenary, true, true);
			    var t = o as Texture;
			    if (t == null)
			    {
				    LogHelper.Error("SetupTexture Failed, Texture is Null, resType: {0}, assetName: {1}, matName: {2}, textureName: {3}",
					    resType, assetName, materialName, tp.TextureName);
				    continue;
			    }
			    mat.SetTexture(tp.PropertyName, t);
		    }
	    }


		/// <summary>
		/// 清除所有asset缓存，不管该缓存是否还有使用
		/// </summary>
		public void ClearAllAssetCache(bool force = true)
		{
			using (var itor = _cachedBundleDic.GetEnumerator())
			{
				while (itor.MoveNext ())
				{
					CHResBundle bundle = itor.Current.Value;
					bundle.UncacheAll (force);
				}
			}

			_cachedBundleDic.Clear ();
			_cachedAssetsTotalSize = 0;
			Resources.UnloadUnusedAssets ();
		}

		private void InternalUnloadUnusedAssets (long scenaryMask2Unload, long resTypeMask)
		{
			_assetToUnload.Clear ();
			using (var itor = _cachedBundleDic.GetEnumerator())
			{
				while (itor.MoveNext ())
				{
					CHResBundle bundle = itor.Current.Value;
					if ((1L << (int) bundle.ResType & resTypeMask) == 0)
					{
						continue;
					}
					if (0 != scenaryMask2Unload)
					{
						bundle.UncacheMask (scenaryMask2Unload);
					}
					if (0 == bundle.ScenaryMask)
					{
						_assetToUnload.Add (itor.Current.Key);
						_cachedAssetsTotalSize -= bundle.Size;
					}
				}
			}
			for (int i = 0; i < _assetToUnload.Count; i++)
			{
				_cachedBundleDic.Remove (_assetToUnload [i]);
			}
			Resources.UnloadUnusedAssets ();
			if (_cachedAssetsTotalSize > s_WarningAssetMemorySize)
			{
				// todo 发出警报
			}
		}


	    /// <summary>
	    /// 缓存bundle和他的依赖
	    /// </summary>
	    /// <param name="bundleName"></param>
	    /// <param name="scenary">Scenary.</param>
	    /// <param name="withTexDependency"></param>
	    /// <param name="logWhenError"></param>
	    private CHResBundle CacheBundleAndDependencies (string bundleName, int scenary, bool withTexDependency, bool logWhenError)
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
			return CacheBundleAndDependencies(bundle, scenary, withTexDependency, logWhenError);
		}

	    /// <summary>
	    /// 缓存bundle和他的依赖
	    /// </summary>
	    /// <param name="bundle">Bundle.</param>
	    /// <param name="scenary">Scenary.</param>
	    /// <param name="withTexDependency"></param>
	    /// <param name="logWhenError"></param>
	    private CHResBundle CacheBundleAndDependencies (CHResBundle bundle, int scenary, bool withTexDependency, bool logWhenError)
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
			GetBundleDependenciesQueue(bundle.AssetBundleName, withTexDependency);
			_bundleToCache.Add(bundle.AssetBundleName);

//			string[] dependencies = _unityManifest.GetAllDependencies(bundle.AssetBundleName);
//			LogHelper.Info("Dependencies cnt: {0}", dependencies.Length);
			for (int i = 0; i < _bundleToCache.Count; i++)
			{
				//				LogHelper.Info("Dependence {0}: {1}", i, dependencies[i]);
                // 判断是否已缓存
				CHResBundle dependenceBundle;
				if (_cachedBundleDic.TryGetValue(_bundleToCache[i], out dependenceBundle))
				{
					if ((dependenceBundle.ScenaryMask & (1 << scenary)) == 0)
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
					CHResBundle dependenceBundle;
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
	    private void GetBundleDependenciesQueue(string bundleName, bool withTexDependency)
	    {
		    if (!withTexDependency)
		    {
			    _separateTextureBundleNameSet.Clear();
			    var bundle = GetBundleByBundleName(bundleName);
			    if (bundle.MaterialDependencies != null)
			    {
				    for (int i = 0; i < bundle.MaterialDependencies.Length; i++)
				    {
					    var md = bundle.MaterialDependencies[i];
					    for (int j = 0; j < md.TextureProperties.Length; j++)
					    {
						    var tp = md.TextureProperties[j];
						    _separateTextureBundleNameSet.Add(GetBundleNameByAssetName(EResType.Texture, tp.TextureName));
					    }
				    }
			    }
		    }
		    string[] dependencies = _unityManifest.GetDirectDependencies(bundleName);
		    for (int i = 0; i < dependencies.Length; i++)
		    {
			    if (!withTexDependency)
			    {
				    if (_separateTextureBundleNameSet.Contains(dependencies[i]))
				    {
					    continue;
				    }
			    }
			    if (!_bundleToCache.Contains(dependencies[i]))
			    {
				    GetBundleDependenciesQueue(dependencies[i], withTexDependency);
				    _bundleToCache.Add(dependencies[i]);
			    }
		    }
	    }

		#endregion

		#region get bundle methods

		private string GetBundleNameByAssetName (EResType resType, string assetNameWithLocaleName)
		{
			if (null == _assetName2BundleNameDictAry[(int) resType] || _assetName2BundleNameDictAry[(int) resType].Count == 0)
			{
				LogHelper.Error("Manifest not mapped when call GetBundleNameByAssetName({0})", assetNameWithLocaleName);
				return null;
			}
			string bundleName;
			_assetName2BundleNameDictAry[(int) resType].TryGetValue(assetNameWithLocaleName, out bundleName);
			return bundleName;
		}

//		private CHResBundle GetBundleByAssetName (string assetNameWithLocaleName)
//		{
//			if (null == _assetName2BundleName || _assetName2BundleName.Count == 0 ||
//				null == _bundleName2Bundle || _bundleName2Bundle.Count == 0)
//			{
//				LogHelper.Error("Manifest not mapped when call GetBundleByAssetName({0})", assetNameWithLocaleName);
//				return null;
//			}
//			string bundleName = GetBundleNameByAssetName(assetNameWithLocaleName);
//			if (string.IsNullOrEmpty(bundleName))
//			{
//				return null;
//			}
//			CHResBundle bundle = null;
//			_bundleName2Bundle.TryGetValue(bundleName, out bundle);
//			return bundle;
//		}

		public CHResBundle GetBundleByBundleName (string bundleName)
		{
			if (null == _bundleName2BundleDict || _bundleName2BundleDict.Count == 0)
			{
				LogHelper.Error("Manifest not mapped when call GetBundleByBundleName({0})", bundleName);
				return null;
			}
			CHResBundle bundle;
			_bundleName2BundleDict.TryGetValue(bundleName, out bundle);
			return bundle;
		}
		#endregion
    }
}