using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Reward
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// 描述
		[ColumnMapping("Description")]
		public string Description;
		// 奖励1类型
		[ColumnMapping("Type1")]
		public int Type1;
		// 奖励1子类型
		[ColumnMapping("SubType1")]
		public int SubType1;
		// 奖励1Id
		[ColumnMapping("Id1")]
		public int Id1;
		// 奖励1数值
		[ColumnMapping("Value1")]
		public int Value1;
		// 奖励2类型
		[ColumnMapping("Type2")]
		public int Type2;
		// 奖励2子类型
		[ColumnMapping("SubType2")]
		public int SubType2;
		// 奖励2Id
		[ColumnMapping("Id2")]
		public int Id2;
		// 奖励2数值
		[ColumnMapping("Value2")]
		public int Value2;
		// 奖励3类型
		[ColumnMapping("Type3")]
		public int Type3;
		// 奖励3子类型
		[ColumnMapping("SubType3")]
		public int SubType3;
		// 奖励3Id
		[ColumnMapping("Id3")]
		public int Id3;
		// 奖励3数值
		[ColumnMapping("Value3")]
		public int Value3;
		// 奖励4类型
		[ColumnMapping("Type4")]
		public int Type4;
		// 奖励4子类型
		[ColumnMapping("SubType4")]
		public int SubType4;
		// 奖励4Id
		[ColumnMapping("Id4")]
		public int Id4;
		// 奖励4数值
		[ColumnMapping("Value4")]
		public int Value4;
		// 奖励5类型
		[ColumnMapping("Type5")]
		public int Type5;
		// 奖励5子类型
		[ColumnMapping("SubType5")]
		public int SubType5;
		// 奖励5Id
		[ColumnMapping("Id5")]
		public int Id5;
		// 奖励5数值
		[ColumnMapping("Value5")]
		public int Value5;
		// 奖励6类型
		[ColumnMapping("Type6")]
		public int Type6;
		// 奖励6子类型
		[ColumnMapping("SubType6")]
		public int SubType6;
		// 奖励6Id
		[ColumnMapping("Id6")]
		public int Id6;
		// 奖励6数值
		[ColumnMapping("Value6")]
		public int Value6;
		// 奖励7类型
		[ColumnMapping("Type7")]
		public int Type7;
		// 奖励7子类型
		[ColumnMapping("SubType7")]
		public int SubType7;
		// 奖励7Id
		[ColumnMapping("Id7")]
		public int Id7;
		// 奖励7数值
		[ColumnMapping("Value7")]
		public int Value7;
		// 奖励8类型
		[ColumnMapping("Type8")]
		public int Type8;
		// 奖励8子类型
		[ColumnMapping("SubType8")]
		public int SubType8;
		// 奖励8Id
		[ColumnMapping("Id8")]
		public int Id8;
		// 奖励8数值
		[ColumnMapping("Value8")]
		public int Value8;
	}

    public class TableRewardAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Reward[] DataArray;
	}
}
