using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Morph
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 名称
        /// </summary>
		[ColumnMapping("Status")]
		public string Status;
		/// <summary>
        /// 名称
        /// </summary>
		[ColumnMapping("Name")]
		public string Name;
		/// <summary>
        /// ZRot
        /// </summary>
		[ColumnMapping("ZRot")]
		public float ZRot;
		/// <summary>
        /// Count
        /// </summary>
		[ColumnMapping("Count")]
		public int Count;
	}

    public class TableMorphAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Morph[] DataArray;
	}
}
