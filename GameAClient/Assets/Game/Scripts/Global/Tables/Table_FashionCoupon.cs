using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_FashionCoupon
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// 名字
		[ColumnMapping("Name")]
		public string Name;
		// 部件类型
		[ColumnMapping("Type")]
		public int Type;
		// 部件Id
		[ColumnMapping("ItemIdx")]
		public int ItemIdx;
		// 部件时长
		[ColumnMapping("ItemTime")]
		public int ItemTime;
		// 折扣
		[ColumnMapping("Discount")]
		public int Discount;
		// 有效期（天）
		[ColumnMapping("ValidTime")]
		public int ValidTime;
	}

    public class TableFashionCouponAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_FashionCoupon[] DataArray;
	}
}
