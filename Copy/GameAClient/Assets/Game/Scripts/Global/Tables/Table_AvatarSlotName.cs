using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_AvatarSlotName
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

    public class TableAvatarSlotNameAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_AvatarSlotName[] DataArray;
	}
}
