using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_PuzzleSummon
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// 消耗
		[ColumnMapping("Cost")]
		public int Cost;
	}

    public class TablePuzzleSummonAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_PuzzleSummon[] DataArray;
	}
}
