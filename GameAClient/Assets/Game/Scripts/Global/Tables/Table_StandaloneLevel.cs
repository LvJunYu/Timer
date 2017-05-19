using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_StandaloneLevel
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
        /// 0普通，123奖励
        /// </summary>
		[ColumnMapping("Type")]
		public int Type;
		/// <summary>
        /// 体力消耗
        /// </summary>
		[ColumnMapping("EnergyCost")]
		public int EnergyCost;
		/// <summary>
        /// 得星条件
        /// </summary>
		[ColumnMapping("StarConditions")]
		public int[] StarConditions;
		/// <summary>
        /// 星1数值
        /// </summary>
		[ColumnMapping("Star1Value")]
		public int Star1Value;
		/// <summary>
        /// 星2数值
        /// </summary>
		[ColumnMapping("Star2Value")]
		public int Star2Value;
		/// <summary>
        /// 星3数值
        /// </summary>
		[ColumnMapping("Star3Value")]
		public int Star3Value;
		/// <summary>
        /// 可使用的增益道具
        /// </summary>
		[ColumnMapping("HelperItems")]
		public int[] HelperItems;
		/// <summary>
        /// 首次通关奖励
        /// </summary>
		[ColumnMapping("WinReward")]
		public int WinReward;
		/// <summary>
        /// 失败通关奖励
        /// </summary>
		[ColumnMapping("LoseReward")]
		public int LoseReward;
		/// <summary>
        /// 胜利条件
        /// </summary>
		[ColumnMapping("WinConditions")]
		public int[] WinConditions;
		/// <summary>
        /// 限时／秒
        /// </summary>
		[ColumnMapping("TimeLimit")]
		public int TimeLimit;
	}

    public class TableStandaloneLevelAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_StandaloneLevel[] DataArray;
	}
}
