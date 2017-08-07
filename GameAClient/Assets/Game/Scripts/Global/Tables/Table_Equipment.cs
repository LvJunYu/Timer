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
        /// 图标
        /// </summary>
		[ColumnMapping("Icon")]
		public string Icon;
		/// <summary>
        /// 模型
        /// </summary>
		[ColumnMapping("Model")]
		public string Model;
		/// <summary>
        /// 生命值
        /// </summary>
		[ColumnMapping("HpAdd")]
		public int HpAdd;
		/// <summary>
        /// 技能Id
        /// </summary>
		[ColumnMapping("SkillId")]
		public int SkillId;
	}

    public class TableEquipmentAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Equipment[] DataArray;
	}
}
