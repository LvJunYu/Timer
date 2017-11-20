using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NewResourceSolution.EditorTool
{
	public class ABUtility
	{
		/// <summary>
		/// 通过路径判断asset的类型
		/// </summary>
		/// <returns>The raw res type.</returns>
		/// <param name="assetPath">Asset path.</param>
		public static EResType GetRawResType (string assetPath)
		{
			if (string.IsNullOrEmpty(assetPath)) return EResType.None;
			var parts = assetPath.Split('/');
			string extension = Path.GetExtension(assetPath);
			for (int i = 0; i < parts.Length; i++)
			{
				if (i == 0 && String.CompareOrdinal(parts[i], ResPath.Assets) != 0)
				{
					return EResType.None;
				}
				if (i == 1 && String.CompareOrdinal(parts[i], ResPath.EditorRawRes) != 0)
				{
					return EResType.None;
				}
				if (i == 2 && String.CompareOrdinal(parts[i], ResPath.Textures) == 0 &&
					String.CompareOrdinal(extension, ResPath.TextureExtension) == 0)
				{
//					if (i == parts.Length - 2)
					{
						return EResType.Texture;
					}
				}
                if (i == 2 && String.CompareOrdinal(parts[i], ResPath.Materials) == 0 &&
                    String.CompareOrdinal(extension, ResPath.MaterialExtension) == 0)
                {
//                    if (i == parts.Length - 2)
                    {
                        return EResType.Material;
                    }
                }
                if (i == 2 && String.CompareOrdinal(parts[i], ResPath.Audio) == 0 &&
                    String.CompareOrdinal(extension, ResPath.AudioExtension) == 0)
                {
                    //                    if (i == parts.Length - 2)
                    {
                        return EResType.AudioClip;
                    }
                }
                if (i == 2 && String.CompareOrdinal(parts[i], ResPath.Fonts) == 0 &&
                    String.CompareOrdinal(extension, ResPath.FontExtension) == 0)
                {
                    //                    if (i == parts.Length - 2)
                    {
                        return EResType.Font;
                    }
                }
                if (i == 2 && String.CompareOrdinal(parts[i], ResPath.MeshDatas) == 0 &&
                    String.CompareOrdinal(extension, ResPath.MeshExtension) == 0)
                {
                    //                    if (i == parts.Length - 2)
                    {
                        return EResType.MeshData;
                    }
                }
                if (i == 2 && String.CompareOrdinal(parts[i], ResPath.Shaders) == 0 &&
                    String.CompareOrdinal(extension, ResPath.ShaderExtension) == 0)
                {
                    //                    if (i == parts.Length - 2)
                    {
                        return EResType.Shader;
                    }
                }
                if (i == 2 && String.CompareOrdinal(parts[i], ResPath.Tables) == 0 &&
                    String.CompareOrdinal(extension, ResPath.TableExtension) == 0)
                {
                    //                    if (i == parts.Length - 2)
                    {
                        return EResType.Table;
                    }
                }
                if (i == 2 && String.CompareOrdinal(parts[i], ResPath.Sprites) == 0 &&
                    String.CompareOrdinal(extension, ResPath.SpriteExtension) == 0)
                {
                    //                  if (i == parts.Length - 2)
                    {
                        return EResType.Sprite;
                    }
                }
                if (i == 2 && String.CompareOrdinal(parts[i], ResPath.Animations) == 0)
                {
                    //                  if (i == parts.Length - 2)
                    {
                        return EResType.Animation;
                    }
                }
				if (i == 5 && String.CompareOrdinal(parts[i], ResPath.Models) == 0)
				{
					if (String.CompareOrdinal(parts[i - 1], ResPath.Prefabs) == 0 &&
						String.CompareOrdinal(extension, ResPath.PrefabExtension) == 0)
					{
						return EResType.ModelPrefab;
					}
				}
                if (i == 5 && String.CompareOrdinal(parts[i], ResPath.Particles) == 0)
                {
                    if (String.CompareOrdinal(parts[i - 1], ResPath.Prefabs) == 0 &&
                        String.CompareOrdinal(extension, ResPath.PrefabExtension) == 0)
                    {
                        return EResType.ParticlePrefab;
                    }
                }
                if (i == 5 && String.CompareOrdinal(parts[i], ResPath.UIPrefabs) == 0)
                {
                    if (String.CompareOrdinal(parts[i - 1], ResPath.Prefabs) == 0 &&
                        String.CompareOrdinal(extension, ResPath.PrefabExtension) == 0)
                    {
                        return EResType.UIPrefab;
                    }
                }
			}
			return EResType.None;
		}

		/// <summary>
		/// 获得打包assetbundle的未压缩输出目录
		/// </summary>
		/// <returns>The output path.</returns>
		/// <param name="buildTarget">Build target.</param>
		public static string GetUnCompressedOutputPath(BuildTarget buildTarget)
		{
			return string.Format(StringFormat.TwoLevelPath, GetOutputPath(buildTarget), ABConstDefine.OutputPathUnCompressed);
		}

		/// <summary>
		/// 获得打包assetbundle的输出目录
		/// </summary>
		/// <returns>The output path.</returns>
		/// <param name="buildTarget">Build target.</param>
		public static string GetOutputPath(BuildTarget buildTarget)
		{
			DirectoryInfo projectRelativeOutputDir = new DirectoryInfo(ResPath.AssetBundleOutputPath);
			string platformFolder = GetPlatformFolderName(buildTarget);
			return string.Format(StringFormat.ThreeLevelPath, Application.dataPath, projectRelativeOutputDir, platformFolder);
		}

		public static string GetPlatformFolderName(BuildTarget buildTarget)
		{
			string platformFolder;
			switch (buildTarget)
			{
				case BuildTarget.Android:
					platformFolder = ResPath.PlatformFolderAndroid;
					break;
				case BuildTarget.iOS:
					platformFolder = ResPath.PlatformFolderiOS;
					break;
				case BuildTarget.StandaloneWindows64:
				case BuildTarget.StandaloneWindows:
					platformFolder = ResPath.PlatformFolderWin;
					break;
				case BuildTarget.StandaloneOSXIntel64:
					platformFolder = ResPath.PlatformFolderOSX;
					break;
				default:
					platformFolder = ResPath.PlatformFolderOSX;
					break;
			}
			return platformFolder;
		}
		

		public static string GetBuildOutputStreamingAssetsPath(BuildTarget buildTarget)
		{
			return string.Format(StringFormat.TwoLevelPath, GetOutputPath(buildTarget), ResPath.StreamingAssets);
		}

		/// <summary>
		/// 根据路径得到bundleName，返回assetName
		/// </summary>
		/// <returns>The to asset bundle name.</returns>
		/// <param name="rootPath">Root path.</param>
		/// <param name="assetPath">Asset path.</param>
		/// <param name="assetBundleName">Asset bundle name.</param>
		public static string PathToAssetBundleName (string rootPath, string assetPath, out string assetBundleName)
		{
			rootPath = rootPath.Replace('\\', '/');
			assetPath = assetPath.Replace('\\', '/');
			assetBundleName = string.Empty;
			// remove extension
			int pointIdx = assetPath.LastIndexOf('.');
            if (pointIdx >= 0)
            {
                assetPath = assetPath.Substring (0, pointIdx);
            }
			string[] parts = assetPath.Split(new[] {rootPath}, StringSplitOptions.None);
			string subPath = parts[parts.Length - 1];
			if (rootPath.Contains(ResPath.LocaleResRoot))
			{
				parts = rootPath.Split(new[] {ResPath.LocaleResRoot}, StringSplitOptions.None);
				if (parts.Length < 2) return string.Empty;
				string localeName = parts[1].Substring(1).Split('/')[0];
				assetBundleName = string.Format(
					"{0}{1}{2}{3}",
					ResDefine.LocaleResBundleNameFirstChar,
					localeName,
					ResDefine.ReplaceSplashCharInAssetBundleName,
					subPath.Substring(1).Replace(ResDefine.SplashCharInAssetBundleName, ResDefine.ReplaceSplashCharInAssetBundleName)
				).ToLower();
			}
			else
			{
				assetBundleName = subPath.Substring(1).Replace(ResDefine.SplashCharInAssetBundleName, ResDefine.ReplaceSplashCharInAssetBundleName).ToLower();
			}
			string[] splitBySplash = subPath.Split('/');
			return splitBySplash[splitBySplash.Length - 1];
		}
	}
}