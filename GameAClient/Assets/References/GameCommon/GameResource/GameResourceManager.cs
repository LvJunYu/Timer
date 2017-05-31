/********************************************************************

** Filename : GameResourceManager
** Author : ake
** Date : 2016/3/14 17:05:23
** Summary : GameResourceManager
***********************************************************************/

using System.Collections.Generic;
using System.IO;
using GameA;
using GameA.Game;
using Spine.Unity;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SoyEngine
{
    public class GameResourceManager:MonoBehaviour
    {
        private SoyResourceManifest _manifest;
        private Dictionary<string, SoyAtlas> _cachedAtlas;
        private Dictionary<string, SkeletonDataAsset> _cachedSpineData;
	    private Dictionary<string, Font> _cachedFont;
	    private Dictionary<string, Texture> _cachedTexture; 
        private Dictionary<string, Object> _cachedMainAsset;
	    private Dictionary<string, AudioClip> _cachedAudioClip;
	    private Dictionary<string, Sprite> _cachedSingleSprite;
	    private Dictionary<string, Texture> _cachedAvatarSpineTexture;


		private PathManager _pathManager;
        private string _gameName;

        private static GameResourceManager _instance;

        #region 属性

        public static GameResourceManager Instance
        {
            get
            {
                return _instance;
            }
        }

	    public PathManager PathMan
	    {
		    get { return _pathManager; }
	    }

        #endregion

        void Awake()
        {
            _instance = this;
        }

        private void OnDestroy()
        {
            
			//atlas
	        {
				var enumerator = _cachedAtlas.GetEnumerator();
				while (enumerator.MoveNext())
				{
					var item = enumerator.Current.Value;
					if (item != null)
					{
						Object.DestroyImmediate(item, true);
					}
				}
			}
			//spineData
			{
				var enumerator = _cachedSpineData.GetEnumerator();
				while (enumerator.MoveNext())
				{
					var item = enumerator.Current.Value;
					if (item != null)
					{
						ResourceUnloadTools.UnloadSpineData(item);
					}
				}
			}
			//textures
			{
				var enumerator = _cachedTexture.GetEnumerator();
				while (enumerator.MoveNext())
				{
					var item = enumerator.Current.Value;
					if (item != null)
					{
						Object.DestroyImmediate(item, true);
					}
				}
			}

			//audioClip
			{
				var enumerator = _cachedAudioClip.GetEnumerator();
				while (enumerator.MoveNext())
				{
					var item = enumerator.Current.Value;
					if (item != null)
					{
						Object.DestroyImmediate(item, true);
					}
				}
			}
			//singleSprite
			{
				var enumerator = _cachedSingleSprite.GetEnumerator();
				while (enumerator.MoveNext())
				{
					var item = enumerator.Current.Value;
					if (item != null)
					{
						Object.DestroyImmediate(item, true);
					}
				}
			}
			//mainAsset
			{
				var enumerator = _cachedMainAsset.GetEnumerator();
				while (enumerator.MoveNext())
				{
					var item = enumerator.Current.Value;
					if (item != null)
					{
						Object.DestroyImmediate(item, true);
					}
				}
			}

            _instance = null;
        }

        public bool Init(string gameName)
        {
            _gameName = gameName;
            _pathManager = new PathManager();
            _manifest = LoadAssetBundleMainAsset<SoyResourceManifest>(_pathManager.GetManifestPath(_gameName), true);
            if (_manifest == null)
            {
                LogHelper.Error("Game {0} load manifest failed path is {1}.", _gameName, _pathManager.GetManifestPath(_gameName));
                return false;
            }
            _manifest.MappingAllAndClearRedundancy();
            _cachedAtlas = new Dictionary<string, SoyAtlas>();
            _cachedSpineData = new Dictionary<string, SkeletonDataAsset>();
			_cachedFont  = new Dictionary<string, Font>();
			_cachedTexture= new Dictionary<string, Texture>();
			_cachedMainAsset = new Dictionary<string, Object>();
			_cachedAudioClip = new Dictionary<string, AudioClip>();
			_cachedSingleSprite = new Dictionary<string, Sprite>();
			_cachedAvatarSpineTexture = new Dictionary<string, Texture>();
#if UNITY_EDITOR
			InitDebugLocalRes();
#endif
			return true;
        }


        public Sprite GetSpriteByName(string spName)
        {
            if (string.IsNullOrEmpty(spName))
            {
                LogHelper.Error("GetSpriteByName failed,{0}", spName);
                return null;
            }
            string atlasName = null;
	        bool useLocale = false;
#if UNITY_EDITOR
			if (SocialApp.Instance.UseLocalDebugRes)
			{
				_localResSpriteAtlasRelationDic.TryGetValue(spName, out atlasName);
				useLocale = true;
			}
#endif
	        if (!useLocale)
	        {
				atlasName = _manifest.GetAtlasNameBySpriteName(spName);
			}
			SoyAtlas atlas = null;
            if (!TrtGetAtlas(atlasName, out atlas))
            {
                LogHelper.Error("Get atlas {0} failed!", atlasName);
                return null;
            }
            return atlas.GetSpriteByName(spName);
        }

        public bool TryGetSpriteByName(string spName, out Sprite sprite)
        {
            sprite = GetSpriteByName(spName);
            return sprite != null;
        }

        public SkeletonDataAsset GetSpineDataByName(string spineDataName,bool addSuffix = true)
        {
	        if (addSuffix)
	        {
				spineDataName = string.Format("{0}_SkeletonData", spineDataName);
			}
			SkeletonDataAsset res;
            if (!TryGetSkeletonDataAssetByName(spineDataName, out res))
            {
                LogHelper.Error("GetSpineDataByName failed spineDataName is {0}",spineDataName);
                return null;
            }
            return res;
        }

	    public byte[] GetConfigFileData(string configName)
	    {
		    string path = _pathManager.GetGameLoadlConfig(configName, _gameName);
		    if (File.Exists(path))
		    {
			    return File.ReadAllBytes(path);
		    }
		    else
		    {
			    return null;
		    }
	    }

		public static T LoadAssetBundleMainAsset<T>(string path, bool unloadFile = true) where T : UnityEngine.Object
		{
			AssetBundle ab = AssetBundle.LoadFromFile(path);
			if (ab == null)
			{
				LogHelper.Error("LoadAssetBundleMainAsset failed path is {0}", path);
				return null;
			}
			T res = ab.mainAsset as T;
			if (unloadFile)
			{
				ab.Unload(false);
			}
			return res;
		}

		public T GetConfigAssetData<T>(string configName)where T : BaseTableAsset
        {
            string path = _pathManager.GetGameLoadlConfig(configName, _gameName);
            if (File.Exists(path))
            {
                return LoadAssetBundleMainAsset<T>(path) as T;
            }
            return null;
        }

        public bool TryGetSpineDataByName(string spineDataName, out SkeletonDataAsset data,bool addSuffix = true)
        {
            data = GetSpineDataByName(spineDataName, addSuffix);
            return data != null;
        }

	    public bool LinkAvatarSpineTexture(SkeletonDataAsset data, string textureName)
	    {
#if UNITY_EDITOR
		    if (SocialApp.Instance.UseLocalDebugRes)
		    {
			    return true;
		    }
#endif
			if (string.IsNullOrEmpty(textureName))
		    {
				LogHelper.Error("LinkAvatarSpineTexture called but textureName is null or empty!");
			    return false;
		    }
		    if (data == null)
		    {
			    LogHelper.Error("LinkAvatarSpineTexture called but SkeletonDataAsset is null!");
			    return false;
		    }
		    if (data.atlasAssets == null || data.atlasAssets[0] == null)
		    {
				LogHelper.Error("LinkAvatarSpineTexture called but SkeletonDataAsset.atlasAssets is null!data.name is {0}", data.name);
			    return false;
		    }
		    var atlasData = data.atlasAssets[0];
		    if (atlasData.materials == null || atlasData.materials.Length == 0)
		    {
			    LogHelper.Error("SkeletonDataAsset {0}.atlasAssets[0] is invalid", data.name);
			    return false;
		    }
			var info = _manifest.GetAssetInfoByName(data.name);
		    if (info.ResType != EResType.SpineData)
		    {
				LogHelper.Error("texture {0} restype is invalid {1}. ", textureName, info.ResType);
			    return false;
		    }
		    var relation = info.GeTextureRelation(textureName);
		    if (relation == null)
		    {
			    LogHelper.Error("SoyAsset {0} GeTextureRelation failed texturename is {1}", info.AssetName,textureName);
			    return false;
		    }
		    Material targetMat = null;
		    for (int i = 0; i < atlasData.materials.Length; i++)
		    {
			    var tmpMat = atlasData.materials[i];
			    if (tmpMat.name == relation.MatName)
			    {
				    targetMat = tmpMat;
				    if (PlatformTools.IsRunInMacEditor())
				    {
					    targetMat.shader = Shader.Find(targetMat.shader.name);
				    }
				    break;
			    }
		    }
		    if (targetMat == null)
		    {
				LogHelper.Error("SkeletonDataAsset {0} doesn't has mat {1}",data.name, relation.MatName);
			    return false;
		    }

			Texture t = null;
		    if (!TryGetAvatarSpineTextureByName(textureName, out t))
		    {
				LogHelper.Error("TryGetAvatarSpineTextureByName {0} failed!", textureName);
			    return false;
		    }
		    targetMat.mainTexture = t;
			return true;
	    }

		public bool TryGetFontAssetByName(string fontName, out Font font)
		{
			if (string.IsNullOrEmpty(fontName))
			{
				LogHelper.Error("TryGetFontAssetByName called but fontName is null or empty");
				font = null;
				return false;
			}

			if (_cachedFont.TryGetValue(fontName, out font))
			{
				return true;
			}
			string wholePath = _pathManager.GetGameLocalAssetBundlePath(fontName, _gameName);
			font = LoadAssetBundleMainAsset<Font>(wholePath);
			if (font == null)
			{
				LogHelper.Error("Get Font {0} failed ,whole path is {1}", fontName, wholePath);
				return false;
			}
			_cachedFont.Add(fontName, font);
			return true;
		}

	    public bool TryGetTextureByName(string textureName, out Texture outTex)
	    {
			if (string.IsNullOrEmpty(textureName))
			{
				LogHelper.Error("TryGetTextureByName called but textureName is null or empty");
				outTex = null;
				return false;
			}

			if (_cachedTexture.TryGetValue(textureName, out outTex))
			{
				return true;
			}
#if UNITY_EDITOR
			if (SocialApp.Instance.UseLocalDebugRes)
			{
				string assetPath = null;
				if (!TryGetDebugResPath(textureName, out assetPath))
				{
					LogHelper.Error("TryGetTextureByName called use debug res but assetPath is invalid!");
					return false;
				}
				var o = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
				if (o != null)
				{
					outTex = Object.Instantiate(o);
					if (outTex != null)
					{
						_cachedTexture.Add(textureName, outTex);
						return true;
					}
				}
				return false;
			}
#endif
			string wholePath = _pathManager.GetGameLocalAssetBundlePath(textureName, _gameName);
			outTex = LoadAssetBundleMainAsset<Texture>(wholePath);
			if (outTex == null)
			{
				LogHelper.Error("Get Texture {0} failed ,whole path is {1}", textureName, wholePath);
				return false;
			}
			_cachedTexture.Add(textureName, outTex);
			return true;
		}

	    public bool TryGetAvatarSpineTextureByName(string textureName, out Texture outTex)
	    {
			if (string.IsNullOrEmpty(textureName))
			{
				LogHelper.Error("TryGetAvatarSpineTextureByName called but textureName is null or empty");
				outTex = null;
				return false;
			}
			if (_cachedAvatarSpineTexture.TryGetValue(textureName, out outTex))
			{
				return true;
			}
			string wholePath = _pathManager.GetGameLocalAssetBundlePath(textureName, _gameName);
			outTex = Resources.Load<Texture> (textureName);
			if (outTex == null) {
				outTex = LoadAssetBundleMainAsset<Texture> (wholePath);
				if (outTex == null) {
					LogHelper.Error ("Get spine Texture {0} failed ,whole path is {1}", textureName, wholePath);
					return false;
				}
			}
			_cachedAvatarSpineTexture.Add(textureName, outTex);
			return true;
		}

	    public bool TryGetAudioClipByName(string audioName, out AudioClip outClip)
	    {
			if (string.IsNullOrEmpty(audioName))
			{
				LogHelper.Error("TryGetAudioClipByName called but audioName is null or empty");
				outClip = null;
				return false;
			}
		    if (_cachedAudioClip.TryGetValue(audioName, out outClip))
		    {
			    return true;
		    }
#if UNITY_EDITOR
			if (SocialApp.Instance.UseLocalDebugRes)
			{
				string assetPath = null;
				if (!TryGetDebugResPath(audioName, out assetPath))
				{
					LogHelper.Error("TryGetAudioClipByName called use debug res but assetPath is invalid!");
					return false;
				}
				var o = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
				if (o != null)
				{
					outClip = Object.Instantiate(o);
					if (outClip != null)
					{
						_cachedAudioClip.Add(audioName, outClip);
						return true;
					}
				}
				return false;
			}
#endif
			string wholePath = _pathManager.GetGameLocalAssetBundlePath(audioName, _gameName);
		    outClip = LoadAssetBundleMainAsset<AudioClip>(wholePath);
		    if (outClip == null)
		    {
			    LogHelper.Error("Get audioClip {0} failed,whole path is {1}",audioName,wholePath);
			    return false;
		    }
			_cachedAudioClip.Add(audioName,outClip);
		    return true;
	    }

	    public bool TryGetSingleSprite(string spriteName, out Sprite outSprite)
	    {
			if (string.IsNullOrEmpty(spriteName))
			{
				LogHelper.Error("TryGetSingleSprite called but spriteName is null or empty");
				outSprite = null;
				return false;
			}
			if (_cachedSingleSprite.TryGetValue(spriteName, out outSprite))
			{
				return true;
			}
#if UNITY_EDITOR
			if (SocialApp.Instance.UseLocalDebugRes)
			{
				string assetPath = null;
				if (!TryGetDebugResPath(spriteName, out assetPath))
				{
					LogHelper.Error("TryGetSingleSprite called use debug res but assetPath is invalid!");
					return false;
				}
				var o = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
				outSprite = o;
				bool res = o != null;
				if (res)
				{
					_cachedSingleSprite.Add(spriteName, outSprite);
				}
				return res;
			}
#endif
			string wholePath = _pathManager.GetGameLocalAssetBundlePath(spriteName, _gameName);
			outSprite = LoadAssetBundleMainAsset<Sprite>(wholePath);
			if (outSprite == null)
			{
				LogHelper.Error("Get SingleSprite {0} failed,whole path is {1}", spriteName, wholePath);
				return false;
			}
			_cachedSingleSprite.Add(spriteName, outSprite);
			return true;
		}

		public Object LoadMainAssetObject(string path)
        {
			if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            if (_cachedMainAsset.ContainsKey(path))
            {
                if (null == _cachedMainAsset [path]) {
                    _cachedMainAsset.Remove (path);
                } else {
                    return _cachedMainAsset [path];
                }
            }

#if UNITY_EDITOR
			if (SocialApp.Instance.UseLocalDebugRes)
			{
				string assetPath = null;
				if (!TryGetDebugResPath(path, out assetPath))
				{
					LogHelper.Error("LoadMainAssetObject called use debug res but assetPath is invalid!");
					return null;
				}
				var o = AssetDatabase.LoadMainAssetAtPath(assetPath);
				if (o != null)
				{
					Object res = GameObject.Instantiate(o,_localResDebugTmpParent.transform);
					_cachedMainAsset.Add(path, res);
					return res;
				}
				else
				{
					LogHelper.Error("LoadMainAssetObject called but debug res {0} is invalid，path is {1}", path, assetPath);
					return null;
				}
			}
#endif

			SoyAssetInfo asset = _manifest.GetAssetInfoByName(path);

            if (asset == null || asset.ResType != EResType.Prefab)
            {
                LogHelper.Error("LoadMainAssetObject {0} falied , by asset == null || asset.ResType != EResType.Prefab! ",path);
                return null;
            }
            if (!LoadDependsList(asset.Depends))
            {
                LogHelper.Error("LoadMainAssetObject {0} failed! LoadDependsList is false ", asset.AssetName);
            }
            SoyMainAsset mainAsset = LoadAssetBundleMainAsset<SoyMainAsset>(_pathManager.GetGameLocalAssetBundlePath(asset.AssetName, _gameName));
            if (mainAsset == null || mainAsset.MainAsset == null)
            {
                LogHelper.Error("LoadMainAssetObject called but mainAsset{0} is invalid!");
                return null;
            }
            if (mainAsset.InjectItems != null)
            {
                for (int i = 0; i < mainAsset.InjectItems.Length; i++)
                {
                    InjectAssetDepends(mainAsset.InjectItems[i]);
                }
            }
            _cachedMainAsset.Add(path, mainAsset.MainAsset);
            return mainAsset.MainAsset;
        }



	    public GameObject LoadClonedGameObject(string path)
	    {
		    Object obj = LoadMainAssetObject(path);
		    if (obj == null)
		    {
			    return null;
		    }
			GameObject go = Instantiate(obj) as GameObject;
		    if (go != null)
		    {
			    CommonTools.ResetTransform(go.transform);
		    }
		    return go;
	    }


        #region private

        private void InjectAssetDepends(InjectItem item)
        {
            if (item == null)
            {
                return;
            }
	        if (!ResourceInjectMan.Instance.GetHandler(item.InjectItemType).InjectDependsAsset(item))
	        {
		        LogHelper.Error("InjectAssetDepends falied item is {0}",item.ToString());
	        }
	        return;
        }

        private bool TrtGetAtlas(string atlasName,out SoyAtlas atlas)
        {
            if (string.IsNullOrEmpty(atlasName))
            {
                LogHelper.Error("TrtGetAtlas called but atlasName is null or empty");
                atlas = null;
                return false;
            }
            if (_cachedAtlas.TryGetValue(atlasName, out atlas))
            {
                return true;
            }
#if UNITY_EDITOR
	        if (SocialApp.Instance.UseLocalDebugRes)
	        {
				string assetPath = null;
				if (!TryGetDebugResPath(atlasName, out assetPath))
				{
					LogHelper.Error("TrtGetAtlas called use debug res but assetPath is invalid!");
					return false;
				}

				var o = AssetDatabase.LoadAssetAtPath<SoyAtlas>(assetPath);
				if (o!=null)
				{
					SoyAtlas resObj = Object.Instantiate(o) as SoyAtlas;
					atlas = resObj;
					_cachedAtlas.Add(atlasName, resObj);
					return resObj!=null;
				}
		        return false;
	        }
#endif
			atlas = LoadAssetBundleMainAsset<SoyAtlas>(_pathManager.GetGameLocalAssetBundlePath(atlasName, _gameName));
            if (atlas == null)
            {
                LogHelper.Error("Get atlas {0} falied ,whole path is {1}", atlasName, _pathManager.GetGameLocalAssetBundlePath(atlasName, _gameName));
                return false;
            }
            _cachedAtlas.Add(atlasName, atlas);
            return true;
        }


        private bool TryGetSkeletonDataAssetByName(string spineDataName, out SkeletonDataAsset data)
        {
            if (string.IsNullOrEmpty(spineDataName))
            {
                LogHelper.Error("TryGetSkeletonDataAssetByName called but spineDataName is null or empty");
                data = null;
                return false;
            }
            if (_cachedSpineData.TryGetValue(spineDataName, out data))
            {
                return true;
            }
#if UNITY_EDITOR
	        if (SocialApp.Instance.UseLocalDebugRes)
	        {
		        string assetPath = null;
		        if (!TryGetDebugResPath(spineDataName, out assetPath))
		        {
					LogHelper.Error("TryGetSkeletonDataAssetByName called use debug res but assetPath is invalid!");
			        return false;
		        }
				
		        var o =  AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(assetPath);
		        if (o!=null)
		        {
					SkeletonDataAsset resObj = CopyDebugResSpineData(o);
			        data = resObj;
					_cachedSpineData.Add(spineDataName, resObj);
			        return resObj != null;
		        }
		        return false;
	        }
#endif
			string wholePath = _pathManager.GetGameLocalAssetBundlePath(spineDataName, _gameName);
            data = LoadAssetBundleMainAsset<SkeletonDataAsset>(wholePath);
            if (data == null)
            {
                LogHelper.Error("Get SkeletonDataAsset {0} falied ,whole path is {1}", spineDataName, wholePath);
                return false;
            }
	        if (Application.platform == RuntimePlatform.OSXEditor)
	        {
				data.atlasAssets[0].materials[0].shader = Shader.Find(data.atlasAssets[0].materials[0].shader.name);
			}
			_cachedSpineData.Add(spineDataName, data);
            return true;
        }

        private bool LoadDependsList(List<string> dependsList)
        {
            if (dependsList == null)
            {
                LogHelper.Error("loadDepends called but depends is null!");
                return false;
            }
            for (int i = 0; i < dependsList.Count; i++)
            {
                SoyAssetInfo info = _manifest.GetAssetInfoByName(dependsList[i]);
                if (info == null)
                {
                    LogHelper.Error("LoadDepends failed depends {0}  {1} is invalid!",i, dependsList[i]);
                    return false;
                }
                if (!LoadDepends(info))
                {
                    LogHelper.Error("LoadDependsList failed ,depends {0} load failed!", dependsList[i]);
                    return false;
                }
            }
            return true;

        }

        private bool LoadDepends(SoyAssetInfo assetInfo)
        {
            if (assetInfo == null)
            {
                LogHelper.Error("LoadDepends called but assetInfo is null!");
                return false;
            }
            switch (assetInfo.ResType)
            {
                case EResType.Atlas:
                {
                    SoyAtlas atlas;
                    if (!TrtGetAtlas(assetInfo.AssetName, out atlas))
                    {
                        return false;
                    }
                    break;
                }
                case EResType.SpineData:
                {
                    SkeletonDataAsset data;
                    if (!TryGetSkeletonDataAssetByName(assetInfo.AssetName, out data))
                    {
                        return false;
                    }
                    break;
                }
				case EResType.Font:
					{
						//字体从包里取 忽略掉
						//Font font;
						//if (!TryGetFontAssetByName(assetInfo.AssetName, out font))
						//{
						//	return false;
						//}
						break;
					}
				case EResType.Texture:
	            {
					Texture data;
					if (!TryGetTextureByName(assetInfo.AssetName, out data))
					{
						return false;
					}
					break;
	            }
				default:
                {
                    LogHelper.Error("LoadDepends called assetInfo.ResType is unexpected {0} asset name is {1} ",assetInfo.ResType,assetInfo.AssetName);
                    return false;
                }
            }

            return true;
        }

#if UNITY_EDITOR

	    private GameObject _localResDebugTmpParent;
	    private Dictionary<string, string> _localDebugResPathDic;
	    private Dictionary<string, string> _localResSpriteAtlasRelationDic;
		private void InitDebugLocalRes()
		{
			_localResDebugTmpParent = new GameObject("_localResDebugTmpParent");
            GameObject.DontDestroyOnLoad (_localResDebugTmpParent);
			_localResDebugTmpParent.SetActive(false);
			string path = Application.dataPath;
			_localDebugResPathDic = new Dictionary<string, string>();
			path = path.Replace('\\', '/');
			int subStartCount = path.LastIndexOf('/');

			string rootPath = Path.Combine(path, BuildToolsConstDefine.ExportAssetRootFolder);
			rootPath= rootPath.Replace('\\', '/');
			///main asset
			PrepareResPathData(string.Format("{0}/{1}", rootPath, BuildToolsConstDefine.ExportMainAssetFolder), subStartCount,false, "*.prefab");
            //图集
            //var atlasAssetPathList = PrepareResPathData(string.Format("{0}/{1}/{2}", rootPath, BuildToolsConstDefine.ExportDependsAssetFolder, BuildToolsConstDefine.ExportDependsAtlasAssetPathFolder),
            // todo 临时改为在客户端工程构造atlas，之后改回在art工程构造atlas
            var atlasAssetPathList = PrepareResPathData (string.Format ("{0}/{1}", path, "App/Resources/Sprite"),
				subStartCount, true, "*.asset");
			InitLocalSpriteAtlasRelation(atlasAssetPathList);
			//音频
			PrepareResPathData(string.Format("{0}/{1}/{2}", rootPath, BuildToolsConstDefine.ExportDependsAssetFolder, BuildToolsConstDefine.ExportDependsAudioPathFolder),
				subStartCount, false, "*.ogg", "*.mp3");
			//sigle sprite
			PrepareResPathData(string.Format("{0}/{1}/{2}", rootPath, BuildToolsConstDefine.ExportDependsAssetFolder, BuildToolsConstDefine.ExportDependsSingleSpriteFolder),
				subStartCount, false,"*.jpg", "*.png");
			//spine data
			PrepareResPathData(string.Format("{0}/{1}/{2}", rootPath, BuildToolsConstDefine.ExportDependsAssetFolder, BuildToolsConstDefine.ExportDependsSpineDataPathFolder),
				subStartCount, false,"*_SkeletonData.asset");
			//texture
			PrepareResPathData(string.Format("{0}/{1}/{2}",rootPath,BuildToolsConstDefine.ExportDependsAssetFolder,BuildToolsConstDefine.ExportDependsTextureAssetPathFolder),
				subStartCount, false, "*.jpg", "*.png");
		}

	    private List<string> PrepareResPathData(string rootPath, int subStartCount,bool returnAssetPathList,params string[] searchPatterns)
	    {
		    if (!Directory.Exists(rootPath))
		    {
				LogHelper.Debug("PrepareResPathData called but rootPath {0} is ", rootPath);
			    return null;
		    }
			DirectoryInfo dir = new DirectoryInfo(rootPath);
		    List<string> resList = null;
		    if (returnAssetPathList)
		    {
				resList = new List<string>();
			}
		    for (int a = 0; a < searchPatterns.Length; a++)
		    {
			    var searchPattern = searchPatterns[a];
				var files = dir.GetFiles(searchPattern, SearchOption.AllDirectories);
				for (int i = 0; i < files.Length; i++)
				{
					var item = files[i];
					var tmpAssetNamePath = item.FullName.Replace('\\', '/');
					tmpAssetNamePath = tmpAssetNamePath.Substring(subStartCount + 1, tmpAssetNamePath.Length - subStartCount - 1);
					string tmpName = IngnoreFileSuffix(item.Name);
					if (_localDebugResPathDic.ContainsKey(tmpName))
					{
						LogHelper.Error("PrepareResPathData called but ,fileName {0} duplicate!", item.Name);
						continue;
					}
					_localDebugResPathDic.Add(tmpName, tmpAssetNamePath);
					if (resList != null)
					{
						resList.Add(tmpAssetNamePath);
					}
				}
			}
		    return resList;
	    }

	    private void InitLocalSpriteAtlasRelation(List<string> atlasAssetPathList)
	    {
		    if (atlasAssetPathList == null)
		    {
			    return;
		    }
			_localResSpriteAtlasRelationDic = new Dictionary<string, string>();

			for (int i = 0; i < atlasAssetPathList.Count; i++)
		    {
			    var item = atlasAssetPathList[i];
				var o = AssetDatabase.LoadAssetAtPath<SoyAtlas>(item);
			    if (o == null)
			    {
					LogHelper.Error("AssetDatabase.LoadAssetAtPath<SoyAtlas>(item) is null! item is {0}",item);
				    continue;
			    }
			    for (int j = 0; j < o.CachedSprites.Count; j++)
			    {
				    var sp = o.CachedSprites[j];
				    if (sp == null)
				    {
						LogHelper.Error("atlas {0} index {1} sprite is null!",o.name,j);
					    continue;
				    }
				    if (_localResSpriteAtlasRelationDic.ContainsKey(sp.name))
				    {
						LogHelper.Error("sprite {0} is duplicated !! atlas name is {1}.", sp.name,o.name);
					    continue;
				    }
					_localResSpriteAtlasRelationDic.Add(sp.name,o.name);
				}
			}
	    }

	    private string IngnoreFileSuffix(string fileName)
	    {
		    if (string.IsNullOrEmpty(fileName))
		    {
				LogHelper.Error("IngnoreFileSuffix called but fileName is null or empty!");
			    return "";
		    }
		    int index = fileName.LastIndexOf('.');
		    if (index <= 0)
		    {
			    return fileName;
		    }
		    else
		    {
			    return fileName.Substring(0, index);
		    }
	    }

	    private bool TryGetDebugResPath(string assetName,out string outValue)
	    {
		    return _localDebugResPathDic.TryGetValue(assetName, out outValue);
	    }

	    private SkeletonDataAsset CopyDebugResSpineData(SkeletonDataAsset data)
	    {
		    if (data == null)
		    {
			    return null;
		    }
		    SkeletonDataAsset res = Object.Instantiate(data);
			res.atlasAssets = new AtlasAsset[data.atlasAssets.Length];
		    for (int i = 0; i < data.atlasAssets.Length; i++)
		    {
			    res.atlasAssets[i] = Object.Instantiate(data.atlasAssets[i]);
				res.atlasAssets[i].materials = new Material[data.atlasAssets[i].materials.Length];
				for (int j = 0; j < res.atlasAssets[i].materials.Length; j++)
			    {
				    res.atlasAssets[i].materials[j] = Object.Instantiate(data.atlasAssets[i].materials[j]);
			    }
		    }
			res.scale = 1f / BuildToolsConstDefine.SpirtePixelsPerUnit;
			return res;
	    }
#endif

#endregion
	}
}

