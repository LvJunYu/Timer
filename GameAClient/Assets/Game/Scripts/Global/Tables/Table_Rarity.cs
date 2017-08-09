using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Rarity
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 颜色
        /// </summary>
		[ColumnMapping("Color")]
		public  string Color;
	}

    public class TableRarityAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Rarity[] DataArray;
	}
}
