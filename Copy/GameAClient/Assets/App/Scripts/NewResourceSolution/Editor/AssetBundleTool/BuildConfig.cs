using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEditor;
using UnityEngine;

namespace NewResourceSolution.EditorTool
{
    public class BuildConfig : ScriptableObject
    {
		#region fields
		/// <summary>
		/// 是否是测试打包
		/// </summary>
		[SerializeField] bool _debug;

	    /// <summary>
		/// 打包包含的语言
		/// </summary>
		[SerializeField] int _includeLocales;

        /// <summary>
        /// 全资源出包
        /// </summary>
		[SerializeField] private bool _fullResPackage;

        /// <summary>
        /// 所有打包资源种类
        /// </summary>
        private readonly List<ResList> _allResLists = new List<ResList>();
	    
	    [SerializeField] private string _appVersion;
	    [SerializeField] private string _minimumAppVersion;
	    /// <summary>
	    /// 基础资源版本
	    /// </summary>
	    [SerializeField] private string _baseResVersion;
	    /// <summary>
	    /// 资源版本
	    /// </summary>
	    [SerializeField] private string _resVersion;

        /// <summary>
        /// 单包亚当资源列表（以非压缩的方式拷贝到streamingAssets）
        /// </summary>
        [SerializeField] private List<string> _singleAdamResList = new List<string>();
        /// <summary>
        /// 文件夹包亚当资源列表（以非压缩的方式拷贝到streamingAssets）
        /// </summary>
        [SerializeField] private List<string> _folderAdamResList = new List<string>();
        private ResList.AssetSortHelper _singleAdamResListSortHelper = new ResList.AssetSortHelper();
        private ResList.AssetSortHelper _folderAdamResListSortHelper = new ResList.AssetSortHelper();
		#endregion

		#region properties
		/// <summary>
		/// 是否是测试打包
		/// </summary>
		public bool Debug
		{
    		get
			{
    			return _debug;
    		}
    		set
			{
    			_debug = value;
    		}
    	}
	    
	    public string AppVersion
	    {
		    get { return _appVersion; }
		    set { _appVersion = value; }
	    }

	    public string MinimumAppVersion
	    {
		    get { return _minimumAppVersion; }
		    set { _minimumAppVersion = value; }
	    }

	    public string BaseResVersion
	    {
		    get { return _baseResVersion; }
		    set { _baseResVersion = value; }
	    }
		/// <summary>
		/// 资源版本
		/// </summary>
		public string ResVersion
		{
			get { return _resVersion; }
			set { _resVersion = value; }
		}


	    /// <summary>
		/// 打包包含的语言
		/// </summary>
		public int IncludeLocales
		{
			get 
			{
				return _includeLocales;
			}
			set
			{
				_includeLocales = value;
			}
		}
        /// <summary>
        /// 全资源出包
        /// </summary>
        public bool FullResPackage
        {
            get
            {
                return _fullResPackage;
            }
            set
            {
                _fullResPackage = value;
            }
        }
        /// <summary>
        /// 所有打包资源种类
        /// </summary>
        public List<ResList> AllResLists
        {
            get { return _allResLists; }
        }

	    /// <summary>
        /// 单包亚当资源列表（以非压缩的方式拷贝到streamingAssets）
        /// </summary>
        public List<string> SingleAdamResList
        {
            get { return _singleAdamResList; }
        }
        /// <summary>
        /// 文件夹包亚当资源列表（以非压缩的方式拷贝到streamingAssets）
        /// </summary>
        public List<string> FolderAdamResList
        {
            get { return _folderAdamResList; }
        }
		#endregion

		#region methods
        public BuildConfig ()
        {
            _allResLists.Add (new ResList (EResType.Animation,      null,           true,   false));
            _allResLists.Add (new ResList (EResType.AudioClip,      "t:AudioClip",  false,  false));
            _allResLists.Add (new ResList (EResType.Font,           "t:Font",       false,  false));
            _allResLists.Add (new ResList (EResType.Material,       "t:Material",   false,  false));
            _allResLists.Add (new ResList (EResType.MeshData,       "t:Mesh",       false,  false));
            _allResLists.Add (new ResList (EResType.Shader,         "t:Shader",     false,  false));
            _allResLists.Add (new ResList (EResType.SpineData,      null,           true,   false,		true));
            _allResLists.Add (new ResList (EResType.Sprite,         "t:Texture",    true,   false));
            _allResLists.Add (new ResList (EResType.Table,          "",             false,  false));
            _allResLists.Add (new ResList (EResType.Texture,        "t:Texture",    false,  false));
            _allResLists.Add (new ResList (EResType.ParticlePrefab, "t:Prefab",     false,  true));
            _allResLists.Add (new ResList (EResType.ModelPrefab, 	"t:Prefab",     false,  true));
	        _allResLists.Add (new ResList (EResType.UIPrefab,       "t:Prefab",     false,  true));
	        _allResLists.Add (new ResList (EResType.Behavior,       null,    		 true,  false));
        }

		public void Clear ()
		{
            for (int i = 0; i < _allResLists.Count; i++)
            {
                _allResLists[i].ClearInPackageList ();
            }
		}
		/// <summary>
		/// 判断一个路径的asset是否是随包资源
		/// </summary>
		/// <returns><c>true</c> if this instance is path in package asset the specified path; otherwise, <c>false</c>.</returns>
		/// <param name="path">Path.</param>
		public bool IsPathInPackageAsset (string path)
		{
			string guid = AssetDatabase.AssetPathToGUID(path);
			return IsGuidInPackageAsset(guid);
		}
		/// <summary>
		/// 判断一个guid的asset是否是随包资源
		/// </summary>
		/// <returns><c>true</c> if this instance is GUID in package asset the specified guid; otherwise, <c>false</c>.</returns>
		/// <param name="guid">GUID.</param>
		public bool IsGuidInPackageAsset (string guid)
		{
			if (string.IsNullOrEmpty(guid))
			{
				return false;
			}
            for (int i = 0; i < _allResLists.Count; i++)
            {
                if (_allResLists [i].InPackageContains (guid)) return true;
            }
            return false;
		}
		/// <summary>
		/// 判断一个路径的asset是否是随包资源
		/// </summary>
		/// <returns><c>true</c> if this instance is path in package asset the specified type path; otherwise, <c>false</c>.</returns>
		/// <param name="type">Type.</param>
		/// <param name="path">Path.</param>
		public bool IsPathInPackageAsset (EResType type, string path)
		{
			string guid = AssetDatabase.AssetPathToGUID(path);
			return IsGuidInPackageAsset(type, guid);
		}
		/// <summary>
		/// 判断一个guid的asset是否是随包资源
		/// </summary>
		/// <returns><c>true</c> if this instance is GUID in package asset the specified type guid; otherwise, <c>false</c>.</returns>
		/// <param name="type">Type.</param>
		/// <param name="guid">GUID.</param>
		public bool IsGuidInPackageAsset (EResType type, string guid)
		{
			if (string.IsNullOrEmpty(guid))
			{
				return false;
			}
            for (int i = 0; i < _allResLists.Count; i++)
            {
                if (_allResLists[i].ResType == type && _allResLists [i].InPackageContains (guid)) return true;
            }
            return false;
		}

	    /// <summary>
	    /// Adds asset to list, return guid
	    /// </summary>
	    /// <returns>The in package asset.</returns>
	    /// <param name="type"></param>
	    /// <param name="path">Path.</param>
	    public string AddInPackageAsset (EResType type, string path)
		{
			string guid = AssetDatabase.AssetPathToGUID(path);
			if (string.IsNullOrEmpty(guid))
			{
				LogHelper.Error("Asset <{0}> does not exist.", path);
				return null;
			}
            for (int i = 0; i < _allResLists.Count; i++)
            {
                if (_allResLists [i].ResType == type)
                {
                    if (!_allResLists [i].AddInPackageAsset (guid))
                    {
                        LogHelper.Warning("Asset <{0}> already in list");
                        return null;
                    }
                    return guid;
                }
            }
            LogHelper.Error("Res type <{0}> not supported", type);
            return null;
		}

        public ResList GetResList (EResType type)
		{
            for (int i = 0; i < _allResLists.Count; i++)
            {
                if (type == _allResLists [i].ResType)
                {
                    return _allResLists [i];
                }
            }
            return null;
		}

        public string AddSingleAdamRes (string path)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrEmpty(guid))
            {
                LogHelper.Error("Asset <{0}> does not exist.", path);
                return null;
            }
            if (_singleAdamResList.Contains (guid))
            {
                LogHelper.Warning("Asset <{0}> already in list");
                return null;
            }
            _singleAdamResList.Add (guid);
            _singleAdamResListSortHelper.Sort (_singleAdamResList);
            return guid;
        }
        public string AddFolderAdamRes (string path)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrEmpty(guid))
            {
                LogHelper.Error("Asset <{0}> does not exist.", path);
                return null;
            }
            if (_folderAdamResList.Contains (guid))
            {
                LogHelper.Warning("Asset <{0}> already in list");
                return null;
            }
            _folderAdamResList.Add (guid);
            _folderAdamResListSortHelper.Sort (_folderAdamResList);
            return guid;
        }
		#endregion
    }

    [Serializable]
    public class ResList
    {
        public EResType ResType;
        public List<string> InPackageGuidList = new List<string>();
        public bool IsFolderRes;
        public bool IsLocaleRes;
        public string SearchFilter;
	    public bool SeparateTexture;

        public bool FoldoutInEditorUI;

        private readonly AssetSortHelper _sortHelper;
        
        public ResList (EResType resType, string searchFilter, bool isFolderList, bool isLocaleRes, bool separateTexture = false)
        {
            ResType = resType;
            IsFolderRes = isFolderList;
            IsLocaleRes = isLocaleRes;
            SearchFilter = searchFilter;
	        SeparateTexture = separateTexture;
            FoldoutInEditorUI = true;

            _sortHelper = new AssetSortHelper ();
        }
        
        public void ClearInPackageList ()
        {
            InPackageGuidList.Clear ();
        }
        
        public bool AddInPackageAsset (string guid)
        {
            if (!InPackageGuidList.Contains (guid))
            {
                InPackageGuidList.Add (guid);
                _sortHelper.Sort (InPackageGuidList);
                return true;
            }
            return false;
        }
        public bool RemoveInPackageAsset (string guid)
        {
            if (InPackageGuidList.Contains (guid))
            {
                InPackageGuidList.Remove (guid);
                _sortHelper.Sort (InPackageGuidList);
                return true;
            }
            return false;
        }
        
        public bool InPackageContains (string guid)
        {
            if (InPackageGuidList.Contains (guid)) return true;
            else return false;
        }
        /// <summary>
        /// 辅助guid列表排序的类，按照guid对应的asset文件路径排序
        /// </summary>
        public class AssetSortHelper
        {
            private Dictionary<string, string> _pathToGuid = new Dictionary<string, string>();
            private List<string> _pathList = new List<string>();
            
            public void Sort (List<string> guidList)
            {
                for (int i = 0; i < guidList.Count; i++)
                {
                    if (!_pathToGuid.ContainsValue(guidList[i]))
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guidList[i]);
                        if (!_pathToGuid.ContainsKey(path))
                        {
                            _pathToGuid.Add(path, guidList[i]);
                        }
                    }
                }
                
                // sort
                _pathList.Clear();
	            using (var itor = _pathToGuid.GetEnumerator())
	            {
		            while (itor.MoveNext())
		            {
			            _pathList.Add(itor.Current.Key);
		            }
	            }
                _pathList.Sort();
                
                // remove deleted asset path
                for (int i = _pathList.Count - 1; i >= 0; i--)
                {
                    if (!guidList.Contains(_pathToGuid[_pathList[i]]))
                    {
                        _pathToGuid.Remove(_pathList[i]);
                        _pathList.RemoveAt(i);
                    }
                }
                
                // reform guidList
                guidList.Clear();
                for (int i = _pathList.Count - 1; i >= 0; i--)
                {
                    guidList.Add(_pathToGuid[_pathList[i]]);
                }
            }
        }
    }
}