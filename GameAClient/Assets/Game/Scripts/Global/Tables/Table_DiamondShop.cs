using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_DiamondShop
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 描述
        /// </summary>
		[ColumnMapping("Desc")]
		public string Desc;
		/// <summary>
        /// 图片地址
        /// </summary>
		[ColumnMapping("ImgUrl")]
		public string ImgUrl;
		/// <summary>
        /// 价格（Q点）
        /// </summary>
		[ColumnMapping("Price")]
		public int Price;
		/// <summary>
        /// 数量
        /// </summary>
		[ColumnMapping("Count")]
		public int Count;
		/// <summary>
        /// 赠送数量
        /// </summary>
		[ColumnMapping("AdditionalCount")]
		public int AdditionalCount;
	}

    public class TableDiamondShopAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_DiamondShop[] DataArray;
	}
}
