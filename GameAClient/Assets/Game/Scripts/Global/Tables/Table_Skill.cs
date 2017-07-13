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
		[ColumnMapping("Bullet")]
		public int Bullet;
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
		[ColumnMapping("AttackDistance")]
		public int AttackDistance;
		/// <summary>
        /// 飞行速度
        /// </summary>
		[ColumnMapping("BulletSpeed")]
		public int BulletSpeed;
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
		[ColumnMapping("Trap")]
		public int Trap;
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
        /// 
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
