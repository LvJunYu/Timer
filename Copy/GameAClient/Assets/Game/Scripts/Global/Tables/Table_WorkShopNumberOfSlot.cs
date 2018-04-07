using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_WorkShopNumberOfSlot
	{
		/// <summary>
        /// 匠人等级
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 槽位的数目
        /// </summary>
		[ColumnMapping("Num")]
		public int Num;
	}

    public class TableWorkShopNumberOfSlotAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_WorkShopNumberOfSlot[] DataArray;
	}
}
