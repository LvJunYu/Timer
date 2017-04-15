using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_AvatarSlotName
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// 名称
		[ColumnMapping("Name")]
		public string Name;
	}

    public class TableAvatarSlotNameAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_AvatarSlotName[] DataArray;
	}
}
