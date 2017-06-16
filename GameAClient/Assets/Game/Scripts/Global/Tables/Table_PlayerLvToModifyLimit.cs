using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_PlayerLvToModifyLimit
	{
		/// <summary>
        /// 序号(等级)
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 修改限制
        /// </summary>
		[ColumnMapping("AltLimit")]
		public int AltLimit;
		/// <summary>
        /// 删除限制
        /// </summary>
		[ColumnMapping("DelLimit")]
		public int DelLimit;
		/// <summary>
        /// 添加限制
        /// </summary>
		[ColumnMapping("AddLimit")]
		public int AddLimit;
	}

    public class TablePlayerLvToModifyLimitAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_PlayerLvToModifyLimit[] DataArray;
	}
}
