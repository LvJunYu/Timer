using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Puzzle
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// 名称
		[ColumnMapping("Name")]
		public string Name;
		// 描述
		[ColumnMapping("Description")]
		public string Description;
		// 等级
		[ColumnMapping("Level")]
		public int Level;
		// 属性加成类型
		[ColumnMapping("AttriBonus")]
		public int AttriBonus;
		// 属性加成数值
		[ColumnMapping("AttriValue")]
		public int AttriValue;
		// 合成所需金币
		[ColumnMapping("MergeCost")]
		public int MergeCost;
	}

    public class TablePuzzleAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Puzzle[] DataArray;
	}
}
