using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_TreasureMap
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 关卡id
        /// </summary>
		[ColumnMapping("Level")]
		public int Level;
		/// <summary>
        /// x坐标
        /// </summary>
		[ColumnMapping("X")]
		public int X;
		/// <summary>
        /// y坐标
        /// </summary>
		[ColumnMapping("Y")]
		public int Y;
	}

    public class TableTreasureMapAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_TreasureMap[] DataArray;
	}
}
