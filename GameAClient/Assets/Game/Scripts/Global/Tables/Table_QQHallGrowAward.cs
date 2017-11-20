using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_QQHallGrowAward
	{
		/// <summary>
        /// 等级
        /// </summary>
		[ColumnMapping("Id")]
		public  int Id;
		/// <summary>
        ///  奖励钻石数目
        /// </summary>
		[ColumnMapping("DiamodNum")]
		public int DiamodNum;
		/// <summary>
        ///  金币数目
        /// </summary>
		[ColumnMapping("CoinNum")]
		public int CoinNum;
	}

    public class TableQQHallGrowAwardAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_QQHallGrowAward[] DataArray;
	}
}
