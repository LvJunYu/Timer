
namespace GameA.Game.AI
{
    public class RunState : FSMStateBase
    {
        public override void Action(FSM fsm)
        {
        }

        public override void EnterState(FSM fsm)
        {
            fsm.IsPatrolCompleted = false;
        }

        public override void ExitState(FSM fsm)
        {
            fsm.Stop();
        }

        protected override void Init()
        {
            _stateType = EFSMStateType.Run;
        }
    }
}

