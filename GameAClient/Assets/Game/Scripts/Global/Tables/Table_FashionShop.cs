using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_FashionShop
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// 所属分页
		[ColumnMapping("PageIdx")]
		public int PageIdx;
		// 种类
		[ColumnMapping("Type")]
		public int Type;
		// 物品序号
		[ColumnMapping("ItemIdx")]
		public int ItemIdx;
		// 性别
		[ColumnMapping("Sex")]
		public int Sex;
	}
    /// <summary>
    /// 时装店资源表
    /// </summary>
    public class TableFashionShopAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_FashionShop[] DataArray;
	}
}
