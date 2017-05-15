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
    [Unit(Id = 5104, Type = typeof(SwitchPress))]
    public class SwitchPress : BlockBase
    {
        protected bool _trigger;
        protected bool _triggerReverse;
        protected List<UnitBase> _units;

        internal override void OnPlay()
        {
            base.OnPlay();
            _units = DataScene2D.Instance.GetControlledUnits(_guid);
            _trigger = false;
            IntVec3 guid = _guid;
            guid.z = GM2DTools.GetRuntimeCreatedUnitDepth();
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(UnitDefine.SwitchTriggerId);
            IntVec2 dataSize = tableUnit.GetDataSize(0, Vector2.one);
            var triggerDir = EDirectionType.Up;
            switch ((EDirectionType) _unitDesc.Rotation)
            {
                case EDirectionType.Up:
                    triggerDir = _triggerReverse ? EDirectionType.Down : EDirectionType.Up;
                    guid.y = _triggerReverse ? guid.y - dataSize.y : _colliderGrid.YMax + 1;
                    break;
                case EDirectionType.Down:
                    triggerDir = _triggerReverse ? EDirectionType.Up : EDirectionType.Down;
                    guid.y = _triggerReverse ? _colliderGrid.YMax + 1 : guid.y - dataSize.y;
                    break;
                case EDirectionType.Left:
                    triggerDir = _triggerReverse ? EDirectionType.Right : EDirectionType.Left;
                    guid.x = _triggerReverse ? _colliderGrid.XMax + 1 : guid.x - dataSize.x;
                    break;
                case EDirectionType.Right:
                    triggerDir = _triggerReverse ? EDirectionType.Left : EDirectionType.Right;
                    guid.x = _triggerReverse ? guid.x - dataSize.x : _colliderGrid.XMax + 1;
                    break;
            }
            var switchTrigger = PlayMode.Instance.CreateUnit(new UnitDesc(UnitDefine.SwitchTriggerId, guid,(byte) triggerDir, Vector2.one)) as SwitchTrigger;
            if (switchTrigger == null)
            {
                LogHelper.Error("CreateUnit switchTrigger Faield,{0}", ToString());
                return;
            }
            switchTrigger.OnPlay();
            switchTrigger.SwitchPress = this;
        }

        protected override void Clear()
        {
            base.Clear();
            _trigger = false;
        }

        public virtual void OnTriggerStart(UnitBase other)
        {
            //LogHelper.Debug("OnTriggerStart {0}", ToString() + "~" + _trans.GetInstanceID());
            _trigger = true;

            if (_units != null)
            {
                for (int i = 0; i < _units.Count; i++)
                {
                    var unit = _units[i];
                    if (unit != null && unit.IsAlive)
                    {
                        unit.OnSwitchPressStart(this);
                    }
                }
            }
        }

        public virtual void OnTriggerEnd()
        {
            //LogHelper.Debug("OnTriggerEnd {0}", ToString());
            _trigger = false;

            if (_units != null)
            {
                for (int i = 0; i < _units.Count; i++)
                {
                    var unit = _units[i];
                    if (unit != null && unit.IsAlive)
                    {
                        unit.OnSwitchPressEnd(this);
                    }
                }
            }
        }
    }
}
