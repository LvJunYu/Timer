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
        /// 攻速
        /// </summary>
		[ColumnMapping("RPS")]
		public int RPS;
		/// <summary>
        /// 持续
        /// </summary>
		[ColumnMapping("Duration")]
		public float Duration;
		/// <summary>
        /// 冷却
        /// </summary>
		[ColumnMapping("Cooldown")]
		public float Cooldown;
		/// <summary>
        /// 治疗
        /// </summary>
		[ColumnMapping("Cure")]
		public float Cure;
		/// <summary>
        /// 伤害
        /// </summary>
		[ColumnMapping("Damage")]
		public float Damage;
		/// <summary>
        /// 范围
        /// </summary>
		[ColumnMapping("Range")]
		public float Range;
		/// <summary>
        /// 距离
        /// </summary>
		[ColumnMapping("AttackDistance")]
		public float AttackDistance;
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
