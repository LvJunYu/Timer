
namespace GameA.Game.AI
{
    ///<summary>
    /// 攻击状态
    ///</summary>
    public class AttackingState : FSMStateBase
    {
        public override void Action(FSM fsm)
        {
            //朝向目标
        }

        public override void EnterState(FSM fsm)
        {
        }

        public override void ExitState(FSM fsm)
        {
        }

        protected override void Init()
        {
            _stateType = EFSMStateType.Attacking;
        }
    }
}

