using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Equipment
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
        /// 模型
        /// </summary>
		[ColumnMapping("Model")]
		public string Model;
		/// <summary>
        /// 生命值
        /// </summary>
		[ColumnMapping("Hp")]
		public int Hp;
		/// <summary>
        /// 魔法值
        /// </summary>
		[ColumnMapping("Mp")]
		public int Mp;
		/// <summary>
        /// 魔法回复值
        /// </summary>
		[ColumnMapping("MpRecover")]
		public int MpRecover;
		/// <summary>
        /// 怒气值
        /// </summary>
		[ColumnMapping("Rp")]
		public int Rp;
		/// <summary>
        /// 怒气回复值
        /// </summary>
		[ColumnMapping("RpRecover")]
		public int RpRecover;
		/// <summary>
        /// 技能Id
        /// </summary>
		[ColumnMapping("SkillIds")]
		public int[] SkillIds;
		/// <summary>
        /// 伤害
        /// </summary>
		[ColumnMapping("Damages")]
		public int[] Damages;
		/// <summary>
        /// 治疗
        /// </summary>
		[ColumnMapping("Cures")]
		public int[] Cures;
		/// <summary>
        /// 护盾
        /// </summary>
		[ColumnMapping("Shields")]
		public int[] Shields;
		/// <summary>
        /// 图标
        /// </summary>
		[ColumnMapping("Icon")]
		public string Icon;
	}

    public class TableEquipmentAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Equipment[] DataArray;
	}
}
