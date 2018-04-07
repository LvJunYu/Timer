/********************************************************************
** Filename : SpritePackerLimitTextureSize1024  
** Author : ake
** Date : 10/10/2016 2:46:07 PM
** Summary : SpritePackerLimitTextureSize1024  
***********************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEngine;


	public class SpritePackerLimitTextureSize1024: UnityEditor.Sprites.IPackerPolicy
	{
		protected class Entry
		{
			public Sprite sprite;
			public AtlasSettings settings;
			public string atlasName;
			public SpritePackingMode packingMode;
		}

		public virtual int GetVersion() { return 1; }

		protected virtual string TagPrefix { get { return "[LIMIT1024]"; } }
		protected virtual bool AllowTightWhenTagged { get { return true; } }

		public void OnGroupAtlases(BuildTarget target, PackerJob job, int[] textureImporterInstanceIDs)
		{
			List<Entry> entries = new List<Entry>();

			foreach (int instanceID in textureImporterInstanceIDs)
			{
				TextureImporter ti = EditorUtility.InstanceIDToObject(instanceID) as TextureImporter;
				TextureFormat dstFormat;
				ColorSpace dstColorSpace;
				int dstCompressionQuality;
				ti.ReadTextureImportInstructions(target,out dstFormat,out dstColorSpace,out dstCompressionQuality);
			

				TextureImporterSettings tis = new TextureImporterSettings();
				ti.ReadTextureSettings(tis);

				Sprite[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(ti.assetPath).Select(x => x as Sprite).Where(x => x != null).ToArray();
				foreach (Sprite sprite in sprites)
				{
					//在这里设置每个图集的参数
					Entry entry = new Entry();
					entry.sprite = sprite;
					entry.settings.format = dstFormat;
					entry.settings.colorSpace = dstColorSpace;
					entry.settings.compressionQuality = dstCompressionQuality;
					entry.settings.filterMode = Enum.IsDefined(typeof(FilterMode), ti.filterMode) ? ti.filterMode : FilterMode.Bilinear;
					entry.settings.maxWidth = 1024;
					entry.settings.maxHeight = 1024;
					entry.atlasName = ParseAtlasName(ti.spritePackingTag);
					entry.packingMode = GetPackingMode(ti.spritePackingTag, tis.spriteMeshType);

					entries.Add(entry);
				}

				Resources.UnloadAsset(ti);
			}

			// First split sprites into groups based on atlas name
			var atlasGroups =
				from e in entries
				group e by e.atlasName;
			foreach (var atlasGroup in atlasGroups)
			{
				int page = 0;
				// Then split those groups into smaller groups based on texture settings
				var settingsGroups =
					from t in atlasGroup
					group t by t.settings;
				foreach (var settingsGroup in settingsGroups)
				{
					string atlasName = atlasGroup.Key;
					if (settingsGroups.Count() > 1)
						atlasName += string.Format(" (Group {0})", page);

					job.AddAtlas(atlasName, settingsGroup.Key);
					foreach (Entry entry in settingsGroup)
					{
						job.AssignToAtlas(atlasName, entry.sprite, entry.packingMode, SpritePackingRotation.None);
					}

					++page;
				}
			}
		}

		protected bool IsTagPrefixed(string packingTag)
		{
			packingTag = packingTag.Trim();
			if (packingTag.Length < TagPrefix.Length)
				return false;
			return (packingTag.Substring(0, TagPrefix.Length) == TagPrefix);
		}

		private string ParseAtlasName(string packingTag)
		{
			string name = packingTag.Trim();
			if (IsTagPrefixed(name))
				name = name.Substring(TagPrefix.Length).Trim();
			return (name.Length == 0) ? "(unnamed)" : name;
		}

		private SpritePackingMode GetPackingMode(string packingTag, SpriteMeshType meshType)
		{
			if (meshType == SpriteMeshType.Tight)
				if (IsTagPrefixed(packingTag) == AllowTightWhenTagged)
					return SpritePackingMode.Tight;
			return SpritePackingMode.Rectangle;
		}
	}