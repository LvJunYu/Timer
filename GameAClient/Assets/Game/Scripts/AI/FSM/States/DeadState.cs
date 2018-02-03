
namespace GameA.Game.AI
{
    ///<summary>
    ///死亡状态
    ///</summary>
    public class DeadState : FSMStateBase
    {
        public override void Action(FSM fsm)
        {
        }

        public override void EnterState(FSM fsm)
        {
        }

        public override void ExitState(FSM fsm)
        {
            
        }

        protected override void Init()
        {
            _stateType = EFSMStateType.Dead;
        }
    }
}

