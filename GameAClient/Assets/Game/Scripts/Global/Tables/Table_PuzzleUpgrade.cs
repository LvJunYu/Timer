using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_PuzzleUpgrade
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 拼图ID
        /// </summary>
		[ColumnMapping("PuzzleID")]
		public int PuzzleID;
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
        /// 升级所需金币
        /// </summary>
		[ColumnMapping("UpgradeCost")]
		public int UpgradeCost;
	}

    public class TablePuzzleUpgradeAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_PuzzleUpgrade[] DataArray;
	}
}
