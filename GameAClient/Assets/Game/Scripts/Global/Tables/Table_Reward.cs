using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Reward
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 描述
        /// </summary>
		[ColumnMapping("Description")]
		public string Description;
		/// <summary>
        /// 奖励1类型ERewardType
        /// </summary>
		[ColumnMapping("Type1")]
		public int Type1;
		/// <summary>
        /// 奖励1子类型
        /// </summary>
		[ColumnMapping("SubType1")]
		public int SubType1;
		/// <summary>
        /// 奖励1Id
        /// </summary>
		[ColumnMapping("Id1")]
		public int Id1;
		/// <summary>
        /// 奖励1数值
        /// </summary>
		[ColumnMapping("Value1")]
		public int Value1;
		/// <summary>
        /// 奖励2类型ERewardType
        /// </summary>
		[ColumnMapping("Type2")]
		public int Type2;
		/// <summary>
        /// 奖励2子类型
        /// </summary>
		[ColumnMapping("SubType2")]
		public int SubType2;
		/// <summary>
        /// 奖励2Id
        /// </summary>
		[ColumnMapping("Id2")]
		public int Id2;
		/// <summary>
        /// 奖励2数值
        /// </summary>
		[ColumnMapping("Value2")]
		public int Value2;
		/// <summary>
        /// 奖励3类型ERewardType
        /// </summary>
		[ColumnMapping("Type3")]
		public int Type3;
		/// <summary>
        /// 奖励3子类型
        /// </summary>
		[ColumnMapping("SubType3")]
		public int SubType3;
		/// <summary>
        /// 奖励3Id
        /// </summary>
		[ColumnMapping("Id3")]
		public int Id3;
		/// <summary>
        /// 奖励3数值
        /// </summary>
		[ColumnMapping("Value3")]
		public int Value3;
		/// <summary>
        /// 奖励4类型ERewardType
        /// </summary>
		[ColumnMapping("Type4")]
		public int Type4;
		/// <summary>
        /// 奖励4子类型
        /// </summary>
		[ColumnMapping("SubType4")]
		public int SubType4;
		/// <summary>
        /// 奖励4Id
        /// </summary>
		[ColumnMapping("Id4")]
		public int Id4;
		/// <summary>
        /// 奖励4数值
        /// </summary>
		[ColumnMapping("Value4")]
		public int Value4;
		/// <summary>
        /// 奖励5类型ERewardType
        /// </summary>
		[ColumnMapping("Type5")]
		public int Type5;
		/// <summary>
        /// 奖励5子类型
        /// </summary>
		[ColumnMapping("SubType5")]
		public int SubType5;
		/// <summary>
        /// 奖励5Id
        /// </summary>
		[ColumnMapping("Id5")]
		public int Id5;
		/// <summary>
        /// 奖励5数值
        /// </summary>
		[ColumnMapping("Value5")]
		public int Value5;
		/// <summary>
        /// 奖励6类型ERewardType
        /// </summary>
		[ColumnMapping("Type6")]
		public int Type6;
		/// <summary>
        /// 奖励6子类型
        /// </summary>
		[ColumnMapping("SubType6")]
		public int SubType6;
		/// <summary>
        /// 奖励6Id
        /// </summary>
		[ColumnMapping("Id6")]
		public int Id6;
		/// <summary>
        /// 奖励6数值
        /// </summary>
		[ColumnMapping("Value6")]
		public int Value6;
		/// <summary>
        /// 奖励7类型ERewardType
        /// </summary>
		[ColumnMapping("Type7")]
		public int Type7;
		/// <summary>
        /// 奖励7子类型
        /// </summary>
		[ColumnMapping("SubType7")]
		public int SubType7;
		/// <summary>
        /// 奖励7Id
        /// </summary>
		[ColumnMapping("Id7")]
		public int Id7;
		/// <summary>
        /// 奖励7数值
        /// </summary>
		[ColumnMapping("Value7")]
		public int Value7;
		/// <summary>
        /// 奖励8类型ERewardType
        /// </summary>
		[ColumnMapping("Type8")]
		public int Type8;
		/// <summary>
        /// 奖励8子类型
        /// </summary>
		[ColumnMapping("SubType8")]
		public int SubType8;
		/// <summary>
        /// 奖励8Id
        /// </summary>
		[ColumnMapping("Id8")]
		public int Id8;
		/// <summary>
        /// 奖励8数值
        /// </summary>
		[ColumnMapping("Value8")]
		public int Value8;
	}

    public class TableRewardAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Reward[] DataArray;
	}
}
