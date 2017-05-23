using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_ProgressUnlock
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
		/// <summary>
        /// 说明
        /// </summary>
		[ColumnMapping("Desc")]
		public string Desc;
		/// <summary>
        /// 图标
        /// </summary>
		[ColumnMapping("Icon")]
		public string Icon;
		/// <summary>
        ///  解锁章节
        /// </summary>
		[ColumnMapping("Chapter")]
		public int Chapter;
		/// <summary>
        /// 解锁关卡
        /// </summary>
		[ColumnMapping("Level")]
		public int Level;
	}

    public class TableProgressUnlockAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_ProgressUnlock[] DataArray;
	}
}
