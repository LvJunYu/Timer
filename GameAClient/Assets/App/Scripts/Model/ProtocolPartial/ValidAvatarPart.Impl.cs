// 角色可以使用的时装数据 | 角色可以使用的时装数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ValidAvatarPart
    {
        #region 字段

        /// <summary>
        /// 可以使用的头部
        /// </summary>
        private Dictionary<int, AvatarPartItem> ItemHead =new Dictionary<int, AvatarPartItem>();
        /// <summary>
        /// 可以使用的上身
        /// </summary>
        private Dictionary<int, AvatarPartItem> ItemUpper= new Dictionary<int, AvatarPartItem>();
        /// <summary>
        /// 可以使用的下身
        /// </summary>
        private Dictionary<int, AvatarPartItem> ItemLower= new Dictionary<int, AvatarPartItem>();
        /// <summary>
        /// 可以使用的附件
        /// </summary>
        private Dictionary<int, AvatarPartItem> ItemAppendage= new Dictionary<int, AvatarPartItem>();
        #endregion

        #region 属性
        public AvatarPartItem GetItemInHeadDictionary(int key)
        {

            AvatarPartItem tmp;
            if (ItemHead.TryGetValue(key, out tmp))
            {
                return tmp;
            }
            return null;

        }
        public AvatarPartItem GetItemInUpperDictionary(int key)
        {
            AvatarPartItem tmp;
            if (ItemUpper.TryGetValue(key, out tmp))
            {
                return tmp;
            }
            return null;
        }
        public AvatarPartItem GetItemInLowerDictionary(int key)
        {
            AvatarPartItem tmp;
            if (ItemLower.TryGetValue(key, out tmp))
            {
                return tmp;
            }
            return null;
        }
        public AvatarPartItem GetItemInAppendageDictionary(int key)
        {
            AvatarPartItem tmp;
            if (ItemAppendage.TryGetValue(key, out tmp))
            {
                return tmp;
            }
            return null;
        }
        /// <summary>
        /// 正在使用的头部
        /// </summary>

        #endregion

        #region 方法
        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();
            ItemHead.Clear();
            ItemUpper.Clear();
            ItemLower.Clear();
            ItemAppendage.Clear();
            if (ItemDataList != null)
            {
                for (int i = 0; i < ItemDataList.Count; i++)
                {
                    if (ItemDataList[i] != null && ItemDataList[i].Type == (int)EAvatarPart.AP_Head)
                    {
                        //if (ItemHead.ContainsKey((int) ItemDataList[i].Id) == false)
                        
                            ItemHead.Add((int) ItemDataList[i].Id, ItemDataList[i]);
                        
                    }
                    else if (ItemDataList[i] != null && ItemDataList[i].Type == (int)EAvatarPart.AP_Upper)
                    {
                        //if (ItemUpper.ContainsKey((int) ItemDataList[i].Id) == false)
                        
                            ItemUpper.Add((int) ItemDataList[i].Id, ItemDataList[i]);
                        
                    }
                    else if (ItemDataList[i] != null && ItemDataList[i].Type == (int)EAvatarPart.AP_Lower)
                    {
                        //if (ItemLower.ContainsKey((int) ItemDataList[i].Id) == false)
                        
                            ItemLower.Add((int) ItemDataList[i].Id, ItemDataList[i]);
                        
                    }
                    else if (ItemDataList[i] != null && ItemDataList[i].Type == (int)EAvatarPart.AP_Appendage)
                    {
                        //if (ItemAppendage.ContainsKey((int) ItemDataList[i].Id) == false)
                        
                            ItemAppendage.Add((int) ItemDataList[i].Id, ItemDataList[i]);
                        
                    }
                }
            }
        }
        #endregion
    }
}