using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_State
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
        /// 状态优先级
        /// </summary>
		[ColumnMapping("StatePriority")]
		public int StatePriority;
		/// <summary>
        /// 类别
        /// </summary>
		[ColumnMapping("StateType")]
		public int StateType;
		/// <summary>
        /// 类别优先级
        /// </summary>
		[ColumnMapping("StateTypePriority")]
		public int StateTypePriority;
		/// <summary>
        /// 是否同类替换
        /// </summary>
		[ColumnMapping("IsReplace")]
		public int IsReplace;
		/// <summary>
        /// 叠加
        /// </summary>
		[ColumnMapping("OverlapType")]
		public int OverlapType;
		/// <summary>
        /// 持续时间
        /// </summary>
		[ColumnMapping("Duration")]
		public int Duration;
		/// <summary>
        /// 触发间隔
        /// </summary>
		[ColumnMapping("IntervalTime")]
		public int IntervalTime;
		/// <summary>
        /// 结束触发状态
        /// </summary>
		[ColumnMapping("EndState")]
		public int EndState;
		/// <summary>
        /// 效果Id
        /// </summary>
		[ColumnMapping("EffectIds")]
		public int[] EffectIds;
		/// <summary>
        /// 效果类型
        /// </summary>
		[ColumnMapping("EffectTypes")]
		public int[] EffectTypes;
		/// <summary>
        /// 效果值
        /// </summary>
		[ColumnMapping("EffectValues")]
		public int[] EffectValues;
		/// <summary>
        /// 效果值上限
        /// </summary>
		[ColumnMapping("EffectMaxValues")]
		public int[] EffectMaxValues;
		/// <summary>
        /// 死亡去除
        /// </summary>
		[ColumnMapping("DeadRemove")]
		public int DeadRemove;
		/// <summary>
        /// 图标
        /// </summary>
		[ColumnMapping("Icon")]
		public string Icon;
		/// <summary>
        /// 特效
        /// </summary>
		[ColumnMapping("Particle")]
		public string Particle;
		/// <summary>
        /// 描述
        /// </summary>
		[ColumnMapping("Summary")]
		public string Summary;
	}

    public class TableStateAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_State[] DataArray;
	}
}
