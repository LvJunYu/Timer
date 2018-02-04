using UnityEngine;
using System.Collections;

namespace GameA.Game.AI
{
    ///<summary>
    /// 徘徊
    ///</summary>
    public class SteeringWander : SteeringBase
    {
        /// <summary>
        /// 徘徊圆半径
        /// </summary>
        public float wanderCircleRadius = 5;

        /// <summary>
        /// 与徘徊圆距离
        /// </summary>
        public float wanderCircleDistance = 6;

        /// <summary>
        /// 徘徊变化的频率
        /// </summary>
        public float wanderFrequency = 2f;
        
        /// <summary>
        /// 目标点相对圆心的向量
        /// </summary>
        private Vector3 relativeFromCircle;

        /// <summary>
        /// 目标点相对自己的向量
        /// </summary>
        private Vector3 relaviteFromSelf;

        /// <summary>
        /// 目标点位置
        /// </summary>
        private Vector3 targetPos;

        public override Vector3 GetSteeringForce()
        {
            //计算期望速度
            _expectSpeed = (targetPos - this.transform.position).normalized * _moveSpeed;
            return (_expectSpeed - loc.CurrentSpeed) * _weight;
        }

        /// <summary>
        /// 重置目标点
        /// </summary>
        /// <returns></returns>
        private IEnumerator SetTargetPos()
        {
            while (true)
            {
                //计算相对圆心向量
                if (loc.IsPlane)
                    relativeFromCircle = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                else
                    relativeFromCircle = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                relativeFromCircle = relativeFromCircle.normalized * wanderCircleRadius;
                //计算相对自己的向量
                relaviteFromSelf = this.transform.forward * wanderCircleDistance + relativeFromCircle;
                //拉远目标点以防止过快到达
                relaviteFromSelf = relaviteFromSelf.normalized * _moveSpeed * wanderFrequency * 2;
                //计算目标点
                targetPos = relaviteFromSelf + this.transform.position;
                //sign.position = targetPos;
                yield return new WaitForSeconds(wanderFrequency);
            }
        }
        //测试目标点位置
        //public Transform sign;

        private void OnEnable()
        {
            StartCoroutine("SetTargetPos");
        }

        private void OnDisable()
        {
            StopCoroutine("SetTargetPos");
        }

    }
}

