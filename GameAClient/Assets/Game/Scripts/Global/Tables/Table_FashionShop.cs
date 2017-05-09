using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_FashionShop
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 所属分页
        /// </summary>
		[ColumnMapping("PageIdx")]
		public int PageIdx;
		/// <summary>
        /// 种类
        /// </summary>
		[ColumnMapping("Type")]
		public int Type;
		/// <summary>
        /// 物品ID
        /// </summary>
		[ColumnMapping("ItemIdx")]
		public int ItemIdx;
		/// <summary>
        /// 性别
        /// </summary>
		[ColumnMapping("Sex")]
		public int Sex;
	}

    public class TableFashionShopAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_FashionShop[] DataArray;
	}
}
