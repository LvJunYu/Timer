using UnityEngine;
using System.Collections;
using System;

namespace GameA.Game.AI
{
    ///<summary>
    /// 拦截
    ///</summary>
    public class SteeringIntercept : SteeringBase
    {
        /// <summary>
        /// 最小相遇时间（模拟相遇）
        /// </summary>
        private float minTime;

        /// <summary>
        /// 目标上一个位置
        /// </summary>
        private Vector3 targetLastPos;

        /// <summary>
        /// 目标速度向量
        /// </summary>
        private Vector3 targetSpeed;

        /// <summary>
        /// 拦截位置
        /// </summary>
        private Vector3 interceptPos;

        /// <summary>
        /// 正前方后方判断允许的角度偏移
        /// </summary>
        private float angelOffest = 2;

        public override Vector3 GetSteeringForce()
        {
            //计算目标与自己正前方的角度
            var angel = Vector3.Angle((_target.position - this.transform.position), this.transform.forward);
            //若在侧面，则拦截目标
            if (angel > angelOffest && angel < (180 - angelOffest))
            {
                //计算目标速度向量
                targetSpeed = (_target.position - targetLastPos) / Time.deltaTime;
                targetLastPos = _target.position;
                //计算最小相遇时间
                minTime = Vector3.Distance(this.transform.position, _target.position) / (_moveSpeed+ targetSpeed.magnitude);
                //预判位置
                interceptPos = targetSpeed * minTime + _target.position;
                //计算期望速度，靠近预判位置
                _expectSpeed = (interceptPos - this.transform.position).normalized * _moveSpeed;
            }
            //若在前方或后方，则直接追目标
            else
            {
                _expectSpeed = (_target.position - this.transform.position).normalized * _moveSpeed;
            }

            return (_expectSpeed - loc.CurrentSpeed) * _weight;
        }
    }
}

