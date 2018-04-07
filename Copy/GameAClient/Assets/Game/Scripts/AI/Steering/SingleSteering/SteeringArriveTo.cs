using UnityEngine;
using System.Collections;
using System;

namespace GameA.Game.AI
{
    ///<summary>
    /// 到达
    ///</summary>
    public class SteeringArriveTo : SteeringBase
    {
        /// <summary>
        /// 到达半径
        /// </summary>
        public float arriveRadius = 1f;

        /// <summary>
        /// 减速区半径
        /// </summary>
        public float slowDownRadius = 10f;

        /// <summary>
        /// 最小速度
        /// </summary>
        public float minSpeed = 1f;

        public override Vector3 GetSteeringForce()
        {
            //计算当前距离
            Vector3 toTarget = _target.position - this.transform.position;
            float distance = toTarget.magnitude;
            //若到达半径内，则停止
            if (distance <= arriveRadius)
                return Vector3.zero;
            //若在减速区外，期望速度为移动速度
            float currentSpeed = _moveSpeed;
            //若在减速区内，期望速度=(距离-到达半径)/(减速区半径-到达半径)*移动速度
            if (distance < slowDownRadius)
            {
                currentSpeed = (distance - arriveRadius) / (slowDownRadius - arriveRadius) * _moveSpeed;
                //防止速度过慢，与动画不吻合
                currentSpeed = currentSpeed < minSpeed ? minSpeed : currentSpeed;
            }
            //计算期望速度
            _expectSpeed = toTarget.normalized * currentSpeed;
            return (_expectSpeed - loc.CurrentSpeed) * _weight;
        }
    }
}

