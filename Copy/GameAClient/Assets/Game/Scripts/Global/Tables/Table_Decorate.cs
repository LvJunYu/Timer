using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Decorate
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 位置
        /// </summary>
		[ColumnMapping("Position")]
		public int Position;
		/// <summary>
        /// 解锁章节
        /// </summary>
		[ColumnMapping("Chapter")]
		public int Chapter;
		/// <summary>
        /// 资源
        /// </summary>
		[ColumnMapping("Resource")]
		public string Resource;
	}

    public class TableDecorateAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Decorate[] DataArray;
	}
}
