using UnityEngine;

namespace GameA.Game.AI
{
    ///<summary>
    /// 操控类
    ///</summary>
    [RequireComponent(typeof(LocomotionController))]
    public abstract class SteeringBase : MonoBehaviour
    {
        /// <summary>
        /// 目标
        /// </summary>
        protected Transform _target;

        /// <summary>
        /// 力的大小
        /// </summary>
        protected float _moveSpeed = 5;

        /// <summary>
        /// 权重
        /// </summary>
        protected float _weight = 1;

        /// <summary>
        /// 期望速度向量
        /// </summary>
        protected Vector3 _expectSpeed;

        /// <summary>
        /// 运动控制器
        /// </summary>
        protected Locomotion loc;

        protected void Awake()
        {
            loc = GetComponent<Locomotion>();
            if (_moveSpeed == 0)
            {
                _moveSpeed = loc.MoveSpeed;
            }
        }

        /// <summary>
        /// 计算操控力
        /// </summary>
        /// <returns></returns>
        public abstract Vector3 GetSteeringForce();
    }
}

