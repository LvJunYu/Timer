///********************************************************************
//
//** Filename : SoyAtlasEditor
//** Author : ake
//** Date : 2016/3/8 18:12:19
//** Summary : SoyAtlasEditor
//***********************************************************************/
//
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using SoyEngine;
//using UnityEditor;
//using UnityEngine;
//using Object = System.Object;
//
//public class SoyAtlasEditor
//{
//	public const string SoyAltasConfig = "SoyAltasConfig.txt";
//
//	private static string _sPath;
//
//    private static string Path
//    {
//        get
//        {
//            if (string.IsNullOrEmpty(_sPath))
//            {
//                _sPath = Application.dataPath.Replace("Assets", "");
//            }
//            return _sPath;
//        }
//    }
//
//	public enum ETextureCompressQuality
//	{
//		ShowAs16Bit = 0,
//		NoCompress,
//		Compress,
//	}
//
//	public enum ESpritePivot
//	{
//		Center =0,
//		TopLeft,
//		Top,
//		TopRight,
//		Left,
//		Right,
//		BottomLeft,
//		Bottom,
//		BottomRight,
//		Custom,
//	}
//
//	public class SoyAtlasAttribute
//	{
//		public string AtlasName;
//		public ETextureCompressQuality Quality = ETextureCompressQuality.ShowAs16Bit;
//		public ESpritePivot Pivot = ESpritePivot.Center;
//	}
//
//    /// <summary>
//	///     批量修改，图集发生文件增减时需要调用
//    /// </summary>
//    [MenuItem("JoyTools/AtlasTools/ModifySingleSprites")]
//    public static void ModifySingleSprites()
//    {
//        string atlasRootPath = string.Format("{0}/{1}/{2}/{3}", Application.dataPath,
//        BuildToolsConstDefine.ExportAssetRootFolder, BuildToolsConstDefine.ExportDependsAssetFolder,
//        BuildToolsConstDefine.ExportDependsSingleSpriteFolder);
//        string[] fileInfo = GetTexturePath(atlasRootPath);
//        int length = fileInfo.Length;
//        for (int i = 0; i < length; i++)
//        {
//            //获取资源路径
//            string path = fileInfo[i];
//            var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
//            if (textureImporter == null)
//            {
//                LogHelper.Error("ModifySingleSprites called but AssetImporter.GetAtPath(path) as TextureImporter is null! path is {0}", path);
//                return;
//            }
//            //Texture Type
//            textureImporter.textureType = TextureImporterType.Sprite;
//            textureImporter.spriteImportMode = SpriteImportMode.Single;
//            textureImporter.spritePackingTag = null;
//            textureImporter.mipmapEnabled = false;
//            textureImporter.filterMode = FilterMode.Point;
//            textureImporter.spritePixelsPerUnit = 128;
//            textureImporter.textureFormat = TextureImporterFormat.ARGB16;
//            textureImporter.fadeout = false;
//            textureImporter.wrapMode = TextureWrapMode.Repeat;
//            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
//            if (sprite == null)
//            {
//                LogHelper.Error("LoadAssetAtPath failed,path is {0}", path);
//                return;
//            }
//            if (sprite.name.StartsWith("Earth"))
//            {
//                var spritePivot = Vector2.one * 0.5f;
//                var size = new Vector2(sprite.textureRect.width, sprite.textureRect.height);
//                var nameStrs = sprite.name.Split('_');
//                int value;
//                if (int.TryParse(nameStrs[1],out value))
//                {
//                    if (HasDir(EDir.Up,value))
//                    {
//                        spritePivot.y = 0.5f + (size.y - 128) * 0.5f / size.y;
//                    }
//                    if (HasDir(EDir.Down, value))
//                    {
//                        spritePivot.y = 0.5f - (size.y - 128) * 0.5f / size.y;
//                    }
//                    if (HasDir(EDir.Left, value))
//                    {
//                        spritePivot.x = 0.5f - (size.x - 128) * 0.5f / size.x;
//                    }
//                    if (HasDir(EDir.Right, value))
//                    {
//                        spritePivot.x = 0.5f + (size.x - 128) * 0.5f / size.x;
//                    }
//                    textureImporter.spritePivot = spritePivot;
//                }
//            }
//            textureImporter.SaveAndReimport();
//        }
//        AssetBuilder.SaveAssets();
//		L.Log ("ModifySingleSprites done.");
//    }
//    public static bool HasDir(EDir eDir,int value)
//    {
//        return (value & (1 << (int)eDir)) != 0;
//    }
//
//    public enum EDir
//    {
//        Left,
//        Down,
//        Right,
//        Up
//    }
//
//	private static TextureImporterSettings _tmpImporterSettings;
//
//    /// <summary>
//    ///     批量修改，图集发生文件增减时需要调用
//    /// </summary>
//    [MenuItem("JoyTools/AtlasTools/ModifyAtlasTextures")]
//    public static void ModifyAtlasTextures()
//    {
//		_tmpImporterSettings = new TextureImporterSettings();
//		List<string> list = GetAtlasDirList();
//        for (int i = 0; i < list.Count; i++)
//        {
//            LoopSetTexture(list[i]);
//        }
//        AssetBuilder.SaveAssets();
//	    _tmpImporterSettings = null;
//		L.Log ("ModifyAtlasTextures done.");
//    }
//    /// <summary>
//    ///     设置贴图属性
//    /// </summary>
//    public static void ReImporterTexture(string path,string atlasName,TextureImporterPlatformSettings[] pSettings,ESpritePivot pivot)
//    {
//		var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
//        if (textureImporter == null)
//        {
//            L.Error(
//                "ReImporterTexture called but AssetImporter.GetAtPath(path) as TextureImporter is null! path is {0}",
//                path);
//            return;
//        }
//
//
//
//		textureImporter.ReadTextureSettings(_tmpImporterSettings);
//
////		textureImporter.SetPlatformTextureSettings(pSettings[0]);
//		textureImporter.SetPlatformTextureSettings ("Standalone", pSettings [0].maxTextureSize, pSettings [0].format, pSettings[0].compressionQuality, false);
//		textureImporter.SetPlatformTextureSettings ("iPhone", pSettings [1].maxTextureSize, pSettings [1].format, pSettings[1].compressionQuality, false);
////		textureImporter.SetPlatformTextureSettings ("Android", pSettings [2].maxTextureSize, pSettings [2].format, (int)TextureCompressionQuality.Fast, false);
//		textureImporter.SetPlatformTextureSettings ("Android", pSettings [2].maxTextureSize, pSettings [2].format, pSettings[2].compressionQuality, false);
//
//	    ModifySpriteTextureImporterSetting(_tmpImporterSettings, pivot);
//		textureImporter.SetTextureSettings(_tmpImporterSettings);
//
//		//Texture Type
//		textureImporter.textureType = TextureImporterType.Sprite;
//		textureImporter.spriteImportMode = SpriteImportMode.Single;
//		textureImporter.spritePackingTag = atlasName;
//		textureImporter.fadeout = false;
//		textureImporter.spriteBorder = _tmpImporterSettings.spriteBorder;
//
//		textureImporter.SaveAndReimport();
//		return;
//
//	}
//
//	private static TextureImporterSettings ModifySpriteTextureImporterSetting(TextureImporterSettings res,ESpritePivot pivot)
//	{
//		res.spriteAlignment = (int) pivot;
//		res.alphaIsTransparency = true;
//		res.spritePixelsPerUnit = 128;
//		res.mipmapEnabled = false;
//		res.filterMode = FilterMode.Bilinear;
//		res.textureType = TextureImporterType.Sprite;
//		res.spriteMode = (int) SpriteImportMode.Single;
//		return res;
//	}
//
//	private static TextureImporterPlatformSettings[] GetSpriteTextureImportPlatformSettings(ETextureCompressQuality q)
//	{
//		TextureImporterPlatformSettings defaultSetting = new TextureImporterPlatformSettings();
//		defaultSetting.compressionQuality = (int)GetTextureImporterFormatByETextureCompressQuality(q);
//		defaultSetting.maxTextureSize = 1024;
//		TextureImporterPlatformSettings iosSetting = new TextureImporterPlatformSettings();
//		iosSetting.compressionQuality = (int)GetTextureImporterFormatByETextureCompressQuality(q);
//		iosSetting.maxTextureSize = 1024;
//		iosSetting.format = TextureImporterFormat.PVRTC_RGBA4;
//		TextureImporterPlatformSettings androidSetting = new TextureImporterPlatformSettings();
//		androidSetting.compressionQuality = (int)GetTextureImporterFormatByETextureCompressQuality(q);
//		androidSetting.maxTextureSize = 1024;
//		androidSetting.format = TextureImporterFormat.ETC2_RGBA8;
//		return new TextureImporterPlatformSettings[3]{defaultSetting, iosSetting, androidSetting};
//	}
//
//	private static TextureCompressionQuality GetTextureImporterFormatByETextureCompressQuality(ETextureCompressQuality quality)
//	{
//		if (quality == ETextureCompressQuality.Compress)
//		{
//			return TextureCompressionQuality.Fast;
//		}
//		else if (quality == ETextureCompressQuality.ShowAs16Bit)
//		{
//			return TextureCompressionQuality.Normal;
//		}
//		else
//		{
//			return TextureCompressionQuality.Best;
//		}
//	}
//
//
//	private static List<string> GetAtlasDirList()
//    {
//        //string atlasRootPath = string.Format("{0}/{1}/{2}/{3}", Application.dataPath,
//            //BuildToolsConstDefine.ExportAssetRootFolder, BuildToolsConstDefine.ExportDependsAssetFolder,
//            //BuildToolsConstDefine.ExportDependsAtlasAssetPathFolder);
//        // todo 临时改为在客户端工程构造atlas，之后改回在art工程构造atlas
//        string atlasRootPath = "Assets/App/Resources/Sprite";
//        var dir = new DirectoryInfo(atlasRootPath);
//        if (!dir.Exists)
//        {
//            L.Error("GetAtlasDirList called but atlasRootPath doesn't exist {0} ", atlasRootPath);
//            return null;
//        }
//        DirectoryInfo[] array = dir.GetDirectories();
//        var res = new List<string>();
//        for (int i = 0; i < array.Length; i++)
//        {
//            res.Add(array[i].FullName);
//        }
//
//        return res;
//    }
//
//    private static void LoopSetTexture(string dir)
//    {
//        string texturePath = string.Format("{0}/{1}", dir, BuildToolsConstDefine.ExportDependsAtlasTextureAssetFolder);
//        string[] fileInfo = GetTexturePath(texturePath);
//
//        int length = fileInfo.Length;
//	    SoyAtlasAttribute att = GetSpriteTag(dir);
//	    if (att == null)
//	    {
//			LogHelper.Error("LoopSetTexture called but dir is invalid! {0}",dir);
//		    return;
//	    }
//		//TextureImporterSettings setting = GetSpriteTextureImporterSetting(att.Pivot);
//	    TextureImporterPlatformSettings[] nSettings = GetSpriteTextureImportPlatformSettings(att.Quality);
//
//		for (int i = 0; i < length; i++)
//        {
//            //获取资源路径
//            string path = fileInfo[i];
//            ReImporterTexture(path, att.AtlasName, nSettings, att.Pivot);
//		}
//	}
//
//    private static string[] GetTexturePath(string dirPath)
//    {
//        //jpg
//        ArrayList jpgList = GetResourcesPath("*.jpg", dirPath);
//        int jpgLength = jpgList.Count;
//        //png
//        ArrayList pngList = GetResourcesPath("*.png", dirPath);
//        int pngLength = pngList.Count;
//        //tga
//        ArrayList tgaList = GetResourcesPath("*.tga", dirPath);
//        int tgaLength = tgaList.Count;
//        var filePath = new string[jpgLength + pngLength + tgaLength];
//        for (int i = 0; i < jpgLength; i++)
//        {
//            filePath[i] = jpgList[i].ToString();
//        }
//        for (int i = 0; i < pngLength; i++)
//        {
//            filePath[i + jpgLength] = pngList[i].ToString();
//        }
//        for (int i = 0; i < tgaLength; i++)
//        {
//            filePath[i + jpgLength + pngLength] = tgaList[i].ToString();
//        }
//        return filePath;
//    }
//
//	private class AtlasTmpData
//	{
//		public string AssetPath;
//		public string AssetMainName;
//	}
//    [MenuItem("JoyTools/AtlasTools/MakeSoyAtlas")]
//    private static void MakeSoyAtlas()
//    {
//        MakeSoyAtlas (null);
//    }
//    public static void MakeSoyAtlas(SoyResourceManifest manifest)
//    {
//        //string rootPath = string.Format("{0}/{1}/{2}", BuildToolsConstDefine.ExportAssetRootFolder,
//        //BuildToolsConstDefine.ExportDependsAssetFolder, BuildToolsConstDefine.ExportDependsAtlasAssetPathFolder);
//        // todo 临时改为在客户端工程构造atlas，之后改回在art工程构造atlas
//        string rootPath = "App/Resources/Sprite";
//        List<string> list = GetValidAtlasAssetPath(rootPath);
//        if (null == manifest) {
//            manifest = AssetBuilder.GetManifestData();
//        }
////        SoyResourceManifest manifest = AssetBuilder.GetManifestData();
//        var maifestContent = new List<SpriteAtlasRelation>();
//        var hasSearchedTexturePath = new List<string>();
//        var totalSprite = new List<Sprite>();
//	    BackUpAtlasAssetMetaFile();
//
//		List<AtlasTmpData> atlasList = new List<AtlasTmpData>();
//		string rootDataPath = Application.dataPath.Replace(BuildToolsConstDefine.AssetRootFolder, "");
//		for (int i = 0; i < list.Count; i++)
//		{
//			string tmpAssetPath = list[i];
//			tmpAssetPath = string.Format("{0}/{1}", tmpAssetPath,
//				BuildToolsConstDefine.ExportDependsAtlasTextureAssetFolder);
//			if (AssetDatabase.IsValidFolder(tmpAssetPath))
//			{
//				string[] findResult = AssetDatabase.FindAssets("t:Sprite", new[] { tmpAssetPath });
//				hasSearchedTexturePath.Clear();
//				for (int j = 0; j < findResult.Length; j++)
//				{
//					string textureAssetPath = AssetDatabase.GUIDToAssetPath(findResult[j]);
//					if (hasSearchedTexturePath.Contains(textureAssetPath))
//					{
//						continue;
//					}
//					List<Sprite> tmpList = GetSpriteAssetByPath(textureAssetPath);
//					hasSearchedTexturePath.Add(textureAssetPath);
//					totalSprite.AddRange(tmpList);
//				}
//
//
//				var atlasAttr = GetAtlasNameByAssetPath(list[i]);
//				if (atlasAttr == null)
//				{
//					LogHelper.Error("GetAtlasNameByAssetPath(list[i]) is null! list[i] is {0} , i is {1}.", list[i], i);
//					continue;
//				}
//				string atlasMainName = atlasAttr.AtlasName;
//				atlasMainName = string.Format("{0}Atlas", atlasMainName);
//				var resDic = new Dictionary<string, Sprite>();
//				for (int z = 0; z < totalSprite.Count; z++)
//				{
//					if (resDic.ContainsKey(totalSprite[z].name))
//					{
//						Debug.LogError(string.Format("Sprite name {0} duplicated path is {1} !", totalSprite[z].name,
//							AssetDatabase.GetAssetPath(totalSprite[z])));
//						return;
//					}
//					resDic.Add(totalSprite[z].name, totalSprite[z]);
//					maifestContent.Add(new SpriteAtlasRelation
//					{
//						AtlasName = atlasMainName,
//						SpriteName = totalSprite[z].name,
//					});
//				}
//				var asset = ScriptableObject.CreateInstance<SoyAtlas>();
//				asset.CachedSprites = totalSprite;
//
//				string atlasAssetPath = string.Format("{0}/{1}{2}", list[i], atlasMainName,
//					BuildToolsConstDefine.AssetSuffix);
//				AssetDatabase.CreateAsset(asset, atlasAssetPath);
//
//				totalSprite = new List<Sprite>();
//				//L.Log("Build atlas {0} complete!!", atlasAssetPath);
//
//				SoyAssetInfo assetInfo = AssetBuilder.CreateSoyAssetInfo(atlasMainName, EResType.Atlas, null);
//				AssetBuilder.AddAssetInfoToManifest(assetInfo);
//				atlasList.Add(new AtlasTmpData()
//				{
//					AssetMainName = atlasMainName,
//					AssetPath = atlasAssetPath,
//				});
//			}
//		}
//
//		AssetBuilder.SaveAssets();
//
//		for (int i = 0; i < atlasList.Count; i++)
//	    {
//		    string filePath = System.IO.Path.Combine(rootDataPath, atlasList[i].AssetPath);
//			AssetBuilder.RevertMetaFile(filePath);
//	    }
//		AssetBuilder.SaveAssets();
//
//		for (int i = 0; i < atlasList.Count; i++)
//		{
//			SoyAtlas tmpAsset = AssetDatabase.LoadAssetAtPath<SoyAtlas>(atlasList[i].AssetPath);
//			BuildingAssetHolder.Instance.AddAtlas(atlasList[i].AssetMainName, tmpAsset);
//		}
//
//		manifest.SpriteAtlasRelationData = maifestContent;
//        AssetBuilder.SaveAssets();
//		L.Log ("MakeSoyAtlas done.");
//    }
//
//	public static void BackUpAtlasAssetMetaFile()
//	{
//		string atlasPath = GetAtlasAssetFilePath();
//		var list = GetResourcesPath("*.asset", atlasPath,SearchOption.AllDirectories);
//		string backupMetaFolderPath = AssetBuilder.GetBackUpMetaFilePath();
//		string tmpPath;
//		foreach (var item in list)
//		{
//			var s = item.ToString();
//			FileInfo info = new FileInfo(s);
//			string metaFileName = string.Format(BuildToolsConstDefine.BackUpMetaFileNameFormat, info.Name);
//			string metaFilePath = string.Format(BuildToolsConstDefine.BackUpMetaFileNameFormat, info.FullName);
//			tmpPath = System.IO.Path.Combine(backupMetaFolderPath, metaFileName);
//
//
//			if (!File.Exists(metaFilePath))
//			{
//				Debug.LogErrorFormat("atlasAsset {0} error!.meta file doesn't.", s);
//				continue;
//			}
//			if (File.Exists(tmpPath))
//			{
//				File.Copy(metaFilePath, tmpPath);
//			}
//		}
//	}
//
//	/// <summary>
//	///     获取指定后掇后的文件路径
//	/// </summary>
//	/// <param name="fileType"></param>
//	/// <returns></returns>
//	private static ArrayList GetResourcesPath(string fileType, string dirPath,SearchOption sopp = SearchOption.TopDirectoryOnly)
//	{
//		var directoryInfo = new DirectoryInfo(dirPath);
//		var filePath = new ArrayList();
//		foreach (FileInfo fi in directoryInfo.GetFiles(fileType, sopp))
//		{
//			string path = fi.FullName;
//			path = path.Remove(0, Path.Length);
//			path = path.Replace('\\', '/');
//			filePath.Add(path);
//		}
//		return filePath;
//	}
//
//	private static string GetAtlasAssetFilePath()
//	{
//        //string assetPath = BuildToolsConstDefine.GetExportAtlasRootPath();
//        // todo 临时改为在客户端工程构造atlas，之后改回在art工程构造atlas
//        string assetPath = "Assets/App/Resources/Sprite";
//		string parent = Application.dataPath.Replace(BuildToolsConstDefine.AssetRootFolder, "");
//		return System.IO.Path.Combine(parent, assetPath);
//	}
//
//	private static SoyAtlasAttribute GetAtlasNameByAssetPath(string path)
//    {
//	    string wholePath = Application.dataPath + path.Substring(7, path.Length - 7);
//	    string configPath = string.Format("{0}/{1}", wholePath, SoyAltasConfig);
//		string[] array = path.Split('/');
//        if (array.Length == 0)
//        {
//            L.Error("GetAtlasNameByAssetPath called but path is invalid!{0}", path);
//	        return null;
//        }
//
//        return ParseSoyAtlasAttr(array[array.Length - 1], configPath);
//    }
//
//	private static SoyAtlasAttribute GetSpriteTag(string filePath)
//	{
//		string configPath = string.Format("{0}/{1}", filePath.Replace('\\', '/'), SoyAltasConfig);
//		string[] strArray = filePath.Replace('\\', '/').Split('/');
//		if (strArray.Length < 3)
//		{
//			L.Error("GetSpriteTag called but filePath is invalid! {0}", filePath);
//			return null;
//		}
//		return ParseSoyAtlasAttr(strArray[strArray.Length - 1], configPath);
//	}
//	
//	private static SoyAtlasAttribute ParseSoyAtlasAttr(string atlasName,string configPath)
//	{
//		if (string.IsNullOrEmpty(atlasName))
//		{
//			LogHelper.Error("ParseSoyAtlasAttr falied atlasName is null or empty!");
//			return null;
//		}
//		if (string.IsNullOrEmpty(configPath))
//		{
//			LogHelper.Error("ParseSoyAtlasAttr falied configPath is null or empty!");
//			return null;
//		}
//		string content = "";
//		if (File.Exists(configPath))
//		{
//			content = File.ReadAllText(configPath);
//		}
//		string[] array = content.Split('_');
//		SoyAtlasAttribute data = new SoyAtlasAttribute();
//		data.AtlasName = atlasName;
//		if (array.Length >= 1)
//		{
//			data.Quality = (ETextureCompressQuality) GetIntValueByString(array[0]);
//		}
//		else
//		{
//			data.Quality = (ETextureCompressQuality)GetIntValueByString("");
//		}
//		if (array.Length >= 2)
//		{
//			data.Pivot = (ESpritePivot)GetIntValueByString(array[1]);
//		}
//		else
//		{
//			data.Pivot = (ESpritePivot)GetIntValueByString("");
//		}
//		return data;
//	}
//
//	private static int GetIntValueByString(string value)
//	{
//		int res = 0;
//		int.TryParse(value, out res);
//		return res;
//	}
//
//
//	private static List<string>  GetValidAtlasAssetPath(string path)
//    {
//        var res = new List<string>();
//        string wholePath = string.Format("{0}/{1}", Application.dataPath, path);
//        var info = new DirectoryInfo(wholePath);
//        DirectoryInfo[] subFolders = info.GetDirectories();
//        string tmpPath;
//        for (int i = 0; i < subFolders.Length; i++)
//        {
//            if (
//                subFolders[i].GetDirectories(BuildToolsConstDefine.ExportDependsAtlasTextureAssetFolder,
//                    SearchOption.TopDirectoryOnly).Length > 0)
//            {
//                tmpPath = subFolders[i].FullName.Substring(Application.dataPath.Length,
//                    subFolders[i].FullName.Length - Application.dataPath.Length);
//                tmpPath = string.Format("{0}{1}", BuildToolsConstDefine.AssetRootFolder, tmpPath);
//                tmpPath = tmpPath.Replace('\\', '/');
//                res.Add(tmpPath);
//            }
//        }
//
//        return res;
//    }
//
//    private static List<Sprite> GetSpriteAssetByPath(string assetPath)
//    {
//        var res = new List<Sprite>();
//        Object[] assetArray = AssetDatabase.LoadAllAssetsAtPath(assetPath);
//        for (int i = 0; i < assetArray.Length; i++)
//        {
//            if (assetArray[i] is Sprite)
//            {
//                res.Add(assetArray[i] as Sprite);
//            }
//        }
//        return res;
//    }
//
//	private static string GetFilePathByAssetPath(string assetPath)
//	{
//		string subPath = assetPath.Substring(6, assetPath.Length - 6);
//		return string.Format("{0}{1}", Application.dataPath, subPath);
//	}
//}