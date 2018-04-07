///********************************************************************
//** Filename : AvatarCharacterEntity  
//** Author : ake
//** Date : 3/6/2017 3:02:46 PM
//** Summary : AvatarCharacterEntity  
//***********************************************************************/
//
//using System.Collections.Generic;
//using UnityEngine;
//public class AvatarCharacterEntity
//{
//	public const string AvatarSpinePathStart = "Assets/JoyGameArt/Depends/SpineData/AvatarCharacter";
//
//	public class SpineMatTexturePair
//	{
//		public string SkeletonDataName;
//		public Material Mat;
//		public Texture Tex;
//	}
//
//	private static AvatarCharacterEntity _instance;
//
//	private List<SpineMatTexturePair> _spineMatTexturePairs;
//
//	public List<SpineMatTexturePair> SpineMatTexturePairs
//	{
//		get { return _spineMatTexturePairs; }
//	}
//
//	public static AvatarCharacterEntity Instance
//	{
//		get
//		{
//			if (_instance == null)
//			{
//				_instance = new AvatarCharacterEntity();
//			}
//			return _instance;
//		}
//	}
//
//	public static void Clear()
//	{
//		if (_instance != null)
//		{
//			_instance._spineMatTexturePairs = null;
//		}
//		_instance = null;
//	}
//
//	public AvatarCharacterEntity()
//	{
//		_spineMatTexturePairs = new List<SpineMatTexturePair>();
//	}
//
//	public List<AvatarSpineTextureRelation> SetAvatarTextureToNull(string skeletonDataAssetPath)
//	{
//		if (string.IsNullOrEmpty(skeletonDataAssetPath))
//		{
//			L.Error("SetAvatarTextureToNull called but skeletonDataAssetPath is null or empty; ");
//			return null;
//		}
//		if (!skeletonDataAssetPath.StartsWith(AvatarSpinePathStart))
//		{
//			return null;
//		}
//		string aName;
//		int index = skeletonDataAssetPath.LastIndexOf('/');
//		if (index > 0)
//		{
//			aName = skeletonDataAssetPath.Substring(index + 1, skeletonDataAssetPath.Length - index-1);
//		}
//		else
//		{
//			aName = skeletonDataAssetPath;
//		}
//		string aNameWithoutSuffix = BackupFileInfo.IgnoreSuffix(aName);
//		string aN = BackupFileInfo.GetSpineNameBySkeletonDataAssetName(aName);
//		var atlasData = BackupFileInfo.GetSpineAtlasData(skeletonDataAssetPath, aN);
//		if (atlasData == null || atlasData.materials == null || atlasData.materials.Length == 0)
//		{
//			L.Error("Spine {0} atlasAsset is invalid!,skeletonDataAssetPath is {1}.", aN, skeletonDataAssetPath);
//			return null;
//		}
//
//		List<AvatarSpineTextureRelation> res = new List<AvatarSpineTextureRelation>();
//		for (int i = 0; i < atlasData.materials.Length; i++)
//		{
//			var tmpMat = atlasData.materials[i];
//			if (tmpMat.mainTexture.name == aN)
//			{
//				continue;
//			}
//			SpineMatTexturePair tmpData = new SpineMatTexturePair();
//			tmpData.SkeletonDataName = aNameWithoutSuffix;
//			tmpData.Mat = tmpMat;
//			tmpData.Tex = tmpMat.mainTexture;
//			tmpMat.mainTexture = null;
//			_spineMatTexturePairs.Add(tmpData);
//
//			AvatarSpineTextureRelation tmpRelation = new AvatarSpineTextureRelation();
//			tmpRelation.MatName = tmpData.Mat.name;
//			tmpRelation.TextureName = tmpData.Tex.name;
//			res.Add(tmpRelation);
//		}
//		return res;
//	}
//
//	public void RevertAllSpineMatTexturePair()
//	{
//		if (_spineMatTexturePairs == null)
//		{
//			return;
//		}
//		for (int i = 0; i < _spineMatTexturePairs.Count; i++)
//		{
//			var item = _spineMatTexturePairs[i];
//			item.Mat.mainTexture = item.Tex;
//		}
//	}
//}
