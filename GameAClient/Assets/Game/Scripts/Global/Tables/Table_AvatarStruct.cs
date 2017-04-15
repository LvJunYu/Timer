using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_AvatarStruct
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// 说明
		[ColumnMapping("Description")]
		public string Description;
		// 头部插槽
		[ColumnMapping("HeadSlots")]
		public int[] HeadSlots;
		// 上身插槽
		[ColumnMapping("UpperSlots")]
		public int[] UpperSlots;
		// 下身插槽
		[ColumnMapping("LowerSlots")]
		public int[] LowerSlots;
		// 附加物插槽
		[ColumnMapping("AppendageSlots")]
		public int[] AppendageSlots;
	}

    public class TableAvatarStructAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_AvatarStruct[] DataArray;
	}
}
