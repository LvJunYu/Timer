using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameA.Game
{
    public class SpinePartsHelper
    {
        // 换装部件
        public enum ESpineParts
        {
            Head, // 头部
            Upper, // 上身
            Lower, // 下身
            Appendage, // 附加
            Max, //
        }

        public static int GetSlotNameId(string slotName)
        {
            for (int i = 0, n = TableManager.Instance.Table_AvatarSlotNameDic.Count; i < n; i++)
            {
                var slotTable = TableManager.Instance.GetAvatarSlotName(i);
                if (slotName == slotTable.Name) return i;
            }
            return -1;
        }

        public static string GetSlotName(ESpineParts partsType, int slotIdx)
        {
            return null;
        }
    }
}