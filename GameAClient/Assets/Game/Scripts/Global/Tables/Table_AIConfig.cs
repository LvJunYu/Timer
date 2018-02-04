using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_AIConfig
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// AI类型
        /// </summary>
		[ColumnMapping("AiType")]
		public int AiType;
		/// <summary>
        /// 继承的AI
        /// </summary>
		[ColumnMapping("InheritAiType")]
		public int InheritAiType;
		/// <summary>
        /// 前置状态
        /// </summary>
		[ColumnMapping("PreState")]
		public string PreState;
		/// <summary>
        /// 目标状态
        /// </summary>
		[ColumnMapping("TargetState")]
		public string TargetState;
		/// <summary>
        /// 转换条件
        /// </summary>
		[ColumnMapping("Constion_1")]
		public string Constion_1;
		/// <summary>
        /// 转换条件参数
        /// </summary>
		[ColumnMapping("ConstionValue_1")]
		public float[] ConstionValue_1;
		/// <summary>
        /// 转换条件
        /// </summary>
		[ColumnMapping("Constion_2")]
		public string Constion_2;
		/// <summary>
        /// 转换条件参数
        /// </summary>
		[ColumnMapping("ConstionValue_2")]
		public float[] ConstionValue_2;
	}

    public class TableAIConfigAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_AIConfig[] DataArray;
	}
}
