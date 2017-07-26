using UnityEngine;

namespace GameA.Game
{
    public enum EInputType
    {
        Left,
        Right,
        Up,
        Down,
        Jump,
        Assist,
        Skill1,
        Skill2,
        Skill3,
        Max
    }
    
    public class ActorInput
    {
        protected ActorBase _actor;
        
        [SerializeField]
        protected float _lastHorizontal;
        [SerializeField]
        protected float _curHorizontal;
        [SerializeField]
        protected float _lastVertical;
        [SerializeField]
        protected float _curVertical;

        [SerializeField]
        protected int _leftInput;
        [SerializeField]
        protected int _rightInput;
        [SerializeField]
        protected int _upInput;
        [SerializeField]
        protected int _downInput;

        [SerializeField]
        protected bool _jumpInput;
        protected bool _lastJumpInput;

        [SerializeField]
        protected bool _assistInput;
        [SerializeField]
        protected bool _lastAssistInput;

        [SerializeField]
        protected bool[] _skillInputs = new bool[3];
        [SerializeField]
        protected bool[] _lastSkillInputs = new bool[3];

        // 跳跃等级
        [SerializeField]
        public int _jumpLevel = 0;
        // 跳跃状态
        [SerializeField] public EJumpState _jumpState;

        [SerializeField]
        public EClimbState _eClimbState;
        // 攀墙跳
        [SerializeField]
        protected bool _climbJump = false;
        protected int _stepY;
        /// <summary>
        /// 起跳的动画时间
        /// </summary>
        protected int _jumpTimer;
    }
}