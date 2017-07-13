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
        /// 小技能伤害
        /// </summary>
		[ColumnMapping("DamageLittle")]
		public int DamageLittle;
		/// <summary>
        /// 大技能伤害
        /// </summary>
		[ColumnMapping("DamageBig")]
		public int DamageBig;
		/// <summary>
        /// 小技能Id
        /// </summary>
		[ColumnMapping("SkillLittleId")]
		public int SkillLittleId;
		/// <summary>
        /// 大技能Id
        /// </summary>
		[ColumnMapping("SkillBigId")]
		public int SkillBigId;
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
