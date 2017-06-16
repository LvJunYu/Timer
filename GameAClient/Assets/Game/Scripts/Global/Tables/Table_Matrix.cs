using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Matrix
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// Name
        /// </summary>
		[ColumnMapping("Name")]
		public string Name;
		/// <summary>
        /// GameType
        /// </summary>
		[ColumnMapping("GameType")]
		public string GameType;
		/// <summary>
        /// MatrixType
        /// </summary>
		[ColumnMapping("MatrixType")]
		public int MatrixType;
		/// <summary>
        /// UnitIds
        /// </summary>
		[ColumnMapping("UnitIds")]
		public int[] UnitIds;
		/// <summary>
        /// Icon
        /// </summary>
		[ColumnMapping("Icon")]
		public string Icon;
		/// <summary>
        /// Summary
        /// </summary>
		[ColumnMapping("Summary")]
		public string Summary;
		/// <summary>
        /// BgResName
        /// </summary>
		[ColumnMapping("BgResName")]
		public string BgResName;
	}

    public class TableMatrixAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Matrix[] DataArray;
	}
}
