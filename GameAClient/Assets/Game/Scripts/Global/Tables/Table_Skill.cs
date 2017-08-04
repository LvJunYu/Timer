using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Skill
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
        /// 伤害
        /// </summary>
		[ColumnMapping("Damage")]
		public int Damage;
		/// <summary>
        /// 治疗
        /// </summary>
		[ColumnMapping("Cure")]
		public int Cure;
		/// <summary>
        /// 魔法消耗
        /// </summary>
		[ColumnMapping("MpCost")]
		public int MpCost;
		/// <summary>
        /// 怒气消耗
        /// </summary>
		[ColumnMapping("RpCost")]
		public int RpCost;
		/// <summary>
        /// CD时间
        /// </summary>
		[ColumnMapping("CDTime")]
		public int CDTime;
		/// <summary>
        /// 吟唱时间
        /// </summary>
		[ColumnMapping("SingTime")]
		public int SingTime;
		/// <summary>
        /// 目标类型
        /// </summary>
		[ColumnMapping("TargetType")]
		public int TargetType;
		/// <summary>
        /// 行为类型
        /// </summary>
		[ColumnMapping("BehaviorType")]
		public int BehaviorType;
		/// <summary>
        /// 行为值
        /// </summary>
		[ColumnMapping("BehaviorValues")]
		public int[] BehaviorValues;
		/// <summary>
        /// 攻击距离
        /// </summary>
		[ColumnMapping("CastRange")]
		public int CastRange;
		/// <summary>
        /// 投射物Id
        /// </summary>
		[ColumnMapping("ProjectileId")]
		public int ProjectileId;
		/// <summary>
        /// 投射物速度
        /// </summary>
		[ColumnMapping("ProjectileSpeed")]
		public int ProjectileSpeed;
		/// <summary>
        /// 击退力
        /// </summary>
		[ColumnMapping("KnockbackForces")]
		public int[] KnockbackForces;
		/// <summary>
        /// 作用方式
        /// </summary>
		[ColumnMapping("EffectMode")]
		public int EffectMode;
		/// <summary>
        /// 作用值
        /// </summary>
		[ColumnMapping("EffectValues")]
		public int[] EffectValues;
		/// <summary>
        /// 击中目标触发状态
        /// </summary>
		[ColumnMapping("TriggerStates")]
		public int[] TriggerStates;
		/// <summary>
        /// 陷阱Id
        /// </summary>
		[ColumnMapping("TrapId")]
		public int TrapId;
		/// <summary>
        /// 说明
        /// </summary>
		[ColumnMapping("Summary")]
		public string Summary;
	}

    public class TableSkillAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Skill[] DataArray;
	}
}
