using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Achievement
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 名称
        /// </summary>
		[ColumnMapping("Name")]
		public string Name;
		/// <summary>
        /// 描述
        /// </summary>
		[ColumnMapping("Description")]
		public string Description;
		/// <summary>
        /// 等级
        /// </summary>
		[ColumnMapping("Level")]
		public int Level;
		/// <summary>
        /// 成就类型
        /// </summary>
		[ColumnMapping("Type")]
		public int Type;
		/// <summary>
        /// 达到成就需要的条件
        /// </summary>
		[ColumnMapping("Condition")]
		public int Condition;
		/// <summary>
        /// 达成成就的奖励
        /// </summary>
		[ColumnMapping("Reward")]
		public int Reward;
	}

    public class TableAchievementAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Achievement[] DataArray;
	}
}
