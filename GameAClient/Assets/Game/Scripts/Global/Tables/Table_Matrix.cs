using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Matrix
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// Name
		[ColumnMapping("Name")]
		public string Name;
		// GameType
		[ColumnMapping("GameType")]
		public string GameType;
		// MatrixType
		[ColumnMapping("MatrixType")]
		public int MatrixType;
		// UnitIds
		[ColumnMapping("UnitIds")]
		public int[] UnitIds;
		// Icon
		[ColumnMapping("Icon")]
		public string Icon;
		// Summary
		[ColumnMapping("Summary")]
		public string Summary;
		// BgResName
		[ColumnMapping("BgResName")]
		public string BgResName;
	}

    public class TableMatrixAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Matrix[] DataArray;
	}
}
