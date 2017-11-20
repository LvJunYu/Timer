/********************************************************************
** Filename : ChangePartsSpineView
** Author : cwc
** Date : 2017/03/03 星期日 下午 6:48:41
** Summary : ChangePartsSpineView
***********************************************************************/

using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 1, PreferedPoolSize = 100, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
    public class ChangePartsSpineView : SpineUnit
    {
        protected Skeleton _skeleton;
        protected Skin _dynamicSkin;
        protected Skin _baseSkin;

        protected string _skeletonName;
        protected int _avatarId;
        protected int[] _partsIds = new int[(int) SpinePartsHelper.ESpineParts.Max];
        protected Dictionary<int, int> _slotName2Index = new Dictionary<int, int>();

        protected override bool OnInit()
        {
            var tableUnit = _unit.TableUnit;
            string skeletonDataAssetName = string.Format("{0}_SkeletonData", tableUnit.Model);
            SkeletonDataAsset data = JoyResManager.Instance.GetAsset<SkeletonDataAsset>(
                EResType.SpineData,
                skeletonDataAssetName,
                (int) EResScenary.Default,
                false
            );
            if (null == data)
            {
                LogHelper.Error("TryGetSpineDataByName Failed! {0}", tableUnit.Model);
                return false;
            }

            _skeletonAnimation.skeletonDataAsset = data;
            _skeletonAnimation.Initialize(true);
            _skeletonAnimation.enabled = true;
            _animation.Set();

            _renderer = _skeletonAnimation.GetComponent<Renderer>();
            _renderer.sortingOrder = UnitManager.Instance.GetSortingOrder(tableUnit);

            _avatarId = 1;
            _skeletonName = _unit.TableUnit.Model;
            _skeleton = _skeletonAnimation.skeleton;
            if (_skeleton == null)
            {
                LogHelper.Error("Try get skeleton failed when init {0}", _unit.TableUnit.Name);
                return false;
            }
            string baseSkinName = string.Format("Skin_{0}_0", _skeletonName);
            _baseSkin = _skeleton.data.FindSkin(baseSkinName);
            if (_baseSkin == null)
            {
                LogHelper.Error("Try get default skin failed when init {0}", _unit.TableUnit.Name);
                return false;
            }
            _dynamicSkin = _skeleton.data.FindSkin("DynamicSkin");
            if (_dynamicSkin == null)
            {
                _dynamicSkin = _baseSkin.Dumplicate("DynamicSkin");
                _skeleton.data.Skins.Add(_dynamicSkin);
            }
            _skeleton.skin = _dynamicSkin;

            for (int i = 0, n = _partsIds.Length; i < n; i++)
            {
                _partsIds[i] = 0;
            }

            InitSlotName2IndexDic();
            return true;
        }

        public bool HomePlayerAvatarViewInit(SkeletonAnimation animationComp)
        {
            _skeletonAnimation = animationComp;
            _renderer = animationComp.GetComponent<Renderer>();
            _skeletonAnimation.enabled = true;

            _avatarId = 0;
            _skeletonName = "SMainBoy0";
            _skeleton = _skeletonAnimation.skeleton;
            if (_skeleton == null)
            {
                LogHelper.Error("Try get skeleton failed when init home avatar");
                return false;
            }
            string baseSkinName = string.Format("Skin_{0}_0", _skeletonName);
            _baseSkin = _skeleton.data.FindSkin(baseSkinName);
            if (_baseSkin == null)
            {
                LogHelper.Error("Try get default skin failed when init home avatar");
                return false;
            }
            _dynamicSkin = _skeleton.data.FindSkin("DynamicSkin");
            if (_dynamicSkin == null)
            {
                _dynamicSkin = _baseSkin.Dumplicate("DynamicSkin");
                _skeleton.data.Skins.Add(_dynamicSkin);
            }
            _skeleton.skin = _dynamicSkin;

            for (int i = 0, n = _partsIds.Length; i < n; i++)
            {
                _partsIds[i] = 0;
            }

            InitSlotName2IndexDic();
            return true;
        }

        private void InitSlotName2IndexDic()
        {
            _slotName2Index.Clear();
            for (int i = 0, n = _skeleton.slots.Count; i < n; i++)
            {
                Slot slot = _skeleton.slots.Items[i];
                string[] splits = slot.data.name.Split('_');
                if (splits.Length < 2)
                {
                    continue;
                }
                string slotName = splits[splits.Length - 1];
                int id = SpinePartsHelper.GetSlotNameId(slotName);
                if (id >= 0)
                {
                    _slotName2Index[id] = i;
                }
            }
        }

        public bool SetParts(int partsId, SpinePartsHelper.ESpineParts partsType, bool homeAvatar = false)
        {
            if (_partsIds[(int) partsType] == partsId)
            {
                return false;
            }
            var partData = TableManager.Instance.GetFashionUnit(partsId);
            if (partData == null)
            {
                LogHelper.Error("Try to apply undefined part:{0} with id:{1}", partsType, partsId);
                return false;
            }
            var tableAvatarStruct = TableManager.Instance.GetAvatarStruct(_avatarId);
            if (tableAvatarStruct == null)
            {
                LogHelper.Error("SetParts Failed: {0} with id:{1}", partsType, partsId);
                return false;
            }
            int[] slotsNameIdxList;
            switch (partsType)
            {
                case SpinePartsHelper.ESpineParts.Head:
                    slotsNameIdxList = tableAvatarStruct.HeadSlots;
                    break;
                case SpinePartsHelper.ESpineParts.Upper:
                    slotsNameIdxList = tableAvatarStruct.UpperSlots;
                    break;
                case SpinePartsHelper.ESpineParts.Lower:
                    slotsNameIdxList = tableAvatarStruct.LowerSlots;
                    break;
                case SpinePartsHelper.ESpineParts.Appendage:
                    slotsNameIdxList = tableAvatarStruct.AppendageSlots;
                    break;
                default:
                    return false;
            }
            string targetSkinName = string.Format("Skin_{0}_{1}", _skeletonName, partData.SkinId);
            var targetSkin = _skeleton.data.FindSkin(targetSkinName);
            if (targetSkin == null)
            {
                LogHelper.Error("Try get skin:{0} in {1} failed.", targetSkinName, _skeletonName);
                return false;
            }
            ExposedList<Slot> slots = _skeleton.slots;
            if (slotsNameIdxList != null)
                for (int i = 0, n = slotsNameIdxList.Length; i < n; i++)
                {
                    int slotIdx;
                    if (!_slotName2Index.TryGetValue(slotsNameIdxList[i], out slotIdx))
                    {
                        continue;
                    }
                    var slotTable = TableManager.Instance.GetAvatarSlotName(slotsNameIdxList[i]);
                    if (slotTable == null)
                    {
                        continue;
                    }
                    string attachmentName = string.Format("{0}/{1}", _skeletonName, slotTable.Name);
                    var attachment = targetSkin.GetAttachment(slotIdx, attachmentName);
                    _dynamicSkin.RemoveAttachment(slotIdx, attachmentName);
                    if (attachment == null)
                    {
                        slots.Items[slotIdx].Attachment = null;
                        continue;
                    }
                    _dynamicSkin.AddAttachment(slotIdx, attachmentName, attachment);
                    slots.Items[slotIdx].Attachment = attachment;
                }
            _skeleton.slots = slots;
            _skeleton.skin = _dynamicSkin;
            _partsIds[(int) partsType] = partsId;
            return true;
        }
    }
}