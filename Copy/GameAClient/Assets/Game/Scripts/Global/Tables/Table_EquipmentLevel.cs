using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_EquipmentLevel
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 等级
        /// </summary>
		[ColumnMapping("Level")]
		public int Level;
		/// <summary>
        /// 武器碎片的数目
        /// </summary>
		[ColumnMapping("WeaponFragment")]
		public int WeaponFragment;
		/// <summary>
        /// 金币数目
        /// </summary>
		[ColumnMapping("GoldCoinNum")]
		public int GoldCoinNum;
		/// <summary>
        /// 生命值
        /// </summary>
		[ColumnMapping("HpAdd")]
		public int HpAdd;
		/// <summary>
        /// 攻击值
        /// </summary>
		[ColumnMapping("AttackAdd")]
		public int AttackAdd;
		/// <summary>
        ///  技能加成
        /// </summary>
		[ColumnMapping("SkillEffect")]
		public int SkillEffect;
	}

    public class TableEquipmentLevelAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_EquipmentLevel[] DataArray;
	}
}
