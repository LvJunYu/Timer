using System.Collections.Generic;

namespace GameA.Game.AI
{
    public abstract class FSMStateBase
    {
        protected EFSMStateType _stateType;
        protected List<TransformState> _transformList;

        public EFSMStateType StateType
        {
            get { return _stateType; }
        }

        public void SetTransformConditions(List<TransformState> transformStates)
        {
            _transformList = transformStates;
        }

        public void Reason(FSM fsm)
        {
            if (_transformList == null) return;
            //遍历每一个转换可能
            for (int i = 0; i < _transformList.Count; i++)
            {
                //检测转换条件列表，如果满足条件
                if (CheckConditions(fsm, _transformList[i].Constions))
                {
                    //通知状态机，进行状态转换，并返回
                    fsm.ChangeActiveState(_transformList[i].TargetStateType);
                    return;
                }
            }
        }

        private bool CheckConditions(FSM fsm, FSMConditionBase[] conditions)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                //若有一个条件不满足，则返回false
                if (conditions[i].CheckCondition(fsm) == false)
                {
                    return false;
                }
            }

            //若都满足，则返回true
            return true;
        }

        public FSMStateBase()
        {
            Init();
        }

        protected abstract void Init();

        public virtual void EnterState(FSM fsm)
        {
        }

        public virtual void ExitState(FSM fsm)
        {
        }

        public abstract void Action(FSM fsm);

        public virtual void Clear()
        {
            _transformList = null;
        }
    }

    public enum EFSMStateType
    {
        None,
        Default,
        Idle,
        Run,
        Dead,
        Pursuit,
        Attacking,
        Dialog,
        Bang,
    }
}