using UnityEngine;
using System.Collections;
using System;

namespace GameA.Game.AI
{
    ///<summary>
    /// 聚集
    ///</summary>
    [RequireComponent(typeof(Radar))]
    public class SteeringGather : SteeringBase
    {
        /// <summary>
        /// 聚集的停止距离
        /// </summary>
        public float arriveDistance = 1;

        /// <summary>
        /// 聚集半径
        /// </summary>
        public float gatherRadius = 5;

        /// <summary>
        /// 雷达
        /// </summary>
        private Radar radar;

        /// <summary>
        /// 所有扫描目标的中心点
        /// </summary>
        private Vector3 centerPos;

        /// <summary>
        /// 重置中心点
        /// </summary>
        private void ResetCenterPos()
        {
            radar.ResetTargets();
            centerPos = Vector3.zero;
            //计算中心点
            for (int i = 0; i < radar.targets.Count; i++)
            {
                centerPos += radar.targets[i].transform.position;
            }
            centerPos = centerPos / radar.targets.Count;
        }

        public override Vector3 GetSteeringForce()
        {
            ResetCenterPos();
            //自身到中心点向量
            var toCenterPos = centerPos - this.transform.position;
            //若已到达则返回0
            if (toCenterPos.magnitude < arriveDistance)
                return Vector3.zero;
            //计算期望速度
            _expectSpeed = toCenterPos.normalized * _moveSpeed;
            return (_expectSpeed - loc.CurrentSpeed) * _weight;
        }

        private void OnEnable()
        {
            radar = GetComponent<Radar>();
            if (gatherRadius != 0)
                radar.scanRadius = gatherRadius;
        }

    }
}

