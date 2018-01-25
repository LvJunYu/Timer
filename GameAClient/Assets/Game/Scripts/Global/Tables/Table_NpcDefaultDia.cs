using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_NpcDefaultDia
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 对话
        /// </summary>
		[ColumnMapping("Dia")]
		public string Dia;
	}

    public class TableNpcDefaultDiaAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_NpcDefaultDia[] DataArray;
	}
}
