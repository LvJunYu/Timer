using System.Collections.Generic;
using System.IO;
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
			textureImporter.textureType = TextureImporterType.Sprite;
			textureImporter.spriteImportMode = SpriteImportMode.Single;
			textureImporter.spritePackingTag = atlasName;
			textureImporter.spritePixelsPerUnit = 128;
			textureImporter.mipmapEnabled = false;
			textureImporter.filterMode = FilterMode.Bilinear;
			textureImporter.fadeout = false;
			textureImporter.wrapMode = TextureWrapMode.Repeat;

			// default
			TextureImporterPlatformSettings defaultSetting = new TextureImporterPlatformSettings();
			defaultSetting.name = "Standalone";
			defaultSetting.overridden = true;
			defaultSetting.compressionQuality = (int)GetTextureImporterFormatByETextureCompressQuality(quality);
			defaultSetting.maxTextureSize = 1024;
			textureImporter.SetPlatformTextureSettings(defaultSetting);
			
			// iOS
			TextureImporterPlatformSettings iosSetting = new TextureImporterPlatformSettings();
			iosSetting.name = "iPhone";
			iosSetting.overridden = true;
			iosSetting.compressionQuality = (int)GetTextureImporterFormatByETextureCompressQuality(quality);
			iosSetting.maxTextureSize = 1024;
			iosSetting.format = ETextureCompressQuality.NoCompress == quality ? TextureImporterFormat.RGBA32 : TextureImporterFormat.PVRTC_RGBA4;
			textureImporter.SetPlatformTextureSettings(iosSetting);

			// android
			TextureImporterPlatformSettings androidSetting = new TextureImporterPlatformSettings();
			androidSetting.name = "Android";
			androidSetting.overridden = true;
			androidSetting.compressionQuality = (int)GetTextureImporterFormatByETextureCompressQuality(quality);
			androidSetting.maxTextureSize = 1024;
			androidSetting.format = ETextureCompressQuality.NoCompress == quality ? TextureImporterFormat.RGBA32 : TextureImporterFormat.ETC2_RGBA8;
			textureImporter.SetPlatformTextureSettings(androidSetting);
			
			textureImporter.SaveAndReimport();
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
