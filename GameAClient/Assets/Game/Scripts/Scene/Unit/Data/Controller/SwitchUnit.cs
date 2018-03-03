using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class SwitchUnit : BlockBase
    {
        protected SwitchTrigger _switchTrigger;

        public virtual int SwitchTriggerId
        {
            get { return UnitDefine.SwitchTriggerPressId; }
        }

        protected virtual bool TriggerReverse
        {
            get { return false; }
        }

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

        internal override bool InstantiateView()
        {
            if (_switchTrigger == null && _guid != IntVec3.zero)
            {
                CreateSwitchTrigger();
            }

            return base.InstantiateView();
        }

        public override void UpdateView(float deltaTime)
        {
            if (_eActiveState != EActiveState.Active || !UseMagic())
            {
                return;
            }

            base.UpdateView(deltaTime);
            if (_isAlive)
            {
                if (_switchTrigger != null)
                {
                    _switchTrigger.UpdateSwitchPos(_deltaPos);
                }
            }
        }

        protected bool CreateSwitchTrigger()
        {
            if (_switchTrigger != null)
            {
                return false;
            }

            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(SwitchTriggerId);
            if (tableUnit == null)
            {
                LogHelper.Error("GetTableUnit Failed, {0}", SwitchTriggerId);
                return false;
            }

            IntVec3 guid = new IntVec3(_curPos.x, _curPos.y, GM2DTools.GetRuntimeCreatedUnitDepth());
            IntVec2 dataSize = tableUnit.GetDataSize(0, Vector2.one);
            var triggerDir = EDirectionType.Up;
            _colliderGrid = _tableUnit.GetColliderGrid(_curPos.x, _curPos.y, _unitDesc.Rotation, _unitDesc.Scale);
            switch ((EDirectionType) _unitDesc.Rotation)
            {
                case EDirectionType.Up:
                    triggerDir = TriggerReverse ? EDirectionType.Down : EDirectionType.Up;
                    guid.y = TriggerReverse ? guid.y - dataSize.y : _colliderGrid.YMax + 1;
                    break;
                case EDirectionType.Down:
                    triggerDir = TriggerReverse ? EDirectionType.Up : EDirectionType.Down;
                    guid.y = TriggerReverse ? _colliderGrid.YMax + 1 : guid.y - dataSize.y;
                    break;
                case EDirectionType.Left:
                    triggerDir = TriggerReverse ? EDirectionType.Right : EDirectionType.Left;
                    guid.x = TriggerReverse ? _colliderGrid.XMax + 1 : guid.x - dataSize.x;
                    break;
                case EDirectionType.Right:
                    triggerDir = TriggerReverse ? EDirectionType.Left : EDirectionType.Right;
                    guid.x = TriggerReverse ? guid.x - dataSize.x : _colliderGrid.XMax + 1;
                    break;
            }

            _switchTrigger =
                PlayMode.Instance.CreateUnit(new UnitDesc(SwitchTriggerId, guid, (byte) triggerDir, Vector2.one)) as
                    SwitchTrigger;
            if (_switchTrigger == null)
            {
                LogHelper.Error("CreateUnit switchTrigger Faield,{0}", ToString());
                return false;
            }

            _switchTrigger.OnPlay();
            _switchTrigger.SwitchUnit = this;
            OnTriggerCreated();
            return true;
        }

        protected virtual void OnTriggerCreated()
        {
            _switchTrigger.SetTrigger(_eActiveState);
        }

        public virtual void OnTriggerChanged(EActiveState value)
        {
        }
    }
}