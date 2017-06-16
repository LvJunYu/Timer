using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_LowerBodyParts
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
        /// 介绍
        /// </summary>
		[ColumnMapping("Description")]
		public string Description;
		/// <summary>
        /// 性别
        /// </summary>
		[ColumnMapping("Sex")]
		public int Sex;
		/// <summary>
        /// 能装备角色
        /// </summary>
		[ColumnMapping("Character")]
		public int Character;
		/// <summary>
        /// 金币价格1天
        /// </summary>
		[ColumnMapping("PriceGoldDay")]
		public int PriceGoldDay;
		/// <summary>
        /// 金币价格7天
        /// </summary>
		[ColumnMapping("PriceGoldWeek")]
		public int PriceGoldWeek;
		/// <summary>
        /// 金币价格30天
        /// </summary>
		[ColumnMapping("PriceGoldMonth")]
		public int PriceGoldMonth;
		/// <summary>
        /// 金币价格永久
        /// </summary>
		[ColumnMapping("PriceGoldPermanent")]
		public int PriceGoldPermanent;
		/// <summary>
        /// 钻石价格一天
        /// </summary>
		[ColumnMapping("PriceDiamondDay")]
		public int PriceDiamondDay;
		/// <summary>
        /// 钻石价格一周
        /// </summary>
		[ColumnMapping("PriceDiamondWeek")]
		public int PriceDiamondWeek;
		/// <summary>
        /// 钻石价格一月
        /// </summary>
		[ColumnMapping("PriceDiamondMonth")]
		public int PriceDiamondMonth;
		/// <summary>
        /// 钻石价格永久
        /// </summary>
		[ColumnMapping("PriceDiamondPermanent")]
		public int PriceDiamondPermanent;
		/// <summary>
        /// 大贴图
        /// </summary>
		[ColumnMapping("BigTexture")]
		public string BigTexture;
		/// <summary>
        /// 小贴图
        /// </summary>
		[ColumnMapping("SmallTexture")]
		public string SmallTexture;
		/// <summary>
        /// 预览图
        /// </summary>
		[ColumnMapping("PreviewTexture")]
		public string PreviewTexture;
		/// <summary>
        /// 所在皮肤id
        /// </summary>
		[ColumnMapping("SkinId")]
		public int SkinId;
	}

    public class TableLowerBodyPartsAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_LowerBodyParts[] DataArray;
	}
}
