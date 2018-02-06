using UnityEngine;
using System.Collections;
using System;

namespace GameA.Game.AI
{
    ///<summary>
    /// 逃避拦截
    ///</summary>
    public class SteeringEvade : SteeringBase
    {
        /// <summary>
        /// 安全距离
        /// </summary>
        public float safeDistance = 10;

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
            Vector3 toTarget = _target.position - this.transform.position;
            //若距离大于最小距离，则返回0
            if (toTarget.magnitude >= safeDistance)
                return Vector3.zero;
            //计算目标与自己正前方的角度
            var angel = Vector3.Angle(toTarget, this.transform.forward);
            //若在侧面，则远离拦截点
            if (angel > angelOffest && angel < (180 - angelOffest))
            {
                //计算目标速度向量
                targetSpeed = (_target.position - targetLastPos) / Time.deltaTime;
                targetLastPos = _target.position;
                //计算最小相遇时间（最精髓）
                minTime = Vector3.Distance(this.transform.position, _target.position) / (_moveSpeed + targetSpeed.magnitude);
                //预判目标的位置
                interceptPos = targetSpeed * minTime + _target.position;
                //计算期望速度，远离预判位置
                _expectSpeed = (this.transform.position - interceptPos).normalized * _moveSpeed;
            }
            //若在前方或后方，则直接远离
            else
            {
                _expectSpeed = (this.transform.position - _target.position).normalized * _moveSpeed;
            }

            return (_expectSpeed - loc.CurrentSpeed) * _weight;
        }
    }
}

