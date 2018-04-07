using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_HonorReport
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 类型
        /// </summary>
		[ColumnMapping("Type")]
		public int Type;
		/// <summary>
        /// 描述
        /// </summary>
		[ColumnMapping("Description")]
		public string Description;
	}

    public class TableHonorReportAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_HonorReport[] DataArray;
	}
}
