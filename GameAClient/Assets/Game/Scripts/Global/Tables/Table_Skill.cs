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
        /// 子弹
        /// </summary>
		[ColumnMapping("ProjectileCount")]
		public int ProjectileCount;
		/// <summary>
        /// CD时间
        /// </summary>
		[ColumnMapping("CDTime")]
		public int CDTime;
		/// <summary>
        /// 冷却时间
        /// </summary>
		[ColumnMapping("ChargeTime")]
		public int ChargeTime;
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
        /// 击退距离
        /// </summary>
		[ColumnMapping("Knockback")]
		public int Knockback;
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
        /// 效果类型
        /// </summary>
		[ColumnMapping("BehaviorType")]
		public int BehaviorType;
		/// <summary>
        /// 效果值
        /// </summary>
		[ColumnMapping("BehaviorValue")]
		public int BehaviorValue;
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
