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
        /// 生命值
        /// </summary>
		[ColumnMapping("Hp")]
		public int Hp;
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
		/// <summary>
        /// 模型
        /// </summary>
		[ColumnMapping("Model")]
		public string Model;
	}

    public class TableEquipmentAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Equipment[] DataArray;
	}
}
