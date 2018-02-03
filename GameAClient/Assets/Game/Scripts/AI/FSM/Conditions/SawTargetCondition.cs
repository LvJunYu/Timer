
namespace GameA.Game.AI
{
    ///<summary>
    /// 发现目标
    ///</summary>
    public class SawTargetCondition : FSMConditionBase
    {
        public override bool CheckCondition(FSM fsm)
        {
            if (fsm.targetTF == null) return false;
            return false;
        }

        protected override void Init()
        {
            ConditonType = EFSMConditionType.SawTarget;
        }
    }
}

