using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_AvatarStruct
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 说明
        /// </summary>
		[ColumnMapping("Description")]
		public string Description;
		/// <summary>
        /// 头部插槽
        /// </summary>
		[ColumnMapping("HeadSlots")]
		public int[] HeadSlots;
		/// <summary>
        /// 上身插槽
        /// </summary>
		[ColumnMapping("UpperSlots")]
		public int[] UpperSlots;
		/// <summary>
        /// 下身插槽
        /// </summary>
		[ColumnMapping("LowerSlots")]
		public int[] LowerSlots;
		/// <summary>
        /// 附加物插槽
        /// </summary>
		[ColumnMapping("AppendageSlots")]
		public int[] AppendageSlots;
	}

    public class TableAvatarStructAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_AvatarStruct[] DataArray;
	}
}
