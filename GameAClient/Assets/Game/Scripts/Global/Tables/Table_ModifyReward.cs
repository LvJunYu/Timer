using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_ModifyReward
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 基础奖励Id（Reward表）
        /// </summary>
		[ColumnMapping("BaseReward")]
		public int BaseReward;
		/// <summary>
        /// 难度奖励系数
        /// </summary>
		[ColumnMapping("DifficultyRewardFactor")]
		public int DifficultyRewardFactor;
	}

    public class TableModifyRewardAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_ModifyReward[] DataArray;
	}
}
