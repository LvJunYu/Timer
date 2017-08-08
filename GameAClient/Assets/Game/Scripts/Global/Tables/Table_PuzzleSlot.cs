using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_PuzzleSlot
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 解锁等级
        /// </summary>
		[ColumnMapping("UnlockLevel")]
		public int UnlockLevel;
	}

    public class TablePuzzleSlotAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_PuzzleSlot[] DataArray;
	}
}
