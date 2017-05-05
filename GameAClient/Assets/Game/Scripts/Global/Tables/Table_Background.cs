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
        /// Model
        /// </summary>
		[ColumnMapping("Model")]
		public string Model;
		/// <summary>
        /// 数据宽
        /// </summary>
		[ColumnMapping("Width")]
		public int Width;
		/// <summary>
        /// Height
        /// </summary>
		[ColumnMapping("Height")]
		public int Height;
		/// <summary>
        /// 数据宽
        /// </summary>
		[ColumnMapping("MinScale")]
		public float MinScale;
		/// <summary>
        /// MaxScale
        /// </summary>
		[ColumnMapping("MaxScale")]
		public float MaxScale;
		/// <summary>
        /// Depth
        /// </summary>
		[ColumnMapping("Depth")]
		public int Depth;
		/// <summary>
        /// SortingLayer
        /// </summary>
		[ColumnMapping("SortingLayer")]
		public int SortingLayer;
		/// <summary>
        /// MoveSpeedX
        /// </summary>
		[ColumnMapping("MoveSpeedX")]
		public float MoveSpeedX;
		/// <summary>
        /// MoveSpeedY
        /// </summary>
		[ColumnMapping("MoveSpeedY")]
		public float MoveSpeedY;
		/// <summary>
        /// RotateSpeed
        /// </summary>
		[ColumnMapping("RotateSpeed")]
		public int RotateSpeed;
		/// <summary>
        /// RotateAngle
        /// </summary>
		[ColumnMapping("RotateAngle")]
		public float RotateAngle;
	}

    public class TableBackgroundAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Background[] DataArray;
	}
}
