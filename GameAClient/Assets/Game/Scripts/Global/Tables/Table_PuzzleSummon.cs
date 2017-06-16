using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_PuzzleSummon
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 消耗
        /// </summary>
		[ColumnMapping("Cost")]
		public int Cost;
	}

    public class TablePuzzleSummonAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_PuzzleSummon[] DataArray;
	}
}
