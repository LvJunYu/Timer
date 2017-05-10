/********************************************************************
** Filename : SwitchPress
** Author : Dong
** Date : 2017/5/8 星期一 下午 2:54:54
** Summary : SwitchPress
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5111, Type = typeof(SwitchPress))]
    public class SwitchPress : BlockBase
    {
        protected bool _trigger;

        internal override void OnPlay()
        {
            base.OnPlay();
            _trigger = false;
            IntVec3 guid = _guid;
            guid.z = GM2DTools.GetRuntimeCreatedUnitDepth();
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(UnitDefine.SwitchTriggerId);
            IntVec2 dataSize = tableUnit.GetDataSize(0, Vector2.one);
            var triggerDir = EDirectionType.Up;
            switch ((EDirectionType)_unitDesc.Rotation)
            {
                case EDirectionType.Up:
                    triggerDir = EDirectionType.Down;
                    guid.y -= dataSize.y;
                    break;
                case EDirectionType.Down:
                    triggerDir = EDirectionType.Up;
                    guid.y = _colliderGrid.YMax + 1;
                    break;
                case EDirectionType.Left:
                    triggerDir = EDirectionType.Right;
                    guid.x = _colliderGrid.XMax + 1;
                    break;
                case EDirectionType.Right:
                    triggerDir = EDirectionType.Left;
                    guid.x -= dataSize.x;
                    break;
            }
            var switchTrigger = PlayMode.Instance.CreateUnit(new UnitDesc(UnitDefine.SwitchTriggerId, guid, (byte)triggerDir, Vector2.one)) as SwitchTrigger;
            if (switchTrigger == null)
            {
                LogHelper.Error("CreateUnit switchTrigger Faield,{0}", ToString());
                return;
            }
            switchTrigger.OnPlay();
            switchTrigger.SwitchPress = this;
        }

        internal override void Reset()
        {
            base.Reset();
            _trigger = false;
        }

        public virtual void OnTriggerStart(UnitBase other)
        {
            LogHelper.Debug("OnTriggerStart {0}", ToString());
            _trigger = true;
        }

        public virtual void OnTriggerEnd()
        {
            LogHelper.Debug("OnTriggerEnd {0}", ToString());
            _trigger = false;
        }
    }
}
