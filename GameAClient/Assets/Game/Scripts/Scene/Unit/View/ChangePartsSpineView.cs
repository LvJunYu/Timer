/********************************************************************
** Filename : ChangePartsSpineView
** Author : cwc
** Date : 2017/03/03 星期日 下午 6:48:41
** Summary : ChangePartsSpineView
***********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoyEngine;
using Spine;
using Spine.Unity;
using NewResourceSolution;

namespace GameA.Game
{
	[Poolable(MinPoolSize = 1, PreferedPoolSize = 100, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
	public class ChangePartsSpineView : SpineUnit {

		protected Skeleton _skeleton;
		protected Skin _dynamicSkin;
		protected Skin _baseSkin;

		protected string _skeletonName;
		protected int _avatarId;
		protected int[] _partsIds = new int[(int)SpinePartsHelper.ESpineParts.Max];
		protected Dictionary<int, int> _slotName2Index = new Dictionary<int, int>();

		public int[] EquipedPartsIds {
			get { return _partsIds; }
		}
		protected override bool OnInit()
		{
			var tableUnit = _unit.TableUnit;
            string skeletonDataAssetName = string.Format ("{0}_SkeletonData", tableUnit.Model);
            SkeletonDataAsset data = ResourcesManager.Instance.GetAsset<SkeletonDataAsset>(
                EResType.SpineData,
                skeletonDataAssetName,
                0
            );
            if (null == data)
			{
				LogHelper.Error("TryGetSpineDataByName Failed! {0}", tableUnit.Model);
				return false;
			}

			_skeletonAnimation.skeletonDataAsset = data;
			LinkBaseSkinTextures ();
			_skeletonAnimation.Initialize(true);
			_skeletonAnimation.enabled = true;
            _animation.Set();

			_renderer = _skeletonAnimation.GetComponent<Renderer>();
			_renderer.sortingOrder = UnitManager.Instance.GetSortingOrder(tableUnit);

			_avatarId = 1;
			_skeletonName = _unit.TableUnit.Model;
			_skeleton = _skeletonAnimation.skeleton;
			if (_skeleton == null) {
				LogHelper.Error("Try get skeleton failed when init {0}", _unit.TableUnit.Name);
				return false;
			}
			string baseSkinName = string.Format("Skin_{0}_0", _skeletonName);
			_baseSkin = _skeleton.data.FindSkin(baseSkinName);
			if (_baseSkin == null) {
				LogHelper.Error("Try get default skin failed when init {0}", _unit.TableUnit.Name);
				return false;
			}
			_dynamicSkin = _skeleton.data.FindSkin("DynamicSkin");
			if (_dynamicSkin == null) {
				_dynamicSkin = _baseSkin.Dumplicate("DynamicSkin");
				_skeleton.data.Skins.Add(_dynamicSkin);
			}
			_skeleton.skin = _dynamicSkin;

			for (int i = 0, n = _partsIds.Length; i < n; i++) {
				_partsIds[i] = 0;
			}

			InitSlotName2IndexDic();
			return true;
		}

		protected void LinkBaseSkinTextures () {
            // todo update api spine
//			var baseHead = TableManager.Instance.GetHeadParts (1);
//			if (baseHead != null && !string.IsNullOrEmpty(baseHead.SmallTexture)) {
//				GameResourceManager.Instance.LinkAvatarSpineTexture (_skeletonAnimation.skeletonDataAsset, baseHead.SmallTexture);
//			}
//			var baseUpper = TableManager.Instance.GetUpperBodyParts (1);
//			if (baseUpper != null && !string.IsNullOrEmpty(baseUpper.SmallTexture)) {
//				GameResourceManager.Instance.LinkAvatarSpineTexture (_skeletonAnimation.skeletonDataAsset, baseUpper.SmallTexture);
//			}
//			var baseLower = TableManager.Instance.GetLowerBodyParts (1);
//			if (baseLower != null && !string.IsNullOrEmpty(baseLower.SmallTexture)) {
//				GameResourceManager.Instance.LinkAvatarSpineTexture (_skeletonAnimation.skeletonDataAsset, baseLower.SmallTexture);
//			}
//			var baseAppendage = TableManager.Instance.GetAppendageParts (1);
//			if (baseAppendage != null && !string.IsNullOrEmpty(baseAppendage.SmallTexture)) {
//				GameResourceManager.Instance.LinkAvatarSpineTexture (_skeletonAnimation.skeletonDataAsset, baseAppendage.SmallTexture);
//			}
		}

		public bool HomePlayerAvatarViewInit(SkeletonAnimation animationComp) {
			_skeletonAnimation = animationComp;
			_renderer = animationComp.GetComponent<Renderer> ();
			_skeletonAnimation.enabled = true;

			_avatarId = 0;
			_skeletonName = "SMainBoy0";
			_skeleton = _skeletonAnimation.skeleton;
			if (_skeleton == null) {
				LogHelper.Error("Try get skeleton failed when init home avatar");
				return false;
			}
			string baseSkinName = string.Format("Skin_{0}_0", _skeletonName);
			_baseSkin = _skeleton.data.FindSkin(baseSkinName);
			if (_baseSkin == null) {
				LogHelper.Error("Try get default skin failed when init home avatar");
				return false;
			}
			_dynamicSkin = _skeleton.data.FindSkin("DynamicSkin");
			if (_dynamicSkin == null) {
				_dynamicSkin = _baseSkin.Dumplicate("DynamicSkin");
				_skeleton.data.Skins.Add(_dynamicSkin);
			}
			_skeleton.skin = _dynamicSkin;

			for (int i = 0, n = _partsIds.Length; i < n; i++) {
				_partsIds[i] = 0;
			}

			InitSlotName2IndexDic();
			return true;
		}

		private void InitSlotName2IndexDic () {
			_slotName2Index.Clear();
			ExposedList<Slot> slots = _skeleton.slots;
			for (int i = 0, n = slots.Count; i < n; i++) {
				Slot slot = slots.Items[i];
//				Debug.Log("Slot" + i + ": " +slot.data.name);
				string[] splits = slot.data.name.Split('_');
				if (splits.Length < 2) continue;
				string slotName = splits[splits.Length - 1];

				int id = SpinePartsHelper.GetSlotNameId (slotName);
				if (id >= 0) {
					_slotName2Index[id] = i;
				}
			}
		}

		public bool SetParts (int partsId, SpinePartsHelper.ESpineParts partsType, bool homeAvatar = false) {
            //TODO DEL 临时代码 空指针
            //if (partsId == 1)
            //{
            //    return false;
            //}

			if (_partsIds[(int)partsType] == partsId) return false;
			string textureName = "";
			int[] slotsNameIdxList = null;
			int skinId = 0;
			if (partsType == SpinePartsHelper.ESpineParts.Head) {
				var partData = TableManager.Instance.GetFashionUnit(partsId);
				if (partData == null) {
					LogHelper.Error ("Try to apply undefined part:{0} with id:{1}", partsType, partsId);
					return false;
				}
				textureName = homeAvatar ? partData.BigTexture : partData.SmallTexture;
				skinId = partData.SkinId;
				slotsNameIdxList = TableManager.Instance.GetAvatarStruct (_avatarId).HeadSlots;
			} else if (partsType == SpinePartsHelper.ESpineParts.Upper) {
				var partData = TableManager.Instance.GetFashionUnit(partsId);
				if (partData == null) {
					LogHelper.Error ("Try to apply undefined part:{0} with id:{1}", partsType, partsId);
					return false;
				}
				textureName = homeAvatar ? partData.BigTexture : partData.SmallTexture;
				skinId = partData.SkinId;
				slotsNameIdxList = TableManager.Instance.GetAvatarStruct (_avatarId).UpperSlots;
			} else if (partsType == SpinePartsHelper.ESpineParts.Lower) {
				var partData = TableManager.Instance.GetFashionUnit(partsId);
				if (partData == null) {
					LogHelper.Error ("Try to apply undefined part:{0} with id:{1}", partsType, partsId);
					return false;
				}
				textureName = homeAvatar ? partData.BigTexture : partData.SmallTexture;
				skinId = partData.SkinId;
				slotsNameIdxList = TableManager.Instance.GetAvatarStruct (_avatarId).LowerSlots;
			} else if (partsType == SpinePartsHelper.ESpineParts.Appendage) {
				var partData = TableManager.Instance.GetFashionUnit(partsId);
				if (partData == null) {
					LogHelper.Error ("Try to apply undefined part:{0} with id:{1}", partsType, partsId);
					return false;
				}
				textureName = homeAvatar ? partData.BigTexture : partData.SmallTexture;
				skinId = partData.SkinId;
				slotsNameIdxList = TableManager.Instance.GetAvatarStruct (_avatarId).AppendageSlots;
			} else {
				return false;
			}

			Skin targetSkin;
			string targetSkinName = string.Format("Skin_{0}_{1}", _skeletonName, skinId);
			targetSkin = _skeleton.data.FindSkin(targetSkinName);
			if (targetSkin == null) {
				LogHelper.Error("Try get skin:{0} in {1} failed.", targetSkinName, _skeletonName);
				return false;
			}
			ExposedList<Slot> slots = _skeleton.slots;
            if(slotsNameIdxList!=null)
			for (int i = 0, n = slotsNameIdxList.Length; i < n; i++) {
				int slotIdx;
				if (!_slotName2Index.TryGetValue (slotsNameIdxList[i], out slotIdx)) {
					//					LogHelper.Error("Try get slot:{0} in {1} failed.",
					//						SpinePartsDefine.SlotsNames[SpinePartsDefine.SlotIdsInParts[(int)partsType][i]],
					//						_skeletonName);
					//					return false;
					continue;
				}
				Attachment attachment;
				var slotTable = TableManager.Instance.GetAvatarSlotName (slotsNameIdxList[i]);
				if (slotTable == null) {
					continue;
				}
				string attachmentName = string.Format ("{0}/{1}", _skeletonName, slotTable.Name);
                attachment = targetSkin.GetAttachment(slotIdx, attachmentName);
                _dynamicSkin.RemoveAttachment(slotIdx, attachmentName);
                if (attachment == null) {
                    //                  LogHelper.Error("Try get attachment:{0} in {1}, skin{2} failed.",
                    //                      attachmentName,
                    //                      _skeletonName,
                    //                      skinId);
                    //                  return false;
                    slots.Items [slotIdx].Attachment = null;
                    continue;
                } else
                {
                        // todo update api spine
//                    if (GameResourceManager.Instance != null && !GameResourceManager.Instance.LinkAvatarSpineTexture (_skeletonAnimation.skeletonDataAsset, textureName)) {
//                        LogHelper.Error ("Link texture: {0} when apply parts:{1},id{2} in {3} failed.", textureName, partsType, partsId, _unit.TableUnit.Name);
//                    }
                    
                    _dynamicSkin.AddAttachment(slotIdx, attachmentName, attachment);
                    
                    slots.Items[slotIdx].Attachment = attachment;
                }
			}


			_skeleton.slots = slots;
			_skeleton.skin = _dynamicSkin;

			_partsIds[(int)partsType] = partsId;

			return true;
		}
	}
}