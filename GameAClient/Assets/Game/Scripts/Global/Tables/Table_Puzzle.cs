using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Puzzle
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
        /// 描述
        /// </summary>
		[ColumnMapping("Description")]
		public string Description;
		/// <summary>
        /// 等级
        /// </summary>
		[ColumnMapping("Level")]
		public int Level;
		/// <summary>
        /// 属性加成类型
        /// </summary>
		[ColumnMapping("AttriBonus")]
		public int AttriBonus;
		/// <summary>
        /// 属性加成数值
        /// </summary>
		[ColumnMapping("AttriValue")]
		public int AttriValue;
		/// <summary>
        /// 合成所需金币
        /// </summary>
		[ColumnMapping("MergeCost")]
		public int MergeCost;
	}

    public class TablePuzzleAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Puzzle[] DataArray;
	}
}
