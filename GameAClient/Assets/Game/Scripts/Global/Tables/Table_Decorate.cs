using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Decorate
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// 位置
		[ColumnMapping("Position")]
		public int Position;
		// 解锁章节
		[ColumnMapping("Chapter")]
		public int Chapter;
		// 资源
		[ColumnMapping("Resource")]
		public string Resource;
	}

    public class TableDecorateAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Decorate[] DataArray;
	}
}
