using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_TreasureMap
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// 关卡id
		[ColumnMapping("Level")]
		public int Level;
		// x坐标
		[ColumnMapping("X")]
		public int X;
		// y坐标
		[ColumnMapping("Y")]
		public int Y;
	}

    public class TableTreasureMapAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_TreasureMap[] DataArray;
	}
}
