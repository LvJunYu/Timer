using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_StandaloneLevel
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
		// 0普通，123奖励
		[ColumnMapping("Type")]
		public int Type;
		// 体力消耗
		[ColumnMapping("EnergyCost")]
		public int EnergyCost;
		// 得星条件
		[ColumnMapping("StarConditions")]
		public int[] StarConditions;
		// 星1数值
		[ColumnMapping("Star1Value")]
		public int Star1Value;
		// 星2数值
		[ColumnMapping("Star2Value")]
		public int Star2Value;
		// 星3数值
		[ColumnMapping("Star3Value")]
		public int Star3Value;
		// 可使用的增益道具
		[ColumnMapping("HelperItems")]
		public int[] HelperItems;
		// 首次通关奖励
		[ColumnMapping("WinReward")]
		public int WinReward;
		// 胜利条件
		[ColumnMapping("WinConditions")]
		public int[] WinConditions;
		// 限时／秒
		[ColumnMapping("TimeLimit")]
		public int TimeLimit;
	}

    public class TableStandaloneLevelAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_StandaloneLevel[] DataArray;
	}
}
