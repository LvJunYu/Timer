using UnityEngine;
using System.Collections;

namespace GameA.Game.AI
{
    ///<summary>
    /// 分散
    ///</summary>
    [RequireComponent(typeof(Radar))]
    public class SteeringSeparation : SteeringBase
    {
        /// <summary>
        /// 分散半径
        /// </summary>
        public float separationDistance = 5;

        /// <summary>
        /// 雷达
        /// </summary>
        private Radar radar;

        public override Vector3 GetSteeringForce()
        {
            radar.ResetTargets();
            //计算所有邻居的排斥力合力
            _expectSpeed = Vector3.zero;
            for (int i = 0; i < radar.targets.Count; i++)
            {
                var FromTarget = this.transform.position - radar.targets[i].transform.position;
                if (FromTarget.magnitude < separationDistance //若小于分隔距离
                    && FromTarget != Vector3.zero)//且不是自己
                {
                    //则计算排斥力
                    _expectSpeed += FromTarget.normalized * _moveSpeed;
                }
            }
            //若没有排斥力，则返回0
            if (_expectSpeed == Vector3.zero)
                return Vector3.zero;
            //否则返回操控力
            return (_expectSpeed - loc.CurrentSpeed) * _weight;
        }

        private void OnEnable()
        {
            radar = GetComponent<Radar>();
        }
    }
}

