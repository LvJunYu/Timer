using UnityEngine;
using System.Collections;
using System;

namespace GameA.Game.AI
{
    ///<summary>
    /// 远离
    ///</summary>
    public class SteeringAwayFrom : SteeringBase
    {
        /// <summary>
        /// 安全距离
        /// </summary>
        public float safeDistance = 10;

        public override Vector3 GetSteeringForce()
        {
            Vector3 fromTarget = this.transform.position - _target.position;
            //若距离大于最小距离，则返回0
            if (fromTarget.magnitude>=safeDistance)
                return Vector3.zero;
            
            _expectSpeed = fromTarget.normalized * _moveSpeed;
            return (_expectSpeed - loc.CurrentSpeed)*_weight;
        }
    }
}

