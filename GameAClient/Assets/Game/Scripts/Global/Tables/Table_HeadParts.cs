using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_HeadParts
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// 名称
		[ColumnMapping("Name")]
		public string Name;
		// 介绍
		[ColumnMapping("Description")]
		public string Description;
		// 性别
		[ColumnMapping("Sex")]
		public int Sex;
		// 能装备角色
		[ColumnMapping("Character")]
		public int Character;
		// 金币价格1天
		[ColumnMapping("PriceGoldDay")]
		public int PriceGoldDay;
		// 金币价格7天
		[ColumnMapping("PriceGoldWeek")]
		public int PriceGoldWeek;
		// 金币价格30天
		[ColumnMapping("PriceGoldMonth")]
		public int PriceGoldMonth;
		// 金币价格永久
		[ColumnMapping("PriceGoldPermanent")]
		public int PriceGoldPermanent;
		// 钻石价格一天
		[ColumnMapping("PriceDiamondDay")]
		public int PriceDiamondDay;
		// 钻石价格一周
		[ColumnMapping("PriceDiamondWeek")]
		public int PriceDiamondWeek;
		// 钻石价格一月
		[ColumnMapping("PriceDiamondMonth")]
		public int PriceDiamondMonth;
		// 钻石价格永久
		[ColumnMapping("PriceDiamondPermanent")]
		public int PriceDiamondPermanent;
		// 大贴图
		[ColumnMapping("BigTexture")]
		public string BigTexture;
		// 小贴图
		[ColumnMapping("SmallTexture")]
		public string SmallTexture;
		// 预览图
		[ColumnMapping("PreviewTexture")]
		public string PreviewTexture;
		// 所在皮肤id
		[ColumnMapping("SkinId")]
		public int SkinId;
	}

    public class TableHeadPartsAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_HeadParts[] DataArray;
	}
}
