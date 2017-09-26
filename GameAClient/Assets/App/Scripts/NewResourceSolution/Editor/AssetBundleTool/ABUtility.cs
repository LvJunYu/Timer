using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using NewResourceSolution;

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
				if (i == 0 && string.Compare(parts[i], ResPath.Assets) != 0)
				{
					return EResType.None;
				}
				if (i == 1 && string.Compare(parts[i], ResPath.EditorRawRes) != 0)
				{
					return EResType.None;
				}
				if (i == 2 && string.Compare(parts[i], ResPath.Textures) == 0 &&
					string.Compare(extension, ResPath.TextureExtension) == 0)
				{
//					if (i == parts.Length - 2)
					{
						return EResType.Texture;
					}
				}
                if (i == 2 && string.Compare(parts[i], ResPath.Materials) == 0 &&
                    string.Compare(extension, ResPath.MaterialExtension) == 0)
                {
//                    if (i == parts.Length - 2)
                    {
                        return EResType.Material;
                    }
                }
                if (i == 2 && string.Compare(parts[i], ResPath.Audio) == 0 &&
                    string.Compare(extension, ResPath.AudioExtension) == 0)
                {
                    //                    if (i == parts.Length - 2)
                    {
                        return EResType.AudioClip;
                    }
                }
                if (i == 2 && string.Compare(parts[i], ResPath.Fonts) == 0 &&
                    string.Compare(extension, ResPath.FontExtension) == 0)
                {
                    //                    if (i == parts.Length - 2)
                    {
                        return EResType.Font;
                    }
                }
                if (i == 2 && string.Compare(parts[i], ResPath.MeshDatas) == 0 &&
                    string.Compare(extension, ResPath.MeshExtension) == 0)
                {
                    //                    if (i == parts.Length - 2)
                    {
                        return EResType.MeshData;
                    }
                }
                if (i == 2 && string.Compare(parts[i], ResPath.Shaders) == 0 &&
                    string.Compare(extension, ResPath.ShaderExtension) == 0)
                {
                    //                    if (i == parts.Length - 2)
                    {
                        return EResType.Shader;
                    }
                }
                if (i == 2 && string.Compare(parts[i], ResPath.Tables) == 0 &&
                    string.Compare(extension, ResPath.TableExtension) == 0)
                {
                    //                    if (i == parts.Length - 2)
                    {
                        return EResType.Table;
                    }
                }
                if (i == 2 && string.Compare(parts[i], ResPath.Sprites) == 0 &&
                    string.Compare(extension, ResPath.SpriteExtension) == 0)
                {
                    //                  if (i == parts.Length - 2)
                    {
                        return EResType.Sprite;
                    }
                }
                if (i == 2 && string.Compare(parts[i], ResPath.Animations) == 0)
                {
                    //                  if (i == parts.Length - 2)
                    {
                        return EResType.Animation;
                    }
                }
				if (i == 5 && string.Compare(parts[i], ResPath.Models) == 0)
				{
					if (string.Compare(parts[i - 1], ResPath.Prefabs) == 0 &&
						string.Compare(extension, ResPath.PrefabExtension) == 0)
					{
						return EResType.ModelPrefab;
					}
				}
                if (i == 5 && string.Compare(parts[i], ResPath.Particles) == 0)
                {
                    if (string.Compare(parts[i - 1], ResPath.Prefabs) == 0 &&
                        string.Compare(extension, ResPath.PrefabExtension) == 0)
                    {
                        return EResType.ParticlePrefab;
                    }
                }
                if (i == 5 && string.Compare(parts[i], ResPath.UIPrefabs) == 0)
                {
                    if (string.Compare(parts[i - 1], ResPath.Prefabs) == 0 &&
                        string.Compare(extension, ResPath.PrefabExtension) == 0)
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
		/// <param name="config">Config.</param>
		/// <param name="buildTarget">Build target.</param>
		public static string GetUnCompressedOutputPath (BuildABConfig config, BuildTarget buildTarget)
		{
			return string.Format(StringFormat.TwoLevelPath, GetOutputPath(config, buildTarget), ABConstDefine.OutputPathUnCompressed);
		}

		/// <summary>
		/// 获得打包assetbundle的输出目录
		/// </summary>
		/// <returns>The output path.</returns>
		/// <param name="config">Config.</param>
		/// <param name="buildTarget">Build target.</param>
		public static string GetOutputPath(BuildABConfig config, BuildTarget buildTarget)
		{
			DirectoryInfo projectParrentDir = new DirectoryInfo(config.OutputPath);
			string platformFolder = ABConstDefine.OutputPathOSX;
			switch (buildTarget)
			{
				case BuildTarget.Android:
					platformFolder = ABConstDefine.OutputPathAndroid;
					break;
				case BuildTarget.iOS:
					platformFolder = ABConstDefine.OutputPathIOS;
					break;
                case BuildTarget.StandaloneWindows64:
					platformFolder = ABConstDefine.OutputPathWindows;
					break;
				case BuildTarget.StandaloneOSXIntel64:
					platformFolder = ABConstDefine.OutputPathOSX;
					break;
				default:
					platformFolder = ABConstDefine.OutputPathOSX;
					break;
			}
			return string.Format(StringFormat.FourLevelPath, Application.dataPath, projectParrentDir, platformFolder, config.Version);
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
			string[] parts = assetPath.Split(new[] {rootPath}, System.StringSplitOptions.None);
			string subPath = parts[parts.Length - 1];
			if (rootPath.Contains(ResPath.LocaleResRoot))
			{
				parts = rootPath.Split(new[] {ResPath.LocaleResRoot}, System.StringSplitOptions.None);
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