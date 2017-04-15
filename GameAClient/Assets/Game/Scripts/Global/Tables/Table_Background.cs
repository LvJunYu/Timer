using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Background
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// Model
		[ColumnMapping("Model")]
		public string Model;
		// 数据宽
		[ColumnMapping("Width")]
		public int Width;
		// Height
		[ColumnMapping("Height")]
		public int Height;
		// 数据宽
		[ColumnMapping("MinScale")]
		public float MinScale;
		// MaxScale
		[ColumnMapping("MaxScale")]
		public float MaxScale;
		// Depth
		[ColumnMapping("Depth")]
		public int Depth;
		// SortingLayer
		[ColumnMapping("SortingLayer")]
		public int SortingLayer;
		// MoveSpeedX
		[ColumnMapping("MoveSpeedX")]
		public float MoveSpeedX;
		// MoveSpeedY
		[ColumnMapping("MoveSpeedY")]
		public float MoveSpeedY;
		// RotateSpeed
		[ColumnMapping("RotateSpeed")]
		public int RotateSpeed;
		// RotateAngle
		[ColumnMapping("RotateAngle")]
		public float RotateAngle;
	}

    public class TableBackgroundAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Background[] DataArray;
	}
}
