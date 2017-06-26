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

		public UnityEngine.Object GetPrefab (EResType resType, string name, int scenary = 0)
		{
            if (RuntimeConfig.Instance.UseAssetBundleRes)
			{
				// todo ab
				return _manifest.GetAsset(name, 0);
			}
			#if UNITY_EDITOR
			else
			{
				if (resType == EResType.Model)
					return _manifest.GetAsset(name, 0, true, ELocale.WW);
				var obj = UnityEditor.AssetDatabase.LoadMainAssetAtPath(ResPathUtility.GetEditorDebugResPath(resType, name));
				if (null == obj)
				{
					obj = UnityEditor.AssetDatabase.LoadMainAssetAtPath(ResPathUtility.GetEditorDebugResPath(resType, name, true, ELocale.WW));
				}
				return obj;
			}
			#else
			return null;
			#endif
		}

        public string GetJson (EResType resType, string name)
        {
            if (RuntimeConfig.Instance.UseAssetBundleRes)
            {
                // todo ab
                return string.Empty;
            }
            #if UNITY_EDITOR
            else
            {
                var textAsset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(ResPathUtility.GetEditorDebugResPath(resType, name)) as UnityEngine.TextAsset;
                return textAsset.text;
            }
			#else
			return string.Empty;
			#endif
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
//				_assetCache = new AssetCache ();
//            }
//            if (null)


//			_assetCache.Init (_manifest);
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