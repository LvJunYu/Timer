
namespace GameA.Game.AI
{
    ///<summary>
    ///生命值为0
    ///</summary>
    public class NoHpCondition : FSMConditionBase
    {
        public override bool CheckCondition(FSM fsm)
        {
            return false;
        }

        protected override void Init()
        {
            ConditonType = EFSMConditionType.NoHp;
        }
    }
}

