
namespace GameA.Game.AI
{
    ///<summary>
    /// 目标进入攻击范围
    ///</summary>
    public class ReachTargetCondition : FSMConditionBase
    {
        public override bool CheckCondition(FSM fsm)
        {
            if (fsm.targetTF == null) return false;
            return false;
        }

        protected override void Init()
        {
            ConditonType = EFSMConditionType.ReachTarget;
        }
    }
}

