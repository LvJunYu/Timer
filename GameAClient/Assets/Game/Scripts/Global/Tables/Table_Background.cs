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
        /// 像素宽
        /// </summary>
		[ColumnMapping("Width")]
		public int Width;
		/// <summary>
        /// 像素高
        /// </summary>
		[ColumnMapping("Height")]
		public int Height;
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
        /// 
        /// </summary>
		[ColumnMapping("Alpha")]
		public float Alpha;
	}

    public class TableBackgroundAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Background[] DataArray;
	}
}
