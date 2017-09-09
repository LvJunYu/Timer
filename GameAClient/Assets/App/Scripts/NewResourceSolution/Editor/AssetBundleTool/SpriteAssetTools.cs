using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SoyEngine;
using UnityEngine;
using UnityEditor;

namespace NewResourceSolution.EditorTool
{
	public static class SpriteAssetTools
	{
		public enum ETextureCompressQuality
		{
			ShowAs16Bit = 0,
			NoCompress,
			Compress,
		}
	
		public enum ESpritePivot
		{
			Center = 0,
			TopLeft,
			Top,
			TopRight,
			Left,
			Right,
			BottomLeft,
			Bottom,
			BottomRight,
			Custom,
		}

		[MenuItem("JoyTools/SetAtlasSpritesSettings")]
		private static void SetAtlasSpritesSettings()
		{
			int totalSpriteCnt = 0;
			string rootPath = ResPathUtility.GetEditorDebugResFolderPath(EResType.Sprite, false);
			if (AssetDatabase.IsValidFolder(rootPath))
			{
				DirectoryInfo rootDirectoryInfo = new DirectoryInfo (rootPath);
				var childDirectorys = rootDirectoryInfo.GetDirectories ();
				for (int k = 0; k < childDirectorys.Length; k++)
				{
					var parts = childDirectorys [k].ToString ().Split (new[] {ResPath.Assets}, System.StringSplitOptions.None);
					string childDirRelatedToUnityProject = 
						string.Format (
							"{0}{1}",
							ResPath.Assets,
							parts [parts.Length - 1]
						);
					var assets = AssetDatabase.FindAssets("t:Texture", new[] {childDirRelatedToUnityProject});
					SetAtlasSetting (rootPath, childDirRelatedToUnityProject, assets);
					totalSpriteCnt += assets.Length;
				}
			}
			AssetDatabase.RemoveUnusedAssetBundleNames();
			LogHelper.Info("SetAtlasSpritesSettings done, total sprite cnt: {0}", totalSpriteCnt);
		}
		
		public static void SetAtlasSetting(
			string rootPath,
			string childFolderPath,
			string[] assetsGuid)
		{
			string bundleName;
			ABUtility.PathToAssetBundleName (rootPath, childFolderPath, out bundleName);
//			CHResBundle bundle = new CHResBundle ();
//			bundle.AssetBundleName = bundleName;
//			string folderGuid = AssetDatabase.AssetPathToGUID(childFolderPath);
//			// todo 这里只区分是否随包发布的资源
//			if (buildABConfig.FullResPackage || buildABConfig.IsGuidInPackageAsset (folderGuid))
//			{
//				bundle.GroupId = ResDefine.ResGroupInPackage;
//			}
//			else
//			{
//				bundle.GroupId = ResDefine.ResGroupNecessary;
//			}

//			List<string> assetNameList = new List<string>();

			for (int i = 0; i < assetsGuid.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(assetsGuid[i]);
				var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
				if (null != textureImporter)
				{
//					string temp;
//					string assetName = ABUtility.PathToAssetBundleName (rootPath, path, out temp);
//					if (!assetNameList.Contains(assetName))
//						assetNameList.Add (assetName);
//					ai.assetBundleName = bundleName;

					SetTextureImporterSetting(textureImporter, bundleName, ETextureCompressQuality.NoCompress);
					EditorUtility.UnloadUnusedAssetsImmediate ();
				}
				else
				{
					LogHelper.Error ("Set sprite setting failed, importer is null, path: {0}", path);
				}
			}
		}
		
		private static void SetTextureImporterSetting (TextureImporter textureImporter, string atlasName, ETextureCompressQuality quality)
		{
			bool isDirty = false;
			
			var textureImportertextureType = TextureImporterType.Sprite;
			var textureImporterspriteImportMode = SpriteImportMode.Single;
			var textureImporterspritePackingTag = atlasName;
			var textureImporterspritePixelsPerUnit = 128;
			var textureImportermipmapEnabled = false;
			var textureImporterfilterMode = FilterMode.Bilinear;
			var textureImporterfadeout = false;
			var textureImporterwrapMode = TextureWrapMode.Repeat;

			if (textureImporter.textureType != textureImportertextureType)
			{
				textureImporter.textureType = textureImportertextureType;
				isDirty = true;
			}
			if (textureImporter.spriteImportMode != textureImporterspriteImportMode)
			{
				textureImporter.spriteImportMode = textureImporterspriteImportMode;
				isDirty = true;
			}
			if (textureImporter.spritePackingTag != textureImporterspritePackingTag)
			{
				textureImporter.spritePackingTag = textureImporterspritePackingTag;
				isDirty = true;
			}
			if (Math.Abs(textureImporter.spritePixelsPerUnit - textureImporterspritePixelsPerUnit) > float.Epsilon)
			{
				textureImporter.spritePixelsPerUnit = textureImporterspritePixelsPerUnit;
				isDirty = true;
			}
			if (textureImporter.mipmapEnabled != textureImportermipmapEnabled)
			{
				textureImporter.mipmapEnabled = textureImportermipmapEnabled;
				isDirty = true;
			}
			if (textureImporter.filterMode != textureImporterfilterMode)
			{
				textureImporter.filterMode = textureImporter.filterMode;
				isDirty = true;
			}
			if (textureImporter.fadeout != textureImporterfadeout)
			{
				textureImporter.fadeout = textureImporterfadeout;
				isDirty = true;
			}
			if (textureImporter.wrapMode != textureImporterwrapMode)
			{
				textureImporter.wrapMode = textureImporterwrapMode;
				isDirty = true;
			}

			// default
			TextureImporterPlatformSettings defaultSetting = new TextureImporterPlatformSettings();
			defaultSetting.name = "Standalone";
			defaultSetting.overridden = true;
			defaultSetting.compressionQuality = (int)GetTextureImporterFormatByETextureCompressQuality(quality);
			defaultSetting.maxTextureSize = 1024;
			var ds = textureImporter.GetPlatformTextureSettings(defaultSetting.name);
			if (ds == null || JsonUtility.ToJson(ds) != JsonUtility.ToJson(defaultSetting))
			{
				textureImporter.SetPlatformTextureSettings(defaultSetting);
				isDirty = true;
			}
			
			// iOS
			TextureImporterPlatformSettings iosSetting = new TextureImporterPlatformSettings();
			iosSetting.name = "iPhone";
			iosSetting.overridden = true;
			iosSetting.compressionQuality = (int)GetTextureImporterFormatByETextureCompressQuality(quality);
			iosSetting.maxTextureSize = 2048;
			iosSetting.format = ETextureCompressQuality.NoCompress == quality ? TextureImporterFormat.RGBA32 : TextureImporterFormat.PVRTC_RGBA4;
			var iOSs = textureImporter.GetPlatformTextureSettings(iosSetting.name);
			if (iOSs == null || JsonUtility.ToJson(iOSs) != JsonUtility.ToJson(iosSetting))
			{
				textureImporter.SetPlatformTextureSettings(iosSetting);
				isDirty = true;
			}

			// android
			TextureImporterPlatformSettings androidSetting = new TextureImporterPlatformSettings();
			androidSetting.name = "Android";
			androidSetting.overridden = true;
			androidSetting.compressionQuality = (int)GetTextureImporterFormatByETextureCompressQuality(quality);
			androidSetting.maxTextureSize = 2048;
			androidSetting.format = ETextureCompressQuality.NoCompress == quality ? TextureImporterFormat.RGBA32 : TextureImporterFormat.ETC2_RGBA8;
			var ands = textureImporter.GetPlatformTextureSettings(androidSetting.name);
			if (ands == null || JsonUtility.ToJson(ands) != JsonUtility.ToJson(androidSetting))
			{
				textureImporter.SetPlatformTextureSettings(androidSetting);
				isDirty = true;
			}

			if (isDirty)
			{
				textureImporter.SaveAndReimport();
			}
		}

		private static TextureCompressionQuality GetTextureImporterFormatByETextureCompressQuality(ETextureCompressQuality quality)
		{
			if (quality == ETextureCompressQuality.Compress)
			{
				return TextureCompressionQuality.Fast;
			}
			else if (quality == ETextureCompressQuality.ShowAs16Bit)
			{
				return TextureCompressionQuality.Normal;
			}
			else
			{
				return TextureCompressionQuality.Best;
			}
		}
	}
	
}
