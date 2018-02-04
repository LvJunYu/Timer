using UnityEngine;
using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game.AI
{
    ///<summary>
    /// 状态机
    ///</summary>
    public class FSM
    {
        public float SightDistance = 10; // 视野范围
        public float RunSpeed = 3; // 追逐跑步速度
        public float WalkSpeed = 2; // 巡逻走路速度
        public float ArrivalDistance = 2f; // 巡逻到达距离
        public float LowHealthRate = 0.3f; // 低生命阈值
        private int _aiType = 1;
        private EFSMStateType _defaultStateType = EFSMStateType.Run; // 默认状态
        private FSMStateBase _defaultState;
        private FSMStateBase _currentState; // 当前状态
        private List<FSMStateBase> _stateList = new List<FSMStateBase>(); // 缓存状态对象
        [HideInInspector] public SightSensor SightSensor; // 视觉感应器
        [HideInInspector] public SteeringFollowPath FollowPath; // 跟随路径组件
        
        [HideInInspector] public Transform targetTF; // 目标TF
        public EPatrolMode EPatrolMode; // 巡逻模式
        public bool IsPatrolCompleted; // 是否完成巡逻
        private bool _run;

        public void TowardToTarget(Vector3 targetPos, float moveSpeed, float stopDistance)
        {
        }

        public void Stop()
        {
        }

        public void ResetTarget()
        {
        }

        /// <summary>
        /// 初始化引用数据、管理对象所需数据及行为
        /// </summary>
        public void Init()
        {
            ConfigFSM();
            InitDefaultState();
        }

        /// <summary>
        /// 初始化状态条件映射
        /// </summary>
        private void ConfigFSM()
        {
            var dic = AIConfigHelper.GetAIConfig(_aiType);
            //遍历每个主键转换为FSMStateID，调用实例方法
            foreach (var preState in dic.Keys)
            {
                GetFSMState(preState).SetTransformConditions(dic[preState]);
            }
        }

        /// <summary>
        /// 初始化默认状态
        /// </summary>
        public void InitDefaultState()
        {
            _defaultState = GetFSMState(_defaultStateType);
            _currentState = _defaultState;
        }

        public void ChangeActiveState(EFSMStateType stateType)
        {
            if (stateType == EFSMStateType.None)
            {
                LogHelper.Error("ChangeActiveState fail, stateType == EFSMStateType.None");
                return;
            }
            _currentState.ExitState(this);
            if (stateType == EFSMStateType.Default)
            {
                _currentState = _defaultState;
            }
            else
            {
                _currentState = GetFSMState(stateType);
            }
            _currentState.EnterState(this);
        }

        public void UpdateLogic()
        {
            if (!_run)
            {
                return;
            }
            if (_currentState != null)
            {
                //检测状态转换
                _currentState.Reason(this);
                //执行当前状态效果
                _currentState.Action(this);
            }
            else
            {
                LogHelper.Error("currentState == null");
            }
        }

        private FSMStateBase GetFSMState(EFSMStateType stateType)
        {
            FSMStateBase state = _stateList.Find(p => p.StateType == stateType);
            if (state == null)
            {
                state = FSMFactory.GetFsmStateObj(stateType);
                _stateList.Add(state);
            }

            return state;
        }

        public void SetEnable(bool value)
        {
            _run = value;
            if (value)
            {
                InitDefaultState();
            }
            else
            {
                Stop();
                ChangeActiveState(_defaultStateType);
            }

        }

    }
}