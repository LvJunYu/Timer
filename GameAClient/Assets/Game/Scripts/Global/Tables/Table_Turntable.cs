using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Turntable
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 名字
        /// </summary>
		[ColumnMapping("Name")]
		public string Name;
		/// <summary>
        /// 奖励1
        /// </summary>
		[ColumnMapping("Reward1")]
		public int Reward1;
		/// <summary>
        /// 奖励2
        /// </summary>
		[ColumnMapping("Reward2")]
		public int Reward2;
		/// <summary>
        /// 奖励3
        /// </summary>
		[ColumnMapping("Reward3")]
		public int Reward3;
		/// <summary>
        /// 奖励4
        /// </summary>
		[ColumnMapping("Reward4")]
		public int Reward4;
		/// <summary>
        /// 奖励5
        /// </summary>
		[ColumnMapping("Reward5")]
		public int Reward5;
		/// <summary>
        /// 奖励6
        /// </summary>
		[ColumnMapping("Reward6")]
		public int Reward6;
		/// <summary>
        /// 奖励7
        /// </summary>
		[ColumnMapping("Reward7")]
		public int Reward7;
		/// <summary>
        /// 奖励8
        /// </summary>
		[ColumnMapping("Reward8")]
		public int Reward8;
		/// <summary>
        /// 概率1
        /// </summary>
		[ColumnMapping("Proportion1")]
		public int Proportion1;
		/// <summary>
        /// 概率2
        /// </summary>
		[ColumnMapping("Proportion2")]
		public int Proportion2;
		/// <summary>
        /// 概率3
        /// </summary>
		[ColumnMapping("Proportion3")]
		public int Proportion3;
		/// <summary>
        /// 概率4
        /// </summary>
		[ColumnMapping("Proportion4")]
		public int Proportion4;
		/// <summary>
        /// 概率5
        /// </summary>
		[ColumnMapping("Proportion5")]
		public int Proportion5;
		/// <summary>
        /// 概率6
        /// </summary>
		[ColumnMapping("Proportion6")]
		public int Proportion6;
		/// <summary>
        /// 概率7
        /// </summary>
		[ColumnMapping("Proportion7")]
		public int Proportion7;
		/// <summary>
        /// 概率8
        /// </summary>
		[ColumnMapping("Proportion8")]
		public int Proportion8;
	}

    public class TableTurntableAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Turntable[] DataArray;
	}
}
