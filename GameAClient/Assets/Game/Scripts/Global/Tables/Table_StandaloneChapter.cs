using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_StandaloneChapter
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// 名字
		[ColumnMapping("Name")]
		public string Name;
		// 描述
		[ColumnMapping("Description")]
		public string Description;
		// 普通关卡
		[ColumnMapping("NormalLevels")]
		public int[] NormalLevels;
		// 奖励关卡
		[ColumnMapping("BonusLevels")]
		public int[] BonusLevels;
		// 抽奖券产出概率
		[ColumnMapping("TicketProbability")]
		public int TicketProbability;
		// 抽奖券可能奖励
		[ColumnMapping("TicketRewards")]
		public int[] TicketRewards;
		// 抽奖券比例
		[ColumnMapping("TicketProportions")]
		public int[] TicketProportions;
		// 拼图产出概率
		[ColumnMapping("PuzzleProbability")]
		public int PuzzleProbability;
		// 拼图可能奖励
		[ColumnMapping("PuzzleRewards")]
		public int[] PuzzleRewards;
		// 拼图产出比例
		[ColumnMapping("PuzzleProportions")]
		public int[] PuzzleProportions;
		// 宝藏奖励
		[ColumnMapping("TreasureRewards")]
		public int[] TreasureRewards;
		// 宝藏比例
		[ColumnMapping("TreasureProportions")]
		public int[] TreasureProportions;
	}

    public class TableStandaloneChapterAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_StandaloneChapter[] DataArray;
	}
}
