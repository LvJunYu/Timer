using UnityEngine;
using System;

namespace GameA.Game.AI
{
    ///<summary>
    /// 路径跟随
    ///</summary>
    public class SteeringFollowPath : SteeringBase
    {
        /// <summary>
        /// 跟随模式
        /// </summary>
        public enum FollowMode { Once, Loop, PingPong }

        /// <summary>
        /// 路点数组
        /// </summary>
        public Transform[] paths;

        /// <summary>
        /// 跟随方式
        /// </summary>
        public EPatrolMode followMode;

        /// <summary>
        /// 到达距离
        /// </summary>
        public float arriveDistance = 1f;

        /// <summary>
        /// 当前路点索引
        /// </summary>
        private int currentIndex = 0;

        public override Vector3 GetSteeringForce()
        {
            if (Vector3.Distance(this.transform.position, paths[currentIndex].position) < arriveDistance)
            {
                if (currentIndex == paths.Length - 1)
                {
                    switch (followMode)
                    {
                        case EPatrolMode.Once:
                            return Vector3.zero;
                        case EPatrolMode.Loop:
                            break;
                        case EPatrolMode.PingPong:
                            Array.Reverse(paths);
                            break;
                    }
                }
                currentIndex = (currentIndex + 1) % paths.Length;
                _target = paths[currentIndex];
            }
            _expectSpeed = (paths[currentIndex].position - this.transform.position).normalized * _moveSpeed;
            return (_expectSpeed - loc.CurrentSpeed) * _weight;
        }
    }
    
    public enum EPatrolMode
    {
        Once,
        Loop,
        PingPong
    }
}

