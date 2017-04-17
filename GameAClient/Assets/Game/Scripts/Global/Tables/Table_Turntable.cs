using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Turntable
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// 名字
		[ColumnMapping("Name")]
		public string Name;
		// 奖励1
		[ColumnMapping("Reward1")]
		public int Reward1;
		// 奖励2
		[ColumnMapping("Reward2")]
		public int Reward2;
		// 奖励3
		[ColumnMapping("Reward3")]
		public int Reward3;
		// 奖励4
		[ColumnMapping("Reward4")]
		public int Reward4;
		// 奖励5
		[ColumnMapping("Reward5")]
		public int Reward5;
		// 奖励6
		[ColumnMapping("Reward6")]
		public int Reward6;
		// 奖励7
		[ColumnMapping("Reward7")]
		public int Reward7;
		// 奖励8
		[ColumnMapping("Reward8")]
		public int Reward8;
		// 概率1
		[ColumnMapping("Proportion1")]
		public int Proportion1;
		// 概率2
		[ColumnMapping("Proportion2")]
		public int Proportion2;
		// 概率3
		[ColumnMapping("Proportion3")]
		public int Proportion3;
		// 概率4
		[ColumnMapping("Proportion4")]
		public int Proportion4;
		// 概率5
		[ColumnMapping("Proportion5")]
		public int Proportion5;
		// 概率6
		[ColumnMapping("Proportion6")]
		public int Proportion6;
		// 概率7
		[ColumnMapping("Proportion7")]
		public int Proportion7;
		// 概率8
		[ColumnMapping("Proportion8")]
		public int Proportion8;
	}

    public class TableTurntableAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Turntable[] DataArray;
	}
}
