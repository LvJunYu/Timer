using UnityEngine;
using System.Collections;
using System;

namespace GameA.Game.AI
{
    ///<summary>
    /// 躲避障碍
    ///</summary>
    public class SteeringAvoiderObstacle : SteeringBase
    {
        /// <summary>
        /// 检测射线长度
        /// </summary>
        public float detectDistance = 10;

        /// <summary>
        /// 推力大小
        /// </summary>
        public float ThrustForce = 30;

        /// <summary>
        /// 射线发射点
        /// </summary>
        public Transform sendPos;

        /// <summary>
        /// 射线碰撞信息
        /// </summary>
        private RaycastHit hit;

        /// <summary>
        /// 障碍物tag
        /// </summary>
        public string obstacleTag = "Obstacle";

        /// <summary>
        /// 射线层
        /// </summary>
        //public LayerMask layerMask;

        public override Vector3 GetSteeringForce()
        {
            //如碰到，则从对方中心点向碰撞点施加一个推力
            if (Physics.Raycast(sendPos.position, sendPos.forward, out hit, detectDistance/*,layerMask.value*/)
                && hit.transform.tag == obstacleTag)
            {
                //获取碰撞体中心点到碰撞点的向量
                _expectSpeed = hit.point - hit.transform.position;
                return _expectSpeed.normalized * ThrustForce * _weight;
            }
            return Vector3.zero;
        }

        private void OnEnable()
        {
            if (sendPos == null)
                sendPos = this.transform.FindChild("eye");
            if (sendPos == null)
                sendPos = this.transform;
        }
    }
}

