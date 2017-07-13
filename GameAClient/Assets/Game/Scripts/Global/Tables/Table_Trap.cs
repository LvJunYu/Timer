using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Trap
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
        /// 持续时间
        /// </summary>
		[ColumnMapping("Duration")]
		public int Duration;
		/// <summary>
        /// 触发范围
        /// </summary>
		[ColumnMapping("TriggerRange")]
		public int TriggerRange;
		/// <summary>
        /// 作用范围
        /// </summary>
		[ColumnMapping("EffectRange")]
		public int EffectRange;
		/// <summary>
        /// 作用类型
        /// </summary>
		[ColumnMapping("EffectType")]
		public int EffectType;
		/// <summary>
        /// 触发状态
        /// </summary>
		[ColumnMapping("TriggerStates")]
		public int TriggerStates;
		/// <summary>
        /// 特效名
        /// </summary>
		[ColumnMapping("Particle")]
		public string Particle;
		/// <summary>
        /// 触发特效名
        /// </summary>
		[ColumnMapping("TriggerParticle")]
		public string TriggerParticle;
		/// <summary>
        /// 音效名
        /// </summary>
		[ColumnMapping("String")]
		public string String;
	}

    public class TableTrapAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Trap[] DataArray;
	}
}
