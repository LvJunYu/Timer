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
        /// 品质
        /// </summary>
		[ColumnMapping("Quality")]
		public int Quality;
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
		/// <summary>
        /// 最大等级
        /// </summary>
		[ColumnMapping("MaxLevel")]
		public int MaxLevel;
		/// <summary>
        /// 组成的碎片
        /// </summary>
		[ColumnMapping("Fragments")]
		public int[] Fragments;
	}

    public class TablePuzzleAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Puzzle[] DataArray;
	}
}
