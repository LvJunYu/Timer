using GameA.Game;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedUnitBase : SharedVariable<UnitBase>
    {
        public static implicit operator SharedUnitBase(UnitBase value) { return new SharedUnitBase { mValue = value }; }
    }
}