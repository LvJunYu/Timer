using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class SwitchUnit : BlockBase
    {
        protected bool _triggerReverse;
        protected SwitchTrigger _switchTrigger;

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            if (_switchTrigger != null)
            {
                PlayMode.Instance.DestroyUnit(_switchTrigger);
                _switchTrigger = null;
            }
        }
        
        protected override void Clear()
        {
            base.Clear();
            CreateSwitchTrigger();
        }

        public virtual void OnTriggerStart(UnitBase other)
        {
            //LogHelper.Debug("OnTriggerStart {0}", ToString() + "~" + _trans.GetInstanceID());
            OnCtrlBySwitch();
        }

        public virtual void OnTriggerEnd()
        {
            //LogHelper.Debug("OnTriggerEnd {0}", ToString());
            OnCtrlBySwitch();
        }

        protected bool CreateSwitchTrigger()
        {
            if (_switchTrigger != null)
            {
                return false;
            }
            IntVec3 guid = _guid;
            guid.z = GM2DTools.GetRuntimeCreatedUnitDepth();
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(UnitDefine.SwitchTriggerId);
            IntVec2 dataSize = tableUnit.GetDataSize(0, Vector2.one);
            var triggerDir = EDirectionType.Up;
            switch ((EDirectionType)_unitDesc.Rotation)
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
            _switchTrigger = PlayMode.Instance.CreateUnit(new UnitDesc(UnitDefine.SwitchTriggerId, guid, (byte)triggerDir, Vector2.one)) as SwitchTrigger;
            if (_switchTrigger == null)
            {
                LogHelper.Error("CreateUnit switchTrigger Faield,{0}", ToString());
                return false;
            }
            _switchTrigger.OnPlay();
            _switchTrigger.SwitchUnit = this;
            return true;
        }
    }
}