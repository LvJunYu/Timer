// 角色可以使用的时装数据 | 角色可以使用的时装数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ValidAvatarPart : SyncronisticData
    {
        #region 字段

        /// <summary>
        /// 可以使用的头部
        /// </summary>
        public Dictionary<int, AvatarPartItem> ItemHead =new Dictionary<int, AvatarPartItem>();

        /// <summary>
        /// 可以使用的上身
        /// </summary>
        public Dictionary<int, AvatarPartItem> ItemUpper= new Dictionary<int, AvatarPartItem>();


        /// <summary>
        /// 可以使用的下身
        /// </summary>
        public Dictionary<int, AvatarPartItem> ItemLower= new Dictionary<int, AvatarPartItem>();


        /// <summary>
        /// 可以使用的附件
        /// </summary>
        public Dictionary<int, AvatarPartItem> ItemAppendage= new Dictionary<int, AvatarPartItem>();


        #endregion

        #region 属性

        /// <summary>
        /// 正在使用的头部
        /// </summary>

        #endregion

        #region 方法
        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();

            if (ItemDataList != null)
            {
                for (int i = 0; i < ItemDataList.Count; i++)
                {
                    if (ItemDataList[i] != null && ItemDataList[i].Type == (int)EAvatarPart.AP_Head)
                    {
                        if (ItemHead.ContainsKey((int) ItemDataList[i].Id) == false)
                        {
                            ItemHead.Add((int) ItemDataList[i].Id, ItemDataList[i]);
                        }
                    }
                    else if (ItemDataList[i] != null && ItemDataList[i].Type == (int)EAvatarPart.AP_Upper)
                    {
                        if (ItemUpper.ContainsKey((int) ItemDataList[i].Id) == false)
                        {
                            ItemUpper.Add((int) ItemDataList[i].Id, ItemDataList[i]);
                        }
                    }
                    else if (ItemDataList[i] != null && ItemDataList[i].Type == (int)EAvatarPart.AP_Lower)
                    {
                        if (ItemLower.ContainsKey((int) ItemDataList[i].Id) == false)
                        {
                            ItemLower.Add((int) ItemDataList[i].Id, ItemDataList[i]);
                        }
                    }
                    else if (ItemDataList[i] != null && ItemDataList[i].Type == (int)EAvatarPart.AP_Appendage)
                    {
                        if (ItemAppendage.ContainsKey((int) ItemDataList[i].Id) == false)
                        {
                            ItemAppendage.Add((int) ItemDataList[i].Id, ItemDataList[i]);
                        }
                    }
                }
            }
        }
        #endregion
    }
}