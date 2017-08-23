using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_StandaloneChapter
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
        /// 描述
        /// </summary>
		[ColumnMapping("Description")]
		public string Description;
		/// <summary>
        /// 普通关卡
        /// </summary>
		[ColumnMapping("NormalLevels")]
		public int[] NormalLevels;
		/// <summary>
        /// 奖励关卡
        /// </summary>
		[ColumnMapping("BonusLevels")]
		public int[] BonusLevels;
		/// <summary>
        /// 解锁等级
        /// </summary>
		[ColumnMapping("UnlockLevel")]
		public int UnlockLevel;
		/// <summary>
        /// 抽奖券产出概率
        /// </summary>
		[ColumnMapping("TicketProbability")]
		public int TicketProbability;
		/// <summary>
        /// 抽奖券可能奖励
        /// </summary>
		[ColumnMapping("TicketRewards")]
		public int[] TicketRewards;
		/// <summary>
        /// 抽奖券比例
        /// </summary>
		[ColumnMapping("TicketProportions")]
		public int[] TicketProportions;
		/// <summary>
        /// 拼图产出概率
        /// </summary>
		[ColumnMapping("PuzzleProbability")]
		public int PuzzleProbability;
		/// <summary>
        /// 拼图可能奖励
        /// </summary>
		[ColumnMapping("PuzzleRewards")]
		public int[] PuzzleRewards;
		/// <summary>
        /// 拼图产出比例
        /// </summary>
		[ColumnMapping("PuzzleProportions")]
		public int[] PuzzleProportions;
		/// <summary>
        /// 宝藏奖励
        /// </summary>
		[ColumnMapping("TreasureRewards")]
		public int[] TreasureRewards;
		/// <summary>
        /// 宝藏比例
        /// </summary>
		[ColumnMapping("TreasureProportions")]
		public int[] TreasureProportions;
	}

    public class TableStandaloneChapterAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_StandaloneChapter[] DataArray;
	}
}
