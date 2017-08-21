using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_CharacterUpgrade
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 属性
        /// </summary>
		[ColumnMapping("Property")]
		public int Property;
		/// <summary>
        /// 等级
        /// </summary>
		[ColumnMapping("Level")]
		public int Level;
		/// <summary>
        /// 数值
        /// </summary>
		[ColumnMapping("Value")]
		public float Value;
		/// <summary>
        /// 升级金币
        /// </summary>
		[ColumnMapping("TrainingPrice")]
		public int TrainingPrice;
		/// <summary>
        /// 升级成长点
        /// </summary>
		[ColumnMapping("DevelopPoint")]
		public int DevelopPoint;
		/// <summary>
        /// 升级时间秒
        /// </summary>
		[ColumnMapping("TrainingTime")]
		public int TrainingTime;
		/// <summary>
        /// 每钻石能抵消的秒数
        /// </summary>
		[ColumnMapping("SecondsPerDiamond")]
		public int SecondsPerDiamond;
		/// <summary>
        /// 说明
        /// </summary>
		[ColumnMapping("Desc")]
		public string Desc;
	}

    public class TableCharacterUpgradeAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_CharacterUpgrade[] DataArray;
	}
}
