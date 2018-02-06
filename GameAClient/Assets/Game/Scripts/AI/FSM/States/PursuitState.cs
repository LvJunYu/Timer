using SoyEngine;

namespace GameA.Game.AI
{
    ///<summary>
    /// 追逐状态
    ///</summary>
    public class PursuitState : FSMStateBase
    {
        public override void Action(FSM fsm)
        {
            if (fsm.targetTF == null)
            {
                LogHelper.Debug("目标不存在");
            }
            //设置移动速度、停止距离、设定目标
        }

        public override void EnterState(FSM fsm)
        {
        }

        public override void ExitState(FSM fsm)
        {
        }

        protected override void Init()
        {
            _stateType = EFSMStateType.Pursuit;
        }
    }
}

