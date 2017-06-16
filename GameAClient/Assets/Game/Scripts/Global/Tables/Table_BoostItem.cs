using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_BoostItem
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 名称
        /// </summary>
		[ColumnMapping("Name")]
		public string Name;
		/// <summary>
        /// 说明
        /// </summary>
		[ColumnMapping("Desc")]
		public string Desc;
		/// <summary>
        /// 图标
        /// </summary>
		[ColumnMapping("Icon")]
		public string Icon;
		/// <summary>
        ///  金币价格
        /// </summary>
		[ColumnMapping("PriceGold")]
		public int PriceGold;
		/// <summary>
        /// 钻石价格
        /// </summary>
		[ColumnMapping("PriceDiamond")]
		public int PriceDiamond;
	}

    public class TableBoostItemAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_BoostItem[] DataArray;
	}
}
