using System;
using SoyEngine;
namespace GameA.Game
{
    [Serializable]
	public partial class Table_Actor
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
        /// 
        /// </summary>
		[ColumnMapping("UnitId")]
		public int UnitId;
		/// <summary>
        /// 血量
        /// </summary>
		[ColumnMapping("Hp")]
		public int Hp;
		/// <summary>
        /// 回血速度
        /// </summary>
		[ColumnMapping("Heal")]
		public int Heal;
		/// <summary>
        /// 回血相隔时间
        /// </summary>
		[ColumnMapping("HealStartTime")]
		public int HealStartTime;
		/// <summary>
        /// 移动速度
        /// </summary>
		[ColumnMapping("Speed")]
		public int Speed;
	}

    public class TableActorAsset:BaseTableAsset
	{
		[UnityEngine.SerializeField]
		public Table_Actor[] DataArray;
	}
}
