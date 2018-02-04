using System;

namespace GameA.Game.AI
{
    public class FSMFactory
    {
        private const string ConditionTypeFormat = "GameA.Game.{0}Condition";
        private const string StateTypeFormat = "GameA.Game.{0}State";

        public static FSMConditionBase GetFsmConditionObj(EFSMConditionType conditionType)
        {
            Type type = Type.GetType(string.Format(ConditionTypeFormat, conditionType));
            if (type == null)
            {
                return null;
            }
            return Activator.CreateInstance(type) as FSMConditionBase;
        }

        public static FSMStateBase GetFsmStateObj(EFSMStateType stateType)
        {
            Type type = Type.GetType(string.Format(StateTypeFormat, stateType));
            if (type == null)
            {
                return null;
            }
            return Activator.CreateInstance(type) as FSMStateBase;
        }
    }
}