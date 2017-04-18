/********************************************************************
** Filename : SwitchPress
** Author : Dong
** Date : 2017/1/8 星期日 下午 10:49:33
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

        internal override void OnPlay()
        {
            base.OnPlay();
            _trigger = false;
            IntVec3 guid = _guid;
            guid.z = GM2DTools.GetRuntimeCreatedUnitDepth();
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(UnitDefine.SwitchTriggerId);
            IntVec2 dataSize = tableUnit.GetDataSize(0, Vector2.one);
            var triggerDir = ERotationType.Up;
            switch ((ERotationType)Rotation)
            {
                case ERotationType.Up:
                    triggerDir = ERotationType.Down;
                    guid.y -= dataSize.y;
                    break;
                case ERotationType.Down:
                    triggerDir = ERotationType.Up;
                    guid.y = ColliderGrid.YMax + 1;
                    break;
                case ERotationType.Left:
                    triggerDir = ERotationType.Right;
                    guid.x = ColliderGrid.XMax + 1;
                    break;
                case ERotationType.Right:
                    triggerDir = ERotationType.Left;
                    guid.x -= dataSize.x;
                    break;
            }
            //var switchTrigger = PlayMode.Instance.CreateUnit(new UnitDesc(UnitDefine.SwitchTriggerId, guid, (byte)triggerDir, Vector2.one)) as SwitchTrigger;
            //if (switchTrigger == null)
            //{
            //    LogHelper.Error("CreateUnit switchTrigger Faield,{0}", ToString());
            //    return;
            //}
            //switchTrigger.OnPlay();
            //switchTrigger.SwitchPress = this;
        }

        internal override void Reset()
        {
            base.Reset();
            _trigger = false;
        }

        public virtual void OnTriggerStart(UnitBase other)
        {
            //LogHelper.Debug("OnTriggerStart {0}",ToString());
            _trigger = true;
        }

        public virtual void OnTriggerEnd()
        {
            //LogHelper.Debug("OnTriggerEnd {0}", ToString());
            _trigger = false;
        }
    }
}
