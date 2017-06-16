using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_FashionCoupon
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 名字
        /// </summary>
		[ColumnMapping("Name")]
		public string Name;
		/// <summary>
        /// 部件类型
        /// </summary>
		[ColumnMapping("Type")]
		public int Type;
		/// <summary>
        /// 部件Id
        /// </summary>
		[ColumnMapping("ItemIdx")]
		public int ItemIdx;
		/// <summary>
        /// 部件时长
        /// </summary>
		[ColumnMapping("ItemTime")]
		public int ItemTime;
		/// <summary>
        /// 折扣
        /// </summary>
		[ColumnMapping("Discount")]
		public int Discount;
		/// <summary>
        /// 有效期（天）
        /// </summary>
		[ColumnMapping("ValidTime")]
		public int ValidTime;
	}

    public class TableFashionCouponAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_FashionCoupon[] DataArray;
	}
}
