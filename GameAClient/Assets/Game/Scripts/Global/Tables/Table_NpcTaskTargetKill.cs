using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_NpcTaskTargetKill
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
	}

    public class TableNpcTaskTargetKillAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_NpcTaskTargetKill[] DataArray;
	}
}
