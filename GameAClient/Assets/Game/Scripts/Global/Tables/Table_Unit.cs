using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Unit
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
        /// 使用显示
        /// </summary>
		[ColumnMapping("Use")]
		public int Use;
		/// <summary>
        /// 图标
        /// </summary>
		[ColumnMapping("Icon")]
		public string Icon;
		/// <summary>
        /// 模型
        /// </summary>
		[ColumnMapping("Model")]
		public string Model;
		/// <summary>
        /// 模型额外
        /// </summary>
		[ColumnMapping("ModelExtras")]
		public string[] ModelExtras;
		/// <summary>
        /// 生命值
        /// </summary>
		[ColumnMapping("Hp")]
		public int Hp;
		/// <summary>
        /// 类别
        /// </summary>
		[ColumnMapping("UnitType")]
		public int UnitType;
		/// <summary>
        /// 类别
        /// </summary>
		[ColumnMapping("UIType")]
		public int UIType;
		/// <summary>
        /// 碰撞层
        /// </summary>
		[ColumnMapping("Layer")]
		public int Layer;
		/// <summary>
        /// 生成类别
        /// </summary>
		[ColumnMapping("GeneratedType")]
		public int GeneratedType;
		/// <summary>
        /// 碰撞类别
        /// </summary>
		[ColumnMapping("ColliderType")]
		public int ColliderType;
		/// <summary>
        /// 是否地面
        /// </summary>
		[ColumnMapping("IsGround")]
		public int IsGround;
		/// <summary>
        /// 蓝石
        /// </summary>
		[ColumnMapping("IsMagicBlock")]
		public int IsMagicBlock;
		/// <summary>
        /// 桥梁
        /// </summary>
		[ColumnMapping("IsBridgeBlock")]
		public int IsBridgeBlock;
		/// <summary>
        /// 激光
        /// </summary>
		[ColumnMapping("IsLazerBlock")]
		public int IsLazerBlock;
		/// <summary>
        /// 风扇
        /// </summary>
		[ColumnMapping("IsFanBlock")]
		public int IsFanBlock;
		/// <summary>
        /// 子弹
        /// </summary>
		[ColumnMapping("IsBulletBlock")]
		public int IsBulletBlock;
		/// <summary>
        /// 数量
        /// </summary>
		[ColumnMapping("Count")]
		public int Count;
		/// <summary>
        /// 数据宽
        /// </summary>
		[ColumnMapping("Width")]
		public int Width;
		/// <summary>
        /// 数据高
        /// </summary>
		[ColumnMapping("Height")]
		public int Height;
		/// <summary>
        /// 碰撞宽
        /// </summary>
		[ColumnMapping("CWidth")]
		public int CWidth;
		/// <summary>
        /// 碰撞高
        /// </summary>
		[ColumnMapping("CHeight")]
		public int CHeight;
		/// <summary>
        /// 碰撞锚点
        /// </summary>
		[ColumnMapping("Anchore")]
		public int Anchore;
		/// <summary>
        /// 模型锚点
        /// </summary>
		[ColumnMapping("ModelAnchore")]
		public int ModelAnchore;
		/// <summary>
        /// 朝向
        /// </summary>
		[ColumnMapping("Direction")]
		public int Direction;
		/// <summary>
        /// 朝向集合
        /// </summary>
		[ColumnMapping("DirectionMask")]
		public int DirectionMask;
		/// <summary>
        /// 移动
        /// </summary>
		[ColumnMapping("MoveDirection")]
		public int MoveDirection;
		/// <summary>
        /// 激活
        /// </summary>
		[ColumnMapping("ActiveState")]
		public int ActiveState;
		/// <summary>
        /// 装载
        /// </summary>
		[ColumnMapping("ChildState")]
		public int[] ChildState;
		/// <summary>
        /// 旋转
        /// </summary>
		[ColumnMapping("RotateState")]
		public int RotateState;
		/// <summary>
        /// 时间状态
        /// </summary>
		[ColumnMapping("TimeState")]
		public float[] TimeState;
		/// <summary>
        /// 文字
        /// </summary>
		[ColumnMapping("TextState")]
		public int TextState;
		/// <summary>
        /// 成对类别
        /// </summary>
		[ColumnMapping("PairType")]
		public int PairType;
		/// <summary>
        /// 成对id
        /// </summary>
		[ColumnMapping("PairUnitIds")]
		public int[] PairUnitIds;
		/// <summary>
        /// 伴随特效
        /// </summary>
		[ColumnMapping("WithEffctName")]
		public string WithEffctName;
		/// <summary>
        /// 破坏音效
        /// </summary>
		[ColumnMapping("DestroyAudioName")]
		public string DestroyAudioName;
		/// <summary>
        /// 破坏特效
        /// </summary>
		[ColumnMapping("DestroyEffectName")]
		public string DestroyEffectName;
		/// <summary>
        /// 描述
        /// </summary>
		[ColumnMapping("Summary")]
		public string Summary;
	}

    public class TableUnitAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Unit[] DataArray;
	}
}
