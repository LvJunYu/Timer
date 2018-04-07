using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_NpcTaskAward
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
	}

    public class TableNpcTaskAwardAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_NpcTaskAward[] DataArray;
	}
}
