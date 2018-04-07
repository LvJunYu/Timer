using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Background
	{
		/// <summary>
        /// 序号
        /// </summary>
		[ColumnMapping("Id")]
		public int Id;
		/// <summary>
        /// 
        /// </summary>
		[ColumnMapping("Group")]
		public int Group;
		/// <summary>
        /// Model
        /// </summary>
		[ColumnMapping("Model")]
		public string Model;
		/// <summary>
        /// 
        /// </summary>
		[ColumnMapping("MinScaleX")]
		public float MinScaleX;
		/// <summary>
        /// 
        /// </summary>
		[ColumnMapping("MinScaleY")]
		public float MinScaleY;
		/// <summary>
        /// MaxScale
        /// </summary>
		[ColumnMapping("MaxScaleX")]
		public float MaxScaleX;
		/// <summary>
        /// MaxScale
        /// </summary>
		[ColumnMapping("MaxScaleY")]
		public float MaxScaleY;
		/// <summary>
        /// Depth
        /// </summary>
		[ColumnMapping("Depth")]
		public int Depth;
		/// <summary>
        /// MoveSpeedX
        /// </summary>
		[ColumnMapping("MoveSpeedX")]
		public float MoveSpeedX;
		/// <summary>
        /// Alpha
        /// </summary>
		[ColumnMapping("Alpha")]
		public float Alpha;
		/// <summary>
        /// 最大数量
        /// </summary>
		[ColumnMapping("MaxCount")]
		public int MaxCount;
		/// <summary>
        /// 跟随镜头速率
        /// </summary>
		[ColumnMapping("FollowMoveRatio")]
		public float FollowMoveRatio;
	}

    public class TableBackgroundAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Background[] DataArray;
	}
}
