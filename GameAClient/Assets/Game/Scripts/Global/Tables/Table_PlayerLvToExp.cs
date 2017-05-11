using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_PlayerLvToExp
	{
		/// <summary>
        /// 序号(等级)
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 冒险家经验
        /// </summary>
		[ColumnMapping("AdvExp")]
		public int AdvExp;
		/// <summary>
        /// 匠人经验
        /// </summary>
		[ColumnMapping("MakerExp")]
		public int MakerExp;
	}

    public class TablePlayerLvToExpAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_PlayerLvToExp[] DataArray;
	}
}
