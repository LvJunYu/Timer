using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_StarRequire
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 描述文字
        /// </summary>
		[ColumnMapping("Desc")]
		public string Desc;
		/// <summary>
        /// 参数数目
        /// </summary>
		[ColumnMapping("ParamCnt")]
		public int ParamCnt;
	}

    public class TableStarRequireAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_StarRequire[] DataArray;
	}
}
