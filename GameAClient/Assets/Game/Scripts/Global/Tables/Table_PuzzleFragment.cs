using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_PuzzleFragment
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
	}

    public class TablePuzzleFragmentAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_PuzzleFragment[] DataArray;
	}
}
