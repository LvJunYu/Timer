﻿using SoyEngine;

namespace GameA.Game
{
    public struct EditRecordData
    {
        public EAction ActionType;
        public UnitDesc UnitDesc;
        public UnitExtra UnitExtra;
        public UnitDesc UnitDescOld;
        public UnitExtra UnitExtraOld;
        public IntVec3 SwitchGuid;
        
        public enum EAction
        {
            None,
            /// <summary>
            /// 添加地块
            /// </summary>
            AddUnit,
            /// <summary>
            /// 删除地块
            /// </summary>
            RemoveUnit,
            /// <summary>
            /// 更新Extra
            /// </summary>
            UpdateExtra,
            /// <summary>
            /// 添加开关连接
            /// </summary>
            AddSwitchConnection,
            /// <summary>
            /// 删除开关连接
            /// </summary>
            RemoveSwitchConnection,
            /// <summary>
            /// 修改场景大小
            /// </summary>
            ChangeMapRect,
        }
    }
}