using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewResourceSolution
{
	public static class ResPathUtility
	{
		/// <summary>
		/// 得到特定资源的原始文件所在文件夹
		/// </summary>
		/// <returns>The editor debug res folder path.</returns>
		/// <param name="resType">Res type.</param>
		/// <param name="localeOverride">If set to <c>true</c> locale override.</param>
		/// <param name="locale">Locale.</param>
		public static string GetEditorDebugResFolderPath (
			EResType resType,
			bool localeOverride = false,
			ELocale locale = ELocale.WW)
		{
			ELocale targetLocale = localeOverride ? locale : LocalizationManager.Instance.CurrentLocale;
            if (EResType.Animation == resType)
            {
                return StringUtil.Format (
                    StringFormat.ThreeLevelPath, 
                    ResPath.Assets, 
                    ResPath.EditorRawRes,
                    ResPath.Animations);
            }
            else if (EResType.AudioClip == resType)
            {
                return StringUtil.Format (
                    StringFormat.ThreeLevelPath, 
                    ResPath.Assets, 
                    ResPath.EditorRawRes,
                    ResPath.Audio);
            }
            else if (EResType.Font == resType)
            {
                return StringUtil.Format (
                    StringFormat.ThreeLevelPath, 
                    ResPath.Assets, 
                    ResPath.EditorRawRes,
                    ResPath.Fonts);
            }
            else if (EResType.Material == resType)
            {
                return StringUtil.Format (
                    StringFormat.ThreeLevelPath, 
                    ResPath.Assets, 
                    ResPath.EditorRawRes,
                    ResPath.Materials);
            }
            else if (EResType.MeshData == resType)
            {
                return StringUtil.Format (
                    StringFormat.ThreeLevelPath, 
                    ResPath.Assets, 
                    ResPath.EditorRawRes,
                    ResPath.MeshDatas);
            }
            else if (EResType.Shader == resType)
            {
                return StringUtil.Format (
                    StringFormat.ThreeLevelPath, 
                    ResPath.Assets, 
                    ResPath.EditorRawRes,
                    ResPath.Shaders);
            }
            else if (EResType.SpineData == resType)
            {
                return StringUtil.Format (
                    StringFormat.ThreeLevelPath, 
                    ResPath.Assets, 
                    ResPath.EditorRawRes,
                    ResPath.SpineDatas);
            }
            else if (EResType.Sprite == resType)
            {
                return StringUtil.Format (
                    StringFormat.ThreeLevelPath, 
                    ResPath.Assets, 
                    ResPath.EditorRawRes,
                    ResPath.Sprites);
            }
            else if (EResType.Table == resType)
            {
                return StringUtil.Format (
                    StringFormat.ThreeLevelPath, 
                    ResPath.Assets, 
                    ResPath.EditorRawRes,
                    ResPath.JsonTables);
            }
            else if (EResType.Texture == resType)
            {
                return StringUtil.Format (
                    StringFormat.ThreeLevelPath, 
                    ResPath.Assets, 
                    ResPath.EditorRawRes,
                    ResPath.Textures);
            }
			else if (EResType.UIPrefab == resType)
			{
				return StringUtil.Format(
					StringFormat.SixLevelPath, 
					ResPath.Assets,
					ResPath.EditorRawRes,
					ResPath.LocaleResRoot,
					LocaleDefine.LocaleNames[(int)targetLocale],
					ResPath.Prefabs,
					ResPath.UIPrefabs);
			}
            else if (EResType.ParticlePrefab == resType)
            {
                return StringUtil.Format(
                    StringFormat.SixLevelPath, 
                    ResPath.Assets,
                    ResPath.EditorRawRes,
                    ResPath.LocaleResRoot,
                    LocaleDefine.LocaleNames[(int)targetLocale],
                    ResPath.Prefabs,
                    ResPath.Particles);
            }
			else if (EResType.ModelPrefab == resType)
			{
				return StringUtil.Format(
					StringFormat.SixLevelPath, 
					ResPath.Assets,
					ResPath.EditorRawRes,
					ResPath.LocaleResRoot,
					LocaleDefine.LocaleNames[(int)targetLocale],
					ResPath.Prefabs,
					ResPath.Models);
			}
			return string.Empty;
		}

		/// <summary>
		/// 得到特定资源的原始文件路径
		/// </summary>
		/// <returns>The editor debug res path.</returns>
		/// <param name="resType">Res type.</param>
		/// <param name="name">Name.</param>
		/// <param name="localeOverride">If set to <c>true</c> locale override.</param>
		/// <param name="locale">Locale.</param>
//		public static string GetEditorDebugResPath (
//			EResType resType,
//			string name,
//			bool localeOverride = false,
//			ELocale locale = ELocale.WW)
//		{
//			string extension = string.Empty;
//			if (EResType.Txt == resType)
//			{
//				extension = ResPath.JsonExtension;
//			}
//            else if (EResType.Table == resType)
//			{
//				extension = ResPath.JsonExtension;
//			}
//			else if (EResType.UIPrefab == resType)
//			{
//				extension = ResPath.PrefabExtension;
//			}
//			else if (EResType.Model == resType)
//			{
//				extension = ResPath.PrefabExtension;
//			}
//			string folder = GetEditorDebugResFolderPath(resType, localeOverride, locale);
//			if (string.IsNullOrEmpty(folder))
//			{
//				return null;
//			}
//			else
//			{
//				return string.Format(
//					"{0}/{1}{2}",
//					folder,
//					name,
//					extension);
//			}
//		}

		public static string GetStreamingAssetsPath ()
		{
			return StringUtil.Format(StringFormat.TwoLevelPath, ResPath.Assets, ResPath.StreamingAssets);
		}
	}
}