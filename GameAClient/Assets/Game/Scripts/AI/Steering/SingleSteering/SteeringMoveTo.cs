using System;
using UnityEngine;

namespace GameA.Game.AI
{
    ///<summary>
    /// 靠近
    ///</summary>
    public class SteeringMoveTo : SteeringBase
    {
        public override Vector3 GetSteeringForce()
        {
            //计算期望速度
            _expectSpeed = (_target.position - this.transform.position).normalized * _moveSpeed;
            return (_expectSpeed - loc.CurrentSpeed)*_weight;
        }
    }
}

