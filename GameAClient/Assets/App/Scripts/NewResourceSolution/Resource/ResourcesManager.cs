using System;
using System.Collections.Generic;
using System.IO;
using GameA;
using SoyEngine;
using UnityEngine;
using EMessengerType = GameA.EMessengerType;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NewResourceSolution
{
	public class ResourcesManager : Singleton<ResourcesManager>
	{
		#region fields
//		private static readonly int s_maxScenary = 32;

        /// <summary>
        /// 自动执行unload操作最小时间间隔
        /// </summary>
//        private static long s_minimumAutoUnloadActionInternal = 20000;

        /// <summary>
        /// 资源目录
        /// </summary>
        private CHRuntimeResManifest _manifest;
        

//        private AssetCache _assetCache;

	    /// <summary>
        /// 上次进行unload操作的时间
        /// </summary>
//        private long _lastUnloadActionTime;

        #if UNITY_EDITOR
        private Dictionary<string, string> _editorResPathDic = new Dictionary<string, string> ();
        #endif


		#endregion

		#region properties
		#endregion

		#region public methods

		public Object GetPrefab(EResType resType, string name, int scenary = 0)
		{
            return GetAsset<Object>(resType, name, scenary, true, true, LocalizationManager.Instance.CurrentLocale);
		}

        public bool TryGetTexture(string name, out Texture texture, int scenary = 0)
        {
            texture = GetAsset<Texture>(EResType.Texture, name, scenary, false);
            if (null != texture)
            {
                return true;
            }
            return false;
        }

        public bool TryGetSprite(string name, out Sprite sprite, int scenary = 0)
        {
            sprite = GetAsset<Sprite>(EResType.Sprite, name, scenary, false);
            if (null != sprite)
            {
                return true;
            }
            return false;
        }

        public Texture GetTexture (string name, int scenary = 0)
        {
            return GetAsset<Texture>(EResType.Texture, name, scenary);
        }

        public Sprite GetSprite (string name, int scenary = 0)
        {
            return GetAsset<Sprite>(EResType.Sprite, name, scenary);
        }

        public AudioClip GetAudio (string name, int scenary = 0)
        {
            return GetAsset<AudioClip>(EResType.AudioClip, name, scenary);
        }

        public string GetJson (string name, int scenary = 0)
        {
            var textAsset = GetAsset<TextAsset>(EResType.Table, name, scenary);
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
            ) where T : Object
        {
            if (null == name) return null;
            #if UNITY_EDITOR
            if (!RuntimeConfig.Instance.UseAssetBundleRes)
            {
                string registedAssetName = isLocaleRes ?
                    StringUtil.Format(StringFormat.TwoLevelPath, LocaleDefine.LocaleNames[(int)locale], name) :
                    name;
                string assetPath;
                if (!_editorResPathDic.TryGetValue(registedAssetName, out assetPath))
                {
                    if (isLocaleRes && ELocale.WW != locale)
                    {
                        registedAssetName = StringUtil.Format(StringFormat.TwoLevelPath, LocaleDefine.LocaleNames[(int)ELocale.WW], name);
                        _editorResPathDic.TryGetValue(registedAssetName, out assetPath);
                    }
                    else
                    {
                        return null;
                    }
                }
                if (string.IsNullOrEmpty(assetPath))
                {
                    return null;
                }
                var obj = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                return obj;
            }
            #endif
            return _manifest.GetAsset(name, scenary, logWhenError, isLocaleRes, locale) as T;
        }

		public void Init ()
		{
//			_lastUnloadActionTime = 0;
            #if UNITY_EDITOR
            if (!RuntimeConfig.Instance.UseAssetBundleRes)
            {
                InitEditorRes ();
                return;
            }
            #endif

            if (null != _manifest)
            {
                _manifest.ClearAllAssetCache ();
            }

            // 读取本地版本
			if (UnityTools.TryGetObjectFromLocal (ResDefine.CHResManifestFileName, out _manifest))
            {
				_manifest.FileLocation = EFileLocation.Persistent;
				LogHelper.Info ("Res in pesistant, ver: {0}", _manifest.Ver);
            }
			else
			{
				// 不存在则读取包内版本
				UnityTools.TryGetObjectFromStreamingAssets (ResDefine.CHResManifestFileName, out _manifest);
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
				}
            }
			else
			{
				LogHelper.Error ("Init resourcesManager failed, can't find any manifest.");
				// todo 通知玩家，退出程序
			}
		}

        #if UNITY_EDITOR
        private void InitEditorRes ()
        {
            List<EditorResType> editorResTypeList = new List<EditorResType>();
            editorResTypeList.Add(new EditorResType(EResType.Animation, null, true, false));
            editorResTypeList.Add(new EditorResType(EResType.AudioClip, "t:AudioClip", false, false));
            editorResTypeList.Add(new EditorResType(EResType.Font, "t:Font", false, false));
            editorResTypeList.Add(new EditorResType(EResType.Material, "t:Material", false, false));
            editorResTypeList.Add(new EditorResType(EResType.MeshData, "t:Mesh", false, false));
            editorResTypeList.Add(new EditorResType(EResType.Shader, "t:Shader", false, false));
            editorResTypeList.Add(new EditorResType(EResType.SpineData, null, true, false));
            editorResTypeList.Add(new EditorResType(EResType.Sprite, "t:Texture", true, false));
            editorResTypeList.Add(new EditorResType(EResType.Table, "", false, false));
            editorResTypeList.Add(new EditorResType(EResType.Texture, "t:Texture", false, false));
            editorResTypeList.Add(new EditorResType(EResType.ParticlePrefab, "t:Prefab", false, true));
            editorResTypeList.Add(new EditorResType(EResType.UIPrefab, "t:Prefab", false, true));
            editorResTypeList.Add(new EditorResType(EResType.ModelPrefab, "t:Prefab", false, true));
            _editorResPathDic.Clear ();
            List<string> allNormalAssetGuids = new List<string> ();
            Dictionary<string, List<string>> allLoaleAssetGuids = new Dictionary<string, List<string>> ();
            for (int i = 0; i < editorResTypeList.Count; i++)
            {
                EditorResType resType = editorResTypeList [i];
                if (resType.IsLocaleRes)
                {
                    foreach (int j in Enum.GetValues(typeof(ELocale)))             
                    {
                        ELocale targetLocale = (ELocale)j;
                        string localeName = LocaleDefine.LocaleNames[(int)targetLocale];
                        List<string> localeAssets;
                        if (!allLoaleAssetGuids.TryGetValue (localeName, out localeAssets))
                        {
                            localeAssets = new List<string> ();
                            allLoaleAssetGuids.Add (localeName, localeAssets);
                        }

                        string rootPath = ResPathUtility.GetEditorDebugResFolderPath(resType.ResType, true, targetLocale);
                        if (AssetDatabase.IsValidFolder(rootPath))
                        {
//                            LogHelper.Info("{0} rootPath: {1}", resType.ResType, rootPath);
                            if (resType.IsFolderRes)
                            {
                                DirectoryInfo rootDirectoryInfo = new DirectoryInfo (rootPath);
                                var childDirectorys = rootDirectoryInfo.GetDirectories ();
                                for (int k = 0; k < childDirectorys.Length; k++)
                                {
                                    var parts = childDirectorys [k].ToString ().Split (new[] {ResPath.Assets}, StringSplitOptions.None);
                                    string childDirRelatedToUnityProject = 
                                        string.Format (
                                            "{0}{1}",
                                            ResPath.Assets,
                                            parts [parts.Length - 1]
                                        );
                                    var assets = AssetDatabase.FindAssets(resType.SearchFilter, new[] {childDirRelatedToUnityProject});
                                    localeAssets.AddRange (assets);
                                }
                            }
                            else
                            {
                                var assets = AssetDatabase.FindAssets(resType.SearchFilter, new[] {rootPath});
                                localeAssets.AddRange (assets);
                            }
                        }
                    }
                }
                else
                {
                    string rootPath = ResPathUtility.GetEditorDebugResFolderPath(resType.ResType);
                    if (AssetDatabase.IsValidFolder(rootPath))
                    {
//                        LogHelper.Info("{0} rootPath: {1}", resType.ResType, rootPath);
                        if (resType.IsFolderRes)
                        {
                            DirectoryInfo rootDirectoryInfo = new DirectoryInfo (rootPath);
                            var childDirectorys = rootDirectoryInfo.GetDirectories ();
                            for (int k = 0; k < childDirectorys.Length; k++)
                            {
                                var parts = childDirectorys [k].ToString ().Split (new[] {ResPath.Assets}, StringSplitOptions.None);
                                string childDirRelatedToUnityProject = 
                                    string.Format (
                                        "{0}{1}",
                                        ResPath.Assets,
                                        parts [parts.Length - 1]
                                    );
                                var assets = AssetDatabase.FindAssets(resType.SearchFilter, new[] {childDirRelatedToUnityProject});
                                allNormalAssetGuids.AddRange (assets);
                            }
                        }
                        else
                        {
                            var assets = AssetDatabase.FindAssets(resType.SearchFilter, new[] {rootPath});
                            allNormalAssetGuids.AddRange (assets);
                        }
                    }
                    else
                    {
                        LogHelper.Error("{0} asset rootPath invalid, path: {1}", resType.ResType, rootPath);
                    }
                }
            }
            for (int i = 0; i < allNormalAssetGuids.Count; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath (allNormalAssetGuids[i]);
                string assetName = Path.GetFileNameWithoutExtension (assetPath);
                if (assetName != null && !_editorResPathDic.ContainsKey(assetName))
                {
                    _editorResPathDic.Add (assetName, assetPath);
                }
            }
            using (var itor = allLoaleAssetGuids.GetEnumerator())
            {
                while (itor.MoveNext ())
                {
                    string localeName = itor.Current.Key;
                    List<string> assets = itor.Current.Value;
                    for (int i = 0; i < assets.Count; i++)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath (assets[i]);
                        string assetName = Path.GetFileNameWithoutExtension (assetPath);
                        assetName = string.Format (StringFormat.TwoLevelPath, localeName, assetName);
                        if (!_editorResPathDic.ContainsKey(assetName))
                        {
                            _editorResPathDic.Add (assetName, assetPath);
                        }
                    }
                }
            }
        }
        #endif

        public void Update ()
        {
            // 定时进行清理asset缓存，以使内存中资源的大小维持在建议大小以内
//			if (_manifest.CachedAssetTotalSize > CHRuntimeResManifest.s_SuggestedAssetMemorySize)
//            {
//                long now = DateTimeUtil.GetNowTicks () / 10000;
//                if (now - _lastUnloadActionTime > s_minimumAutoUnloadActionInternal)
//                {
//					_manifest.UnloadUnusedAssets (0);
//                }
//            }
        }

        /// <summary>
        /// 卸载某一特定使用场景的asset
        /// </summary>
        /// <param name="scenary">Scenary.</param>
        public void UnloadScenary (int scenary)
        {
            #if UNITY_EDITOR
            if (!RuntimeConfig.Instance.UseAssetBundleRes) return;
            #endif

			int maskToUnload = 1 << scenary;
            _manifest.UnloadUnusedAssets (maskToUnload);
        }

//		public void ForceUnloadScenary (int scenary)
//		{
//			int maskToUnload = 1 << scenary;
//			_inUseScenaryMask &= ~maskToUnload;
//			_manifest.ForceUnloadUnusedAssets (maskToUnload);
//		}
        #endregion

        #region private methods

        #endregion

        #region update&download
		public void CheckApplicationAndResourcesVersion ()
		{
            #if UNITY_EDITOR
            if (!RuntimeConfig.Instance.UseAssetBundleRes)
            {
                Messenger.Broadcast(EMessengerType.OnResourcesCheckStart);
                CoroutineProxy.Instance.StartCoroutine(
                    CoroutineProxy.RunNextFrame(() =>
                    {
                        Messenger.Broadcast(EMessengerType.OnResourcesCheckFinish);
                        SocialApp.Instance.LoginAfterUpdateResComplete();
                    }));
                return;
            }
            #endif
		    if (EFileLocation.StreamingAsset == _manifest.FileLocation)
		    {
		        CoroutineManager.StartCoroutine(VersionUpdater.CheckVerInternal(null, _manifest));
		    }
		    else
		    {
		        CHRuntimeResManifest buildInManifest;
		        UnityTools.TryGetObjectFromStreamingAssets (ResDefine.CHResManifestFileName, out buildInManifest);
		        CoroutineManager.StartCoroutine(VersionUpdater.CheckVerInternal(_manifest, buildInManifest));
		    }
		}
		#endregion

        #if UNITY_EDITOR
        /// <summary>
        /// 生成 运行时读取编辑器资源 所用的目录 的辅助类
        /// </summary>
        private class EditorResType
        {
            public EResType ResType;
            public bool IsFolderRes;
            public bool IsLocaleRes;
            public string SearchFilter;
            public EditorResType (EResType resType, string searchFilter, bool isFolderRes, bool isLocaleRes)
            {
                ResType = resType;
                IsFolderRes = isFolderRes;
                IsLocaleRes = isLocaleRes;
                SearchFilter = searchFilter;
            }
        }
        #endif

	}
}