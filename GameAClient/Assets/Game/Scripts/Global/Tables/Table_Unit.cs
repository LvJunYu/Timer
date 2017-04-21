using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Unit
	{
		// 序号
		[ColumnMapping("Id")]
		public int Id;
		// 名字
		[ColumnMapping("Name")]
		public string Name;
		// 使用显示
		[ColumnMapping("Use")]
		public int Use;
		// 图标
		[ColumnMapping("Icon")]
		public string Icon;
		// 模型
		[ColumnMapping("Model")]
		public string Model;
		// 类别
		[ColumnMapping("UnitType")]
		public int UnitType;
		// 碰撞层
		[ColumnMapping("Layer")]
		public int Layer;
		// 生成类别
		[ColumnMapping("GeneratedType")]
		public int GeneratedType;
		// 碰撞类别
		[ColumnMapping("ColliderType")]
		public int ColliderType;
		// 数量
		[ColumnMapping("Count")]
		public int Count;
		// 数据宽
		[ColumnMapping("Width")]
		public int Width;
		// 数据高
		[ColumnMapping("Height")]
		public int Height;
		// 碰撞宽
		[ColumnMapping("CWidth")]
		public int CWidth;
		// 碰撞高
		[ColumnMapping("CHeight")]
		public int CHeight;
		// 碰撞锚点
		[ColumnMapping("Anchore")]
		public int Anchore;
		// 模型锚点
		[ColumnMapping("ModelAnchore")]
		public int ModelAnchore;
		// 父亲类别
		[ColumnMapping("ParentType")]
		public int ParentType;
		// 孩子类别
		[ColumnMapping("ChildType")]
		public int ChildType;
		// 初始移动方向
		[ColumnMapping("OriginMoveDirection")]
		public int OriginMoveDirection;
		// 初始蓝石移动方向
		[ColumnMapping("OriginMagicDirection")]
		public int OriginMagicDirection;
		// 移动方向集合
		[ColumnMapping("MoveDirectionMask")]
		public int MoveDirectionMask;
		// 旋转方向集合
		[ColumnMapping("RotationMask")]
		public int RotationMask;
		// 行走音效
		[ColumnMapping("WalkAudioNames")]
		public string[] WalkAudioNames;
		// 行走特效
		[ColumnMapping("WalkEffectName")]
		public string WalkEffectName;
		// 破坏音效
		[ColumnMapping("DestroyAudioName")]
		public string DestroyAudioName;
		// 破坏特效
		[ColumnMapping("DestroyEffectName")]
		public string DestroyEffectName;
	}

    public class TableUnitAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Unit[] DataArray;
	}
}
