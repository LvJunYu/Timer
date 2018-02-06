
namespace GameA.Game.AI
{
    ///<summary>
    /// 丢失目标
    ///</summary>
    public class LoseTargetCondition : FSMConditionBase
    {
        public override bool CheckCondition(FSM fsm)
        {
            if (fsm.targetTF == null) return true;
            return false;
        }

        protected override void Init()
        {
            ConditonType = EFSMConditionType.LoseTarget;
        }
    }
}

