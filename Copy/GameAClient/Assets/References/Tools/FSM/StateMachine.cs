/*********************************************************
 * 开发人员：donglee
 * 创建时间：2013/7/14 14:23:23
 * 描述说明：StateMachine
 * *******************************************************/

using System;

namespace SoyEngine.FSM
{
    public class StateMachine<T, TSbase> where T : class where TSbase : State<T>
    {
        protected readonly T _owner;
        protected TSbase _currentState;
        protected TSbase _globalState;
        protected TSbase _previousState;
        public event Action<TSbase, TSbase> BeforeChangeStateCallback;
        public event Action<TSbase, TSbase> AfterChangeStateCallback;

        public virtual TSbase CurrentState
        {
            get { return _currentState; }
        }

        public virtual TSbase GlobalState
        {
            get { return _globalState; }
            set { _globalState = value; }
        }

        public virtual TSbase PreviousState
        {
            get { return _previousState; }
        }

        public StateMachine(T owner)
        {
            _owner = owner;
            _currentState = null;
            _previousState = null;
            _globalState = null;
        }

        public void Update()
        {
            if (_globalState != null)
            {
                _globalState.Execute(_owner);
            }

            if (_currentState != null)
            {
                _currentState.Execute(_owner);
            }
        }

        public void ChangeState(TSbase newState)
        {
            if (newState == null)
            {
                LogHelper.Error("<StateMachine ChangeState>: trying to assign null state to current");
                return;
            }
            if (newState == _currentState)
            {
                LogHelper.Warning("<StateMachine ChangeState>: assign same state to current");
            }
            if (null != BeforeChangeStateCallback)
            {
                BeforeChangeStateCallback.Invoke(_currentState, newState);
            }
            
            _previousState = _currentState;
            if (_currentState != null)
            {
                _currentState.Exit(_owner);
            }
            _currentState = newState;
            _currentState.Enter(_owner);
            
            if (null != AfterChangeStateCallback)
            {
                AfterChangeStateCallback.Invoke(_previousState, _currentState);
            }
        }

        public void RevertToPreviousState()
        {
            ChangeState(_previousState);
        }

        public bool IsInState(TSbase s)
        {
            if (_currentState == s)
            {
                return true;
            }
            return false;
        }
    }
}