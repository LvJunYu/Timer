using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Morph
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// 名称
		[ColumnMapping("Status")]
		public string Status;
		// 名称
		[ColumnMapping("Name")]
		public string Name;
		// ZRot
		[ColumnMapping("ZRot")]
		public float ZRot;
		// Count
		[ColumnMapping("Count")]
		public int Count;
	}

    public class TableMorphAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Morph[] DataArray;
	}
}
