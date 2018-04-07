using UnityEngine;

namespace GameA.Game.AI
{
    ///<summary>
    /// 运动控制
    ///</summary>
    public class LocomotionController : Locomotion
    {
        private int _timer;

        /// <summary>
        /// 移动
        /// </summary>
        public void Move()
        {
            //若已停止移动，则立即重新计算操控矢量，防止出现不恰当的停滞
            if (CurrentSpeed == Vector3.zero)
            {
                CaculateSteeringVector();
            }

            //将操控矢量叠加在当前速度向量上
            //操控矢量相当于到达期望速度所需要发生的变化向量，以Time.deltaTime的比例变化，类似Lerp如下
            //currentSpeed = Vector3.Lerp(currentSpeed, currentSpeed + steeringVector, acceleration * Time.deltaTime);
            CurrentSpeed +=
                SteeringVector * _acceleration * Time.deltaTime; //在steeringSpeed不变的情况下1/acceleration秒内转换到期望速度
            //限制速度
            CurrentSpeed = Vector3.ClampMagnitude(CurrentSpeed, MaxMoveSpeed);
            //移动
//            transform.position += (CurrentSpeed * Time.deltaTime);
        }

        /// <summary>
        /// 旋转
        /// </summary>
        public void LookAt()
        {
            if (CurrentSpeed != Vector3.zero)
            {
//                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(CurrentSpeed),
//                    RotateSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        public void PlayAnimation()
        {
            //chAnim.PlayAnimation("run");
        }

        public void UpdateLogic()
        {
            if (!_run)
            {
                return;
            }
            if(_timer>0)
            {
                _timer--;
            }
            else
            {
                CaculateSteeringVector();
                _timer = 10;
            }
            
            Move();
            LookAt();
            PlayAnimation();
        }
    }
}