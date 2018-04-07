using GameA.Game;
using SoyEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class GetUnitBaseProperty<T> : JoyAction where T : SharedVariable
    {
        public SharedUnitBase UnitBase;
        public T StoreValue;

        protected UnitBase _unit;

        public override void OnStart()
        {
            base.OnStart();
            _unit = UnitBase.Value;
            if (_unit == null)
            {
                LogHelper.Error("_unit == null");
            }
        }
    }
}